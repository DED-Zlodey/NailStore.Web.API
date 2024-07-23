namespace NailStore.Core.Models.ResponseModels.Services;

public class ResponseGetServiceModelCore
{
    /// <summary>
    /// Информация о текущей странице, кол-ве всех страниц, следующей и предыдущей страницах
    /// </summary>
    public ResponsePageInfoModelCore PageInfo { get; set; }
    /// <summary>
    /// Список сервисов
    /// </summary>
    public List<ResponseServiceModelCore> Services { get; set; }
}