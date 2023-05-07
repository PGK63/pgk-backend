using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        var fileInfo = new FileInfo(patch);
        var package = new ExcelPackage(fileInfo);
        var ws = package.Workbook.Worksheets["Лист1"];
        
        ws.Cells[1, 1].Value = $"Ведомость посещаемости гр. {group.Speciality.NameAbbreviation}-{group.Course}{group.Number} за {request.Month.GetNameValue()} {year}г.";;
        ws.Cells[47, 29].Value = students.Count().ToString();
        ws.Cells[46, 13].Value = getFIO(group.ClassroomTeacher);
        ws.Cells[47, 10].Value = getFIO(group.Department.DepartmentHead);

        var countStatementsRow = rows.Where(u => u.Cause == RaportichkaCause.STATEMENTS).Sum(u => u.Hours);
        var countSicknessRow = rows.Where(u => u.Cause == RaportichkaCause.SICKNESS).Sum(u => u.Hours);
        var countAbsenteeismRow = rows.Where(u => u.Cause == RaportichkaCause.ABSENTEEISM).Sum(u => u.Hours);
        var countPrikazRow = rows.Where(u => u.Cause == RaportichkaCause.PRIKAZ).Sum(u => u.Hours);

        ws.Cells[44, 34].Value = countStatementsRow;
        ws.Cells[44, 35].Value = countSicknessRow;
        ws.Cells[44, 36].Value = countAbsenteeismRow;
        ws.Cells[44, 37].Value = countPrikazRow;

        if (group.Headman != null)
        {
            ws.Cells[45, 14].Value = getFIO(group.Headman);
        }

        foreach (var (student, index) in students.WithIndex())
        {
            ws.Cells[index + 4, 2].Value = getFIO(student);
            var studentRows = rows.Where(u => u.Student.Id == student.Id);
            var countStudentStatementsRow = studentRows.Where(u => u.Cause == RaportichkaCause.STATEMENTS).Sum(u => u.Hours);
            var countStudentSicknessRow = studentRows.Where(u => u.Cause == RaportichkaCause.SICKNESS).Sum(u => u.Hours);
            var countStudentAbsenteeismRow = studentRows.Where(u => u.Cause == RaportichkaCause.ABSENTEEISM).Sum(u => u.Hours);
            var countStudentPrikazRow = studentRows.Where(u => u.Cause == RaportichkaCause.PRIKAZ).Sum(u => u.Hours);

            if(countStudentStatementsRow > 0)
            {
                ws.Cells[index + 4, 34].Value = countStudentStatementsRow;
            }

            if (countStudentSicknessRow > 0)
            {
                ws.Cells[index + 4, 35].Value = countStudentSicknessRow;
            }

            if (countStudentAbsenteeismRow > 0)
            {
                ws.Cells[index + 4, 36].Value = countStudentAbsenteeismRow;
            }

            if (countStudentPrikazRow > 0)
            {
                ws.Cells[index + 4, 37].Value = countStudentPrikazRow;
            }

            for (int i = 1; i < countDays; i++)
            {
                var rowHorseCount = studentRows
                    .Where(u => u.Raportichka.Date.Day == i)
                    .Sum(u => u.Hours);
                
                if(rowHorseCount > 0)
                {
                    ws.Cells[index + 4, i + 2].Value = rowHorseCount;
                }
            }
        }

        for (int i = 1; i < countDays; i++)
        {
            var sum = rows.Where(u => u.Raportichka.Date.Day == i).Sum(u => u.Hours);

            if(sum > 0)
            {
                ws.Cells[44, i + 2].Value = sum;
            }
        }

        package.Save();
        
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