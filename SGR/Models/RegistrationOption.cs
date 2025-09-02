namespace SGR.Models
{
    /// <summary>
    /// Classe est�tica que cont�m op��es pr�-definidas para os diferentes campos do invent�rio.
    /// Usado para popular menus suspensos e valida��o de dados.
    /// </summary>
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
        /// Lista de clientes dispon�veis no sistema.
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
        /// T�cnicos respons�veis dispon�veis.
        /// </summary>
        public static readonly IReadOnlyList<string> Responsible =
        [
            "Daniel Marques",
            "R�ben Marques",
            "Miguel Lemos"
        ];
    }
}