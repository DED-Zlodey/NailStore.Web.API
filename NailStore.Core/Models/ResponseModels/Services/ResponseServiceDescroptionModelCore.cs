namespace NailStore.Core.Models.ResponseModels.Services;

public class ResponseServiceDescroptionModelCore
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
    /// Порядковый номер параграфа
    /// </summary>
    public short Number { get; set; }
    /// <summary>
    /// Текст параграфа
    /// </summary>
    public string Text { get; set; }
}
