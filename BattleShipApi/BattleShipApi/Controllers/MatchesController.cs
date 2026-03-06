using Microsoft.AspNetCore.Mvc;
using BattleShipApi.Data;
using BattleShipApi.Models;
using BattleShipApi.Dtos;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BattleShipApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly DataContext _context;

        public MatchController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CreateMatchResponse>> CreateMatch()
        {
            var match = new Match
            {
                MatchId = Guid.NewGuid(),
                MatchCode = new Random().Next(1000, 9999),
                Status = "Waiting for players",
                CreatedTime = DateTime.UtcNow
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            var response = new CreateMatchResponse
            {
                MatchId = match.MatchId,
                MatchCode = match.MatchCode
            };



            return Ok(response);
        }

        [HttpPost("join")]
        public async Task<ActionResult<JoinMatchResponse>> JoinMatch([FromBody] JoinMatchRequest req)
        {
            var match = await _context.Matches
            .Include(m => m.MatchPlayers)
            .FirstOrDefaultAsync(m => m.MatchCode == req.MatchCode);

            if (match == null)
            {
                return NotFound("Match not found");
            }

            if (match.Status != "Waiting for players")
            {
                return BadRequest("Match is not available for joining");
            }

            var player = new Player
            {
                PlayerId = Guid.NewGuid(),
                PlayerName = req.PlayerName,
                MatchId = match.MatchId
            };

            match.MatchPlayers.Add(player);
            if (match.MatchPlayers.Count == 2)
            {
                match.Status = "Ready to start";
            }


            await _context.SaveChangesAsync();


            return Ok(new JoinMatchResponse
            {
                MatchId = match.MatchId,
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                PlayerNumber = player.PlayerNumber
            });
        }

        [HttpGet("{matchId}")]

        public async Task<ActionResult<Match>> GetMatchState(Guid matchId)
        {
            var match = await _context.Matches
                .Include(m => m.MatchPlayers)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return NotFound("Match not found");
            }

            var playerCount = match?.MatchPlayers.Count ?? 0;

            if (playerCount > 0) {
                for (int i = 0; i < playerCount; i++)
                {
                    match?.MatchPlayers[i].PlayerNumber = i + 1; // Assign player numbers based on their order in the list
                }
            }

            if(match?.MatchPlayers.Count == 2)
            {
                match.Status = "Ready to start";
            }

            if(match?.MatchPlayers.Count < 2)
            {
                match.Status = "Waiting for players";
                
            }

            await _context.SaveChangesAsync();

            return Ok(match); // Return the match state (for demonstration purposes)
        }

    }
}
