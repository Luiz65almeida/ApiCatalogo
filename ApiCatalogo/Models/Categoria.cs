namespace ApiCatalogo.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Categorias")]
public class Categoria {

    public Categoria(){   
        Produtos = new Collection<Produto>();
     }

    [Key]
    public long CategoriaId { get; set; }

    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get;set; }

    public ICollection<Produto>? Produtos { get; set; }    
}