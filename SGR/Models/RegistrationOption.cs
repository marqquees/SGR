namespace SGR.Models
{
    /// <summary>
    /// Classe estática que contém opções pré-definidas para os diferentes campos do inventário.
    /// Usado para popular menus suspensos e validação de dados.
    /// </summary>
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
        /// Lista de clientes disponíveis no sistema.
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
        /// Categorias de equipamentos suportadas pelo sistema.
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
        /// Marcas de equipamentos suportadas pelo sistema.
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
        /// Técnicos responsáveis disponíveis.
        /// </summary>
        public static readonly IReadOnlyList<string> Responsible =
        [
            "Daniel Marques",
            "Rúben Marques",
            "Miguel Lemos"
        ];
    }
}