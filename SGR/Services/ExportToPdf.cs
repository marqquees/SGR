using System.Collections.Concurrent;
using Microsoft.JSInterop;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGR.Models;

namespace SGR.Services;

public class ExportToPdf(IWebHostEnvironment environment, IJSRuntime jsRuntime, ILogger<ExportToPdf> logger)
{
    private static readonly ConcurrentDictionary<string, DateTime> TempFiles = new();

    public async Task ExportEquipmentToPdfAndNavigate(Equipment? selectedEquipment)
    {
        try
        {
            if (selectedEquipment is null) return;

            // Gera o PDF e obtém o caminho do arquivo.
            string result = await ExportEquipmentToPdf(selectedEquipment);

            // Se o resultado for um caminho válido, abre o PDF em uma nova aba.
            if (result.StartsWith("/temp_pdf/")) 
                await jsRuntime.InvokeVoidAsync("open", result, "_blank");
        }
        catch (Exception error)
        {
            logger.LogError(error, "Erro ao exportar o ficheiro em PDF.");
        }
    }
    
    private async Task<string> ExportEquipmentToPdf(Equipment selectedEquipment)
    {
        try
        {
            // Configuração para licença comunitária.
            Settings.License = LicenseType.Community;

            // Gera o PDF em memória.
            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Element(c => ComposeHeader(c, selectedEquipment));
                    page.Content().Element(c => ComposeContent(c, selectedEquipment));
                });
            }).GeneratePdf();

            // Cria um nome de ficheiro único baseado no ID do equipamento.
            string fileName = $"Equipamento_{selectedEquipment.Id}.pdf";

            // Define o caminho para a pasta temporária onde os PDFs serão armazenados.
            string folderPath = Path.Combine(environment.WebRootPath, "temp_pdf");
            
            // Salva o ficheiro no servidor.
            string filePath = Path.Combine(folderPath, fileName);

            // Garante que o diretório existe.
            Directory.CreateDirectory(folderPath);

            // Escreve os bytes do PDF no ficheiro.
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            // Adiciona o ficheiro ao dicionário de ficheiros temporários para limpeza futura.
            TempFiles[filePath] = DateTime.Now;

            // Retorna o caminho relativo do ficheiro para download.
            return $"/temp_pdf/{fileName}";
        }
        catch (Exception error)
        {
            logger.LogError(error, "Erro ao gerar o PDF.");
            return string.Empty;
        }
    }
    
    private static void ComposeHeader(IContainer container, Equipment? equipment)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                if (equipment is null) return;
                column.Item().Text("Detalhes do Equipamento").FontSize(20).Bold();
                column.Item().Text($"{DateTime.Now:dd/MM/yyyy}").FontSize(10);
            });
        });
    }
    
    private static void ComposeContent(IContainer container, Equipment? equipment)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Controlo
            column.Item().Text("Controlo").FontSize(14).Bold();
            column.Item().PaddingBottom(5).LineHorizontal(1);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Text("Data de Registo").Bold();
                table.Cell().Text(equipment?.DateRegister.ToString("dd/MM/yyyy"));
                table.Cell().Text("Estado").Bold();
                table.Cell().Text(equipment?.State);
                table.Cell().Text("Responsável").Bold();
                table.Cell().Text(equipment?.Responsible);
            });

            column.Item().Height(20);

            // Cliente e Utilizador
            column.Item().Text("Cliente e Utilizador").FontSize(14).Bold();
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
            });
            
            column.Item().Height(20);
            
            // Identificação do Equipamento
            column.Item().Text("Identificação do Equipamento").FontSize(14).Bold();
            column.Item().PaddingBottom(5).LineHorizontal(1);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Text("Categoria").Bold();
                table.Cell().Text(equipment?.Category);
                table.Cell().Text("Fabricante").Bold();
                table.Cell().Text(equipment?.Manufacturer);
                table.Cell().Text("Modelo").Bold();
                table.Cell().Text(equipment?.Model);
                table.Cell().Text("Número de Série").Bold();
                table.Cell().Text(equipment?.SerialNumber);
            });
            
            column.Item().Height(20);

            // Especificações Técnicas
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

            // Manutenção
            column.Item().Text("Manutenção").FontSize(14).Bold();
            column.Item().PaddingBottom(5).LineHorizontal(1);

            column.Item().Text("Avaria").Bold();
            column.Item().Text(equipment?.Damage);
            column.Item().Height(5);
            column.Item().Text("Observação").Bold();
            column.Item().Text(equipment?.Note);
        });
    }

    public static ConcurrentDictionary<string, DateTime> GetTempFiles() => TempFiles;
    
    public static void RemoveFromTempFiles(string filePath) => TempFiles.TryRemove(filePath, out _);
}