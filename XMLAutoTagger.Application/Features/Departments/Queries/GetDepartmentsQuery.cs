using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using XMLAutoTagger.Application.Features.Departments.DTOs;

namespace XMLAutoTagger.Application.Features.Departments.Queries
{
    public record GetDepartmentsQuery : IRequest<List<DepartmentDto>>;
}
