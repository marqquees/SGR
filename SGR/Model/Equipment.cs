using System.ComponentModel.DataAnnotations;

namespace SGR.Model
{
    /// <summary>
    /// Entidade de domínio que representa um equipamento no sistema de reparação.
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
        public string? State { get; set; }
        public string? Customer { get; set; }
        public string? User { get; set; }
        public string? Category { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Processor { get; set; }
        public string? MemoryRAM { get; set; }
        public string? Storage { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Note { get; set; }
        public string? Damage { get; set; }
        public string? Responsible { get; set; }
    }
}