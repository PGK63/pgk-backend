using System.ComponentModel.DataAnnotations;
using AutoMapper;
using PGK.Application.App.User.Queries.GetUserLits;
using PGK.Application.Common.Mappings;
using PGK.Domain.Journal;

namespace PGK.Application.App.Journal.Commands.CreateJournalSubjectColumn;

public class CreateJournalColumnDto : IMapWith<JournalSubjectColumn>
{
    [Key] public int Id { get; set; }
    [Required] public JournalEvaluation Evaluation { get; set; }
    [Required] public DateTime Date { get; set; }
    
    [Key] public int RowId { get; set; }
    [Required] public UserDto RowStudent { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<JournalSubjectColumn, CreateJournalColumnDto>()
            .ForMember(
                name: "RowId",
                memberOptions: u => 
                    u.MapFrom(m => m.Row.Id))
            .ForMember(
                name: "RowStudent",
                memberOptions: u => 
                    u.MapFrom(m => m.Row.Student));
    }
}