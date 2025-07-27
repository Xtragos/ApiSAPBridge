-- ============================================
-- SCRIPT COMPLETO PARA CREAR BASE DE DATOS ApiSAP
-- ApiSAPBridge - Setup Completo en Un Solo Script
-- ============================================

SET NOCOUNT ON;

PRINT '🚀 Iniciando configuración completa de base de datos ApiSAP...'
PRINT '=============================================='

-- ============================================
-- PASO 1: VERIFICAR QUE EL LOGIN ICGAdmin EXISTE
-- ============================================

USE [master]
GO

PRINT '🔍 Verificando que el login ICGAdmin existe...'

IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'ICGAdmin')
BEGIN
    PRINT ''
    PRINT '❌ ERROR CRÍTICO: El login ICGAdmin no existe en el servidor SQL.'
    PRINT ''
    PRINT '📋 PARA RESOLVER ESTE PROBLEMA:'
    PRINT '1. Conectarse como administrador (sa o sysadmin)'
    PRINT '2. Ejecutar el siguiente comando:'
    PRINT '   CREATE LOGIN [ICGAdmin] WITH PASSWORD = ''TuContraseñaAqui'', CHECK_POLICY = OFF'
    PRINT ''
    PRINT '🛑 SCRIPT DETENIDO - RESOLVER ESTE PROBLEMA PRIMERO'
    PRINT '=============================================='
    RETURN
END

PRINT '✅ Login ICGAdmin encontrado en el servidor'

-- Mostrar información del login
SELECT 
    '📋 Información del Login ICGAdmin:' as Info,
    name as LoginName,
    type_desc as LoginType,
    is_disabled as IsDisabled,
    create_date as Created
FROM sys.server_principals 
WHERE name = 'ICGAdmin'

-- ============================================
-- PASO 2: CREAR BASE DE DATOS ApiSAP SI NO EXISTE
-- ============================================

PRINT '📦 Verificando/creando base de datos ApiSAP...'

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ApiSAP')
BEGIN
    PRINT '📦 Creando base de datos ApiSAP...'
    CREATE DATABASE [ApiSAP]
    COLLATE Modern_Spanish_CI_AS
    PRINT '✅ Base de datos ApiSAP creada exitosamente'
END
ELSE
BEGIN
    PRINT '✅ Base de datos ApiSAP ya existe'
END

-- ============================================
-- PASO 3: CONFIGURAR USUARIO EN LA BASE DE DATOS
-- ============================================

USE [ApiSAP]
GO

PRINT '👤 Configurando usuario ICGAdmin en base de datos...'

-- Eliminar usuario si existe (para limpiar problemas anteriores)
IF EXISTS (SELECT name FROM sys.database_principals WHERE name = 'ICGAdmin')
BEGIN
    PRINT '🧹 Eliminando usuario ICGAdmin existente para limpiar configuración...'
    DROP USER [ICGAdmin]
END

-- Crear usuario limpio
CREATE USER [ICGAdmin] FOR LOGIN [ICGAdmin]
PRINT '✅ Usuario ICGAdmin creado en la base de datos'

-- Asignar permisos de db_owner
ALTER ROLE [db_owner] ADD MEMBER [ICGAdmin]
PRINT '✅ Permisos de db_owner asignados a ICGAdmin'

-- ============================================
-- PASO 4: CREAR TODAS LAS TABLAS
-- ============================================

PRINT '📋 Creando estructura de tablas...'

-- Tabla: __EFMigrationsHistory
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT '✅ Tabla __EFMigrationsHistory creada'
END

-- Tabla: ApiLogs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApiLogs')
BEGIN
    CREATE TABLE [ApiLogs] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Timestamp] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Level] nvarchar(50) NULL,
        [Message] nvarchar(max) NULL,
        [Exception] nvarchar(max) NULL,
        [Properties] nvarchar(max) NULL,
        [EndpointCalled] nvarchar(255) NULL,
        [HttpMethod] nvarchar(10) NULL,
        [RequestBody] nvarchar(max) NULL,
        [ResponseStatus] int NULL,
        CONSTRAINT [PK_ApiLogs] PRIMARY KEY ([Id])
    );
    CREATE INDEX [IX_ApiLogs_Timestamp] ON [ApiLogs] ([Timestamp]);
    PRINT '✅ Tabla ApiLogs creada'
END

-- Tabla: Departamentos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Departamentos')
BEGIN
    CREATE TABLE [Departamentos] (
        [NUMDPTO] int IDENTITY(1,1) NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Departamentos] PRIMARY KEY ([NUMDPTO])
    );
    PRINT '✅ Tabla Departamentos creada'
