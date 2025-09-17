using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGR.Data;
using SGR.Model;
using System.Data.Common;

namespace SGR.Service
{
    /// <summary>
    /// Servi�o que gerencia opera��es CRUD para equipamentos.
    /// </summary>
    public class EquipmentOperation(EquipmentContext context, ILogger<EquipmentOperation> logger)
    {
        private readonly EquipmentContext _context = context;
        private readonly ILogger<EquipmentOperation> _logger = logger;

        /// <summary>
        /// Obt�m assincronamente uma lista de todos os equipamentos da base de dados.
        /// </summary>
        /// <remarks>Este m�todo obt�m todos os registos de equipamentos da fonte de dados. Se ocorrer
        /// um erro durante a opera��o, � devolvida uma lista vazia e o erro � registado.</remarks>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado da tarefa cont�m uma lista de  
        /// <see cref="Equipment"/> objetos. Se n�o forem encontrados equipamentos, a lista estar� vazia.</returns>
        public async Task<List<Equipment>> ListEquipmentAsync()
        {
            try
            {
                return await _context.Equipment.ToListAsync();
            }
            catch (DbException error)
            {
                _logger.LogError(error, "Erro ao listar os equipamentos. " +
                                        "Detalhes: {ErrorMessage}", error.Message);
                return [];
            }
        }

        /// <summary>
        /// Adiciona assincronamente uma nova entidade de equipamento � base de dados.
        /// </summary>
        /// <remarks>Se ocorrer um erro durante a opera��o, o m�todo regista o erro e devolve a
        /// inst�ncia original do <paramref name="equipment"/> sem quaisquer altera��es.</remarks>
        /// <param name="equipment">A entidade <see cref="Equipment"/> a ser adicionada. Este par�metro n�o pode ser nulo.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado da tarefa cont�m a entidade 
        /// <see cref="Equipment"/> adicionada com os seus valores gerados pela base de dados preenchidos.</returns>
        public async Task<Equipment> AddEquipmentAsync(Equipment equipment)
        {
            try
            {
                EntityEntry<Equipment> r = await _context.Equipment.AddAsync(equipment);
                await _context.SaveChangesAsync();

                return r.Entity;
            }
            catch (DbException error)
            {
                _logger.LogError(error, "Erro ao adicionar o equipamento {EquipmentId}. " +
                    "Detalhes: {ErrorMessage}", equipment.Id, error.Message);
                return equipment;
            }
        }

