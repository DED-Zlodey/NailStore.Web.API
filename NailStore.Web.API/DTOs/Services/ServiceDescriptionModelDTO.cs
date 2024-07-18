namespace NailStore.Web.API.DTOs.Services
{
    public class ServiceDescriptionModelDTO : IComparable<ServiceDescriptionModelDTO>
    {
        /// <summary>
        /// Идентификатор параграфа
        /// </summary>
        public long? DescriptionId { get; set; }
        /// <summary>
        /// Идентификатор услуги, к которой принадлежит параграф
        /// </summary>
        public int? ServiceId { get; set; }
        /// <summary>
        /// Порядковый номер параграфа
        /// </summary>
        public short Number { get; set; }
        /// <summary>
        /// Текст параграфа
        /// </summary>
        public required string Text { get; set; }

        public int CompareTo(ServiceDescriptionModelDTO? other)
        {
            if (other == null)
                return 1;

            else
                return Number.CompareTo(other.Number);
        }
    }
}
