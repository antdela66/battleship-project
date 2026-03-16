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
                CreatedTime = DateTime.UtcNow ,
                Winner = ""
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            var player = new Player()
            {
                
            };
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

            var playerNumber = match.MatchPlayers.Count + 1;

            if(playerNumber > 2)
            {
                return BadRequest("Match is already full");
            }

            var player = new Player
            {
                PlayerId = Guid.NewGuid(),
                PlayerName = req.PlayerName,
                MatchId = match.MatchId,
                PlayerNumber = playerNumber
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var playerCount = await _context.Players.CountAsync(p => p.MatchId == match.MatchId);

            if (playerCount == 2)
            {
                match.Status = "PlacingShips";
                await _context.SaveChangesAsync();
            }

            return Ok(new JoinMatchResponse
            {
                MatchId = match.MatchId,
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                PlayerNumber = player.PlayerNumber
            });
        }

        [HttpGet("{matchCode}/state")]

        public async Task<ActionResult<MatchStateResponse>> GetMatchState(string matchCode, [FromQuery] Guid playerId)
        {
            if (playerId == Guid.Empty)
            {
                return BadRequest("PlayerId is required");
            }

            var match = await _context.Matches
                .Include(m => m.MatchPlayers)
                .FirstOrDefaultAsync(m => m.MatchCode.ToString() == matchCode);

            if (match == null)
            {
                return NotFound("Match not found");
            }

            var player = match.MatchPlayers
                .FirstOrDefault(p => p.PlayerId == playerId);

            if (player == null) {
                return NotFound("Player not found in this match");
            }

            var opponent = match.MatchPlayers
                .FirstOrDefault(p => p.PlayerId != playerId);

            var yourShips = await _context.Ships
                .Where(s => s.PlayerId == playerId)
                .ToListAsync();

            var yourShipIds = yourShips
                .Select(s => s.ShipsId).ToList();

            var yourShipCells = await _context.ShipStatus
                .Where(c => yourShipIds.Contains(c.ShipId))
                .ToListAsync();

            var opponentShipsIds = new List<Ships>();
        
            var opponentShipIds = new List<Guid>();

            var opponentShipCells = new List<ShipStatus>();

            if(opponent != null) {
                opponentShipsIds = await _context.Ships
                    .Where(s => s.PlayerId == opponent.PlayerId)
                    .ToListAsync();
                opponentShipIds = opponentShipsIds
                    .Select(s => s.ShipsId).ToList();
                opponentShipCells = await _context.ShipStatus
                    .Where(c => opponentShipIds.Contains(c.ShipId))
                    .ToListAsync();
            }

            var moves = await _context.Moves
                .Where(m => m.MatchId == match.MatchId)
                .OrderBy(match => match.MoveTime)
                .ToListAsync();

            var yourBoard = new List<CellState>();
            var opponentBoard = new List<CellState>();

            for (int y =0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var yourCell = yourShipCells.FirstOrDefault(c => c.X == x && c.Y == y);

                    var shotAtYourCell = moves.FirstOrDefault(m => 
                        m.DefenderId == player.PlayerId && 
                        m.AttackerId == opponent!.PlayerId &&
                        m.X == x && 
                        m.Y == y
                     );

                    string yourCellStatus = "Empty";

                    if (yourCell != null)
                    {
                        var allShipCells = yourShipCells
                            .Where(c => c.ShipId == yourCell.ShipId)
                            .ToList();

                        var shipSunk = allShipCells.All(c => c.IsHit);

                        if (shipSunk)
                            yourCellStatus = "Sunk";
                        else if (yourCell.IsHit)
                            yourCellStatus = "Hit";
                        else
                            yourCellStatus = "Ship";
                    }
                    else if (shotAtYourCell != null && shotAtYourCell.Result == "Miss")
                    {
                        yourCellStatus = "Miss";
                    }
                   

                    yourBoard.Add(new CellState
                    {
                        X = x,
                        Y = y,
                        Status = yourCellStatus
                    });

                    var shotAtOpponentCell = opponent == null ? null : moves.FirstOrDefault(c =>
                        c.AttackerId == player.PlayerId &&
                        c.DefenderId == opponent!.PlayerId &&
                        c.X == x &&
                        c.Y == y
                     );

                    var opponentCell = opponentShipCells.FirstOrDefault(c => c.X == x && c.Y ==y);

                    string opponentCellStatus = "Unknown";

                    if (shotAtOpponentCell != null)
                    {
                        if (shotAtOpponentCell.Result == "Miss")
                        {
                            opponentCellStatus = "Miss";
                        }
                        else if (opponentCell != null)
                        {
                            var allOpponentShipCells = opponentShipCells
                                .Where(c => c.ShipId == opponentCell.ShipId)
                                .ToList();

                            var opponentShipSunk = allOpponentShipCells.All(c => c.IsHit);

                            opponentCellStatus = opponentShipSunk ? "Sunk" : "Hit";
                        }
                    }
                    
                    opponentBoard.Add(new CellState
                    {
                        X = x,
                        Y = y,
                        Status = opponentCellStatus
                    });
                }
            }

            var MoveResults = moves.Select(m => new MoveResult
            {
                X = m.X,
                Y = m.Y,
                Result = m.Result,
                MoveTime = m.MoveTime
            }).ToList();

                var response = new MatchStateResponse
                {
                    MatchCode = match.MatchCode.ToString(),
                    Status = match.Status,
                    CurrentTurn = match.CurrentTurn,
                    Winner = match.Winner,
                    PlayerId = player.PlayerId,
                    PlayerNumber = player.PlayerNumber,
                    PlayerName = player.PlayerName,
                    OpponentName = opponent != null ? opponent.PlayerName: "Waiting for opponent",
                    PlayerIsReady = player.IsReady,
                    OpponentIsReady = opponent != null ? opponent.IsReady : false,
                    PlayerGrid = yourBoard,
                    OpponentGrid = opponentBoard,
                    Moves = MoveResults
                };

            return Ok(response);
        }

    }
}
