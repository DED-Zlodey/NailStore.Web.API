using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Core.Models.ResponseModels.Services;
using NailStore.Repositories;

namespace NailStore.Application;

public class ProviderService : IProviderService<Guid>
{
    private readonly IServiceRepository<Guid> _serviceRepository;

    public ProviderService(IServiceRepository<Guid> serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }
    
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
    public async Task<ResponseModelCore<string>> AddServiceAsync(Guid userId, int categoryId, string serviceName, string[] descs, decimal price, short durationTime)
    {
        if (price < 0)
        {
            return new ResponseModelCore<string>
            {
                Header = new()
                {
                    Error = "Стоимость услуги не может быть отрицательной",
                    StatusCode = 400
                },
                Result = "Стоимость услуги не может быть отрицательной",
            };
        }

        if (durationTime < 0)
        {
            return new ResponseModelCore<string>
            {
                Header = new()
                {
                    Error = "Длительность процедуры не может быть отрицательной",
                    StatusCode = 400
                },
                Result = "Длительность процедуры не может быть отрицательной",
            };
        }
        return await _serviceRepository.AddServiceAsync(userId, categoryId, serviceName, descs, price, durationTime);
    }
    /// <summary>
    /// Получить все услуги для категории по ее идентификатору
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="pageNumber">Номер запрашиваемой страницы</param>
    /// <param name="pageSize">Количество записей на страницу</param>
    /// <returns></returns>
    public async Task<ResponseModelCore<ResponseGetServiceModelCore>> GetServicesByCategoryAsync(int categoryId, int pageNumber, int pageSize)
    {
        return await _serviceRepository.GetServicesByCategoryAsync(categoryId, pageNumber, pageSize);
    }

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
    public async Task<ResponseModelCore<ResponseGetServiceModelCore>> GetAllServicesByUserIdAsync(Guid userId, int pageNumber, int pageSize)
    {
        return await _serviceRepository.GetAllServicesByUserIdAsync(userId, pageNumber, pageSize);
    }

    /// <summary>
    /// Удаление услуги по ее идентификатору
    /// </summary>
    /// <param name="serviceId">Идентификатор услуги</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns></returns>
    public async Task<ResponseModelCore<string>> RemoveServiceAsync(int serviceId, Guid userId)
    {
        return await _serviceRepository.RemoveServiceAsync(serviceId, userId);
    }
}
