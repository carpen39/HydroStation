using HydroStation.Hydro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SapphireDb;
using System;
using FileContextCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace HydroStation.Storage
{
    // Change DbContext to SapphireDbContext
    public class HydroContext : DbContext
    {
        public HydroContext() : base()
        {
        }

        public HydroContext(DbContextOptions<HydroContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseFileContextDatabase(location: Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Database"));
        }

        public DbSet<DeviceData> DeviceData { get; set; }
        public DbSet<DeviceConfig> DeviceConfig { get; set; }
    }
}
