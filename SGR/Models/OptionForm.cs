namespace SGR.Models;

/// <summary>
/// Classe estática que define as opções pré-definidas para os campos de seleção no formulário de adionar/registar um equipamento.
/// </summary>
public static class OptionForm
{
    /// <summary>
    /// Lista de opções para o campo Estado.
    /// </summary>
    public static readonly IReadOnlyList<string> State =
    [
        "AVARIADO",
        "EM REPARAÇÃO",
        "AGUARDANDO PEÇA",
        "REPARADO",
        "ENTREGUE AO CLIENTE",
    ];

    /// <summary>
    /// Lista de opções para o campo Cliente.
    /// </summary>
    public static readonly IReadOnlyList<string> Customer =
    [
        "RuIIuR CV",
        "Acail SA",
        "Acail Gás",
        "Acail Ferro",
        "Acail Espanha",
        "Acail Açores",
        "Acail Angola",
        "AFG",
        "Ecofiltra",
        "Facal",
        "JMM",
        "MAF",
        "Pretec",
        "Douro Suites",
        "IIB",
        "Enacol"
    ];

    /// <summary>
    /// Lista de opções para o campo Categoria.
    /// </summary>
    public static readonly IReadOnlyList<string> Category =
    [
        "AIO",
        "Torre",
        "Mini Torre",
        "Portátil",
        "Monitor",
        "Impressora",
        "Servidor",
        "Router",
        "Switch",
        "Tablet",
        "Smartphone",
        "NAS",
        "PDA"
    ];

   /// <summary>
   /// Lista de opções para o campo Fabricante.
   /// </summary>
    public static readonly IReadOnlyList<string> Manufacturer =
    [
        "Lenovo",
        "Dell",
        "HP",
        "Samsung",
        "Toshiba"
    ];

    /// <summary>
    /// Lista de opções para o campo "Responsável".
    /// </summary>
    public static readonly IReadOnlyList<string> Responsible =
    [
        "Daniel Marques",
        "Rúben Marques",
        "Miguel Lemos"
    ];
}