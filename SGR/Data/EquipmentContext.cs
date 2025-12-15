using Microsoft.EntityFrameworkCore;
using SGR.Models;

namespace SGR.Data;

/// <summary>
/// Esta classe é responsável por criar o contexto da base de dados para a entidade "Equipment".
/// Ela herda da classe DbContext do Entity Framework Core, permitindo que a aplicação interaja com a base de dados utilizando objetos.
/// O construtor recebe as opções de configuração do contexto, que são passadas para a classe base.
/// </summary>
/// <param name="option">
/// As opções de configuração do contexto, que incluem informações como a string de conexão e outras configurações relacionadas à base de dados.
/// </param>
public class EquipmentContext(DbContextOptions<EquipmentContext> option) : DbContext(option)
{
   /// <summary>
   /// A propriedade DbSet representa a coleção de equipamentos na base de dados.
   /// Ela permite que a aplicação realize operações de CRUD sobre os registos dos equipamentos.
   /// </summary>
    public DbSet<Equipment> Equipment { get; set; }
}