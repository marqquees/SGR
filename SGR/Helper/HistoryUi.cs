namespace SGR.Helper
{
    /// <summary>
    /// Classe responsável por auxiliar na renderização da interface do Estado do Equipamento.
    /// </summary>
    public static class HistoryUi
    {
        // Mapeamento estático das classes de estilo para cada estado.
        private static readonly Dictionary<string, string> StateBadgeClasses = new()
        {
            { "AVARIADO", "bg-danger text-white" },
            { "EM REPARAÇÃO", "bg-warning text-dark" },
            { "AGUARDANDO PEÇA", "bg-info text-white" },
            { "REPARADO", "bg-success text-white" },
            { "ENTREGUE AO CLIENTE", "bg-primary text-white" }
        };

        /// <summary>
        /// Método responsável por obter a classe CSS para o badge de Estado do Equipamento.
        /// </summary>
        /// <param name="state">O estado do equipamento para o qual obter a classe CSS.</param>
        /// <returns>A classe CSS correspondente ao estado ou classe padrão se não encontrado.</returns>
        public static string GetStateBadgeClass(string state)
        {
            // Verifica se o estado é válido e existe no dicionário.
            if (string.IsNullOrWhiteSpace(state))
                return "bg-secondary text-white";

            // Normaliza o estado removendo espaços extras e convertendo para maiúsculas.
            var normalizedState = state.Trim().ToUpperInvariant();
            
            // Verifica se o estado existe no dicionário.
            if (StateBadgeClasses.TryGetValue(normalizedState, out string? badgeClass))
                return badgeClass;
            
            // Retorna a classe padrão se não encontrar.
            return "bg-secondary text-white";
        }
    }
}
