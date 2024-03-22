namespace Server.Objects.DTOs.Client
{
    public class LoginPassiveClientRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginPassiveClientRequestDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}