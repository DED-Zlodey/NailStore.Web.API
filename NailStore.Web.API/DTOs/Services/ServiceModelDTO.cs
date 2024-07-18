namespace NailStore.Web.API.DTOs.Services
{
    /// <summary>
    /// Модель услуги
    /// </summary>
    public class ServiceModelDTO
    {
        /// <summary>
        /// Название категории к которой принадлежит услуга
        /// </summary>
        public string? CategoryName { get; set; }
        /// <summary>
        /// Идентификатор услуги
        /// </summary>
        public int? ServiceId { get; set; }
        /// <summary>
        /// Идентификатор категории услуг к которой принадлежит услуга
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Наименование услуги
        /// </summary>
        public required string ServiceName { get; set; }
        /// <summary>
        /// Длительность процедуры в минутах
        /// </summary>
        public short DurationTime { get; set; }
        /// <summary>
        /// Стоимость в рублях
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Список параграфов описания услуги
        /// </summary>
        public List<ServiceDescriptionModelDTO> DescriptionList { get; set; }
        /// <summary>
        /// Модель мастера оказывающего услугу
        /// </summary>
        public MasterUserDTO? Master { get; set; }
    }
}
