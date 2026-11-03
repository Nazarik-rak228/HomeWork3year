using API_First_Lesson.Models;
namespace API_First_Lesson.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
    }
}
