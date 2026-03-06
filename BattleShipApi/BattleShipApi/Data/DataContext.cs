using BattleShipApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BattleShipApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Player> Players { get; set; }

        public DbSet<ShipStatus> ShipStatus { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<Ships> Ships { get; set; }

        public DbSet<Moves> Moves { get; set; }


    }
}
