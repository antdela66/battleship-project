namespace BattleShipApi.Dtos
{
    public class JoinMatchRequest
    {
        public int MatchCode { get; set; }
        public string PlayerName { get; set; } = string.Empty;
    }
}