        /// <summary>
        /// Procura assincronamente uma entidade de equipamento pelo seu identificador �nico (ID).
        /// </summary>
        /// <remarks>Este m�todo executa uma consulta � base de dados para obter a entidade de equipamento com o
        /// ID especificado. A consulta � executada sem rastreamento, o que significa que a entidade devolvida n�o � rastreada pelo
        /// contexto. Se ocorrer um erro durante a opera��o, o m�todo regista o erro e devolve <see langword="null"/>.</remarks>
        /// <param name="id">O identificador �nico do equipamento a procurar.</param>
        /// <returns>A entidade <see cref="Equipment"/> se encontrada, caso contr�rio, <see langword="null"/>.</returns>
        public async Task<Equipment?> SearchByIdAsync(int id)
        {
            try
            {
                return await _context.Equipment.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (DbException error)
            {
                _logger.LogError(error, "Erro ao pesquisar o equipamento com ID {EquipmentId}. " +
                    "Detalhes: {ErrorMessage}", id, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Atualiza os dados do equipamento na base de dados.
        /// </summary>
        /// <remarks>Se a entidade especificada j� estiver a ser rastreada pelo contexto, os seus valores s�o
        /// atualizados. Caso contr�rio, a entidade � marcada para atualiza��o na base de dados. As altera��es s�o persistidas na base de dados
        /// aquando da chamada deste m�todo.</remarks>
        /// <param name="equipment">A entidade <see cref="Equipment"/> a atualizar. A entidade deve ter um identificador v�lido.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado da tarefa cont�m a entidade <see cref="Equipment"/> atualizada.</returns>
        public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                // Verifica se a entidade j� est� sendo rastreada pelo contexto.
                EntityEntry? trackedEntity = _context.ChangeTracker.Entries<Equipment>()
                    .FirstOrDefault(e => e.Entity.Id == equipment.Id);

                // Se estiver sendo rastreada, atualiza os valores, caso contr�rio, atualiza os valores.
                if (trackedEntity != null)
                    _context.Entry(trackedEntity.Entity).CurrentValues.SetValues(equipment);
                else
                    _context.Equipment.Update(equipment);

                await _context.SaveChangesAsync();

                return equipment;
            }
            catch (DbException error)
            {
                _logger.LogError(error, "Erro ao atualizar o equipamento {EquipmentCategory}. " +
                    "Detalhes: {ErrorMessage}", equipment.Category, error.Message);

                return equipment;
            }
        }

        /// <summary>
        /// Remove o equipamento com o identificador especificado da base de dados.
        /// </summary>
        /// <remarks>Este m�todo tenta remover o equipamento com o identificador fornecido da
        /// base de dados. Se o equipamento n�o for encontrado, o m�todo devolve <see langword="false"/> sem efetuar
        /// quaisquer altera��es. Em caso de erro durante a opera��o, o m�todo regista o erro e tamb�m devolve <see langword="false"/>.</remarks>
        /// <param name="equipment">O equipamento a ser removido.</param>
        /// <returns>Devolve <see langword="true"/> se o equipamento foi removido com sucesso, caso contr�rio, <see langword="false"/>.
        /// Devolve <see langword="false"/> se o equipamento com o identificador especificado n�o existir.</returns>
        public async Task<bool> RemoveEquipmentAsync(Equipment equipment)
        {
            try
            {
                Equipment? equipmentToRemove = await _context.Equipment.FindAsync(equipment.Id);

                if (equipmentToRemove == null)
                    return false;

                _context.Equipment.Remove(equipmentToRemove);
                await _context.SaveChangesAsync();
                await ReseedIdentityColumnAsync();

                return true;
            }
            catch (DbException error)
            {
                _logger.LogError(error, "Erro ao remover o equipamento com ID {EquipmentId}. " +
                    "Detalhes: {ErrorMessage}", equipment, error.Message);
                return false;
            }
        }

        /// <summary>
        /// Filtra os equipamentos baseado no termo de pesquisa.
        /// </summary>
        public List<Equipment> FilterEquipmentList(string searchTerm, List<Equipment> equipmentList)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return equipmentList;

            searchTerm = searchTerm.ToLower();
            return [.. equipmentList.Where(i =>
                i.Id.ToString().Contains(searchTerm) ||
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
        /// Redefine a sequ�ncia de IDs da tabela EquipmentContext para continuar a partir do maior ID existente.
        /// </summary>
        /// <returns>True se a opera��o foi bem-sucedida, False caso contr�rio.</returns>
        private async Task ReseedIdentityColumnAsync()
        {
            const string tableName = "Equipment";

            try
            {
                // Verifica se h� registros na tabela antes de tentar redefinir.
                bool hasRecords = await _context.Equipment.AnyAsync();
                if (!hasRecords)
                {
                    await _context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
                    return;
                }

                // Obt�m o maior ID atual na tabela.
                int maxId = await GetMaxIdSafelyAsync();

                // Verifica se � necess�rio redefinir (se o pr�ximo valor seria diferente do esperado).
                Nullable<int> currentIdentityValue = await GetCurrentIdentityValueAsync(tableName);
                if (currentIdentityValue.HasValue && currentIdentityValue == maxId + 1) return;

                // Redefine o valor da coluna ID.
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT(@tableName, RESEED, @maxId)",
                    new SqlParameter("@tableName", tableName),
                    new SqlParameter("@maxId", maxId));
            }
            catch (Exception error)
            {
                int maxId = await GetMaxIdSafelyAsync();
                _logger.LogError(error, "Erro ao redefinir a coluna ID da tabela {TableName}. MaxId: {MaxId}", tableName, maxId);
            }
        }

        /// <summary>
        /// Obt�m o valor atual da coluna Id da tabela EquipmentContext.
        /// </summary>
        /// <param name="tableName">Nome da tabela.</param>
        /// <returns>Valor atual do Id ou nulo se houver erro.</returns>
        private async Task<int?> GetCurrentIdentityValueAsync(string tableName)
        {
            try
            {
                await using DbCommand command = _context.Database.GetDbConnection().CreateCommand();
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
        /// Obt�m o maior ID de forma segura, tratando casos onde a tabela est� vazia.
        /// </summary>
        /// <returns>Maior ID ou 0 se n�o houver registros.</returns>
        private async Task<int> GetMaxIdSafelyAsync()
        {
            try
            {
                bool hasRecords = await _context.Equipment.AnyAsync();
                if (!hasRecords) return 0;

                int maxId = await _context.Equipment.MaxAsync(e => e.Id);
                return maxId;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Erro ao obter o maior ID da tabela Equipment.");
                return 0;
            }
        }
    }
}