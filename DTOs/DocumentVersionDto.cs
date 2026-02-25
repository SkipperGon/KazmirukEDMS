namespace KazmirukEDMS.DTOs
{
    /// <summary>
    /// DTO, представляющий версию документа при чтении.
    /// Содержит метаданные версии и информацию о подписи.
    /// </summary>
    public class DocumentVersionDto
    {
        /// <summary>
        /// Уникальный идентификатор версии документа.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор документа, к которому относится версия.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Порядковый номер версии документа.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Путь к файлу версии документа в хранилище.
        /// </summary>
        public string FilePath { get; set; } = null!;

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Checksum { get; set; }

        /// <summary>
        /// Пользователь, подписавший документ.
        /// </summary>
        public string? SignedBy { get; set; }

        /// <summary>
        /// Дата и время подписания документа.
        /// </summary>
        public DateTime? SignedAt { get; set; }

        /// <summary>
        /// Дата и время создания версии.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего версию.
        /// </summary>
        public string? CreatedById { get; set; }
    }
}