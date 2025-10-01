using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGR.Data;
using SGR.Model;
using System.Data.Common;

namespace SGR.Service
{
    /// <summary>
    /// Serviço que gerencia operações CRUD para equipamentos.
    /// </summary>
    public class EquipmentOperation(EquipmentContext context, ILogger<EquipmentOperation> logger)
    {
        private readonly EquipmentContext _context = context;
        private readonly ILogger<EquipmentOperation> _logger = logger;

        /// <summary>
        /// Obtém assincronamente uma lista de todos os equipamentos da base de dados.
        /// </summary>
        /// <remarks>Este método obtém todos os registos de equipamentos da fonte de dados. Se ocorrer
        /// um erro durante a operação, é devolvida uma lista vazia e o erro é registado.</remarks>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém uma lista de  
        /// <see cref="Equipment"/> objetos. Se não forem encontrados equipamentos, a lista estará vazia.</returns>
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
        /// Adiciona assincronamente uma nova entidade de equipamento à base de dados.
        /// </summary>
        /// <remarks>Se ocorrer um erro durante a operação, o método regista o erro e devolve a
        /// instância original do <paramref name="equipment"/> sem quaisquer alterações.</remarks>
        /// <param name="equipment">A entidade <see cref="Equipment"/> a ser adicionada. Este parâmetro não pode ser nulo.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a entidade 
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
        /// Procura assincronamente uma entidade de equipamento pelo seu identificador único (ID).
        /// </summary>
        /// <remarks>Este método executa uma consulta à base de dados para obter a entidade de equipamento com o
        /// ID especificado. A consulta é executada sem rastreamento, o que significa que a entidade devolvida não é rastreada pelo
        /// contexto. Se ocorrer um erro durante a operação, o método regista o erro e devolve <see langword="null"/>.</remarks>
        /// <param name="id">O identificador único do equipamento a procurar.</param>
        /// <returns>A entidade <see cref="Equipment"/> se encontrada, caso contrário, <see langword="null"/>.</returns>
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
        /// <remarks>Se a entidade especificada já estiver a ser rastreada pelo contexto, os seus valores são
        /// atualizados. Caso contrário, a entidade é marcada para atualização na base de dados. As alterações são persistidas na base de dados
        /// aquando da chamada deste método.</remarks>
        /// <param name="equipment">A entidade <see cref="Equipment"/> a atualizar. A entidade deve ter um identificador válido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a entidade <see cref="Equipment"/> atualizada.</returns>
        public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                // Verifica se a entidade já está sendo rastreada pelo contexto.
                EntityEntry? trackedEntity = _context.ChangeTracker.Entries<Equipment>()
                    .FirstOrDefault(e => e.Entity.Id == equipment.Id);

                // Se estiver sendo rastreada, atualiza os valores, caso contrário, atualiza os valores.
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
        /// <remarks>Este método tenta remover o equipamento com o identificador fornecido da
        /// base de dados. Se o equipamento não for encontrado, o método devolve <see langword="false"/> sem efetuar
        /// quaisquer alterações. Em caso de erro durante a operação, o método regista o erro e também devolve <see langword="false"/>.</remarks>
        /// <param name="equipment">O equipamento a ser removido.</param>
        /// <returns>Devolve <see langword="true"/> se o equipamento foi removido com sucesso, caso contrário, <see langword="false"/>.
        /// Devolve <see langword="false"/> se o equipamento com o identificador especificado não existir.</returns>
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
        /// Filtra a lista de equipamentos com base no termo de pesquisa.
        /// </summary>
        /// <remarks>
        /// A função verifica se o termo de pesquisa é nulo, vazio ou composto apenas por espaços em branco.
        /// </remarks>
        /// <param name="searchTerm">
        /// O termo a ser pesquisado dentro do cliente, categoria ou número de série do equipamento. Se for nulo, vazio ou
        /// composto apenas por espaços em branco, a lista original <paramref name="equipmentList"/> é retornada sem filtro.
        /// <param name="equipmentList"> A lista de equipamentos a ser filtrada. 
        /// Cada item de equipamento é verificado em relação ao termo de pesquisa. </param>
        /// <returns>
        /// A lista filtrada de equipamentos onde o cliente, categoria ou número de série contém o termo de pesquisa especificado.
        /// Se o termo de pesquisa for nulo, vazio ou composto por espaços em branco, o método retorna a lista original.
        public List<Equipment> FilterEquipment(string searchTerm, List<Equipment> equipmentList)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return equipmentList;

            return [.. equipmentList.Where(equipment =>
                equipment.Customer?.Contains(searchTerm) == true ||
                equipment.Category?.Contains(searchTerm) == true ||
                equipment.SerialNumber?.Contains(searchTerm) == true
            )];
        }

        /// <summary>
        /// Redefine a sequência de IDs da tabela EquipmentContext para continuar a partir do maior ID existente.
        /// </summary>
        /// <returns>True se a operação foi bem-sucedida, False caso contrário.</returns>
        private async Task ReseedIdentityColumnAsync()
        {
            const string tableName = "Equipment";

            try
            {
                // Verifica se há registros na tabela antes de tentar redefinir.
                bool hasRecords = await _context.Equipment.AnyAsync();
                if (!hasRecords)
                {
                    await _context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
                    return;
                }

                // Obtém o maior ID atual na tabela.
                int maxId = await GetMaxIdSafelyAsync();

                // Verifica se é necessário redefinir (se o próximo valor seria diferente do esperado).
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
        /// Obtém o valor atual da coluna Id da tabela EquipmentContext.
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
        /// Obtém o maior ID de forma segura, tratando casos onde a tabela está vazia.
        /// </summary>
        /// <returns>Maior ID ou 0 se não houver registros.</returns>
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