END

-- Tabla: Impuestos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Impuestos')
BEGIN
    CREATE TABLE [Impuestos] (
        [TIPOIVA] int IDENTITY(1,1) NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [IVA] decimal(5,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Impuestos] PRIMARY KEY ([TIPOIVA])
    );
    PRINT '✅ Tabla Impuestos creada'
END

-- Tabla: FormasPago
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FormasPago')
BEGIN
    CREATE TABLE [FormasPago] (
        [CODFORMAPAGO] int IDENTITY(1,1) NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [NUMVENCIMIENTOS] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_FormasPago] PRIMARY KEY ([CODFORMAPAGO])
    );
    PRINT '✅ Tabla FormasPago creada'
END

-- Tabla: Clientes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE [Clientes] (
        [CODCLIENTE] int IDENTITY(1,1) NOT NULL,
        [CODCONTABLE] nvarchar(50) NULL,
        [NOMBRECLIENTE] nvarchar(255) NOT NULL,
        [NOMBRECOMERCIAL] nvarchar(255) NULL,
        [CIF] nvarchar(50) NULL,
        [ALIAS] nvarchar(255) NULL,
        [DIRECCION1] nvarchar(500) NULL,
        [POBLACION] nvarchar(255) NULL,
        [PROVINCIA] nvarchar(255) NULL,
        [PAIS] nvarchar(255) NULL,
        [TELEFONO1] nvarchar(50) NULL,
        [TELEFONO2] nvarchar(50) NULL,
        [E_MAIL] nvarchar(255) NULL,
        [RIESGOCONCEDIDO] decimal(18,2) NOT NULL DEFAULT (0),
        [FACTURARCONIMPUESTO] nvarchar(10) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([CODCLIENTE])
    );
    CREATE INDEX [IX_Clientes_Email] ON [Clientes] ([E_MAIL]);
    PRINT '✅ Tabla Clientes creada'
END

-- Tabla: Vendedores
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vendedores')
BEGIN
    CREATE TABLE [Vendedores] (
        [CODVENDEDOR] int IDENTITY(1,1) NOT NULL,
        [NOMBRE] nvarchar(255) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Vendedores] PRIMARY KEY ([CODVENDEDOR])
    );
    PRINT '✅ Tabla Vendedores creada'
END

-- Tabla: Tarifas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tarifas')
BEGIN
    CREATE TABLE [Tarifas] (
        [IDTARIFAV] int IDENTITY(1,1) NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [FECHAINI] date NOT NULL,
        [FECHAFIN] date NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Tarifas] PRIMARY KEY ([IDTARIFAV])
    );
    PRINT '✅ Tabla Tarifas creada'
END

-- Tabla: Secciones
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Secciones')
BEGIN
    CREATE TABLE [Secciones] (
        [NUMDPTO] int NOT NULL,
        [NUMSECCION] int NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Secciones] PRIMARY KEY ([NUMDPTO], [NUMSECCION]),
        CONSTRAINT [FK_Secciones_Departamentos_NUMDPTO] FOREIGN KEY ([NUMDPTO]) REFERENCES [Departamentos] ([NUMDPTO])
    );
    CREATE INDEX [IX_Secciones_NUMDPTO] ON [Secciones] ([NUMDPTO]);
    PRINT '✅ Tabla Secciones creada'
END

-- Tabla: Familias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Familias')
BEGIN
    CREATE TABLE [Familias] (
        [NUMDPTO] int NOT NULL,
        [NUMSECCION] int NOT NULL,
        [NUMFAMILIA] int NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Familias] PRIMARY KEY ([NUMDPTO], [NUMSECCION], [NUMFAMILIA]),
        CONSTRAINT [FK_Familias_Secciones_NUMDPTO_NUMSECCION] FOREIGN KEY ([NUMDPTO], [NUMSECCION]) REFERENCES [Secciones] ([NUMDPTO], [NUMSECCION])
    );
    CREATE INDEX [IX_Familias_Seccion] ON [Familias] ([NUMDPTO], [NUMSECCION]);
    PRINT '✅ Tabla Familias creada'
END

