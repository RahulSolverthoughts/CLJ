using System;
using MediatR;
using XMLAutoTagger.Application.Features.Users.DTOs;

namespace XMLAutoTagger.Application.Features.Profile.Queries
{
    public class GetMyProfileQuery : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }

        public GetMyProfileQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
