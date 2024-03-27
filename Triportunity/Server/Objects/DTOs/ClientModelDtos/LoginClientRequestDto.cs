namespace Server.Objects.DTOs.ClientModelDtos
{
    public class LoginClientRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginClientRequestDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}