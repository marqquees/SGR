using SGR.Models;

namespace SGR.Service
{
    /// <summary>
    /// Gerenciador de modais respons�vel por controlar o estado e as opera��es das janelas modais na interface do usu�rio.
    /// </summary>
    /// <remarks>
    /// Esta classe fornece funcionalidades para:
    /// <list type="bullet">
    /// <item><description>Visualizar detalhes de um equipamento em um modal.</description></item>
    /// <item><description>Ativar/desativar o modo de edi��o.</description></item>
    /// <item><description>Confirmar/cancelar exclus�o de equipamentos.</description></item>
    /// <item><description>Gerenciar o estado dos modais e notificar a interface sobre mudan�as.</description></item>
    /// </list>
    /// � um servi�o Scoped que mant�m o estado durante uma sess�o de usu�rio.
    /// </remarks>
    public class ModalManager
    {
        /// <summary>
        /// Obt�m ou define o item de equipamento atualmente selecionado para visualiza��o ou edi��o.
        /// </summary>
        public Equipment? SelectedItem { get; set; }
        
        /// <summary>
        /// Obt�m ou define o item de equipamento selecionado para exclus�o.
        /// </summary>
        public Equipment? ItemToDelete { get; set; }
        
        /// <summary>
        /// Mant�m uma c�pia do estado original do item selecionado para possibilitar restaura��o em caso de cancelamento da edi��o.
        /// </summary>
        public Equipment? OriginalItem { get; set; }
        
        /// <summary>
        /// Indica se o modal est� em modo de edi��o.
        /// </summary>
        public bool IsEditing { get; set; }
        
        /// <summary>
        /// Indica se o modal de confirma��o de exclus�o est� vis�vel.
        /// </summary>
        public bool ShowDeleteConfirm { get; set; }

        /// <summary>
        /// Evento disparado quando o estado do modal � alterado, permitindo que os componentes UI se atualizem.
        /// </summary>
        public event Action? StateChanged;

        /// <summary>
        /// Configura o modal para exibir os detalhes de um equipamento.
        /// </summary>
        /// <param name="item">O equipamento cujos detalhes ser�o exibidos.</param>
        /// <remarks>
        /// Este m�todo cria uma c�pia profunda do item para permitir a restaura��o do estado original
        /// caso a edi��o seja cancelada.
        /// </remarks>
        public void ViewDetails(Equipment item)
        {
            SelectedItem = item;
            // Criar uma c�pia completa do item original para restaurar todos os campos.
            OriginalItem = new Equipment
            {
                Id = item.Id,
                DateRegister = item.DateRegister,
                State = item.State,
                Customer = item.Customer,
                User = item.User,
                Category = item.Category,
                Brand = item.Brand,
                Model = item.Model,
                SerialNumber = item.SerialNumber,
                Processor = item.Processor,
                MemoryRAM = item.MemoryRAM,
                Storage = item.Storage,
                OperatingSystem = item.OperatingSystem,
                Note = item.Note,
                Responsible = item.Responsible
            };

            IsEditing = false;
            ShowDeleteConfirm = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// Ativa o modo de edi��o para o item selecionado.
        /// </summary>
        public void EnableEditing()
        {
            IsEditing = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Cancela a edi��o do item selecionado, restaurando os valores originais.
        /// </summary>
        /// <remarks>
        /// Este m�todo restaura todos os campos do item para os valores originais
        /// que foram armazenados quando o item foi selecionado para visualiza��o.
        /// </remarks>
        public void CancelEditing()
        {
            if (OriginalItem != null && SelectedItem != null)
            {
                // Restaurar todos os campos do item original.
                SelectedItem.DateRegister = OriginalItem.DateRegister;
                SelectedItem.State = OriginalItem.State;
                SelectedItem.Customer = OriginalItem.Customer;
                SelectedItem.User = OriginalItem.User;
                SelectedItem.Category = OriginalItem.Category;
                SelectedItem.Brand = OriginalItem.Brand;
                SelectedItem.Model = OriginalItem.Model;
                SelectedItem.SerialNumber = OriginalItem.SerialNumber;
                SelectedItem.Processor = OriginalItem.Processor;
                SelectedItem.MemoryRAM = OriginalItem.MemoryRAM;
                SelectedItem.Storage = OriginalItem.Storage;
                SelectedItem.OperatingSystem = OriginalItem.OperatingSystem;
                SelectedItem.Note = OriginalItem.Note;
                SelectedItem.Responsible = OriginalItem.Responsible;
            }
            
            IsEditing = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// Exibe o modal de confirma��o de exclus�o para um equipamento.
        /// </summary>
        /// <param name="item">O equipamento a ser exclu�do.</param>
        public void ConfirmDelete(Equipment item)
        {
            ItemToDelete = item;
            ShowDeleteConfirm = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Cancela o processo de exclus�o e fecha o modal de confirma��o.
        /// </summary>
        public void CancelDelete()
        {
            ItemToDelete = null;
            ShowDeleteConfirm = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// Fecha o modal de detalhes e limpa o estado atual.
        /// </summary>
        public void CloseModal()
        {
            SelectedItem = null;
            OriginalItem = null;
            IsEditing = false;
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