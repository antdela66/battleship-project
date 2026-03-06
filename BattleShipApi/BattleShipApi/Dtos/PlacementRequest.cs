namespace BattleShipApi.Dtos
{
    public class PlacementRequest
    {
        public Guid PlayerId { get; set; }

        public string ShipType { get; set; } = string.Empty;
        public int StartX { get; set; }
        public int StartY { get; set; }
        public string Orientation { get; set; } = string.Empty;
    }
}
