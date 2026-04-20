using System.ComponentModel.DataAnnotations;

namespace SGR.Models;

/// <summary>
/// Classe que representa um equipamento registado no sistema, contendo informações detalhadas sobre o equipamento,
/// seu estado, cliente associado, e outros atributos relevantes para o processo de registo e acompanhamento de reparações.
/// </summary>
public class Equipment
{
    [Key] 
    public int Id { get; init; }

    [Required(ErrorMessage = "Este campo é obrigatório.")]
    public DateOnly DateRegister { get; set; }
    
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(20)]
    public string? State { get; set; }
    
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(20)]
    public string? Customer { get; set; }
    
    [StringLength(100)]
    public string? User { get; set; }
    
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(15)]
    public string? Category { get; set; }
    
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(10)]
    public string? Manufacturer { get; set; }
    
    [StringLength(50)]
    public string? Model { get; set; }
    
    [StringLength(50)]
    public string? SerialNumber { get; set; }
    
    [StringLength(50)]
    public string? Processor { get; set; }
    
    [StringLength(10)]
    public string? MemoryRAM { get; set; }
    
    [StringLength(10)]
    public string? Storage { get; set; }
    
    [StringLength(30)]
    public string? OperatingSystem { get; set; }
    
    [StringLength(300, ErrorMessage = "Este campo deve conter no máximo 300 caracteres.")]
    public string? Note { get; set; }
    
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [MaxLength(500, ErrorMessage = "Este campo deve conter no máximo 500 caracteres.")]
    public string? Damage { get; set; }
    
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(20)]
    public string? Responsible { get; set; }
}