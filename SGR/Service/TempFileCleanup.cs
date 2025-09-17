using SGR.Model;

namespace SGR.Service
{
    /// <summary>
    /// Serviço em segundo plano responsavel por eliminar arquivos .PDF temporários.
    /// </summary>
    /// <remarks>
    /// Inicializa uma nova instancia da classe <see cref="TempFileCleanup"/>.
    /// </remarks>
    /// <param name="logger">Logger para registrar informações e erros.</param>
    public class TempFileCleanup(ILogger<TempFileCleanup> logger) : BackgroundService
    {
        private readonly ILogger<TempFileCleanup> _logger = logger;
        private readonly TimeSpan _deleteAfter = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1440);

        /// <summary>
        /// Método executado quando o serviço iniciado.
        /// </summary>
        /// <param name="stoppingToken">Token de cancelamento para parar o serviço.</param>
        /// <returns>Tarefa que representa a operação assincrona.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Limpeza de Arquivo Temporário iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupTempFiles();
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Erro ao limpar arquivos temporários. Detalhes: {error.Messagem}", error.Message);
                }

                // Aguarda o intervalo de verificação antes de executar novamente.
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        /// <summary>
        /// Método responsável por limpar arquivos temporários que foram criados há mais de 30 minutos.
        /// </summary>
        /// <returns>Tarefa que representa a operação assincrona.</returns>
        private Task CleanupTempFiles()
        {
            var tempFiles = ExportToPdf.GetTempFiles();
            var now = DateTime.Now;
            var filesToDelete = tempFiles.Where(f => now - f.Value > _deleteAfter).ToList();

            foreach (var file in filesToDelete)
            {
                try
                {
                    if (File.Exists(file.Key))
                    {
                        File.Delete(file.Key);
                        _logger.LogInformation("Arquivo temporário excluído: {FilePath}", file.Key);
                    }
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Erro ao excluir arquivo temporário: {FilePath}", file.Key);
                }
            }
            return Task.CompletedTask;
        }
    }
}