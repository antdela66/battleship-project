namespace BattleShipApi.Dtos
{
    public class FireShotResponse
    {
        public bool IsHit { get; set; }
        public bool IsSunk { get; set; }
        public bool IsGameOver { get; set; }
        public string Result { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string CurrentTurn { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;

    }
}
