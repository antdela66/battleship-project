using Microsoft.AspNetCore.Mvc;
using BattleShipApi.Data;
using BattleShipApi.Models;
using BattleShipApi.Dtos;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Web;

namespace BattleShipApi.Controllers
{
    [Route("api/match/{matchCode}")]
    [ApiController]
    public class PlacementController : ControllerBase
    {
        private readonly DataContext _context;
        public PlacementController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("placement")]

        public async Task<ActionResult> PlaceShip(string matchCode, [FromBody] PlacementRequest req)
        {
            if (req == null)
                return BadRequest(new { message = "request body is null" });
            if (req.PlayerId == Guid.Empty)
                return BadRequest(new { message = "PlayerId is required" });
            if (string.IsNullOrEmpty(req.ShipType))
                return BadRequest(new { message = "ShipType is required" });

            var match = await _context.Matches
                .Include(m => m.MatchPlayers)
                .FirstOrDefaultAsync(m => m.MatchCode.ToString() == matchCode);

            if (match == null)
                return NotFound(new { message = "Match not found" });

            if (match.Status != "PlacingShips")
                return BadRequest(new { message = "Match is not in placement phase" });

            var player = match.MatchPlayers.FirstOrDefault(p => p.PlayerId == req.PlayerId);

            if (player == null)
                return NotFound(new { message = "Player not found in this match" });

            var shipSize = req.ShipType.ToLower() switch
            {
                "carrier" => 5,
                "battleship" => 4,
                "cruiser" => 3,
                "submarine" => 3,
                "destroyer" => 2,
                _ => throw new ArgumentException("Invalid ship type")
            };

            if (req.Orientation != "Horizontal" && req.Orientation != "Vertical")
                return BadRequest(new { message = "Invalid orientation" });

            var alreadyPlacedShips = await _context.Ships
                .Where(s => s.PlayerId == req.PlayerId)
                .ToListAsync();

            if (alreadyPlacedShips.Count >= 5 ) {
                return BadRequest(new { message = "All ships placed" });
            }

            if (alreadyPlacedShips.Any(s => s.ShipType.ToLower() == req.ShipType.ToLower()))
                {
                return BadRequest(new { message = $"{req.ShipType} has already been placed" });
            }
            var newCells = new List<(int X, int Y)>();

            for (int i = 0; i < shipSize; i++)
            {
                int x = req.StartX + (req.Orientation == "Horizontal" ? i : 0);
                int y = req.StartY + (req.Orientation == "Vertical" ? i : 0);
                if (x < 0 || x >= 10 || y < 0 || y >= 10)
                    return BadRequest(new { message = "Ship placement is out of bounds" });
                newCells.Add((x, y));
            }

            

            foreach (var cell in newCells)
            {
                if (alreadyPlacedShips.Any(s =>
                    (s.StartX <= cell.X && cell.X < s.StartX + (s.Orientation == "Horizontal" ? s.Size : 1)) &&
                    (s.StartY <= cell.Y && cell.Y < s.StartY + (s.Orientation == "Vertical" ? s.Size : 1))))
                {
                    return BadRequest(new { message = "Ship placement overlaps with existing ship" });
                }
            }

            var ship = new Ships
            {
                ShipsId = Guid.NewGuid(),
                PlayerId = req.PlayerId,
                ShipType = req.ShipType,
                Size = shipSize,
                StartX = req.StartX,
                StartY = req.StartY,
                Orientation = req.Orientation
            };

            _context.Ships.Add(ship);
            await _context.SaveChangesAsync();

            var shipCells = newCells.Select(c => new ShipStatus
            {
                ShipId = ship.ShipsId,
                X = c.X,
                Y = c.Y,
                IsHit = false
            }).ToList();

            _context.ShipStatus.AddRange(shipCells);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ship placed successfully" });

        }

        [HttpGet("board/{playerId}")]

        public async Task<ActionResult<List<CellState>>> GetBoardState(string matchCode, Guid playerId)
        {
            var match = await _context.Matches
                .Include(m => m.MatchPlayers)
                .FirstOrDefaultAsync(m => m.MatchCode.ToString() == matchCode);

            if (match == null)
                return NotFound(new { message = "Match not found" });

            var player = match.MatchPlayers.FirstOrDefault(p => p.PlayerId == playerId);
            if (player == null)
                return NotFound(new { message = "Player not found in this match" });

            var ships = await _context.Ships
                .Where(s => s.PlayerId == playerId)
                .ToListAsync();

            var shipCells = await _context.ShipStatus
                .Where(ss => ships.Select(s => s.ShipsId).Contains(ss.ShipId))
                .ToListAsync();

            var boardState = new List<CellState>();

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var cell = shipCells.FirstOrDefault(c => c.X == x && c.Y == y);
                    
                    string cellStatus = "empty";

                    if(cell != null)
                    {
                        cellStatus = cell.IsHit ? "hit" : "ship";
                    }

                    boardState.Add(new CellState
                    {
                        X = x,
                        Y = y,
                        Status = cell != null ? (cell.IsHit ? "Hit" : "Ship") : "Empty"
                    });
                }
            }



            return Ok(boardState);
        }


        [HttpPost("ready")]
        public async Task<ActionResult> PlayerReady(string matchCode, [FromBody] ReadyRequest req)
        {
            var match = await _context.Matches
                .Include(m => m.MatchPlayers)
                .FirstOrDefaultAsync(m => m.MatchCode.ToString() == matchCode);

            if (match == null)
                return NotFound(new { message = "Match not found" });

            if (match.Status != "PlacingShips")
                return BadRequest(new { message = "Match is not in placement phase" });

            var player = match.MatchPlayers
                .FirstOrDefault(p => p.PlayerId == req.PlayerId);

            if (player == null)
                return NotFound(new { message = "Player not found in this match" });

            var shipCount = await _context.Ships
                .CountAsync(s => s.PlayerId == req.PlayerId);

            if (shipCount != 5)
                return BadRequest(new { message = "All 5 ships must be placed before readying up" });

            player.IsReady = true;

            if (match.MatchPlayers.All(p => p.IsReady))
            {
                match.Status = "InProgress";
                match.CurrentTurn = "1";
                match.StartTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Player is ready" });
        }
     
    }
}
