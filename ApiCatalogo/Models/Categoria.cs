namespace ApiCatalogo.Models;
using System.Collections.ObjectModel;


public class Categoria {

    public Categoria(){   
        Produtos = new Collection<Produto>();
}

    public long CategoriaId { get; set; }

    public string? Nome { get; set; }

    public string? ImagemUrl { get;set; }

    public ICollection<Produto>? Produtos { get; set; }    
}