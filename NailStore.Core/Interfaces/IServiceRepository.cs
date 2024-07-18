using NailStore.Core.Models;

namespace NailStore.Core.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с услугами
    /// </summary>
    public interface IServiceRepository
    {
        /// <summary>
        /// Добавить услугу
        /// </summary>
        /// <param name="userId">Идентификатор пользовтаеля, которому принадлежит услуга</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="serviceName">Название услуги</param>
        /// <param name="descs">Список параграфов описания услуги</param>
        /// <param name="price">Стоимость услуги</param>
        /// <param name="durationTime">Длительность процедуры</param>
        /// <returns>Вернет объект ответа</returns>
        Task<ResponseModelCore> AddServiceAsync(Guid userId, int categoryId, string serviceName, string[] descs, decimal price, short durationTime);
        Task<ResponseModelCore> GetServicesByCategoryAsync(int categoryId);
    }
}