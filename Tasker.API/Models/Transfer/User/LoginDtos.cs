namespace Tasker.API.Models.Transfer.User
{
    public class LoginRequestModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    public class LoginResponseModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // public required LoginTokenResponseModel Tokens { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
    // public class LoginTokenResponseModel
    // {
    //     public string AccessToken { get; set; } = string.Empty;
    // }
}
