namespace NailStore.Core.Models.ResponseModels.Services;

public struct ResponsePageInfoModelCore
{
    /// <summary>
    /// Размер страницы (сколько пользователей на страницу выводится)
    /// </summary>
    public int PageSize { get; }
    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public int PageNumber { get; }
    /// <summary>
    /// Всего страниц
    /// </summary>
    public int TotalPages { get; }
    /// <summary>
    /// Флаг указывающий на существование предыдущей страницы. <b>True</b> страница существует, <b>False</b> не существует
    /// </summary>
    public bool HasPreviousPage => PageNumber < TotalPages;
    /// <summary>
    /// Флаг указывающий на существование следующей страницы. <b>True</b> страница существует, <b>False</b> не существует
    /// </summary>
    public bool HasNextPage => PageNumber > 1;

    public ResponsePageInfoModelCore(int countItems, int pageNumber, int pageSize)
    {
        PageSize = pageSize;
        TotalPages = (short)Math.Ceiling(countItems / (double)pageSize);
        PageNumber = pageNumber;
    }
}