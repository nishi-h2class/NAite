using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NAiteEntities.Models
{
    public partial class NAiteContext : DbContext
    {
        private string _connectionString = string.Empty;

        private MySqlServerVersion _serverVersion;

        public NAiteContext()
        {

        }

        public NAiteContext(string connectionString, MySqlServerVersion serverVersion)
        {
            _connectionString = connectionString;
            _serverVersion = serverVersion;
        }

        public NAiteContext(DbContextOptions<NAiteContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ItemField> ItemFields { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<ItemFile> ItemFiles { get; set; }
        public DbSet<ItemRow> ItemRows { get; set; }
        public DbSet<ItemDataImport> ItemDataImports { get; set; }
        public DbSet<ItemDataImportField> ItemDataImportFields { get; set; }
        public DbSet<ItemData> ItemDatas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString) && _serverVersion != null)
            {
                optionsBuilder.UseMySql(_connectionString, _serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.LoginId)
                    .IsUnicode(false);
            });

            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
