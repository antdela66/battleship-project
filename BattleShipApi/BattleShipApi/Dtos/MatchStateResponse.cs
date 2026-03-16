namespace BattleShipApi.Dtos
{
    public class MatchStateResponse
    {
        public string MatchCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CurrentTurn { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;

        public Guid PlayerId { get; set; }
        public int PlayerNumber { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string OpponentName { get; set; } = string.Empty;

        public bool PlayerIsReady { get; set; }
        public bool OpponentIsReady { get; set; }

        public List<CellState> PlayerGrid { get; set; } = new();
        public List<CellState> OpponentGrid { get; set; } = new();
        public List<MoveResult> Moves { get; set; } = new();
    }
}
