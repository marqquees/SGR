using System.ComponentModel.DataAnnotations;

namespace SGR.Models
{
    /// <summary>
    /// Entidade de domínio que representa um equipamento no sistema de reparação.
    /// Mapeada diretamente para a tabela History no banco de dados.
    /// </summary>
    /// <remarks>
    /// Esta classe define a estrutura de dados para armazenamento persistente de informações
    /// sobre equipamentos registrados no sistema, incluindo suas características técnicas
    /// e estado atual de reparação.
    /// </remarks>
    public class Equipment
    {
        [Key]
        public int Id { get; set; }
        public DateOnly DateRegister { get; set; }
        [MaxLength(20), Required(ErrorMessage = "Este campo é obrigatório.")]
        public string State { get; set; } = string.Empty;
        [MaxLength(50), Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Customer { get; set; }
        [MaxLength(50)]
        public string? User { get; set; }
        [MaxLength(20), Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Category { get; set; } = string.Empty;
        [MaxLength(30), Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Brand { get; set; } = string.Empty;
        [StringLength(30, ErrorMessage = "Este campo não pode exceder 30 caracteres.")]
        public string? Model { get; set; }
        [StringLength(20, ErrorMessage = "Este campo não pode exceder 20 caracteres.")]
        public string? SerialNumber { get; set; }
        [MaxLength(30)]
        public string? Processor { get; set; }
        [MaxLength(15)]
        public string? MemoryRAM { get; set; }
        [MaxLength(15)]
        public string? Storage { get; set; }
        [MaxLength(20)]
        public string? OperatingSystem { get; set; }
        [StringLength(500, ErrorMessage = "Este campo não pode exceder 500 caracteres.")]
        public string? Note { get; set; }
        [MaxLength(30), Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Responsible { get; set; }
    }
}