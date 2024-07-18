using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Core.Models.ResponseModels.Services;
using NailStore.Data;
using NailStore.Data.Models;

namespace NailStore.Repositories;

public class ServiceRepository: IServiceRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServiceRepository> _logger;

    public ServiceRepository(ApplicationDbContext context, ILogger<ServiceRepository> logger)
    {
        _context = context;
        _logger = logger;
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
        try
        {
            var category = await _context.CategoriesServices.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            if (category == null)
            {
                _logger.LogError("{method} Категория c Id:{id} не найдена.", nameof(AddServiceAsync), categoryId);
                return new ResponseModelCore
                {
                    Header = new()
                    {
                        Error = "Категория не найдена",
                        StatusCode = 404
                    }
                };
            }
            var descriptions = new List<ServiceDescriptionModel>();
            for (int i = 0; i < descs.Length; i++)
            {
                descriptions.Add(new ServiceDescriptionModel
                {
                    Number = (short)(i + 1),
                    Text = descs[i]
                });
            }
            await _context.Services.AddAsync(new ServiceModel
            {
                DurationTime = durationTime,
                Price = price,
                ServiceName = serviceName,
                UserId = userId,
                CategoryId = categoryId,
                ServiceDescriptions = descriptions
            });
            await _context.SaveChangesAsync();
            return new()
            {
                Header = new()
                {
                    StatusCode = 200
                },
                Body = new()
                {
                    Message = "Услуга успешно добавлена"
                }
            };
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "{method} Не удалось добавить услугу. Reason: {reason}", nameof(AddServiceAsync), ex.Message);
            return new()
            {
                Header = new()
                {
                    StatusCode = 500,
                    Error = "Не удалось добавить услугу. Что-то пошло не так :("
                }
            };
        }
    }
    public async Task<ResponseModelCore> GetServicesByCategoryAsync(int categoryId)
    {
        var response = new ResponseModelCore 
        {
            Header = new()
            {
                Error = "Что-то пошло не так:(",
                StatusCode = 500
            }
        };
        var services = await _context.Services.Where(x => x.CategoryId == categoryId).Select(x => new ResponseServiceModelCore
        {
            CategoryName = x.Category.CategoryName,
            CategoryId = x.CategoryId,
            Descriptions = x.ServiceDescriptions.Select(c => new ResponseServiceDescroptionModelCore
            {
                DescriptionId = c.DescriptionId,
                Number = c.Number,
                ServiceId = x.ServiceId,
                Text = c.Text,
            }).ToList(),
            DurationTime = x.DurationTime,
            Master =new()
            {
                NameMaster = x.User.UserName!
            },
            Price = x.Price,
            ServiceName = x.ServiceName,
            ServiceId = x.ServiceId
        }).ToListAsync();
        response.Body = new ResponseBodyCore
        {
            Services = services
        };
        response.Header.StatusCode = 200;
        return response;
    }
}
