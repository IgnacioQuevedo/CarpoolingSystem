namespace Server.Objects.DTOs.Client
{
    public class LoginClientPassiveRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginClientPassiveRequestDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}