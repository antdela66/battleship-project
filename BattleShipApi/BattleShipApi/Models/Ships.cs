using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class Ships
    {
        [Key]
        public Guid ShipsId { get; set; }

        public Guid PlayerId { get; set; }

        public string ShipType { get; set; } = string.Empty;

        public int Size { get; set; }

        public string Orientation { get; set; } = string.Empty;

        public int StartX { get; set; }

        public int StartY { get; set; }
    }
}
