﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiSAPBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndpointCalled = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    CODCLIENTE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CODCONTABLE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NOMBRECLIENTE = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NOMBRECOMERCIAL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CIF = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ALIAS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DIRECCION1 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    POBLACION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PROVINCIA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PAIS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TELEFONO1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TELEFONO2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    E_MAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RIESGOCONCEDIDO = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    FACTURARCONIMPUESTO = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.CODCLIENTE);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    NUMDPTO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.NUMDPTO);
                });

            migrationBuilder.CreateTable(
                name: "FormasPago",
                columns: table => new
                {
                    CODFORMAPAGO = table.Column<int>(type: "int", maxLength: 10, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NUMVENCIMIENTOS = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPago", x => x.CODFORMAPAGO);
                });

            migrationBuilder.CreateTable(
                name: "Impuestos",
                columns: table => new
                {
                    TIPOIVA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IVA = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impuestos", x => x.TIPOIVA);
                });

            migrationBuilder.CreateTable(
                name: "MethodConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAutomaticSync = table.Column<bool>(type: "bit", nullable: false),
                    SyncIntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastExecuted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SqlConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Server = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Database = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UseIntegratedSecurity = table.Column<bool>(type: "bit", nullable: false),
                    ConnectionTimeout = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SwaggerConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwaggerConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarifas",
                columns: table => new
                {
                    IDTARIFAV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FECHAINI = table.Column<DateTime>(type: "date", nullable: false),
                    FECHAFIN = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifas", x => x.IDTARIFAV);
                });

            migrationBuilder.CreateTable(
                name: "Vendedores",
                columns: table => new
                {
                    CODVENDEDOR = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOMBRE = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedores", x => x.CODVENDEDOR);
                });

            migrationBuilder.CreateTable(
                name: "Secciones",
                columns: table => new
                {
                    NUMDPTO = table.Column<int>(type: "int", nullable: false),
                    NUMSECCION = table.Column<int>(type: "int", nullable: false),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secciones", x => new { x.NUMDPTO, x.NUMSECCION });
                    table.ForeignKey(
                        name: "FK_Secciones_Departamentos_NUMDPTO",
                        column: x => x.NUMDPTO,
                        principalTable: "Departamentos",
                        principalColumn: "NUMDPTO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    CODARTICULO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DESCRIPADIC = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TIPOIMPUESTO = table.Column<int>(type: "int", nullable: false),
                    DPTO = table.Column<int>(type: "int", nullable: true),
                    SECCION = table.Column<int>(type: "int", nullable: true),
                    FAMILIA = table.Column<int>(type: "int", nullable: true),
                    UNID1C = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UNID1V = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    REFPROVEEDOR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    USASTOCKS = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    IMPUESTOCOMPRA = table.Column<int>(type: "int", nullable: true),
                    DESCATALOGADO = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    UDSTRASPASO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TIPOARTICULO = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.CODARTICULO);
                    table.ForeignKey(
                        name: "FK_Articulos_Departamentos_DPTO",
                        column: x => x.DPTO,
                        principalTable: "Departamentos",
                        principalColumn: "NUMDPTO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articulos_Impuestos_TIPOIMPUESTO",
                        column: x => x.TIPOIMPUESTO,
                        principalTable: "Impuestos",
                        principalColumn: "TIPOIVA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FACTURAS",
                columns: table => new
                {
                    NUMSERIE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NUMFACTURA = table.Column<int>(type: "int", nullable: false),
                    N = table.Column<int>(type: "int", nullable: false),
                    FECHA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CODCLIENTE = table.Column<int>(type: "int", nullable: false),
                    CODVENDEDOR = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    TOTALBRUTO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TOTALIMPUESTOS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TOTDTOCOMERCIAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TOTALNETO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TIPODOC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FECHACREADO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHAMODIFICADO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FACTURAS", x => new { x.NUMSERIE, x.NUMFACTURA, x.N });
                    table.ForeignKey(
                        name: "FK_FACTURAS_Clientes_CODCLIENTE",
                        column: x => x.CODCLIENTE,
                        principalTable: "Clientes",
                        principalColumn: "CODCLIENTE",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FACTURAS_Vendedores_CODVENDEDOR",
                        column: x => x.CODVENDEDOR,
                        principalTable: "Vendedores",
                        principalColumn: "CODVENDEDOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Familias",
                columns: table => new
                {
                    NUMDPTO = table.Column<int>(type: "int", nullable: false),
                    NUMSECCION = table.Column<int>(type: "int", nullable: false),
                    NUMFAMILIA = table.Column<int>(type: "int", nullable: false),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Familias", x => new { x.NUMDPTO, x.NUMSECCION, x.NUMFAMILIA });
                    table.ForeignKey(
                        name: "FK_Familias_Secciones_NUMDPTO_NUMSECCION",
                        columns: x => new { x.NUMDPTO, x.NUMSECCION },
                        principalTable: "Secciones",
                        principalColumns: new[] { "NUMDPTO", "NUMSECCION" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloLineas",
                columns: table => new
                {
                    CODARTICULO = table.Column<int>(type: "int", nullable: false),
                    TALLA = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    COLOR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODBARRAS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    COSTEMEDIO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    COSTESTOCK = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ULTIMOCOSTE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CODBARRAS2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CODBARRAS3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DESCATALOGADO = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloLineas", x => new { x.CODARTICULO, x.TALLA, x.COLOR });
                    table.ForeignKey(
                        name: "FK_ArticuloLineas_Articulos_CODARTICULO",
                        column: x => x.CODARTICULO,
                        principalTable: "Articulos",
                        principalColumn: "CODARTICULO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Precios",
                columns: table => new
                {
                    IDTARIFAV = table.Column<int>(type: "int", nullable: false),
                    CODARTICULO = table.Column<int>(type: "int", nullable: false),
                    TALLA = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    COLOR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODBARRAS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PBRUTO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DTO = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PNETO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Precios", x => new { x.IDTARIFAV, x.CODARTICULO, x.TALLA, x.COLOR });
                    table.ForeignKey(
                        name: "FK_Precios_Articulos_CODARTICULO",
                        column: x => x.CODARTICULO,
                        principalTable: "Articulos",
                        principalColumn: "CODARTICULO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Precios_Tarifas_IDTARIFAV",
                        column: x => x.IDTARIFAV,
                        principalTable: "Tarifas",
                        principalColumn: "IDTARIFAV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FACTURADETALLES",
                columns: table => new
                {
                    SERIE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NUMERO = table.Column<int>(type: "int", nullable: false),
                    N = table.Column<int>(type: "int", nullable: false),
                    LINEA = table.Column<int>(type: "int", nullable: false),
                    CODARTICULO = table.Column<int>(type: "int", nullable: false),
                    REFERENCIA = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TALLA = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    COLOR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TIPOIMPUESTO = table.Column<int>(type: "int", nullable: false),
                    UNIDADESTOTAL = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PRECIO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DTO = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TOTAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FACTURADETALLES", x => new { x.SERIE, x.NUMERO, x.N, x.LINEA });
                    table.ForeignKey(
                        name: "FK_FACTURADETALLES_Articulos_CODARTICULO",
                        column: x => x.CODARTICULO,
                        principalTable: "Articulos",
                        principalColumn: "CODARTICULO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FACTURADETALLES_FACTURAS_SERIE_NUMERO_N",
                        columns: x => new { x.SERIE, x.NUMERO, x.N },
                        principalTable: "FACTURAS",
                        principalColumns: new[] { "NUMSERIE", "NUMFACTURA", "N" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FACTURADETALLES_Impuestos_TIPOIMPUESTO",
                        column: x => x.TIPOIMPUESTO,
                        principalTable: "Impuestos",
                        principalColumn: "TIPOIVA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FACTURAPAGOS",
                columns: table => new
                {
                    SERIE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NUMERO = table.Column<int>(type: "int", nullable: false),
                    N = table.Column<int>(type: "int", nullable: false),
                    POSICION = table.Column<int>(type: "int", nullable: false),
                    CODTIPOPAGO = table.Column<int>(type: "int", nullable: false),
                    IMPORTE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FACTURAPAGOS", x => new { x.SERIE, x.NUMERO, x.N, x.POSICION });
                    table.ForeignKey(
                        name: "FK_FACTURAPAGOS_FACTURAS_SERIE_NUMERO_N",
                        columns: x => new { x.SERIE, x.NUMERO, x.N },
                        principalTable: "FACTURAS",
                        principalColumns: new[] { "NUMSERIE", "NUMFACTURA", "N" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FACTURAPAGOS_FormasPago_CODTIPOPAGO",
                        column: x => x.CODTIPOPAGO,
                        principalTable: "FormasPago",
                        principalColumn: "CODFORMAPAGO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiLogs_Timestamp",
                table: "ApiLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloLineas_CodigoBarras",
                table: "ArticuloLineas",
                column: "CODBARRAS");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_Departamento",
                table: "Articulos",
                columns: new[] { "DPTO", "SECCION", "FAMILIA" });

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_TIPOIMPUESTO",
                table: "Articulos",
                column: "TIPOIMPUESTO");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "E_MAIL");

            migrationBuilder.CreateIndex(
                name: "IX_FACTURADETALLES_CODARTICULO",
                table: "FACTURADETALLES",
                column: "CODARTICULO");

            migrationBuilder.CreateIndex(
                name: "IX_FACTURADETALLES_TIPOIMPUESTO",
                table: "FACTURADETALLES",
                column: "TIPOIMPUESTO");

            migrationBuilder.CreateIndex(
                name: "IX_FACTURAPAGOS_CODTIPOPAGO",
                table: "FACTURAPAGOS",
                column: "CODTIPOPAGO");

            migrationBuilder.CreateIndex(
                name: "IX_FACTURAS_CODCLIENTE",
                table: "FACTURAS",
                column: "CODCLIENTE");

            migrationBuilder.CreateIndex(
                name: "IX_FACTURAS_CODVENDEDOR",
                table: "FACTURAS",
                column: "CODVENDEDOR");

            migrationBuilder.CreateIndex(
                name: "IX_Familias_Seccion",
                table: "Familias",
                columns: new[] { "NUMDPTO", "NUMSECCION" });

            migrationBuilder.CreateIndex(
                name: "IX_MethodConfigurations_MethodHttpMethod",
                table: "MethodConfigurations",
                columns: new[] { "MethodName", "HttpMethod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Precios_CODARTICULO",
                table: "Precios",
                column: "CODARTICULO");

            migrationBuilder.CreateIndex(
                name: "IX_Precios_Tarifa",
                table: "Precios",
                column: "IDTARIFAV");

            migrationBuilder.CreateIndex(
                name: "IX_Secciones_NUMDPTO",
                table: "Secciones",
                column: "NUMDPTO");

            migrationBuilder.CreateIndex(
                name: "IX_SqlConfigurations_ServerDatabase",
                table: "SqlConfigurations",
                columns: new[] { "Server", "Database" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwaggerConfigurations_MethodHttpMethod",
                table: "SwaggerConfigurations",
                columns: new[] { "MethodName", "HttpMethod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_Key",
                table: "SystemConfigurations",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogs");

            migrationBuilder.DropTable(
                name: "ArticuloLineas");

            migrationBuilder.DropTable(
                name: "FACTURADETALLES");

            migrationBuilder.DropTable(
                name: "FACTURAPAGOS");

            migrationBuilder.DropTable(
                name: "Familias");

            migrationBuilder.DropTable(
                name: "MethodConfigurations");

            migrationBuilder.DropTable(
                name: "Precios");

            migrationBuilder.DropTable(
                name: "SecurityConfigurations");

            migrationBuilder.DropTable(
                name: "SqlConfigurations");

            migrationBuilder.DropTable(
                name: "SwaggerConfigurations");

            migrationBuilder.DropTable(
                name: "SystemConfigurations");

            migrationBuilder.DropTable(
                name: "FACTURAS");

            migrationBuilder.DropTable(
                name: "FormasPago");

            migrationBuilder.DropTable(
                name: "Secciones");

            migrationBuilder.DropTable(
                name: "Articulos");

            migrationBuilder.DropTable(
                name: "Tarifas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Vendedores");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Impuestos");
        }
    }
}
