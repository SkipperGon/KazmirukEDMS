using KazmirukEDMS.Models;

namespace KazmirukEDMS.DTOs
{
    /// <summary>
    /// DTO, представляющий документ при чтении из системы.
    /// Используется для отображения данных документа в UI или API.
    /// </summary>
    public class DocumentDto
    {
        /// <summary>
        /// Уникальный идентификатор документа.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Заголовок (наименование) документа.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Регистрационный номер документа.
        /// </summary>
        public string? RegistrationNumber { get; set; }

        /// <summary>
        /// Текущий статус документа в жизненном цикле
        /// (например: черновик, на согласовании, утверждён и т.д.).
        /// </summary>
        public DocumentStatus Status { get; set; }

        /// <summary>
        /// Дата и время создания документа.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего документ.
        /// Обычно соответствует ID пользователя в системе аутентификации.
        /// </summary>
        public string? CreatedById { get; set; }

        /// <summary>
        /// Идентификатор текущей (актуальной) версии документа.
        /// </summary>
        public Guid? CurrentVersionId { get; set; }
    }
}