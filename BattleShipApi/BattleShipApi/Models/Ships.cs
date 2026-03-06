using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class Ships
    {
        [Key]
        public int ShipsId { get; set; }

        public Guid PlayerId { get; set; }

        object ShipType { get; set; } = new object();

        public int Size { get; set; }

        public string Orientation { get; set; } = string.Empty;

        public int StartX { get; set; }

        public int StartY { get; set; }
    }
}
