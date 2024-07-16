using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCatalogo.Migrations
{
    
    public partial class PopulaCategorias2 : Migration
    {
        
        protected override void Up(MigrationBuilder mb)
        {

            mb.Sql("INSERT INTO Produtos (Nome, Descricao, preco, ImagemUrl, estoque, DataCadastro, CategoriaId)" + "Values ('Guaraná', 'Refrigerante de Guaraná 1000ml', 3.54, 'guarana.jpg', 50, now(), 1)");

        }

       
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");
        }
    }
}
