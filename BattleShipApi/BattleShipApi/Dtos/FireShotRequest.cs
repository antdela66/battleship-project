namespace BattleShipApi.Dtos
{
    public class FireShotRequest
    {
        public Guid PlayerId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

    }
}
