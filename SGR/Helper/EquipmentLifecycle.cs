namespace SGR.Helper;

/// <summary>
///     Classe responsável por gerenciar o ciclo de vida dos equipamentos baseado no estado.
///     Implementa regras de transição de estado e validações para o fluxo de trabalho de reparação.
/// </summary>
public static class EquipmentLifecycle
{
    /// <summary>
    ///     Dicionário que define as transições válidas de estado para cada estado atual.
    ///     Chave: Estado atual.
    ///     Valor: Lista de estados para os quais é possível transitar.
    /// </summary>
    private static readonly Dictionary<string, List<string>> StateTransitions = new()
    {
        {
            "AVARIADO",
            new List<string> { "EM REPARAÇÃO", "AGUARDANDO PEÇA" }
        },
        {
            "EM REPARAÇÃO",
            new List<string> { "REPARADO", "AGUARDANDO PEÇA", "AVARIADO" }
        },
        {
            "AGUARDANDO PEÇA",
            new List<string> { "EM REPARAÇÃO", "AVARIADO" }
        },
        {
            "REPARADO",
            new List<string> { "ENTREGUE AO CLIENTE", "EM REPARAÇÃO" }
        },
        {
            "ENTREGUE AO CLIENTE",
            new List<string> { "AVARIADO" }
        }
    };

    /// <summary>
    ///     Ordem cronológica típica dos estados para exibição de progresso no workflow.
    /// </summary>
    private static readonly List<string> TypicalProgressOrder =
    [
        "AVARIADO",
        "EM REPARAÇÃO",
        "REPARADO",
        "ENTREGUE AO CLIENTE"
    ];

    /// <summary>
    ///     Verifica se uma transição de estado é válida.
    /// </summary>
    /// <param name="currentState">Estado atual do equipamento.</param>
    /// <param name="newState">Novo estado desejado.</param>
    /// <returns>True se a transição for válida, false caso contrário.</returns>
    public static bool IsValidTransition(string currentState, string newState)
    {
        if (string.IsNullOrWhiteSpace(currentState) || string.IsNullOrWhiteSpace(newState))
            return false;

        // Normaliza os estados para comparação.
        var normalizedCurrentState = currentState.Trim().ToUpperInvariant();
        var normalizedNewState = newState.Trim().ToUpperInvariant();

        // Permite manter o mesmo estado.
        if (normalizedCurrentState == normalizedNewState)
            return true;

        var validStates = GetValidNextStates(normalizedCurrentState);
        return validStates.Contains(normalizedNewState);
    }

    /// <summary>
    ///     Obtém a lista de estados válidos para transição a partir do estado atual.
    /// </summary>
    /// <param name="currentState">Estado atual do equipamento.</param>
    /// <returns>Lista de estados para os quais é possível transitar.</returns>
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
    ///     Calcula o progresso do equipamento baseado no estado atual para o workflow.
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
            // Estados especiais como "AGUARDANDO PEÇA" têm progresso baseado em "EM REPARAÇÃO".
            if (normalizedState == "AGUARDANDO PEÇA")
                index = TypicalProgressOrder.IndexOf("EM REPARAÇÃO");
            else
                return 0;
        }

        // Calcula percentual baseado na posição na ordem típica.
        return (int)Math.Round((double)(index + 1) / TypicalProgressOrder.Count * 100);
    }

    /// <summary>
    ///     Obtém a classe CSS completa para o badge do estado.
    /// </summary>
    /// <param name="state">Estado do equipamento.</param>
    /// <returns>Classe CSS completa para o badge.</returns>
    public static string GetStateBadgeClass(string state)
    {
        return EquipmentUi.GetStateBadgeClass(state);
    }

    /// <summary>
    ///     Obtém o índice da fase atual no workflow para cálculos de progresso.
    /// </summary>
    /// <param name="state">Estado atual do equipamento.</param>
    /// <returns>Índice da fase (-1 se não encontrado).</returns>
    public static int GetPhaseIndex(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return -1;

        var normalizedState = state.Trim().ToUpperInvariant();
        var index = TypicalProgressOrder.IndexOf(normalizedState);

        // Se for "AGUARDANDO PEÇA", retorna o índice de "EM REPARAÇÃO".
        if (index == -1 && normalizedState == "AGUARDANDO PEÇA")
            index = TypicalProgressOrder.IndexOf("EM REPARAÇÃO");

        return index;
    }

    /// <summary>
    ///     Verifica se um estado representa uma fase concluída em relação ao estado atual.
    /// </summary>
    /// <param name="currentState">Estado atual do equipamento.</param>
    /// <param name="phaseState">Estado da fase a verificar.</param>
    /// <returns>True se a fase foi concluída.</returns>
    public static bool IsPhaseCompleted(string currentState, string phaseState)
    {
        var currentIndex = GetPhaseIndex(currentState);
        var phaseIndex = GetPhaseIndex(phaseState);

        return currentIndex > phaseIndex && currentIndex != -1 && phaseIndex != -1;
    }

    /// <summary>
    ///     Verifica se um estado representa a fase ativa atual.
    /// </summary>
    /// <param name="currentState">Estado atual do equipamento.</param>
    /// <param name="phaseState">Estado da fase a verificar.</param>
    /// <returns>True se a fase está ativa.</returns>
    public static bool IsPhaseActive(string currentState, string phaseState)
    {
        if (string.IsNullOrWhiteSpace(currentState) || string.IsNullOrWhiteSpace(phaseState))
            return false;

        var normalizedCurrentState = currentState.Trim().ToUpperInvariant();
        var normalizedPhaseState = phaseState.Trim().ToUpperInvariant();

        // Caso especial: "AGUARDANDO PEÇA" é considerado ativo na fase "EM REPARAÇÃO".
        if (normalizedCurrentState == "AGUARDANDO PEÇA" && normalizedPhaseState == "EM REPARAÇÃO")
            return true;

        return normalizedCurrentState == normalizedPhaseState;
    }
}