namespace MainServer.Objects.DTOs.ClientModelDtos
{
    public class LoginUserRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginUserRequestDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}