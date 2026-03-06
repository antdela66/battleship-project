using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class Match
    {
        [Key]
        public Guid MatchId { get; set; }

        public int MatchCode { get; set; }

        public List<Player> MatchPlayers { get; set; } = new List<Player>();

        public string Status { get; set; } = string.Empty;

        public string CurrentTurn { get; set; } = string.Empty;

        public string Winner { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
