using AutoMapper;
using PGK.Application.Common.Mappings;
using PGK.Domain.User.History;
using System.ComponentModel.DataAnnotations;

namespace PGK.Application.App.User.Queries.GetHistoryList
{
    public class HistoryDto : IMapWith<History>
    {
        [Key] public int Id { get; set; }
        [Required] public int ContentId { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public HistoryType Type { get; set; }
        [Required] public DateTime Date { get; set; } = new DateTime();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<History, HistoryDto>();
        }
    }
}
