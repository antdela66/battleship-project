namespace BattleShipApi.Dtos
{
    public class MatchStatusResponse
    {
        public Guid MatchId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string CurrentTurn { get; set; } = string.Empty;

        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public bool IsPlayer1Turn { get; set; }
        public bool IsMatchOver { get; set; }
    }
}
