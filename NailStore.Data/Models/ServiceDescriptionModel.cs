namespace NailStore.Data.Models;

/// <summary>
/// Модель описания услуги.
/// Представляет сущность описания услуги и содержит свойства, относящиеся к описанию.
/// </summary>
public class ServiceDescriptionModel
{
    /// <summary>
    /// Уникальный идентификатор описания.
    /// </summary>
    public long DescriptionId { get; set; }

    /// <summary>
    /// Идентификатор услуги, к которой принадлежит описание.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Навигационное свойство услуги, к которой принадлежит описание.
    /// </summary>
    public ServiceModel Service { get; set; }

    /// <summary>
    /// Порядковый номер описания в рамках услуги.
    /// </summary>
    public short Number { get; set; }

    /// <summary>
    /// Текст описания.
    /// </summary>
    public string Text { get; set; }
}
