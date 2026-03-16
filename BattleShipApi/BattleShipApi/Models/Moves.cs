using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class Moves
    {
        [Key]
        public Guid MovesId { get; set; }

        public Guid MatchId { get; set; }

        public Guid AttackerId { get; set; }

        public Guid DefenderId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Result { get; set; } = string.Empty;

        public DateTime MoveTime { get; set; }
    }
}
