namespace BattleShipApi.Dtos
{
    public class MoveResult
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Result { get; set; } = string.Empty;
        public DateTime MoveTime { get; set; }

    }
}
