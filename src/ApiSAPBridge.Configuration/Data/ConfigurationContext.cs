using Microsoft.EntityFrameworkCore;
using ApiSAPBridge.Configuration.Models;

namespace ApiSAPBridge.Configuration.Data
{
    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext(DbContextOptions<ConfigurationContext> options) : base(options)
        {
        }

        public DbSet<SqlConfiguration> SqlConfigurations { get; set; }
        public DbSet<MethodConfiguration> MethodConfigurations { get; set; }
        public DbSet<SwaggerConfiguration> SwaggerConfigurations { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<SecurityConfiguration> SecurityConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de SqlConfiguration
            modelBuilder.Entity<SqlConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Server).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Database).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Username).HasMaxLength(255);
                entity.Property(e => e.Password).HasMaxLength(500);
                entity.HasIndex(e => new { e.Server, e.Database }).IsUnique();
            });

            // Configuración de MethodConfiguration
            modelBuilder.Entity<MethodConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MethodName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.HttpMethod).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Endpoint).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => new { e.MethodName, e.HttpMethod }).IsUnique();
            });

            // Configuración de SwaggerConfiguration
            modelBuilder.Entity<SwaggerConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MethodName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.HttpMethod).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Endpoint).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => new { e.MethodName, e.HttpMethod }).IsUnique();
            });

            // Configuración de SystemConfiguration
            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Value).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.Key).IsUnique();
            });

            // Configuración de SecurityConfiguration
            modelBuilder.Entity<SecurityConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            });

            // Datos semilla
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Configuración por defecto del sistema
            modelBuilder.Entity<SystemConfiguration>().HasData(
                new SystemConfiguration
                {
                    Id = 1,
                    Key = "DefaultSyncInterval",
                    Value = "30",
                    Description = "Intervalo por defecto de sincronización en minutos"
                },
                new SystemConfiguration
                {
                    Id = 2,
                    Key = "MaxLoginAttempts",
                    Value = "3",
                    Description = "Número máximo de intentos de login antes del bloqueo"
                },
                new SystemConfiguration
                {
                    Id = 3,
                    Key = "LockoutDurationMinutes",
                    Value = "15",
                    Description = "Duración del bloqueo en minutos tras exceder intentos"
                }
            );

            // Configuración de seguridad por defecto (contraseña: "admin123")
            modelBuilder.Entity<SecurityConfiguration>().HasData(
                new SecurityConfiguration
                {
                    Id = 1,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
                }
            );

            // Métodos por defecto del API
            modelBuilder.Entity<MethodConfiguration>().HasData(
                new MethodConfiguration
                {
                    Id = 1,
                    MethodName = "SyncCustomers",
                    HttpMethod = "POST",
                    Endpoint = "/api/sync/customers",
                    IsEnabled = true,
                    IsAutomaticSync = true,
                    SyncIntervalMinutes = 30,
                    Description = "Sincronización de clientes desde SAP"
                },
                new MethodConfiguration
                {
                    Id = 2,
                    MethodName = "SyncProducts",
                    HttpMethod = "POST",
                    Endpoint = "/api/sync/products",
                    IsEnabled = true,
                    IsAutomaticSync = false,
                    SyncIntervalMinutes = 60,
                    Description = "Sincronización de productos desde SAP"
                },
                new MethodConfiguration
                {
                    Id = 3,
                    MethodName = "SyncOrders",
                    HttpMethod = "POST",
                    Endpoint = "/api/sync/orders",
                    IsEnabled = false,
                    IsAutomaticSync = false,
                    SyncIntervalMinutes = 15,
                    Description = "Sincronización de órdenes desde SAP"
                }
            );

            // Configuración Swagger por defecto
            modelBuilder.Entity<SwaggerConfiguration>().HasData(
                new SwaggerConfiguration
                {
                    Id = 1,
                    MethodName = "GetCustomers",
                    HttpMethod = "GET",
                    Endpoint = "/api/customers",
                    IsVisible = true,
                    Category = "Customers",
                    Description = "Obtiene lista de clientes"
                },
                new SwaggerConfiguration
                {
                    Id = 2,
                    MethodName = "CreateCustomer",
                    HttpMethod = "POST",
                    Endpoint = "/api/customers",
                    IsVisible = false,
                    Category = "Customers",
                    Description = "Crea un nuevo cliente"
                },
                new SwaggerConfiguration
                {
                    Id = 3,
                    MethodName = "DeleteCustomer",
                    HttpMethod = "DELETE",
                    Endpoint = "/api/customers/{id}",
                    IsVisible = false,
                    Category = "Customers",
                    Description = "Elimina un cliente"
                },
                new SwaggerConfiguration
                {
                    Id = 4,
                    MethodName = "GetProducts",
                    HttpMethod = "GET",
                    Endpoint = "/api/products",
                    IsVisible = true,
                    Category = "Products",
                    Description = "Obtiene lista de productos"
                }
            );
        }
    }
}