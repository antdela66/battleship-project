using System.ComponentModel.DataAnnotations;

namespace BattleShipApi.Models
{
    public class ShipStatus
    {
        [Key]
        public int ShipCellId { get; set; }

        public int ShipId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool IsHit { get; set; }

    }
}
