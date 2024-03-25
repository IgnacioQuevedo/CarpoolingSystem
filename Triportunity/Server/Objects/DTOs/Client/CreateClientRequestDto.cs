namespace Server.Objects.DTOs.Client
{
    public class CreateClientRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public CreateClientRequestDto(string username,string password)
        {
            Username = username;
            Password = password;
        }
    }
}