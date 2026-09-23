namespace API_First_Lesson.Models
{
    public class UserUpdateDTO
    {
        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }
}