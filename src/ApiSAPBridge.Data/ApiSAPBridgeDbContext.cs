using Microsoft.EntityFrameworkCore;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.Entities;

namespace ApiSAPBridge.Data
{
    public class ApiSAPBridgeDbContext : DbContext
    {
        public ApiSAPBridgeDbContext(DbContextOptions<ApiSAPBridgeDbContext> options) : base(options)
        {
        }

        // DbSets para todas las entidades
        public DbSet<Departamento> Departamentos { get; set; } = null!;
        public DbSet<Seccion> Secciones { get; set; } = null!;
        public DbSet<Familia> Familias { get; set; } = null!;
        public DbSet<Vendedor> Vendedores { get; set; } = null!;
        public DbSet<Impuesto> Impuestos { get; set; } = null!;
        public DbSet<FormaPago> FormasPago { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Tarifa> Tarifas { get; set; } = null!;
        public DbSet<Articulo> Articulos { get; set; } = null!;
        public DbSet<ArticuloLinea> ArticuloLineas { get; set; } = null!;
        public DbSet<Precio> Precios { get; set; } = null!;
        public DbSet<ApiLog> ApiLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar entidades
            ConfigureDepartamento(modelBuilder);
            ConfigureSeccion(modelBuilder);
            ConfigureFamilia(modelBuilder);
            ConfigureVendedor(modelBuilder);
            ConfigureImpuesto(modelBuilder);
            ConfigureFormaPago(modelBuilder);
            ConfigureCliente(modelBuilder);
            ConfigureTarifa(modelBuilder);
            ConfigureArticulo(modelBuilder);
            ConfigureArticuloLinea(modelBuilder);
            ConfigurePrecio(modelBuilder);
            ConfigureApiLog(modelBuilder);

            // Configurar índices para optimización
            ConfigureIndexes(modelBuilder);
        }

