using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Data.Models;

namespace NailStore.Application;

public class ProviderService : IProviderService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly ILogger<ProviderService> _logger;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ProviderService(IServiceRepository serviceRepository, ILogger<ProviderService> logger, UserManager<UserEntity> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _serviceRepository = serviceRepository;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
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
    public async Task<ResponseModelCore> GetServicesByCategoryAsync(int categoryId)
    {
        return await _serviceRepository.GetServicesByCategoryAsync(categoryId);
    }
}
