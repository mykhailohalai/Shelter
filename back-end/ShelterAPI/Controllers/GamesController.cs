using Microsoft.AspNetCore.Mvc;
using ShelterAPI.DTOs.Requests;
using ShelterAPI.Services;

namespace ShelterAPI.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController(IGameService gameService) : ControllerBase
    {
        // ─── CREATE / JOIN ──────────────────────────────────────────────────

        /// <summary>POST /api/games — створити нову гру</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameRequest req, CancellationToken ct)
        {
            var result = await gameService.CreateGameAsync(req.HostName, ct);
            return StatusCode(201, result);
        }

        /// <summary>POST /api/games/join — приєднатись за кодом</summary>
        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinGameRequest req, CancellationToken ct)
        {
            var result = await gameService.JoinGameAsync(req.Code, req.PlayerName, ct);
            return Ok(result);
        }

        // ─── STATE ──────────────────────────────────────────────────────────

        /// <summary>GET /api/games/{gameId}/state — стан гри</summary>
        [HttpGet("{gameId:guid}/state")]
        public async Task<IActionResult> GetState(Guid gameId, CancellationToken ct)
        {
            var playerId = GetPlayerId();
            var state = await gameService.GetStateAsync(gameId, playerId, ct);
            return Ok(state);
        }

        // ─── ACTIONS ────────────────────────────────────────────────────────

        /// <summary>POST /api/games/{gameId}/actions/start — розпочати гру (ведучий)</summary>
        [HttpPost("{gameId:guid}/actions/start")]
        public async Task<IActionResult> Start(Guid gameId, CancellationToken ct)
        {
            await gameService.StartGameAsync(gameId, GetPlayerId(), ct);
            return NoContent();
        }

        /// <summary>POST /api/games/{gameId}/actions/reveal-bunker — відкрити картку бункера</summary>
        [HttpPost("{gameId:guid}/actions/reveal-bunker")]
        public async Task<IActionResult> RevealBunker(Guid gameId, CancellationToken ct)
        {
            await gameService.RevealBunkerCardAsync(gameId, GetPlayerId(), ct);
            return NoContent();
        }

        /// <summary>POST /api/games/{gameId}/actions/reveal-card — гравець відкриває свою картку</summary>
        [HttpPost("{gameId:guid}/actions/reveal-card")]
        public async Task<IActionResult> RevealCard(Guid gameId, [FromBody] RevealCardRequest req, CancellationToken ct)
        {
            await gameService.RevealCardAsync(gameId, GetPlayerId(), req.CardId, ct);
            return NoContent();
        }

        /// <summary>POST /api/games/{gameId}/actions/vote — проголосувати</summary>
        [HttpPost("{gameId:guid}/actions/vote")]
        public async Task<IActionResult> Vote(Guid gameId, [FromBody] VoteRequest req, CancellationToken ct)
        {
            await gameService.VoteAsync(gameId, GetPlayerId(), req.TargetId, ct);
            return NoContent();
        }

        /// <summary>POST /api/games/{gameId}/actions/next-phase — перейти до наступної фази (ведучий)</summary>
        [HttpPost("{gameId:guid}/actions/next-phase")]
        public async Task<IActionResult> NextPhase(Guid gameId, CancellationToken ct)
        {
            await gameService.NextPhaseAsync(gameId, GetPlayerId(), ct);
            return NoContent();
        }

        /// <summary>POST /api/games/{gameId}/actions/play-special — розіграти спеціальну картку</summary>
        [HttpPost("{gameId:guid}/actions/play-special")]
        public async Task<IActionResult> PlaySpecial(Guid gameId, [FromBody] PlaySpecialRequest req, CancellationToken ct)
        {
            await gameService.PlaySpecialCardAsync(gameId, GetPlayerId(), req.TargetId, ct);
            return NoContent();
        }

        // ─── HELPER ─────────────────────────────────────────────────────────

        private Guid GetPlayerId()
        {
            var header = Request.Headers["X-Player-Id"].FirstOrDefault();
            return Guid.TryParse(header, out var id) ? id : Guid.Empty;
        }
    }
}
