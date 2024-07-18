namespace NailStore.Core.Models.ResponseModels.Services;

public class ResponseServiceModelCore
{
    /// <summary>
    /// Идентификатор услуги
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// Идентификатор категории услуг к которой принадлежит услуга
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Название категории к которой принадлежит услуга
    /// </summary>
    public string CategoryName { get; set; }
    /// <summary>
    /// Модель мастера, который оказывает услугу
    /// </summary>
    public MasterUserModelCore Master { get; set; }
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
    public List<ResponseServiceDescroptionModelCore> Descriptions { get; set; }
}
