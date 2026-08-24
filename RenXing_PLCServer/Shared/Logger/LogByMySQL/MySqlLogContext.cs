using Shared.Config;
using Shared.Logger.ILogger.Models;
using Microsoft.EntityFrameworkCore;

namespace Shared.Logger.LogByMySQL
{
    public class MySqlLogContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql(Settings.ConfigData.LogDbConnString,
                ServerVersion.AutoDetect(Settings.ConfigData.LogDbConnString));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LogItem>(l =>
            {
                l.HasKey(o => o.Id);
                l.Property(o => o.Date).HasMaxLength(50);
                l.Property(o => o.Grade).HasMaxLength(20);
                l.Property(o => o.Source).HasMaxLength(1024);
                l.Property(o => o.Message).HasMaxLength(1024);
            });
        }

        public DbSet<LogItem> logitems { get; set; }
    }
}
