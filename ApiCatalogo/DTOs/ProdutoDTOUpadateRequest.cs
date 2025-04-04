using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.DTOs;

public class ProdutoDTOUpadateRequest: IValidatableObject
{
    [Range(1,9999, ErrorMessage = "Estoque deve estar entre 1 e 9999")]
    public float Estoque { get; set; }

    public DateTime DataCadastro { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataCadastro <= DateTime.Now.Date)
        {
            yield return new ValidationResult("Data de cadastro deve ser maior que a data atual",
                new[] { nameof(DataCadastro) });
        }
    }
}
