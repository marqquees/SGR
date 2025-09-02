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
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? State { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Customer { get; set; }
        
        public string? User { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Category { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Model { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? SerialNumber { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Processor { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? MemoryRAM { get; set; }
       
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Storage { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? OperatingSystem { get; set; }
        
        public string? Note { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string? Responsible { get; set; }
    }
}