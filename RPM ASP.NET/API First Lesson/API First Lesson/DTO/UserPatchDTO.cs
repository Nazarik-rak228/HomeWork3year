namespace API_First_Lesson.Models
{
    public class UserPatchDTO
    {
        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        public int? RoleId { get; set; }

        public bool? IsActive { get; set; }
    }
}