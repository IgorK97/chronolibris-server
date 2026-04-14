using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record CreateShelfInputModel([MaxLength(256)]
        string Name);
}
