using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IronXL;
using PGK.Application.Interfaces;
using PGK.Application.Common.Exceptions;
using Constants = PGK.Application.Common.Constants;
using PGK.Application.Common.Extentions;
using PGK.Application.Common.Date;
using PGK.Domain.Raportichka;

namespace Market.Application.App.VedomostAttendance;

public class GetVedomostAttendanceQueryHandler : IRequestHandler<GetVedomostAttendanceQuery, byte[]>
{
    private readonly IPGKDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetVedomostAttendanceQueryHandler(IPGKDbContext dbContext, IMapper mapper) =>
        (_dbContext, _mapper) = (dbContext, mapper);
    
    public async Task<byte[]> Handle(GetVedomostAttendanceQuery request, CancellationToken cancellationToken)
    {
        var month = (int)request.Month;
        var year = request.Year;
        int countDays = DateTime.DaysInMonth(year, month);

        var group = await _dbContext.Groups
            .Include(u => u.Speciality)
            .Include(u => u.Headman)
            .Include(u => u.ClassroomTeacher)
            .Include(u => u.Students)
            .Include(u => u.Department)
                .ThenInclude(u => u.DepartmentHead)
            .Include(u => u.Raportichkas)
                .ThenInclude(u => u.Rows)
                    .ThenInclude(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == request.GroupId);

        if (group == null)
        {
            throw new NotFoundException(nameof(PGK.Domain.Group.Group.Id), request.GroupId);
        }

        var rows = group.Raportichkas.Select(u => u.Rows).SelectMany(x => x)
            .Where(u => u.Raportichka.Date.Year == year && u.Raportichka.Date.Month == month);

        var students = group.Students.OrderBy(u => u.LastName);
        
        var directory = $"{Constants.VACATIONVEDOMOST_ATTENDANCE_PATH}{year}/{month}/";
        var patch = $"{directory}{request.GroupId}.xls";

        if (File.Exists(patch))
            File.Delete(patch);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.Copy($"{Constants.VACATIONVEDOMOST_ATTENDANCE_PATH}vedomost_pattern.xls", patch);

        var wb = WorkBook.Load(patch);
        var ws = wb.GetWorkSheet("Лист1");
        ws.Rows[0].Columns[0].Value = $"Ведомость посещаемости гр. {group.Speciality.NameAbbreviation}-{group.Course}{group.Number} за {request.Month.GetNameValue()} {year}г.";;
        ws.Rows[46].Columns[24].Value = students.Count().ToString();
        ws.Rows[45].Columns[12].Value = getFIO(group.ClassroomTeacher);
        ws.Rows[46].Columns[9].Value = getFIO(group.Department.DepartmentHead);

        var countStatementsRow = rows.Where(u => u.Cause == RaportichkaCause.STATEMENTS).Sum(u => u.Hours);
        var countSicknessRow = rows.Where(u => u.Cause == RaportichkaCause.SICKNESS).Sum(u => u.Hours);
        var countAbsenteeismRow = rows.Where(u => u.Cause == RaportichkaCause.ABSENTEEISM).Sum(u => u.Hours);
        var countPrikazRow = rows.Where(u => u.Cause == RaportichkaCause.PRIKAZ).Sum(u => u.Hours);

        ws.Rows[43].Columns[33].Value = countStatementsRow;
        ws.Rows[43].Columns[34].Value = countSicknessRow;
        ws.Rows[43].Columns[35].Value = countAbsenteeismRow;
        ws.Rows[43].Columns[36].Value = countPrikazRow;

        if (group.Headman != null)
        {
            ws.Rows[44].Columns[13].Value = getFIO(group.Headman);
        }

        foreach (var (student, index) in students.WithIndex())
        {
            ws.Rows[index + 3].Columns[1].Value = getFIO(student);
            var studentRows = rows.Where(u => u.Student.Id == student.Id);
            var countStudentStatementsRow = studentRows.Where(u => u.Cause == RaportichkaCause.STATEMENTS).Sum(u => u.Hours);
            var countStudentSicknessRow = studentRows.Where(u => u.Cause == RaportichkaCause.SICKNESS).Sum(u => u.Hours);
            var countStudentAbsenteeismRow = studentRows.Where(u => u.Cause == RaportichkaCause.ABSENTEEISM).Sum(u => u.Hours);
            var countStudentPrikazRow = studentRows.Where(u => u.Cause == RaportichkaCause.PRIKAZ).Sum(u => u.Hours);

            if(countStudentStatementsRow > 0)
            {
                ws.Rows[index + 3].Columns[33].Value = countStudentStatementsRow;
            }

            if (countStudentSicknessRow > 0)
            {
                ws.Rows[index + 3].Columns[34].Value = countStudentSicknessRow;
            }

            if (countStudentAbsenteeismRow > 0)
            {
                ws.Rows[index + 3].Columns[35].Value = countStudentAbsenteeismRow;
            }

            if (countStudentPrikazRow > 0)
            {
                ws.Rows[index + 3].Columns[36].Value = countStudentPrikazRow;
            }

            for (int i = 1; i < countDays; i++)
            {
                var rowHorseCount = studentRows
                    .Where(u => u.Raportichka.Date.Day == i)
                    .Sum(u => u.Hours);
                
                if(rowHorseCount > 0)
                {
                    ws.Rows[index + 3].Columns[i + 1].Value = rowHorseCount;
                }
            }
        }

        for (int i = 1; i < countDays; i++)
        {
            var sum = rows.Where(u => u.Raportichka.Date.Day == i).Sum(u => u.Hours);

            if(sum > 0)
            {
                ws.Rows[43].Columns[i + 1].Value = sum;
            }
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