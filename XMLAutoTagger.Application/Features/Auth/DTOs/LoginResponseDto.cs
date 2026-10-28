using System;

namespace XMLAutoTagger.Application.Features.Auth.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string AvatarInitials { get; set; } = string.Empty;
        public string AvatarColourHex { get; set; } = "#185FA5";
    }
}
