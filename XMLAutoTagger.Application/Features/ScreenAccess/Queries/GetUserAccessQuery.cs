using System;
using MediatR;
using XMLAutoTagger.Application.Features.ScreenAccess.DTOs;

namespace XMLAutoTagger.Application.Features.ScreenAccess.Queries
{
    public record GetUserAccessQuery(Guid UserId) : IRequest<UserAccessDetailDto?>;
}
