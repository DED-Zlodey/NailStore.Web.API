﻿using NailStore.Core.Models;

namespace NailStore.Core.Interfaces;

/// <summary>
/// Сервис для работы с услугами
/// </summary>
public interface IProviderService<T>
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
    /// Получить все услуги для категории по ее идентификатору
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
}
