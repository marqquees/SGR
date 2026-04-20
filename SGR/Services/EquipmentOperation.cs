using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGR.Data;
using SGR.Models;

namespace SGR.Services;

public class EquipmentOperation(EquipmentContext context, ILogger<EquipmentOperation> logger)
{
    /// <summary>
    /// Carrega a lista de equipamentos.
    /// </summary>
    /// <returns>
    /// Uma lista de equipamentos ou uma lista vazia se ocorrer algum erro durante o carregamento dos dados.
    /// </returns>
    public async Task<List<Equipment>> ListEquipmentAsync()
    {
        try
        {
            return await context.Equipment.ToListAsync();
        }
        catch (Exception error)
        {
            logger.LogError(error.Message, "Erro ao carregar a lista dos equipamentos. ");
            throw;
        }
    }
    
    /// <summary>
    /// Adiciona um novo equipamento.
    /// </summary>
    /// <param name="equipment">
    /// O equipamento a ser adicionado.
    /// </param>
    /// <returns>
    /// O equipamento adicionado ou lança uma exceção se o número de série já existir ou se ocorrer algum erro durante a adição do equipamento.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando o número de série do equipamento já existe.
    /// </exception>
    public async Task<Equipment> AddEquipmentAsync(Equipment equipment)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(equipment.SerialNumber))
            {
                bool serialNumberExists = await context.Equipment.AnyAsync(e => e.SerialNumber == equipment.SerialNumber);
                if (serialNumberExists)
                    throw new InvalidOperationException($"O número de série {equipment.SerialNumber} já existe.");
            }
            
            EntityEntry<Equipment> e = await context.Equipment.AddAsync(equipment);
            await context.SaveChangesAsync();
            return e.Entity;
        }
        catch (Exception error)
        {
            logger.LogError(error.Message, "Erro ao adicionar o equipamento.");
            throw;
        }
    }

    /// <summary>
    /// Atualiza os dados de um equipamento existente.
    /// </summary>
    /// <param name="equipment">
    /// O equipamento com os dados atualizados.
    /// O equipamento deve conter um ID válido para que a atualização seja realizada corretamente.
    /// </param>
    /// <returns>
    /// O equipamento atualizado ou lança uma exceção se ocorrer algum erro durante a atualização do equipamento.
    /// </returns>
    public async Task<Equipment> UpdateDataEquipmentAsync(Equipment equipment)
    {
        try
        {
            // Verifica se a entidade já está sendo rastreada pelo contexto.
            EntityEntry? trackedEquipment = context.ChangeTracker.Entries<Equipment>()
                .FirstOrDefault(e => e.Entity.Id == equipment.Id);

            // Se a entidade já está sendo rastreada, atualiza os valores.
            if (trackedEquipment != null) context.Entry(trackedEquipment.Entity).CurrentValues.SetValues(equipment);
            else context.Equipment.Update(equipment);

            await context.SaveChangesAsync();
            return equipment;
        }
        catch (Exception error)
        {
            logger.LogError(error.Message, "Erro ao atualizar o equipamento com S/N {SerialNumber}.", equipment.SerialNumber);
            throw;
        }
    }

    /// <summary>
    /// Busca um equipamento pelo seu ID.
    /// </summary>
    /// <param name="idEquipment">
    /// O ID do equipamento a ser buscado.
    /// </param>
    /// <returns>
    /// O equipamento encontrado ou nulo se nenhum equipamento com o ID fornecido for encontrado ou se ocorrer algum erro durante a busca do equipamento.
    /// </returns>
    public async Task<Equipment?> FindEquipmentByIdAsync(int idEquipment)
    {
        try
        {
            return await context.Equipment.AsNoTracking().FirstOrDefaultAsync(e => e.Id == idEquipment);
        }
        catch (Exception error)
        {
            logger.LogError(error.Message, "Erro ao buscar o equipamento com ID {IdEquipment}.", idEquipment);
            throw;
        }
    }
   
    /// <summary>
    /// Remove um equipamento pelo seu ID.
    /// </summary>
    /// <param name="idEquipment">
    /// O ID do equipamento a ser removido.
    /// </param>
    /// <returns>
    /// Verdadeiro se o equipamento foi removido com sucesso,
    /// falso se nenhum equipamento com o ID fornecido for encontrado ou se ocorrer algum erro durante a remoção do equipamento.
    /// </returns>
    public async Task<bool> RemoveEquipmentAsync(int idEquipment)
    {
        try
        {
            // Verifica se o equipamento existe antes de tentar removê-lo.
            Equipment? equipment = await context.Equipment.FindAsync(idEquipment);

            if (equipment is null) return false;

            context.Equipment.Remove(equipment);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception error)
        {
            logger.LogError(error.Message, "Erro ao remover o equipamento com ID {IdEquipment}.", idEquipment);
            throw;
        }
    }
}