using System.ComponentModel.DataAnnotations;

namespace PGK.Application.App.Raportichka.Row.Commands.CreateRow
{
    public class CreateRaportichkaRowVm
    {
        [Required] public List<int> Id { get; set; }

    }
}
