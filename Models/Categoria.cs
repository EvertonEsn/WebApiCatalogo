namespace APICatalogo.Models;
// So tem atributos -> Classe anemica
public class Categoria
{
    public int CategoriaId { get; set; }

    public string? Nome  { get; set; }
    
    public string? ImagemUrl { get; set; }
    
    public ICollection<Produto>? Produtos { get; set; }

    public Categoria()
    {
        Produtos = new List<Produto>();
    }
}