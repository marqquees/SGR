using Microsoft.EntityFrameworkCore;
using SGR.Model;

namespace SGR.Data
{
    /// <summary>
    /// Representa o contexto da base de dados para gerir dados de equipamentos.
    /// </summary>
    /// <remarks>Este contexto é utilizado para interagir com a tabela da base de dados que contém equipamentos registados. 
    /// Fornece acesso ao <see cref="DbSet{TEntity}"/> de entidades <see cref="Equipment"/>, 
    /// permitindo operações CRUD e consultas LINQ.</remarks>
    /// <param name="options">As opções de configuração para o contexto da base de dados.</param>
    public class EquipmentContext(DbContextOptions<EquipmentContext> options) : DbContext(options)
    {
        /// <summary>
        /// Conjunto de dados que representa a tabela de equipamentos.
        /// </summary>
        public DbSet<Equipment> Equipment { get; set; }
    }
}
