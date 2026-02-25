using KazmirukEDMS.Models;

namespace KazmirukEDMS.DTOs
{
    /// <summary>
    /// DTO для обновления данных документа.
    /// Все поля опциональны — обновляются только переданные значения.
    /// </summary>
    public class DocumentUpdateDto
    {
        /// <summary>
        /// Новое наименование документа.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Новый регистрационный номер документа.
        /// </summary>
        public string? RegistrationNumber { get; set; }

        /// <summary>
        /// Новый статус документа в жизненном цикле.
        /// </summary>
        public DocumentStatus? Status { get; set; }
    }
}