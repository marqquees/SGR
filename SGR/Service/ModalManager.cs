using SGR.Model;

namespace SGR.Service
{
    /// <summary>
    /// Gerenciador de modais respons�vel por controlar o estado e as opera��es das janelas modais na interface do usu�rio.
    /// </summary>
    /// <remarks>
    /// Esta classe fornece funcionalidades para:
    /// <list type="bullet">
    /// <item><description>Confirmar/cancelar exclus�o de equipamentos.</description></item>
    /// <item><description>Gerenciar o estado dos modais e notificar a interface sobre mudan�as.</description></item>
    /// </list>
    /// � um servi�o Scoped que mant�m o estado durante uma sess�o de usu�rio.
    /// </remarks>
    public class ModalManager
    {
        /// <summary>
        /// Obt�m ou define o equipamento selecionado para exclus�o.
        /// </summary>
        public Equipment? EquipmentToRemove { get; set; }

        /// <summary>
        /// Indica se o modal de confirma��o de exclus�o est� vis�vel.
        /// </summary>
        public bool ShowRemoveConfirm { get; set; }

        /// <summary>
        /// Evento disparado quando o estado do modal � alterado, permitindo que os componentes UI se atualizem.
        /// </summary>
        public event Action? StateChanged;

        /// <summary>
        /// Exibe o modal de confirma��o de exclus�o para um equipamento.
        /// </summary>
        /// <param name="equipment">O equipamento a ser exclu�do.</param>
        public void ConfirmRemove(Equipment equipment)
        {
            EquipmentToRemove = equipment;
            ShowRemoveConfirm = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Cancela o processo de exclus�o e fecha o modal de confirma��o.
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
        /// Este m�todo invoca o evento StateChanged, permitindo que os componentes
        /// que est�o observando este evento possam atualizar sua interface.
        /// </remarks>
        private void NotifyStateChanged() => StateChanged?.Invoke();
    }
}