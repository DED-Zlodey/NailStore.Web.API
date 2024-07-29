using NailStore.Core.Interfaces;
using NailStore.Core.Models;

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
    public async Task<ResponseModelCore> AddServiceAsync(Guid userId, int categoryId, string serviceName, string[] descs, decimal price, short durationTime)
    {
        return await _serviceRepository.AddServiceAsync(userId, categoryId, serviceName, descs, price, durationTime);
    }
    /// <summary>
    /// Получить все услуги для категории по ее идентификатору
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="pageNumber">Номер запрашиваемой страницы</param>
    /// <param name="pageSize">Количество записей на страницу</param>
    /// <returns></returns>
    public async Task<ResponseModelCore> GetServicesByCategoryAsync(int categoryId, int pageNumber, int pageSize)
    {
        return await _serviceRepository.GetServicesByCategoryAsync(categoryId, pageNumber, pageSize);
    }

    /// <summary>
    /// Удаление услуги по ее идентификатору
    /// </summary>
    /// <param name="serviceId">Идентификатор услуги</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns></returns>
    public async Task<ResponseModelCore> RemoveServiceAsync(int serviceId, Guid userId)
    {
        return await _serviceRepository.RemoveServiceAsync(serviceId, userId);
    }
}
