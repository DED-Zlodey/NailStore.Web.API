namespace NailStore.Web.API.DTOs.Services;

public struct PageInfo
{
    /// <summary>
    /// Размер страницы (сколько пользователей на страницу выводится)
    /// </summary>
    public short PageSize { get; }
    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public short PageNumber { get; }
    /// <summary>
    /// Всего страниц
    /// </summary>
    public short TotalPages { get; }
    /// <summary>
    /// Флаг указывающий на существование предыдущей страницы. <b>True</b> страница существует, <b>False</b> не существует
    /// </summary>
    public bool HasPreviousPage => PageNumber < TotalPages;
    /// <summary>
    /// Флаг указывающий на существование следующей страницы. <b>True</b> страница существует, <b>False</b> не существует
    /// </summary>
    public bool HasNextPage => PageNumber > 1;

    public PageInfo(short countItems, short pageNumber, short pageSize)
    {
        PageSize = pageSize;
        TotalPages = (short)Math.Ceiling(countItems / (double)pageSize);
        PageNumber = pageNumber;
    }
}