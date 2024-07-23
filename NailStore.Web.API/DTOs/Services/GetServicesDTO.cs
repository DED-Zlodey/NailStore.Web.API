namespace NailStore.Web.API.DTOs.Services;
/// <summary>
/// Модель получения списка услуг
/// </summary>
public struct GetServicesDTO
{
    /// <summary>
    /// Информация о текущей странице, кол-ве всех страниц, следующей и предыдущей страницах
    /// </summary>
    public PageInfo PageInfo { get; set; }
    /// <summary>
    /// Список сервисов
    /// </summary>
    public List<ServiceModelDTO> Services { get; set; }
}