using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SGR.Data;
using SGR.Models;

namespace SGR.Service
{
    /// <summary>
    /// Serviço que gerencia operações CRUD para equipamentos.
    /// </summary>
    public class OperationService(EquipmentContext context, ILogger<OperationService> logger)
    {
        private readonly EquipmentContext _context = context;
        private readonly ILogger<OperationService> _logger = logger;

        /// <summary>
        /// Lista todos os equipamentos do histórico.
        /// </summary>
        public async Task<List<Equipment>> ListEquipmentAsync()
        {
            return await _context.Equipment.ToListAsync();
        }

        /// <summary>
        /// Salva um equipamento.
        /// </summary>
        public async Task<bool> SaveEquipmentAsync(Equipment item)
        {
            try
            {
                if (item.Id == 0) _context.Equipment.Add(item);
                else
                {
                    var existingItem = await _context.Equipment.FindAsync(item.Id);
                    if (existingItem == null) 
                        return false;
                    _context.Entry(existingItem).CurrentValues.SetValues(item);
                }

                await _context.SaveChangesAsync();
                await ReseedIdentityColumnAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao salvar o item com ID {item.Id}. Detalhes: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remove um equipamento do banco de dados.
        /// </summary>
        public async Task<bool> DeleteEquipmentAsync(Equipment item)
        {
            try
            {
                var entity = await _context.Equipment.FindAsync(item.Id);
                if (entity == null) return false;

                _context.Equipment.Remove(entity);
                await _context.SaveChangesAsync();
                await ReseedIdentityColumnAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao remover o item com ID {ItemId}. Detalhes: {ErrorMessage}", item.Id, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Filtra equipamentos baseado em termo de pesquisa.
        /// </summary>
        public List<Equipment> FilterHistory(string searchTerm, List<Equipment> items)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return items;

            searchTerm = searchTerm.ToLower();
            return [.. items.Where(i =>
                i.Id.ToString().Contains(searchTerm) == true ||
                i.DateRegister.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                i.State?.Contains(searchTerm) == true ||
                i.Customer?.Contains(searchTerm) == true ||
                i.User?.Contains(searchTerm) == true ||
                i.Category?.Contains(searchTerm) == true ||
                i.Brand?.Contains(searchTerm) == true ||
                i.Model?.Contains(searchTerm) == true ||
                i.SerialNumber?.Contains(searchTerm) == true ||
                i.Processor?.Contains(searchTerm) == true ||
                i.MemoryRAM?.Contains(searchTerm) == true ||
                i.Storage?.Contains(searchTerm) == true ||
                i.OperatingSystem?.Contains(searchTerm) == true ||
                i.Note?.Contains(searchTerm) == true ||
                i.Responsible?.Contains(searchTerm) == true
            )];
        }

        /// <summary>
        /// Redefine a sequência de IDs da tabela History para continuar a partir do maior ID existente.
        /// </summary>
        /// <returns>True se a operação foi bem-sucedida, False caso contrário.</returns>
        private async Task ReseedIdentityColumnAsync()
        {
            const string tableName = "History";
            
            try
            {
                // Verifica se há registros na tabela antes de tentar redefinir.
                var hasRecords = await _context.Equipment.AnyAsync();
                if (!hasRecords)
                {
                    await _context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
                    return;
                }

                // Obtém o maior ID atual na tabela.
                var maxId = await GetMaxIdSafelyAsync();
                
                // Verifica se é necessário redefinir (se o próximo valor seria diferente do esperado).
                var currentIdentityValue = await GetCurrentIdentityValueAsync(tableName);
                if (currentIdentityValue.HasValue && currentIdentityValue == maxId + 1) return;

                // Redefine o valor da coluna ID.
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT(@tableName, RESEED, @maxId)", 
                    new SqlParameter("@tableName", tableName), 
                    new SqlParameter("@maxId", maxId));
            }
            catch
            {
                int maxId = await GetMaxIdSafelyAsync();
                _logger.LogError("Erro ao redefinir a coluna ID da tabela {TableName}. MaxId: {MaxId}", tableName, maxId);
            }
        }

        /// <summary>
        /// Obtém o valor atual da coluna identity da tabela History.
        /// </summary>
        /// <param name="tableName">Nome da tabela.</param>
        /// <returns>Valor atual da identity ou null se houver erro.</returns>
        private async Task<int?> GetCurrentIdentityValueAsync(string tableName)
        {
            try
            {
                await using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = $"SELECT IDENT_CURRENT('{tableName}')";
                
                await _context.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();
                
                if (result != null && result != DBNull.Value) return Convert.ToInt32(result);

                return null;
            }
            catch
            {
                _logger.LogError("Erro ao obter o valor atual da coluna ID da tabela {TableName}.", tableName);
                return null;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        /// <summary>
        /// Obtém o maior ID de forma segura, tratando casos onde a tabela está vazia.
        /// </summary>
        /// <returns>Maior ID ou 0 se não houver registros.</returns>
        private async Task<int> GetMaxIdSafelyAsync()
        {
            try
            {
                var hasRecords = await _context.Equipment.AnyAsync();
                if (!hasRecords) return 0;
                        
                var maxId = await _context.Equipment.MaxAsync(e => e.Id);
                return maxId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o maior ID da tabela History.");
                return 0;
            }
        }
    }
}