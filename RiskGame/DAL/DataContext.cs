using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RiskGame.DAL
{
    public class DataContext : DbContext
    {
        public DataContext()
          : base("DefaultConnection")
        {
            Database.SetInitializer<DataContext>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Risk> Risk { get; set; }
        public DbSet<RiskOption> RiskOption { get; set; }
        public DbSet<GameRoom> GameRoom { get; set; }
        public DbSet<GameBattle> GameBattle { get; set; }
        public DbSet<UserGameBattle> UserGameBattle { get; set; }
        public DbSet<UserGameRoom> UserGameRoom { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<User>().ToTable("Users").HasKey(u => new { u.UserId });
            builder.Entity<Risk>().ToTable("Risk").HasKey(u => new { u.RiskId });
            builder.Entity<RiskOption>().ToTable("RiskOption").HasKey(u => new { u.RiskOptionId });
            builder.Entity<GameRoom>().ToTable("GameRoom").HasKey(u => new { u.GameRoomId });
            builder.Entity<GameBattle>().ToTable("GameBattle").HasKey(u => new { u.GameBattleId });
            builder.Entity<UserGameBattle>().ToTable("UserGameBattle").HasKey(u => new { u.UserGameBattleId });
            builder.Entity<UserGameRoom>().ToTable("UserGameRoom").HasKey(u => new { u.UserGameRoomId });

        }
    }
}