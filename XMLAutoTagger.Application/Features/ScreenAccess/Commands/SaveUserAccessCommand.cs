using System;
using System.Collections.Generic;
using MediatR;

namespace XMLAutoTagger.Application.Features.ScreenAccess.Commands
{
    public class ScreenOverrideInput
    {
        public Guid ScreenId { get; set; }
        public string PermLevel { get; set; } = "none";
    }

    public class SaveUserAccessCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public List<ScreenOverrideInput> Overrides { get; set; } = new();
    }
}
