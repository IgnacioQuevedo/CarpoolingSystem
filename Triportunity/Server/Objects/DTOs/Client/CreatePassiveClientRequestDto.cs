namespace Server.Objects.DTOs.Client
{
    public class CreatePassiveClientRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public CreatePassiveClientRequestDto(string username,string password)
        {
            Username = username;
            Password = password;
        }
    }
}