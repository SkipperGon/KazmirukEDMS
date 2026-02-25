namespace KazmirukEDMS.DTOs
{
    /// <summary>
    /// DTO для создания новой версии документа.
    /// Используется при загрузке нового файла или изменении содержимого.
    /// </summary>
    public class DocumentVersionCreateDto
    {
        /// <summary>
        /// Путь к файлу версии документа в файловом хранилище
        /// </summary>
        public string FilePath { get; set; } = null!;

        /// <summary>
        /// MIME-тип файла
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// Контрольная сумма файла
        /// Используется для проверки целостности.
        /// </summary>
        public string? Checksum { get; set; }

        /// <summary>
        /// Электронная подпись файла.
        /// </summary>
        public byte[]? Signature { get; set; }

        /// <summary>
        /// Идентификатор или имя пользователя,
        /// подписавшего документ электронной подписью.
        /// </summary>
        public string? SignedBy { get; set; }
    }
}