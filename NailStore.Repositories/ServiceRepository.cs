using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NailStore.Core.Interfaces;
using NailStore.Core.Models;
using NailStore.Core.Models.ResponseModels.Services;
using NailStore.Data;
using NailStore.Data.Models;

namespace NailStore.Repositories;

public class ServiceRepository: IServiceRepository<Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServiceRepository> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
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
            var category = await _context.CategoriesServices.AsNoTracking().Select(x => new CategoryServiceModel
            {
                CategoryId = x.CategoryId,
            }).FirstOrDefaultAsync(x => x.CategoryId == categoryId);
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

    /// <summary>
    /// Получить все услуги из категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="pageNumber">Номер запрашиваемой страницы</param>
    /// <param name="pageSize">Количество записей на страницу</param>
    /// <returns></returns>
    public async Task<ResponseModelCore> GetServicesByCategoryAsync(int categoryId, int pageNumber, int pageSize)
    {
        var response = new ResponseModelCore 
        {
            Header = new()
            {
                Error = "Что-то пошло не так:(",
                StatusCode = 500
            }
        };
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }
        if (pageSize <= 0)
        {
            pageSize = 10;
        }
        if (pageSize > 15)
        {
            pageSize = 15;
        }
        var services = _context.Services.Where(x => x.CategoryId == categoryId).Select(x => new ResponseServiceModelCore
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
        });
        var countServices = await services.CountAsync();
        var resultServices = await services.OrderBy(x => x.ServiceId).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        var result =
        response.Body = new ()
        {
            GetServices = new ()
            {
                PageInfo = new ResponsePageInfoModelCore(countServices, pageNumber, pageSize),
                Services = resultServices
            }
        };
        response.Header.StatusCode = 200;
        return response;
    }
    /// <summary>
    /// Удаление услуги по ее идентификатору
    /// </summary>
    /// <param name="serviceId">Идентификатор услуги</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns></returns>
    public async Task<ResponseModelCore> RemoveServiceAsync(int serviceId, Guid userId)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.ServiceId == serviceId && x.UserId == userId);
        if (service != null)
        {
            _context.Remove(service);
            await _context.SaveChangesAsync();
            return new()
            {
                Header = new()
                {
                    StatusCode = 200,
                },
                Body = new()
                {
                    Message = "Услуга успешно удалена"
                }
            };
        }
        return new()
        {
            Header = new()
            {
                StatusCode = 404,
                Error = "Не удалось удалить услугу. Услуга не найдена или не принадлежит пользователю"
            }
        };
    }
}
