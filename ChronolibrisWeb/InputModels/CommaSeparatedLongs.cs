using System.ComponentModel.DataAnnotations;

//Вместо этого подхода можно было либо вообще передавать как список изначально long, либо Fluent Validation использовать
namespace ChronolibrisWeb.InputModels
{
    public class CommaSeparatedLongs : ValidationAttribute
    {
        public int MaxCount { get; set; }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var strValue = value as string;
            if (string.IsNullOrWhiteSpace(strValue)) return ValidationResult.Success;

            var parts = strValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > MaxCount)
                return new ValidationResult($"Максимум {MaxCount} id за запрос");

            if (!parts.All(p => long.TryParse(p.Trim(), out _)))
                return new ValidationResult("ids должны быть целыми числами, разделенными запятой");

            return ValidationResult.Success;
        }
    }
}
