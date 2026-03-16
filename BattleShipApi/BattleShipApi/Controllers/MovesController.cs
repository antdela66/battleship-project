using BattleShipApi.Data;
using BattleShipApi.Dtos;
using BattleShipApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BattleShipApi.Controllers
{
    [Route("api/match")]
    [ApiController]
    public class MovesController : ControllerBase
    {
        private readonly DataContext _context;

        public MovesController(DataContext context) 
        {
            _context = context; 
        }

        [HttpPost("{MatchCode}/fire")]
        public async Task<ActionResult<FireShotResponse>> FireShot( string MatchCode, [FromBody] FireShotRequest req)
        {
            if (req == null)
                return BadRequest("request body is null");
            if (req.X < 0 || req.X > 9 || req.Y < 0 || req.Y > 9)
                return BadRequest(new { message = "Invalid coordinates" });

            var match = _context.Matches
                .Include(m => m.MatchPlayers)
                .FirstOrDefault(m => m.MatchCode.ToString() == MatchCode);

            if (match == null)
                return BadRequest("Match not found");
            if (match.Status != "InProgress")
                return BadRequest(new { message = "Match is not in progress" });

            var attacker = _context.Players
                .FirstOrDefault(p => p.PlayerId == req.PlayerId);
            if (attacker == null)
                return BadRequest("Player not found");
            if (match.CurrentTurn != attacker.PlayerNumber.ToString())
                return BadRequest(new { message = "It's not your turn" });

            var defender = _context.Players
                .FirstOrDefault(p => p.MatchId == match.MatchId && p.PlayerNumber != attacker.PlayerNumber);
            if (defender == null)
                return BadRequest(new { message = "Defender not found" });

            var existingMove = await _context.Moves
                .AnyAsync(m => 
                    m.MatchId == match.MatchId && 
                    m.AttackerId == attacker.PlayerId &&
                    m.X == req.X && 
                    m.Y == req.Y);

            if (existingMove)
                return BadRequest(new { message = "You have already fired at this position" });

            var hitCell = await _context.ShipStatus
                .Where(sc => sc.X == req.X && sc.Y == req.Y)
                .Join(
                    _context.Ships.Where(s => s.PlayerId == defender.PlayerId),
                    sc => sc.ShipId,
                    s => s.ShipsId,
                    (sc, s) => new { sc, s }
                    )
                .FirstOrDefaultAsync();

            bool isHit = hitCell != null;
            bool isSunk = false;
            bool isGameOver = false;
            string result = string.Empty;
            string winner = string.Empty;
            string message;

            if (isHit)
            {
                hitCell!.sc.IsHit = true;

                var shipCells = await _context.ShipStatus
                    .Where(sc => sc.ShipId == hitCell.s.ShipsId)
                    .ToListAsync();

                isSunk = shipCells.All(sc => sc.IsHit);

                var remainingCells = await _context.ShipStatus
                    .Join(
                        _context.Ships.Where(s => s.PlayerId == defender.PlayerId),
                        sc => sc.ShipId,
                        s => s.ShipsId,
                        (sc, s) => sc
                    )
                    .CountAsync(sc => !sc.IsHit);

                Console.WriteLine($"Remaining cells: {remainingCells}");
                Console.WriteLine($"Winner before save: {match.Winner}");
                Console.WriteLine($"Status before save: {match.Status}");

                if (remainingCells == 0)
                {
                    isGameOver = true;
                    result = "Sunk";
                    message = $"Congratulations {attacker.PlayerName}, you won!";
                    winner = attacker.PlayerName;

                    match.Winner = attacker.PlayerName;
                    match.Status = "Completed";
                    match.EndTime = DateTime.UtcNow;
                }
                else if (isSunk)
                {
                    result = "Sunk";
                    message = "Hit and Sunk a Ship!";
                }
                else
                {
                    result = "Hit";
                    message = "You have Hit a Ship!";
                }
            }

            else
            {
                result = "Miss";
                message = "You have Missed!";
            }
            var move = new Moves
            {
               MatchId = match.MatchId,
               AttackerId = attacker.PlayerId,
               DefenderId = defender.PlayerId,
               X = req.X,
               Y = req.Y,
               Result = result,
               MoveTime = DateTime.UtcNow
            };

            _context.Moves.Add(move);

            if (!isGameOver)
            {
                match.CurrentTurn = defender.PlayerNumber.ToString();
            }

            await _context.SaveChangesAsync();

            return Ok(new FireShotResponse
            {
                IsHit = isHit,
                IsSunk = isSunk,
                IsGameOver = isGameOver,
                Result = result,
                Message = message,
                CurrentTurn = match.CurrentTurn,
                Winner = match.Winner
            });
        }

    }
}
