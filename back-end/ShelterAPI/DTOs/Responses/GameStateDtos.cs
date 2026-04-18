namespace ShelterAPI.DTOs.Responses
{
    public record CreateGameResponse(Guid GameId, Guid PlayerId, string Code);

    public record JoinGameResponse(Guid GameId, Guid PlayerId);

    public record CardDto(Guid Id, string Type, string? Content, bool Revealed);

    public record SpecialCardDto(Guid Id, string Title, string Description, bool Played);

    public record PlayerDto(
        Guid Id,
        string Name,
        List<CardDto> Cards,
        SpecialCardDto? SpecialCard,
        bool IsExiled,
        bool IsSpeaking);

    public record BunkerCardDto(Guid Id, int Slot, string? Content, bool Revealed);

    public record CatastropheDto(Guid Id, string Title, string Description);

    public record VoteRecordDto(Guid VoterId, string VoterName, Guid? TargetId);

    public record GameStateResponse(
        Guid Id,
        string Code,
        string Phase,
        int Round,
        int VotingInRound,
        int MaxVotingsInRound,
        List<PlayerDto> Players,
        Guid? ActiveSpeakerId,
        CatastropheDto Catastrophe,
        List<BunkerCardDto> BunkerCards,
        List<VoteRecordDto> Votes,
        Guid? MyVote,
        Guid? ExiledPlayerId,
        int BunkerCapacity,
        bool IsHost,
        Guid MyId);
}
