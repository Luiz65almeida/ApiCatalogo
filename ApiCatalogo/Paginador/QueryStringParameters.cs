namespace ApiCatalogo.Paginador;

public abstract class QueryStringParameters
{
    
    const int MaxPagesize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = MaxPagesize;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > MaxPagesize) ? MaxPagesize : value;
        }
    }
    
}