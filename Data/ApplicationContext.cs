using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pedidos.Domain;
using System;
using System.Linq;

namespace Pedidos.Data
{
    public class ApplicationContext : DbContext
    {
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(p => p.AddConsole());
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging()
                .UseSqlServer("Data source=(localdb)\\v11.0;Initial Catalog=CursoEFCore; Integrated Security=true;",

                // abaixo configuração de Resiliencia de conexao
                p => p.EnableRetryOnFailure(
                    maxRetryCount: 2, 
                    maxRetryDelay: TimeSpan.FromSeconds(5), 
                    errorNumbersToAdd: null)
                .MigrationsHistoryTable("curso_ef_core")
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1 Opção
            // modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            // modelBuilder.ApplyConfiguration(new PedidoConfiguration());
            // ...

            // 2 Opção
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }

        protected void OnConfiguring(ModelBuilder optionsBuilder)
        {
            foreach (var item in optionsBuilder.Model.GetEntityTypes())
            {
                var properties = item.GetProperties().Where(p => p.ClrType == typeof(string));
                foreach (var prop in properties)
                {
                    if (string.IsNullOrEmpty(prop.GetColumnType())&& ! prop.GetMaxLength().HasValue)
                    {
                        prop.SetMaxLength(100);
                        prop.SetColumnType("VARCHAR(100)");
                    }
                }
            }
        }

        }
}
