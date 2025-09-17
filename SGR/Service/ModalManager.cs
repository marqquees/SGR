using SGR.Model;

namespace SGR.Service
{
    /// <summary>
    /// Gerenciador de modais responsável por controlar o estado e as operações das janelas modais na interface do usuário.
    /// </summary>
    /// <remarks>
    /// Esta classe fornece funcionalidades para:
    /// <list type="bullet">
    /// <item><description>Confirmar/cancelar exclusão de equipamentos.</description></item>
    /// <item><description>Gerenciar o estado dos modais e notificar a interface sobre mudanças.</description></item>
    /// </list>
    /// É um serviço Scoped que mantém o estado durante uma sessão de usuário.
    /// </remarks>
    public class ModalManager
    {
        /// <summary>
        /// Obtém ou define o equipamento selecionado para exclusão.
        /// </summary>
        public Equipment? EquipmentToRemove { get; set; }

        /// <summary>
        /// Indica se o modal de confirmação de exclusão está visível.
        /// </summary>
        public bool ShowRemoveConfirm { get; set; }

        /// <summary>
        /// Evento disparado quando o estado do modal é alterado, permitindo que os componentes UI se atualizem.
        /// </summary>
        public event Action? StateChanged;

        /// <summary>
        /// Exibe o modal de confirmação de exclusão para um equipamento.
        /// </summary>
        /// <param name="equipment">O equipamento a ser excluído.</param>
        public void ConfirmRemove(Equipment equipment)
        {
            EquipmentToRemove = equipment;
            ShowRemoveConfirm = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Cancela o processo de exclusão e fecha o modal de confirmação.
        /// </summary>
        public void CancelRemove()
        {
            EquipmentToRemove = null;
            ShowRemoveConfirm = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// Notifica os componentes UI que o estado do modal foi alterado.
        /// </summary>
        /// <remarks>
        /// Este método invoca o evento StateChanged, permitindo que os componentes
        /// que estão observando este evento possam atualizar sua interface.
        /// </remarks>
        private void NotifyStateChanged() => StateChanged?.Invoke();
    }
}