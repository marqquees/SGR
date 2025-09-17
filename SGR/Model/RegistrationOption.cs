namespace SGR.Model
{
    /// <summary>
    /// Fornece opções predefinidas para processos de registro, incluindo estados, clientes, categorias, marcas e
    /// técnicos responsáveis.
    /// </summary>
    /// <remarks>Esta classe contém coleções estáticas somente leitura que representam várias opções predefinidas
    /// comumente usadas em fluxos de trabalho de registro e reparo de equipamentos. Essas opções incluem possíveis estados de
    /// equipamentos, nomes de clientes, categorias de equipamentos, marcas e técnicos responsáveis.</remarks>
    public static class RegistrationOption
    {
        /// <summary>
        /// Estados possíveis de um equipamento no processo de reparação.
        /// </summary>
        public static readonly IReadOnlyList<string> State =
        [
            "AVARIADO",
            "EM REPARAÇÃO",
            "AGUARDANDO PEÇA",
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
        /// Técnicos responsáveis.
        /// </summary>
        public static readonly IReadOnlyList<string> Responsible =
        [
            "Daniel Marques",
            "Rúben Marques",
            "Miguel Lemos"
        ];
    }
}