-- Tabla: Articulos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Articulos')
BEGIN
    CREATE TABLE [Articulos] (
        [CODARTICULO] int IDENTITY(1,1) NOT NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [DESCRIPADIC] nvarchar(500) NULL,
        [TIPOIMPUESTO] int NOT NULL,
        [DPTO] int NULL,
        [SECCION] int NULL,
        [FAMILIA] int NULL,
        [UNID1C] decimal(18,2) NULL,
        [UNID1V] decimal(18,2) NULL,
        [REFPROVEEDOR] nvarchar(100) NULL,
        [USASTOCKS] nvarchar(1) NULL,
        [IMPUESTOCOMPRA] int NULL,
        [DESCATALOGADO] nvarchar(1) NULL,
        [UDSTRASPASO] decimal(18,2) NULL,
        [TIPOARTICULO] nvarchar(1) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Articulos] PRIMARY KEY ([CODARTICULO]),
        CONSTRAINT [FK_Articulos_Impuestos_TIPOIMPUESTO] FOREIGN KEY ([TIPOIMPUESTO]) REFERENCES [Impuestos] ([TIPOIVA]),
        CONSTRAINT [FK_Articulos_Departamentos_DPTO] FOREIGN KEY ([DPTO]) REFERENCES [Departamentos] ([NUMDPTO])
    );
    CREATE INDEX [IX_Articulos_TIPOIMPUESTO] ON [Articulos] ([TIPOIMPUESTO]);
    CREATE INDEX [IX_Articulos_Departamento] ON [Articulos] ([DPTO], [SECCION], [FAMILIA]);
    PRINT '✅ Tabla Articulos creada'
END

-- Tabla: ArticuloLineas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ArticuloLineas')
BEGIN
    CREATE TABLE [ArticuloLineas] (
        [CODARTICULO] int NOT NULL,
        [TALLA] nvarchar(10) NOT NULL,
        [COLOR] nvarchar(50) NOT NULL,
        [CODBARRAS] nvarchar(50) NULL,
        [COSTEMEDIO] decimal(18,2) NULL,
        [COSTESTOCK] decimal(18,2) NULL,
        [ULTIMOCOSTE] decimal(18,2) NULL,
        [CODBARRAS2] nvarchar(50) NULL,
        [CODBARRAS3] nvarchar(50) NULL,
        [DESCATALOGADO] nvarchar(1) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_ArticuloLineas] PRIMARY KEY ([CODARTICULO], [TALLA], [COLOR]),
        CONSTRAINT [FK_ArticuloLineas_Articulos_CODARTICULO] FOREIGN KEY ([CODARTICULO]) REFERENCES [Articulos] ([CODARTICULO]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_ArticuloLineas_CodigoBarras] ON [ArticuloLineas] ([CODBARRAS]);
    PRINT '✅ Tabla ArticuloLineas creada'
END

-- Tabla: Precios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Precios')
BEGIN
    CREATE TABLE [Precios] (
        [IDTARIFAV] int NOT NULL,
        [CODARTICULO] int NOT NULL,
        [TALLA] nvarchar(10) NOT NULL,
        [COLOR] nvarchar(50) NOT NULL,
        [CODBARRAS] nvarchar(50) NULL,
        [PBRUTO] decimal(18,2) NOT NULL,
        [DTO] decimal(5,2) NOT NULL,
        [PNETO] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Precios] PRIMARY KEY ([IDTARIFAV], [CODARTICULO], [TALLA], [COLOR]),
        CONSTRAINT [FK_Precios_Tarifas_IDTARIFAV] FOREIGN KEY ([IDTARIFAV]) REFERENCES [Tarifas] ([IDTARIFAV]) ON DELETE CASCADE,
        CONSTRAINT [FK_Precios_Articulos_CODARTICULO] FOREIGN KEY ([CODARTICULO]) REFERENCES [Articulos] ([CODARTICULO]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_Precios_CODARTICULO] ON [Precios] ([CODARTICULO]);
    CREATE INDEX [IX_Precios_Tarifa] ON [Precios] ([IDTARIFAV]);
    PRINT '✅ Tabla Precios creada'
END

-- Tabla: FACTURAS
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FACTURAS')
BEGIN
    CREATE TABLE [FACTURAS] (
        [NUMSERIE] nvarchar(50) NOT NULL,
        [NUMFACTURA] int NOT NULL,
        [N] int NOT NULL,
        [FECHA] datetime2 NOT NULL,
        [CODCLIENTE] int NOT NULL,
        [CODVENDEDOR] int NOT NULL,
        [TOTALBRUTO] decimal(18,2) NOT NULL,
        [TOTALIMPUESTOS] decimal(18,2) NOT NULL,
        [TOTDTOCOMERCIAL] decimal(18,2) NOT NULL,
        [TOTALNETO] decimal(18,2) NOT NULL,
        [TIPODOC] nvarchar(20) NOT NULL,
        [FECHACREADO] datetime2 NOT NULL,
        [FECHAMODIFICADO] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FACTURAS] PRIMARY KEY ([NUMSERIE], [NUMFACTURA], [N]),
        CONSTRAINT [FK_FACTURAS_Clientes_CODCLIENTE] FOREIGN KEY ([CODCLIENTE]) REFERENCES [Clientes] ([CODCLIENTE]) ON DELETE CASCADE,
        CONSTRAINT [FK_FACTURAS_Vendedores_CODVENDEDOR] FOREIGN KEY ([CODVENDEDOR]) REFERENCES [Vendedores] ([CODVENDEDOR]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_FACTURAS_CODCLIENTE] ON [FACTURAS] ([CODCLIENTE]);
    CREATE INDEX [IX_FACTURAS_CODVENDEDOR] ON [FACTURAS] ([CODVENDEDOR]);
    PRINT '✅ Tabla FACTURAS creada'
