using System.Collections.Concurrent;
using SGR.Models;

namespace SGR.Services;

public class TempFileCleanup(ILogger<TempFileCleanup> logger) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _deleteAfter = TimeSpan.FromMinutes(5);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                CleanupTempFile();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Erro ao eliminar os ficheiros temporários.");
            }

            // Aguarda o intervalo de verificação antes de executar novamente.
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
    
    private void CleanupTempFile()
    {
        ConcurrentDictionary<String, DateTime> tempFiles = ExportToPdf.GetTempFiles();
        DateTime now = DateTime.Now;
        List<KeyValuePair<String, DateTime>> filesToDelete = tempFiles.Where(f => 
            now - f.Value > _deleteAfter).ToList();

        foreach (KeyValuePair<String, DateTime> file in filesToDelete)
        {
            try
            {
                if (File.Exists(file.Key))
                {
                    File.Delete(file.Key);
                    logger.LogInformation("Ficheiro temporário excluído do Disco: {FilePath}", file.Key);
                }

                ExportToPdf.RemoveFromTempFiles(file.Key);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Erro ao remover o ficheiro temporário: {FilePath}", file.Key);
            }
        }
    }
}