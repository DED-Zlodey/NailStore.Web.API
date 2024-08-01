namespace NailStore.Data.Models;

/// <summary>
/// Класс модели для связи города и услуги.
/// Представляет сущность связи между городом и услугой и содержит свойства, относящиеся к этой связи.
/// </summary>
public class CityToService
{
    /// <summary>
    /// Идентификатор города.
    /// </summary>
    public int CityId { get; set; }

    /// <summary>
    /// Идентификатор услуги.
    /// </summary>
    public int ServiceId { get; set; }
}