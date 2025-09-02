namespace SGR.Helper
{
    /// <summary>
    /// Classe respons�vel por gerenciar o ciclo de vida dos equipamentos baseado no estado.
    /// Implementa regras de transi��o de estado e valida��es para o fluxo de trabalho de repara��o.
    /// </summary>
    public static class EquipmentLifecycle
    {
        /// <summary>
        /// Dicion�rio que define as transi��es v�lidas de estado para cada estado atual.
        /// Chave: Estado atual.
        /// Valor: Lista de estados para os quais � poss�vel transitar.
        /// </summary>
        private static readonly Dictionary<string, List<string>> StateTransitions = new()
        {
            {
                "AVARIADO",
                new List<string> { "EM REPARA��O", "AGUARDANDO PE�A" }
            },
            {
                "EM REPARA��O",
                new List<string> { "REPARADO", "AGUARDANDO PE�A", "AVARIADO" }
            },
            {
                "AGUARDANDO PE�A",
                new List<string> { "EM REPARA��O", "AVARIADO" }
            },
            {
                "REPARADO",
                new List<string> { "ENTREGUE AO CLIENTE", "EM REPARA��O" }
            },
            {
                "ENTREGUE AO CLIENTE",
                // Permite reabrir caso necess�rio.
                new List<string> { "AVARIADO" }
            }
        };

        /// <summary>
        /// Ordem cronol�gica t�pica dos estados para exibi��o de progresso no workflow.
        /// </summary>
        private static readonly List<string> TypicalProgressOrder =
        [
            "AVARIADO",
            "EM REPARA��O",
            "REPARADO",
            "ENTREGUE AO CLIENTE"
        ];

        /// <summary>
        /// Verifica se uma transi��o de estado � v�lida.
        /// </summary>
        /// <param name="currentState">Estado atual do equipamento.</param>
        /// <param name="newState">Novo estado desejado.</param>
        /// <returns>True se a transi��o for v�lida, False caso contr�rio.</returns>
        public static bool IsValidTransition(string currentState, string newState)
        {
            if (string.IsNullOrWhiteSpace(currentState) || string.IsNullOrWhiteSpace(newState))
                return false;

            // Normaliza os estados para compara��o.
            var normalizedCurrentState = currentState.Trim().ToUpperInvariant();
            var normalizedNewState = newState.Trim().ToUpperInvariant();

            // Permite manter o mesmo estado.
            if (normalizedCurrentState == normalizedNewState)
                return true;

            var validStates = GetValidNextStates(normalizedCurrentState);
            return validStates.Contains(normalizedNewState);
        }

        /// <summary>
        /// Obt�m a lista de estados v�lidos para transi��o a partir do estado atual.
        /// </summary>
        /// <param name="currentState">Estado atual do equipamento.</param>
        /// <returns>Lista de estados para os quais � poss�vel transitar.</returns>
        public static List<string> GetValidNextStates(string currentState)
        {
            if (string.IsNullOrWhiteSpace(currentState))
                return [];

            var normalizedCurrentState = currentState.Trim().ToUpperInvariant();
            return StateTransitions.TryGetValue(normalizedCurrentState, out var validStates)
                ? [.. validStates]
                : [];
        }

        /// <summary>
        /// Calcula o progresso do equipamento baseado no estado atual para o workflow.
        /// </summary>
        /// <param name="state">Estado atual do equipamento.</param>
        /// <returns>Percentual de progresso (0 - 100).</returns>
        public static int GetProgressPercentage(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return 0;

            var normalizedState = state.Trim().ToUpperInvariant();
            var index = TypicalProgressOrder.IndexOf(normalizedState);

            if (index == -1)
            {
                // Estados especiais como "AGUARDANDO PE�A" t�m progresso baseado em "EM REPARA��O".
                if (normalizedState == "AGUARDANDO PE�A")
                    index = TypicalProgressOrder.IndexOf("EM REPARA��O");
                else
                    return 0;
            }

            // Calcula percentual baseado na posi��o na ordem t�pica.
            return (int)Math.Round((double)(index + 1) / TypicalProgressOrder.Count * 100);
        }

        /// <summary>
        /// Obt�m a classe CSS completa para o badge do estado.
        /// </summary>
        /// <param name="state">Estado do equipamento.</param>
        /// <returns>Classe CSS completa para o badge.</returns>
        public static string GetStateBadgeClass(string state)
        {
            return HistoryUi.GetStateBadgeClass(state);
        }

        /// <summary>
        /// Obt�m o �ndice da fase atual no workflow para c�lculos de progresso.
        /// </summary>
        /// <param name="state">Estado atual do equipamento.</param>
        /// <returns>�ndice da fase (-1 se n�o encontrado).</returns>
        public static int GetPhaseIndex(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return -1;

            var normalizedState = state.Trim().ToUpperInvariant();
            var index = TypicalProgressOrder.IndexOf(normalizedState);

            // Se for "AGUARDANDO PE�A", retorna o �ndice de "EM REPARA��O".
            if (index == -1 && normalizedState == "AGUARDANDO PE�A")
                index = TypicalProgressOrder.IndexOf("EM REPARA��O");

            return index;
        }

        /// <summary>
        /// Verifica se um estado representa uma fase conclu�da em rela��o ao estado atual.
        /// </summary>
        /// <param name="currentState">Estado atual do equipamento.</param>
        /// <param name="phaseState">Estado da fase a verificar.</param>
        /// <returns>True se a fase foi conclu�da.</returns>
        public static bool IsPhaseCompleted(string currentState, string phaseState)
        {
            var currentIndex = GetPhaseIndex(currentState);
            var phaseIndex = GetPhaseIndex(phaseState);

            return currentIndex > phaseIndex && currentIndex != -1 && phaseIndex != -1;
        }

        /// <summary>
        /// Verifica se um estado representa a fase ativa atual.
        /// </summary>
        /// <param name="currentState">Estado atual do equipamento.</param>
        /// <param name="phaseState">Estado da fase a verificar.</param>
        /// <returns>True se a fase est� ativa.</returns>
        public static bool IsPhaseActive(string currentState, string phaseState)
        {
            if (string.IsNullOrWhiteSpace(currentState) || string.IsNullOrWhiteSpace(phaseState))
                return false;

            var normalizedCurrentState = currentState.Trim().ToUpperInvariant();
            var normalizedPhaseState = phaseState.Trim().ToUpperInvariant();

            // Caso especial: "AGUARDANDO PE�A" � considerado ativo na fase "EM REPARA��O".
            if (normalizedCurrentState == "AGUARDANDO PE�A" && normalizedPhaseState == "EM REPARA��O")
                return true;

            return normalizedCurrentState == normalizedPhaseState;
        }
    }
}