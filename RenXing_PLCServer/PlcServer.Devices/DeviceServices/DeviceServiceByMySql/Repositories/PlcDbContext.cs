using Microsoft.EntityFrameworkCore;
using PlcServer.Devices.Models;
using Shared.Config;

namespace PlcServer.Devices.DeviceServices.DeviceServiceByMySql.Repositories
{
    public class PlcDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql(Settings.ConfigData.PlcSvrDbConnString,
                ServerVersion.AutoDetect(Settings.ConfigData.PlcSvrDbConnString));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PlcDevice>(d =>
            {
                d.HasKey(o => o.PlcId);
                d.Property(o => o.PlcName).IsRequired().HasMaxLength(50);
                d.Property(o => o.DriverAssemblyName).IsRequired().HasMaxLength(100);
                d.Property(o => o.DriverClassName).IsRequired().HasMaxLength(100);
                d.Property(o => o.ConnectParameter).HasMaxLength(100);
            });
            modelBuilder.Entity<PlcNode>(n =>
            {
                n.HasKey(o => o.NodeId);
                n.Property(o => o.NodeName).IsRequired().HasMaxLength(50);
                n.Property(o => o.NodeAddr).IsRequired().HasMaxLength(50);
                n.Property(o => o.NodeAccess).IsRequired().HasMaxLength(20);
                n.Property(o => o.NodeType).IsRequired().HasMaxLength(20);
                n.Property(o => o.PlcName).IsRequired().HasMaxLength(50);
                n.Property(o => o.Remark).HasMaxLength(100);
            });
        }

        public DbSet<PlcDevice>? PlcDevices { get; set; }

        public DbSet<PlcNode>? PlcNodes { get; set; }
    }
}
