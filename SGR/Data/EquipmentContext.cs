using Microsoft.EntityFrameworkCore;
using SGR.Models;

namespace SGR.Data
{
    /// <summary>
    /// Contexto de banco de dados para gerenciar entidades do histórico de equipamentos.
    /// Responsável pela conexão e interação com o banco de dados SQL Server.
    /// </summary>
    /// <remarks>
    /// Este contexto é configurado no startup da aplicação com a string de conexão adequada.
    /// </remarks>
    /// <param name="options">Opções de configuração para o contexto do banco de dados.</param>
    public class EquipmentContext(DbContextOptions<EquipmentContext> options) : DbContext(options)
    {
        /// <summary>
        /// Conjunto de dados que representa a tabela de histórico de equipamentos no banco de dados.
        /// </summary>
        public DbSet<Equipment> Equipment { get; set; }
    }
}
