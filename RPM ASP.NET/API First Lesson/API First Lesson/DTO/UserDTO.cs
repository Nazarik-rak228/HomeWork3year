namespace API_First_Lesson.Models
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public int? CartId { get; set; }

        public bool IsActive { get; set; }
    }
}
