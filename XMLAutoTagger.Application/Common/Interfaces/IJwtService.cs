using System;
using System.Collections.Generic;
using System.Text;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
