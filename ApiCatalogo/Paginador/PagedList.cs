﻿namespace ApiCatalogo.Paginador;

public class PagedList<T> : List<T> where T : class
{

    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> itens, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(itens);
    } 

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int PageSize)
    {
        var count = source.Count();
        var itens = source.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();
        
        return new PagedList<T>(itens, count, pageNumber, PageSize);
    }
}
