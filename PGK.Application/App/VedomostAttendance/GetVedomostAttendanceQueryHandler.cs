﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PGK.Application.Common;
using PGK.Application.Interfaces;

namespace Market.Application.App.VedomostAttendance;

public class GetVedomostAttendanceQueryHandler : IRequestHandler<GetVedomostAttendanceQuery, Stream>
{
    private readonly IPGKDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetVedomostAttendanceQueryHandler(IPGKDbContext dbContext, IMapper mapper) =>
        (_dbContext, _mapper) = (dbContext, mapper);
    
    public async Task<Stream> Handle(GetVedomostAttendanceQuery request, CancellationToken cancellationToken)
    {
        var month = request.Date.Month;
        var year = request.Date.Year;
        
        var rows = _dbContext.RaportichkaRows
            .Include(u => u.Raportichka)
                .ThenInclude(u => u.Group)
            .Where(u => u.Raportichka.Group.Id == request.GroupId)
            .Where(u => u.Raportichka.Date.Month == month && u.Raportichka.Date.Year == year);

        var students = await _dbContext.StudentsUsers
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Group.Id == request.GroupId);

        var patch = $"{Constants.VACATIONVEDOMOST_ATTENDANCE_PATH}${year}/${month}/${request.GroupId}.xls";
        
        if(File.Exists(patch)) 
            File.Delete(patch);

        File.Create(patch);

        using var package = new ExcelPackage(patch);

        return package.Stream;
    }
}