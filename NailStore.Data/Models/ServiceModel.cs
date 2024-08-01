namespace NailStore.Data.Models;

public class ServiceModel
{
    /// <summary>
    /// Идентификатор услуги
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// Идентификатор категории которой принадлежит услуга
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Идентификатор мастера, который оказывает данную услугу
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Наименование услуги
    /// </summary>
    public string? ServiceName { get; set; }
    /// <summary>
    /// Длительность процедуры в минутах
    /// </summary>
    public short DurationTime { get; set; }
    /// <summary>
    /// Стоимость в рублях
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// Навигационное свойство пользовтаеля, которому принадлежит услуга
    /// </summary>
    public UserEntity? User { get; set; }
    /// <summary>
    /// Список параграфов для описания услуги
    /// </summary>
    public List<ServiceDescriptionModel>? ServiceDescriptions { get; set; }
    /// <summary>
    /// Категория которой принадлежит данная услуга
    /// </summary>
    public CategoryServiceModel? Category { get; set; }
    /// <summary>
    /// Города, в которых предоставляется услуга
    /// </summary>
    public List<City>? Cities { get; set; }
    /// <summary>
    /// Краткое описание услуги
    /// </summary>
    public string? DescriptionService { get; set; }
}