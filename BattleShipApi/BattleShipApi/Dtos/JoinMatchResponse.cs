namespace BattleShipApi.Dtos
{
    public class JoinMatchResponse
    {
        public Guid MatchId { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int PlayerNumber { get; set; }
    }
}
