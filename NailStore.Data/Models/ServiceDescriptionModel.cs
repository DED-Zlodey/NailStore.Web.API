namespace NailStore.Data.Models;

public class ServiceDescriptionModel
{
    /// <summary>
    /// Идентификатор параграфа
    /// </summary>
    public long DescriptionId { get; set; }
    /// <summary>
    /// Идентификатор услуги, к которой принадлежит параграф
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// Навигационное свойство услуги к которой принадлежит параграф
    /// </summary>
    public ServiceModel Service { get; set; }
    /// <summary>
    /// Порядковый номер параграфа
    /// </summary>
    public short Number { get; set; }
    /// <summary>
    /// Текст параграфа
    /// </summary>
    public string Text { get; set; }
}
