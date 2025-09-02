using SGR.Models;

namespace SGR.Service
{
    /// <summary>
    /// Gerenciador de modais responsável por controlar o estado e as operações das janelas modais na interface do usuário.
    /// </summary>
    /// <remarks>
    /// Esta classe fornece funcionalidades para:
    /// <list type="bullet">
    /// <item><description>Visualizar detalhes de um equipamento em um modal.</description></item>
    /// <item><description>Ativar/desativar o modo de edição.</description></item>
    /// <item><description>Confirmar/cancelar exclusão de equipamentos.</description></item>
    /// <item><description>Gerenciar o estado dos modais e notificar a interface sobre mudanças.</description></item>
    /// </list>
    /// É um serviço Scoped que mantém o estado durante uma sessão de usuário.
    /// </remarks>
    public class ModalManager
    {
        /// <summary>
        /// Obtém ou define o item de equipamento atualmente selecionado para visualização ou edição.
        /// </summary>
        public Equipment? SelectedItem { get; set; }
        
        /// <summary>
        /// Obtém ou define o item de equipamento selecionado para exclusão.
        /// </summary>
        public Equipment? ItemToDelete { get; set; }
        
        /// <summary>
        /// Mantém uma cópia do estado original do item selecionado para possibilitar restauração em caso de cancelamento da edição.
        /// </summary>
        public Equipment? OriginalItem { get; set; }
        
        /// <summary>
        /// Indica se o modal está em modo de edição.
        /// </summary>
        public bool IsEditing { get; set; }
        
        /// <summary>
        /// Indica se o modal de confirmação de exclusão está visível.
        /// </summary>
        public bool ShowDeleteConfirm { get; set; }

        /// <summary>
        /// Evento disparado quando o estado do modal é alterado, permitindo que os componentes UI se atualizem.
        /// </summary>
        public event Action? StateChanged;

        /// <summary>
        /// Configura o modal para exibir os detalhes de um equipamento.
        /// </summary>
        /// <param name="item">O equipamento cujos detalhes serão exibidos.</param>
        /// <remarks>
        /// Este método cria uma cópia profunda do item para permitir a restauração do estado original
        /// caso a edição seja cancelada.
        /// </remarks>
        public void ViewDetails(Equipment item)
        {
            SelectedItem = item;
            // Criar uma cópia completa do item original para restaurar todos os campos.
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
        /// Ativa o modo de edição para o item selecionado.
        /// </summary>
        public void EnableEditing()
        {
            IsEditing = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Cancela a edição do item selecionado, restaurando os valores originais.
        /// </summary>
        /// <remarks>
        /// Este método restaura todos os campos do item para os valores originais
        /// que foram armazenados quando o item foi selecionado para visualização.
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
        /// Exibe o modal de confirmação de exclusão para um equipamento.
        /// </summary>
        /// <param name="item">O equipamento a ser excluído.</param>
        public void ConfirmDelete(Equipment item)
        {
            ItemToDelete = item;
            ShowDeleteConfirm = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Cancela o processo de exclusão e fecha o modal de confirmação.
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
        /// Este método invoca o evento StateChanged, permitindo que os componentes
        /// que estão observando este evento possam atualizar sua interface.
        /// </remarks>
        private void NotifyStateChanged() => StateChanged?.Invoke();
    }
}