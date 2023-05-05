using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IronXL;
using PGK.Application.Interfaces;
using PGK.Application.Common.Exceptions;
using Constants = PGK.Application.Common.Constants;
using PGK.Application.Common.Extentions;

namespace Market.Application.App.VedomostAttendance;

public class GetVedomostAttendanceQueryHandler : IRequestHandler<GetVedomostAttendanceQuery, byte[]>
{
    private readonly IPGKDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetVedomostAttendanceQueryHandler(IPGKDbContext dbContext, IMapper mapper) =>
        (_dbContext, _mapper) = (dbContext, mapper);
    
    public async Task<byte[]> Handle(GetVedomostAttendanceQuery request, CancellationToken cancellationToken)
    {
        var month = request.Date.Month;
        var year = request.Date.Year;

        var group = await _dbContext.Groups
            .Include(u => u.Speciality)
            .Include(u => u.Headman)
            .Include(u => u.ClassroomTeacher)
            .Include(u => u.Department)
                .ThenInclude(u => u.DepartmentHead)
            .FirstOrDefaultAsync(u => u.Id == request.GroupId);

        if (group == null)
        {
            throw new NotFoundException(nameof(PGK.Domain.Group.Group.Id), request.GroupId);
        }
        
        var rows = _dbContext.RaportichkaRows
            .Include(u => u.Raportichka)
                .ThenInclude(u => u.Group)
            .Where(u => u.Raportichka.Group.Id == request.GroupId)
            .Where(u => u.Raportichka.Date.Month == month && u.Raportichka.Date.Year == year);

        var students = await _dbContext.StudentsUsers
            .Include(u => u.Group)
            .Where(u => u.Group.Id == request.GroupId)
            .OrderBy(u => u.LastName)
            .ToListAsync();
        
        var directory = $"{Constants.VACATIONVEDOMOST_ATTENDANCE_PATH}{year}/{month}/";
        var patch = $"{directory}{request.GroupId}.xls";

        if (File.Exists(patch))
            File.Delete(patch);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.Copy($"{Constants.VACATIONVEDOMOST_ATTENDANCE_PATH}vedomost_pattern.xls", patch);

        var wb = WorkBook.Load(patch);
        var ws = wb.GetWorkSheet("Лист1");
        ws.Rows[0].Columns[0].Value = $"Ведомость посещаемости гр. {group.Speciality.NameAbbreviation}-{group.Course}{group.Number} за {request.Date:MMMM} {year}г.";;
        ws.Rows[46].Columns[24].Value = students.Count.ToString();
        ws.Rows[45].Columns[12].Value = getFIO(group.ClassroomTeacher);
        ws.Rows[46].Columns[9].Value = getFIO(group.Department.DepartmentHead);

        if (group.Headman != null)
        {
            ws.Rows[44].Columns[13].Value = getFIO(group.Headman);
        }

        foreach (var (student, index) in students.WithIndex())
        {
            ws.Rows[index + 3].Columns[1].Value = getFIO(student);
        }

        wb.Save();
        
        return await File.ReadAllBytesAsync(patch, cancellationToken);
    }

    private string getFIO(PGK.Domain.User.User user)
    {
        if (string.IsNullOrEmpty(user.MiddleName))
        {
           return $"{user.LastName} {user.FirstName[0]}.";
        }
        else
        {
            return $"{user.LastName} {user.FirstName[0]}. {user.MiddleName[0]}.";
        }

    }
}