namespace SGR.Model
{
    /// <summary>
    /// Fornece op��es predefinidas para processos de registro, incluindo estados, clientes, categorias, marcas e
    /// t�cnicos respons�veis.
    /// </summary>
    /// <remarks>Esta classe cont�m cole��es est�ticas somente leitura que representam v�rias op��es predefinidas
    /// comumente usadas em fluxos de trabalho de registro e reparo de equipamentos. Essas op��es incluem poss�veis estados de
    /// equipamentos, nomes de clientes, categorias de equipamentos, marcas e t�cnicos respons�veis.</remarks>
    public static class RegistrationOption
    {
        /// <summary>
        /// Estados poss�veis de um equipamento no processo de repara��o.
        /// </summary>
        public static readonly IReadOnlyList<string> State =
        [
            "AVARIADO",
            "EM REPARA��O",
            "AGUARDANDO PE�A",
            "REPARADO",
            "ENTREGUE AO CLIENTE"
        ];

        /// <summary>
        /// Lista de clientes.
        /// </summary>
        public static readonly IReadOnlyList<string> Customer =
        [
            "RuIIuR",
            "AFG",
            "ContruMadeira",
            "Acail",
            "Facal",
            "EcoFiltra",
            "JMM",
            "Curvar"
        ];

        /// <summary>
        /// Categorias de equipamentos.
        /// </summary>
        public static readonly IReadOnlyList<string> Category =
        [
            "Torre",
            "Port�til",
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
        /// Marcas de equipamentos.
        /// </summary>
        public static readonly IReadOnlyList<string> Brand =
        [
            "Lenovo",
            "Dell",
            "HP",
            "Samsung",
            "Toshiba"
        ];

        /// <summary>
        /// T�cnicos respons�veis.
        /// </summary>
        public static readonly IReadOnlyList<string> Responsible =
        [
            "Daniel Marques",
            "R�ben Marques",
            "Miguel Lemos"
        ];
    }
}