using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace APICatalogo.DTOs;

public class ProdutoDTOUpdateResponse : IValidatableObject
{
    [Range(1, 9999, ErrorMessage = "Estoque deve estar entre 1 e 9999.")]
    public float Estoque { get; set; }
    
    public DateTime DataCadastro { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataCadastro.Date <= DateTime.Now)
        {
            yield return new ValidationResult("A data deve ser maior ou igual a data atual.",
                new[] { nameof(this.DataCadastro) });
        }
    }
}