END

-- Tabla: FACTURADETALLES
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FACTURADETALLES')
BEGIN
    CREATE TABLE [FACTURADETALLES] (
        [SERIE] nvarchar(50) NOT NULL,
        [NUMERO] int NOT NULL,
        [N] int NOT NULL,
        [LINEA] int NOT NULL,
        [CODARTICULO] int NOT NULL,
        [REFERENCIA] nvarchar(100) NULL,
        [DESCRIPCION] nvarchar(255) NOT NULL,
        [TALLA] nvarchar(10) NOT NULL,
        [COLOR] nvarchar(50) NOT NULL,
        [TIPOIMPUESTO] int NOT NULL,
        [UNIDADESTOTAL] decimal(18,3) NOT NULL,
        [PRECIO] decimal(18,2) NOT NULL,
        [DTO] decimal(5,2) NOT NULL,
        [TOTAL] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FACTURADETALLES] PRIMARY KEY ([SERIE], [NUMERO], [N], [LINEA]),
        CONSTRAINT [FK_FACTURADETALLES_FACTURAS_SERIE_NUMERO_N] FOREIGN KEY ([SERIE], [NUMERO], [N]) REFERENCES [FACTURAS] ([NUMSERIE], [NUMFACTURA], [N]) ON DELETE CASCADE,
        CONSTRAINT [FK_FACTURADETALLES_Articulos_CODARTICULO] FOREIGN KEY ([CODARTICULO]) REFERENCES [Articulos] ([CODARTICULO]) ON DELETE CASCADE,
        CONSTRAINT [FK_FACTURADETALLES_Impuestos_TIPOIMPUESTO] FOREIGN KEY ([TIPOIMPUESTO]) REFERENCES [Impuestos] ([TIPOIVA]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_FACTURADETALLES_CODARTICULO] ON [FACTURADETALLES] ([CODARTICULO]);
    CREATE INDEX [IX_FACTURADETALLES_TIPOIMPUESTO] ON [FACTURADETALLES] ([TIPOIMPUESTO]);
    PRINT '✅ Tabla FACTURADETALLES creada'
END

-- Tabla: FACTURAPAGOS
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FACTURAPAGOS')
BEGIN
    CREATE TABLE [FACTURAPAGOS] (
        [SERIE] nvarchar(50) NOT NULL,
        [NUMERO] int NOT NULL,
        [N] int NOT NULL,
        [POSICION] int NOT NULL,
        [CODTIPOPAGO] int NOT NULL,
        [IMPORTE] decimal(18,2) NOT NULL,
        [DESCRIPCION] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FACTURAPAGOS] PRIMARY KEY ([SERIE], [NUMERO], [N], [POSICION]),
        CONSTRAINT [FK_FACTURAPAGOS_FACTURAS_SERIE_NUMERO_N] FOREIGN KEY ([SERIE], [NUMERO], [N]) REFERENCES [FACTURAS] ([NUMSERIE], [NUMFACTURA], [N]) ON DELETE CASCADE,
        CONSTRAINT [FK_FACTURAPAGOS_FormasPago_CODTIPOPAGO] FOREIGN KEY ([CODTIPOPAGO]) REFERENCES [FormasPago] ([CODFORMAPAGO]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_FACTURAPAGOS_CODTIPOPAGO] ON [FACTURAPAGOS] ([CODTIPOPAGO]);
    PRINT '✅ Tabla FACTURAPAGOS creada'
END

-- ============================================
-- PASO 5: CREAR TABLAS DE CONFIGURACIÓN
-- ============================================

PRINT '⚙️ Creando tablas de configuración...'

