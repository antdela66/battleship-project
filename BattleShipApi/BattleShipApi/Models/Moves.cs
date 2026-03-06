using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class Moves
    {
        [Key]
        public int MovesId { get; set; }

        public int MatchId { get; set; }

        public int AttackerId { get; set; }

        public int DefenderId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Result { get; set; } = string.Empty;

        public DateTime MoveTime { get; set; }
    }
}
