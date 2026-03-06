using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class Player
    {
        [Key]
        public Guid PlayerId { get; set; }

        public Guid MatchId { get; set; }
        public int PlayerNumber { get; set; } // 1 or 2 

        public string PlayerName { get; set; } = string.Empty;

        public bool IsReady { get; set; }

        public bool IsPlaying { get; set; }

        public bool IsBot { get; set; }

    }
}
