namespace ApiCatalogo.Paginador;

public class ProdutoParameters
{

    const int MaxPagesize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize;
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
