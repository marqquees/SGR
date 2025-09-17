using Microsoft.AspNetCore.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Concurrent;

namespace SGR.Model
{
    /// <summary>
    /// Classe responsável por exportar os detalhes de um equipamento para PDF.
    /// </summary>
    /// <remarks>
    /// Inicializa uma nova instância da classe <see cref="ExportToPdf"/>.
    /// </remarks>
    /// <param name="environment">O ambiente de alojamento que fornece informações sobre o ambiente da aplicação Web, 
    /// como a raiz do conteúdo e os caminhos da raiz da Web.</param>
    /// <param name="navigationManager">O gestor de navegação utilizado para gerir URIs e a navegação na aplicação.</param>
    public class ExportToPdf(IWebHostEnvironment environment, NavigationManager navigationManager)
    {

        /// <summary>
        /// Dicionário estático para rastrear arquivos temporários e seus horários de criação.
        /// </summary>
        private static readonly ConcurrentDictionary<string, DateTime> TempFiles = new();

        /// <summary>
        /// Método responsável por exportar os detalhes do equipamento para PDF e navegar para o arquivo.
        /// </summary>
        /// <param name="selectedItem">O item de inventário a ser exportado para PDF.</param>
        /// <returns>String contendo o caminho do arquivo ou mensagem de erro.</returns>
        public async Task<string> ExportEquipmentToPdfAndNavigate(Equipment? selectedItem)
        {
            try
            {
                if (selectedItem == null)
                    return "Erro: item não selecionado.";

                // Gera o PDF e obtém o caminho do arquivo.
                string result = await ExportEquipmentToPdf(selectedItem);

                // Se o resultado começar com "/temp-pdf/", navega para o arquivo.
                if (result.StartsWith("/temp-pdf/"))
                    navigationManager.NavigateTo(result, true);

                return result;
            }
            catch (Exception error)
            {
                return $"Erro ao exportar o arquivo PDF: {error.Message}";
            }
        }
        /// <summary>
        /// Metodo responsável por exportar os detalhes do equipamento para PDF.
        /// </summary>
        public async Task<string> ExportEquipmentToPdf(Equipment? selectedItem)
        {
            try
            {
                if (selectedItem == null)
                    return "Erro: Item não selecionado.";

                // Configuração para licença comunitária.
                QuestPDF.Settings.License = LicenseType.Community;

                // Gera o PDF em memória.
                var pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Element(c => ComposeHeader(c, selectedItem));
                        page.Content().Element(c => ComposeContent(c, selectedItem));
                    });
                }).GeneratePdf();

                // Cria um nome de arquivo único baseado no ID do equipamento e timestamp.
                var fileName = $"Equipamento_{selectedItem.Id}.pdf";

                // Salva o arquivo no servidor.
                var filePath = Path.Combine(environment.WebRootPath, "temp-pdf", fileName);

                // Garante que o diretório existe.
                Directory.CreateDirectory(Path.Combine(environment.WebRootPath, "temp-pdf"));

                // Escreve os bytes do PDF no arquivo.
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Adiciona o arquivo ao dicionário de arquivos temporários para limpeza posterior.
                TempFiles[filePath] = DateTime.Now;

                // Retorna o caminho relativo do arquivo para download.
                return $"/temp-pdf/{fileName}";
            }
            catch (Exception error)
            {
                return $"Erro ao gerar PDF: {error.Message}";
            }
        }

        /// <summary>
        /// Método responsável por compor o cabeçalho do PDF com os detalhes do equipamento.
        /// </summary>
        private static void ComposeHeader(IContainer container, Equipment? item)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    if (item != null)
                        column.Item().Text($"Detalhes do Equipamento #{item.Id}").FontSize(20).Bold();
                    column.Item().Text($"Data de geração: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                });
            });
        }

        /// <summary>
        /// Método responsável por compor o conteúdo do PDF com as informações do equipamento.
        /// </summary>
        private static void ComposeContent(IContainer container, Equipment? equipment)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // Informações básicas.
                column.Item().Text("Informações Básicas").FontSize(14).Bold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Text("Data de Registro").Bold();
                    table.Cell().Text(equipment?.DateRegister.ToString("dd/MM/yyyy"));
                    table.Cell().Text("Estado").Bold();
                    table.Cell().Text(equipment?.State);
                });

                column.Item().Height(20);

                // Detalhes do equipamento.
                column.Item().Text("Detalhes do Equipamento").FontSize(14).Bold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Text("Cliente").Bold();
                    table.Cell().Text(equipment?.Customer);
                    table.Cell().Text("Utilizador").Bold();
                    table.Cell().Text(equipment?.User);
                    table.Cell().Text("Categoria").Bold();
                    table.Cell().Text(equipment?.Category);
                    table.Cell().Text("Marca").Bold();
                    table.Cell().Text(equipment?.Brand);
                    table.Cell().Text("Modelo").Bold();
                    table.Cell().Text(equipment?.Model);
                    table.Cell().Text("Número de Série").Bold();
                    table.Cell().Text(equipment?.SerialNumber);
                });

                column.Item().Height(20);

                // Especificações técnicas.
                column.Item().Text("Especificações Técnicas").FontSize(14).Bold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Text("Processador").Bold();
                    table.Cell().Text(equipment?.Processor);
                    table.Cell().Text("Memória RAM").Bold();
                    table.Cell().Text(equipment?.MemoryRAM);
                    table.Cell().Text("Armazenamento").Bold();
                    table.Cell().Text(equipment?.Storage);
                    table.Cell().Text("Sistema Operativo").Bold();
                    table.Cell().Text(equipment?.OperatingSystem);
                });

                column.Item().Height(20);

                // Manutenção e controle.
                column.Item().Text("Manutenção e Controlo").FontSize(14).Bold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Text("Responsável").Bold();
                    table.Cell().Text(equipment?.Responsible);
                });

                column.Item().Height(10);
                column.Item().Text("Observação").Bold();
                column.Item().Text(equipment?.Note);
            });
        }

        /// <summary>
        /// Obtém o dicionário de arquivos temporários para limpeza.
        /// </summary>
        /// <returns>Dicionário de arquivos temporários.</returns>
        public static ConcurrentDictionary<string, DateTime> GetTempFiles()
        {
            return TempFiles;
        }

        /// <summary>
        /// Remove um arquivo do dicionário de arquivos temporários.
        /// </summary>
        /// <param name="filePath">Caminho do arquivo a ser removido.</param>
        public static void RemoveFromTempFiles(string filePath)
        {
            TempFiles.TryRemove(filePath, out _);
        }
    }
}