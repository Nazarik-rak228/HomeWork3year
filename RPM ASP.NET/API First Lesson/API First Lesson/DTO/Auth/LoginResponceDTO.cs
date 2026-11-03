namespace API_First_Lesson.DTO.Auth
{
    public class LoginResponceDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }
}
