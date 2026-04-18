namespace ShelterAPI.DTOs.Requests
{
    public class JoinGameRequest
    {
        public string Code { get; init; } = string.Empty;
        public string PlayerName { get; init; } = string.Empty;
    }
}
