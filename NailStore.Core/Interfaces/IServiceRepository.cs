using NailStore.Core.Models;

namespace NailStore.Core.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с услугами
    /// </summary>
    public interface IServiceRepository<T>
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
        Task<ResponseModelCore> AddServiceAsync(T userId, int categoryId, string serviceName, string[] descs, decimal price, short durationTime);

        /// <summary>
        /// Получить все услуги из переданной категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="pageNumber">Номер запрашиваемой страницы</param>
        /// <param name="pageSize">Количество записей на страницу</param>
        /// <returns></returns>
        Task<ResponseModelCore> GetServicesByCategoryAsync(int categoryId, int pageNumber, int pageSize);
        /// <summary>
        /// Удаление услуги по ее идентификатору
        /// </summary>
        /// <param name="serviceId">Идентификатор услуги</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<ResponseModelCore> RemoveServiceAsync(int serviceId, T userId);

        /// <summary>
        /// Получает все услуги, принадлежащие определенному пользователю, с поддержкой пагинации.
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя, чьи услуги необходимо получить.</param>
        /// <param name="pageNumber">Номер страницы для получения. Если меньше или равно 0, по умолчанию используется 1.</param>
        /// <param name="pageSize">Количество записей на странице. Если меньше или равно 0, по умолчанию используется 10. Если больше 15, то устанавливается 15.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. Результатом задачи является объект <see cref="ResponseModelCore"/> со следующими свойствами:
        /// - <see cref="ResponseModelCore.Header"/>: Содержит код состояния HTTP и сообщение об ошибке, если таковое имеется.
        /// - <see cref="ResponseModelCore.Body"/>: Содержит полученные услуги и информацию о пагинации.
        /// </returns>
        Task<ResponseModelCore> GetAllServicesByUserIdAsync(T userId, int pageNumber, int pageSize);
    }
}