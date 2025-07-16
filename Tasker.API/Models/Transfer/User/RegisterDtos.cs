namespace Tasker.API.Models.Transfer.User
{
    public class UserRegisterRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
