// ApiSAPBridgeDbContext.cs - Configuraciones corregidas

using Microsoft.EntityFrameworkCore;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.Entities;
using ApiSAPBridge.Models.Configuration;

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
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }
        public DbSet<FacturaPago> FacturaPagos { get; set; }
        public DbSet<SqlConfiguration> SqlConfigurations { get; set; } = null!;
        public DbSet<MethodConfiguration> MethodConfigurations { get; set; } = null!;
        public DbSet<SwaggerConfiguration> SwaggerConfigurations { get; set; } = null!;
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; } = null!;
        public DbSet<SecurityConfiguration> SecurityConfigurations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar entidades principales
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

            // Configuraciones de Facturación
            ConfigureFactura(modelBuilder);
            ConfigureFacturaDetalle(modelBuilder);
            ConfigureFacturaPago(modelBuilder);

            // Configuraciones de Configuration
            ConfigureSqlConfiguration(modelBuilder);
            ConfigureMethodConfiguration(modelBuilder);
            ConfigureSwaggerConfiguration(modelBuilder);
            ConfigureSystemConfiguration(modelBuilder);
            ConfigureSecurityConfiguration(modelBuilder);

            // Configurar índices para optimización
            ConfigureIndexes(modelBuilder);
        }

        private void ConfigureDepartamento(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("Departamentos");
                entity.HasKey(e => e.NUMDPTO);

                entity.Property(e => e.NUMDPTO)
                    .ValueGeneratedOnAdd();

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
                entity.HasKey(e => new { e.NUMDPTO, e.NUMSECCION });

                entity.Property(e => e.NUMDPTO)
                    .HasColumnOrder(0);

                entity.Property(e => e.NUMSECCION)
                    .HasColumnOrder(1);

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
                entity.HasKey(e => new { e.NUMDPTO, e.NUMSECCION, e.NUMFAMILIA });

                entity.Property(e => e.NUMDPTO)
                    .HasColumnOrder(0);

                entity.Property(e => e.NUMSECCION)
                    .HasColumnOrder(1);

                entity.Property(e => e.NUMFAMILIA)
                    .HasColumnOrder(2);

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

                entity.Property(e => e.CODVENDEDOR)
                    .ValueGeneratedOnAdd();

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

                entity.Property(e => e.TIPOIVA)
                    .ValueGeneratedOnAdd();

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

                // ❌ PROBLEMA PRINCIPAL: Quitamos HasMaxLength en un int IDENTITY
                entity.Property(e => e.CODFORMAPAGO)
                    .ValueGeneratedOnAdd(); // Explícitamente IDENTITY

                entity.Property(e => e.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NUMVENCIMIENTOS)
                    .IsRequired();

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

                entity.Property(e => e.CODCLIENTE)
                    .ValueGeneratedOnAdd();

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

                entity.Property(e => e.IDTARIFAV)
                    .ValueGeneratedOnAdd();

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

                entity.Property(e => e.CODARTICULO)
                    .ValueGeneratedOnAdd();

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
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Departamento)
                    .WithMany(d => d.Articulos)
                    .HasForeignKey(e => e.DPTO)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureArticuloLinea(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticuloLinea>(entity =>
            {
                entity.ToTable("ArticuloLineas");
                entity.HasKey(e => new { e.CODARTICULO, e.TALLA, e.COLOR });

                entity.Property(e => e.CODARTICULO)
                    .HasColumnOrder(0);

                entity.Property(e => e.TALLA)
                    .HasMaxLength(10)
                    .HasColumnOrder(1);

                entity.Property(e => e.COLOR)
                    .HasMaxLength(50)
                    .HasColumnOrder(2);

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
                entity.HasKey(e => new { e.IDTARIFAV, e.CODARTICULO, e.TALLA, e.COLOR });

                entity.Property(e => e.IDTARIFAV)
                    .HasColumnOrder(0);

                entity.Property(e => e.CODARTICULO)
                    .HasColumnOrder(1);

                entity.Property(e => e.TALLA)
                    .HasMaxLength(10)
                    .HasColumnOrder(2);

                entity.Property(e => e.COLOR)
                    .HasMaxLength(50)
                    .HasColumnOrder(3);

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

        private void ConfigureFactura(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("FACTURAS");
                entity.HasKey(f => new { f.NUMSERIE, f.NUMFACTURA, f.N });

                entity.Property(f => f.NUMSERIE)
                    .HasMaxLength(50)
                    .HasColumnName("NUMSERIE");

                entity.Property(f => f.NUMFACTURA)
                    .HasColumnName("NUMFACTURA");

                entity.Property(f => f.N)
                    .HasColumnName("N");

                entity.Property(f => f.FECHA)
                    .HasColumnName("FECHA");

                entity.Property(f => f.CODCLIENTE)
                    .HasColumnName("CODCLIENTE");

                entity.Property(f => f.CODVENDEDOR)
                    .HasMaxLength(20)
                    .HasColumnName("CODVENDEDOR");

                entity.Property(f => f.TOTALBRUTO)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("TOTALBRUTO");

                entity.Property(f => f.TOTALIMPUESTOS)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("TOTALIMPUESTOS");

                entity.Property(f => f.TOTDTOCOMERCIAL)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("TOTDTOCOMERCIAL");

                entity.Property(f => f.TOTALNETO)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("TOTALNETO");

                entity.Property(f => f.TIPODOC)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("TIPODOC");

                entity.Property(f => f.FECHACREADO)
                    .HasColumnName("FECHACREADO");

                entity.Property(f => f.FECHAMODIFICADO)
                    .HasColumnName("FECHAMODIFICADO");

                // Relaciones
                entity.HasOne(f => f.Cliente)
                    .WithMany()
                    .HasForeignKey(f => f.CODCLIENTE)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Vendedor)
                    .WithMany()
                    .HasForeignKey(f => f.CODVENDEDOR)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureFacturaDetalle(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FacturaDetalle>(entity =>
            {
                entity.ToTable("FACTURADETALLES");
                entity.HasKey(fd => new { fd.SERIE, fd.NUMERO, fd.N, fd.LINEA });

                entity.Property(fd => fd.SERIE)
                    .HasMaxLength(50)
                    .HasColumnName("SERIE");

                entity.Property(fd => fd.NUMERO)
                    .HasColumnName("NUMERO");

                entity.Property(fd => fd.N)
                    .HasColumnName("N");

                entity.Property(fd => fd.LINEA)
                    .HasColumnName("LINEA");

                entity.Property(fd => fd.CODARTICULO)
                    .HasColumnName("CODARTICULO");

                entity.Property(fd => fd.REFERENCIA)
                    .HasMaxLength(100)
                    .HasColumnName("REFERENCIA");

                entity.Property(fd => fd.DESCRIPCION)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("DESCRIPCION");

                entity.Property(fd => fd.TALLA)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("TALLA");

                entity.Property(fd => fd.COLOR)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("COLOR");

                entity.Property(fd => fd.TIPOIMPUESTO)
                    .HasColumnName("TIPOIMPUESTO");

                entity.Property(fd => fd.UNIDADESTOTAL)
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("UNIDADESTOTAL");

                entity.Property(fd => fd.PRECIO)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("PRECIO");

                entity.Property(fd => fd.DTO)
                    .HasColumnType("decimal(5,2)")
                    .HasColumnName("DTO");

                entity.Property(fd => fd.TOTAL)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("TOTAL");

                // Relaciones
                entity.HasOne(fd => fd.Factura)
                    .WithMany(f => f.Detalles)
                    .HasForeignKey(fd => new { fd.SERIE, fd.NUMERO, fd.N })
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(fd => fd.Articulo)
                    .WithMany()
                    .HasForeignKey(fd => fd.CODARTICULO)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(fd => fd.Impuesto)
                    .WithMany()
                    .HasForeignKey(fd => fd.TIPOIMPUESTO)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureFacturaPago(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FacturaPago>(entity =>
            {
                entity.ToTable("FACTURAPAGOS");
                entity.HasKey(fp => new { fp.SERIE, fp.NUMERO, fp.N, fp.POSICION });

                entity.Property(fp => fp.SERIE)
                    .HasMaxLength(50)
                    .HasColumnName("SERIE");

                entity.Property(fp => fp.NUMERO)
                    .HasColumnName("NUMERO");

                entity.Property(fp => fp.N)
                    .HasColumnName("N");

                entity.Property(fp => fp.POSICION)
                    .HasColumnName("POSICION");

                entity.Property(fp => fp.CODTIPOPAGO)
                    .HasColumnName("CODTIPOPAGO");

                entity.Property(fp => fp.IMPORTE)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("IMPORTE");

                entity.Property(fp => fp.DESCRIPCION)
                    .HasMaxLength(100)
                    .HasColumnName("DESCRIPCION");

                // Relaciones
                entity.HasOne(fp => fp.Factura)
                    .WithMany(f => f.Pagos)
                    .HasForeignKey(fp => new { fp.SERIE, fp.NUMERO, fp.N })
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(fp => fp.FormaPago)
                    .WithMany()
                    .HasForeignKey(fp => fp.CODTIPOPAGO)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureSqlConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlConfiguration>(entity =>
            {
                entity.ToTable("SqlConfigurations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Server)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Database)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .HasMaxLength(500);

                entity.Property(e => e.UseIntegratedSecurity)
                    .IsRequired();

                entity.Property(e => e.ConnectionTimeout)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => new { e.Server, e.Database })
                    .IsUnique()
                    .HasDatabaseName("IX_SqlConfigurations_ServerDatabase");
            });
        }

        private void ConfigureMethodConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MethodConfiguration>(entity =>
            {
                entity.ToTable("MethodConfigurations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.MethodName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.HttpMethod)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Endpoint)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IsEnabled)
                    .IsRequired();

                entity.Property(e => e.IsAutomaticSync)
                    .IsRequired();

                entity.Property(e => e.SyncIntervalMinutes)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.LastExecuted);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => new { e.MethodName, e.HttpMethod })
                    .IsUnique()
                    .HasDatabaseName("IX_MethodConfigurations_MethodHttpMethod");
            });
        }

        private void ConfigureSwaggerConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SwaggerConfiguration>(entity =>
            {
                entity.ToTable("SwaggerConfigurations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.MethodName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.HttpMethod)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Endpoint)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IsVisible)
                    .IsRequired();

                entity.Property(e => e.Category)
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => new { e.MethodName, e.HttpMethod })
                    .IsUnique()
                    .HasDatabaseName("IX_SwaggerConfigurations_MethodHttpMethod");
            });
        }

        private void ConfigureSystemConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.ToTable("SystemConfigurations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.Key)
                    .IsUnique()
                    .HasDatabaseName("IX_SystemConfigurations_Key");
            });
        }

        private void ConfigureSecurityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SecurityConfiguration>(entity =>
            {
                entity.ToTable("SecurityConfigurations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.LoginAttempts)
                    .IsRequired();

                entity.Property(e => e.LastLogin);

                entity.Property(e => e.LockedUntil);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");
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