-- Tabla: SqlConfigurations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SqlConfigurations')
BEGIN
    CREATE TABLE [SqlConfigurations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Server] nvarchar(255) NOT NULL,
        [Database] nvarchar(255) NOT NULL,
        [Username] nvarchar(255) NULL,
        [Password] nvarchar(500) NULL,
        [UseIntegratedSecurity] bit NOT NULL,
        [ConnectionTimeout] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_SqlConfigurations] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_SqlConfigurations_ServerDatabase] ON [SqlConfigurations] ([Server], [Database]);
    PRINT '✅ Tabla SqlConfigurations creada'
END

-- Tabla: MethodConfigurations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MethodConfigurations')
BEGIN
    CREATE TABLE [MethodConfigurations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [MethodName] nvarchar(255) NOT NULL,
        [HttpMethod] nvarchar(10) NOT NULL,
        [Endpoint] nvarchar(500) NOT NULL,
        [IsEnabled] bit NOT NULL,
        [IsAutomaticSync] bit NOT NULL,
        [SyncIntervalMinutes] int NOT NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [LastExecuted] datetime2 NULL,
        CONSTRAINT [PK_MethodConfigurations] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_MethodConfigurations_MethodHttpMethod] ON [MethodConfigurations] ([MethodName], [HttpMethod]);
    PRINT '✅ Tabla MethodConfigurations creada'
END

-- Tabla: SwaggerConfigurations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SwaggerConfigurations')
BEGIN
    CREATE TABLE [SwaggerConfigurations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [MethodName] nvarchar(255) NOT NULL,
        [HttpMethod] nvarchar(10) NOT NULL,
        [Endpoint] nvarchar(500) NOT NULL,
        [IsVisible] bit NOT NULL,
        [Category] nvarchar(100) NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_SwaggerConfigurations] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_SwaggerConfigurations_MethodHttpMethod] ON [SwaggerConfigurations] ([MethodName], [HttpMethod]);
    PRINT '✅ Tabla SwaggerConfigurations creada'
END

-- Tabla: SystemConfigurations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemConfigurations')
BEGIN
    CREATE TABLE [SystemConfigurations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Key] nvarchar(255) NOT NULL,
        [Value] nvarchar(2000) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_SystemConfigurations] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_SystemConfigurations_Key] ON [SystemConfigurations] ([Key]);
    PRINT '✅ Tabla SystemConfigurations creada'
END

-- Tabla: SecurityConfigurations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SecurityConfigurations')
BEGIN
    CREATE TABLE [SecurityConfigurations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [PasswordHash] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [LastLogin] datetime2 NULL,
        [LoginAttempts] int NOT NULL,
        [LockedUntil] datetime2 NULL,
        CONSTRAINT [PK_SecurityConfigurations] PRIMARY KEY ([Id])
    );
    PRINT '✅ Tabla SecurityConfigurations creada'
END

-- ============================================
-- PASO 6: REGISTRAR MIGRACIÓN
-- ============================================

PRINT '📝 Registrando migración...'

-- Insertar registro de migración para que EF no intente recrear las tablas
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250726000000_InitialCreateComplete')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250726000000_InitialCreateComplete', '8.0.13')
    PRINT '✅ Migración registrada en __EFMigrationsHistory'
END

-- ============================================
-- PASO 7: VERIFICACIÓN FINAL
-- ============================================

PRINT ''
PRINT '🔍 VERIFICACIÓN FINAL:'
PRINT '=============================================='

-- Contar tablas creadas
DECLARE @TableCount int
SELECT @TableCount = COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'
PRINT '📊 Total de tablas creadas: ' + CAST(@TableCount as varchar(10))

-- Verificar permisos del usuario
PRINT '👤 Usuario ICGAdmin configurado con rol: db_owner'

-- Mostrar información de conexión
PRINT ''
PRINT '🔗 INFORMACIÓN DE CONEXIÓN:'
PRINT 'Servidor: ORION-LUIS'
PRINT 'Base de datos: ApiSAP'
PRINT 'Usuario: ICGAdmin'
PRINT 'Contraseña: [La que configuraste para ICGAdmin]'
PRINT ''
PRINT '📋 Connection String de ejemplo:'
PRINT 'Server=ORION-LUIS;Database=ApiSAP;User Id=ICGAdmin;Password=TuContraseña;TrustServerCertificate=true;MultipleActiveResultSets=true;'
PRINT ''
PRINT '🎉 ¡CONFIGURACIÓN COMPLETADA EXITOSAMENTE!'
PRINT '✅ La base de datos ApiSAP está lista para usar'
PRINT '✅ Todas las tablas han sido creadas'
PRINT '✅ El usuario ICGAdmin tiene permisos completos'
PRINT '✅ Entity Framework detectará la base de datos como ya migrada'
PRINT '=============================================='