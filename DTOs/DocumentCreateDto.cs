namespace KazmirukEDMS.DTOs
{
    /// <summary>
    /// DTO для создания нового документа в системе электронного документооборота.
    /// Используется при первичной регистрации документа.
    /// </summary>
    public class DocumentCreateDto
    {
        /// <summary>
        /// Заголовок (наименование) документа.
        /// Обязательное поле.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Регистрационный номер документа.
        /// Может назначаться вручную или автоматически системой.
        /// </summary>
        public string? RegistrationNumber { get; set; }
    }
}