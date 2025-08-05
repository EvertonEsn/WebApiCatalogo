using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models;
// So tem atributos -> Classe anemica

[Table("Categorias")]
public class Categoria
{
    [Key]
    public int CategoriaId { get; set; }
    
    [Required]
    [StringLength(80)]
    public string? Nome  { get; set; }
    
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    
    public ICollection<Produto>? Produtos { get; set; }

    public Categoria()
    {
        Produtos = new List<Produto>();
    }
}