        private void ConfigureDepartamento(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("Departamentos");
                entity.HasKey(e => e.NUMDPTO);

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureSeccion(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Seccion>(entity =>
            {
                entity.ToTable("Secciones");

                // Llave compuesta
                entity.HasKey(e => new { e.NUMDPTO, e.NUMSECCION });

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Relación con Departamento
                entity.HasOne(e => e.Departamento)
                    .WithMany(d => d.Secciones)
                    .HasForeignKey(e => e.NUMDPTO)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureFamilia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Familia>(entity =>
            {
                entity.ToTable("Familias");

                // Llave compuesta
                entity.HasKey(e => new { e.NUMDPTO, e.NUMSECCION, e.NUMFAMILIA });

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Relación con Seccion
                entity.HasOne(e => e.Seccion)
                    .WithMany(s => s.Familias)
                    .HasForeignKey(e => new { e.NUMDPTO, e.NUMSECCION })
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureVendedor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vendedor>(entity =>
            {
                entity.ToTable("Vendedores");
                entity.HasKey(e => e.CODVENDEDOR);

                entity.Property(e => e.NOMBRE)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureImpuesto(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Impuesto>(entity =>
            {
                entity.ToTable("Impuestos");
                entity.HasKey(e => e.TIPOIVA);

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.IVA)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureFormaPago(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FormaPago>(entity =>
            {
                entity.ToTable("FormasPago");
                entity.HasKey(e => e.CODFORMAPAGO);

                entity.Property(e => e.CODFORMAPAGO)
                    .HasMaxLength(10);

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureCliente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(e => e.CODCLIENTE);

                entity.Property(e => e.CODCONTABLE)
                    .HasMaxLength(50);

                entity.Property(e => e.NOMBRECLIENTE)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NOMBRECOMERCIAL)
                    .HasMaxLength(255);

                entity.Property(e => e.CIF)
                    .HasMaxLength(50);

                entity.Property(e => e.ALIAS)
                    .HasMaxLength(255);

                entity.Property(e => e.DIRECCION1)
                    .HasMaxLength(500);

                entity.Property(e => e.POBLACION)
                    .HasMaxLength(255);

                entity.Property(e => e.PROVINCIA)
                    .HasMaxLength(255);

                entity.Property(e => e.PAIS)
                    .HasMaxLength(255);

                entity.Property(e => e.TELEFONO1)
                    .HasMaxLength(50);

                entity.Property(e => e.TELEFONO2)
                    .HasMaxLength(50);

                entity.Property(e => e.E_MAIL)
                    .HasMaxLength(255);

                entity.Property(e => e.RIESGOCONCEDIDO)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.FACTURARCONIMPUESTO)
                    .HasMaxLength(10);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureTarifa(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.ToTable("Tarifas");
                entity.HasKey(e => e.IDTARIFAV);

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FECHAINI)
                    .HasColumnType("date");

                entity.Property(e => e.FECHAFIN)
                    .HasColumnType("date");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureArticulo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.ToTable("Articulos");
                entity.HasKey(e => e.CODARTICULO);

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DESCRIPADIC)
                    .HasMaxLength(500);

                entity.Property(e => e.UNID1C)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.UNID1V)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.REFPROVEEDOR)
                    .HasMaxLength(100);

                entity.Property(e => e.USASTOCKS)
                    .HasMaxLength(1);

                entity.Property(e => e.DESCATALOGADO)
                    .HasMaxLength(1);

                entity.Property(e => e.UDSTRASPASO)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.TIPOARTICULO)
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Relaciones
                entity.HasOne(e => e.Impuesto)
                    .WithMany(i => i.Articulos)
                    .HasForeignKey(e => e.TIPOIMPUESTO)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Departamento)
                    .WithMany(d => d.Articulos)
                    .HasForeignKey(e => e.DPTO)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureArticuloLinea(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticuloLinea>(entity =>
            {
                entity.ToTable("ArticuloLineas");

                // Llave compuesta
                entity.HasKey(e => new { e.CODARTICULO, e.TALLA, e.COLOR });

                entity.Property(e => e.TALLA)
                    .HasMaxLength(10);

                entity.Property(e => e.COLOR)
                    .HasMaxLength(50);

                entity.Property(e => e.CODBARRAS)
                    .HasMaxLength(50);

                entity.Property(e => e.COSTEMEDIO)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.COSTESTOCK)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ULTIMOCOSTE)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CODBARRAS2)
                    .HasMaxLength(50);

                entity.Property(e => e.CODBARRAS3)
                    .HasMaxLength(50);

                entity.Property(e => e.DESCATALOGADO)
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Relación con Articulo
                entity.HasOne(e => e.Articulo)
                    .WithMany(a => a.Lineas)
                    .HasForeignKey(e => e.CODARTICULO)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurePrecio(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Precio>(entity =>
            {
                entity.ToTable("Precios");

                // Llave compuesta
                entity.HasKey(e => new { e.IDTARIFAV, e.CODARTICULO, e.TALLA, e.COLOR });

                entity.Property(e => e.TALLA)
                    .HasMaxLength(10);

                entity.Property(e => e.COLOR)
                    .HasMaxLength(50);

                entity.Property(e => e.CODBARRAS)
                    .HasMaxLength(50);

                entity.Property(e => e.PBRUTO)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DTO)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.PNETO)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Relaciones
                entity.HasOne(e => e.Tarifa)
                    .WithMany(t => t.Precios)
                    .HasForeignKey(e => e.IDTARIFAV)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Articulo)
                    .WithMany(a => a.Precios)
                    .HasForeignKey(e => e.CODARTICULO)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureApiLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiLog>(entity =>
            {
                entity.ToTable("ApiLogs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Level)
                    .HasMaxLength(50);

                entity.Property(e => e.EndpointCalled)
                    .HasMaxLength(255);

                entity.Property(e => e.HttpMethod)
                    .HasMaxLength(10);
            });
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Índices para optimización de consultas
            modelBuilder.Entity<Seccion>()
                .HasIndex(e => e.NUMDPTO)
                .HasDatabaseName("IX_Secciones_NUMDPTO");

            modelBuilder.Entity<Familia>()
                .HasIndex(e => new { e.NUMDPTO, e.NUMSECCION })
                .HasDatabaseName("IX_Familias_Seccion");

            modelBuilder.Entity<Articulo>()
                .HasIndex(e => new { e.DPTO, e.SECCION, e.FAMILIA })
                .HasDatabaseName("IX_Articulos_Departamento");

            modelBuilder.Entity<Precio>()
                .HasIndex(e => e.IDTARIFAV)
                .HasDatabaseName("IX_Precios_Tarifa");

            modelBuilder.Entity<ApiLog>()
                .HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_ApiLogs_Timestamp");

            modelBuilder.Entity<Cliente>()
                .HasIndex(e => e.E_MAIL)
                .HasDatabaseName("IX_Clientes_Email");

            modelBuilder.Entity<ArticuloLinea>()
                .HasIndex(e => e.CODBARRAS)
                .HasDatabaseName("IX_ArticuloLineas_CodigoBarras");
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Added &&
                    entry.Entity.GetType().GetProperty("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}