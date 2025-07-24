using Mapster;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.API.Mapping
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            // ========================================
            // CONFIGURACIONES BÁSICAS DE ENTIDADES
            // ========================================

            // Departamento
            TypeAdapterConfig<DepartamentoDto, Departamento>.NewConfig()
                .Map(dest => dest.NUMDPTO, src => src.NUMDPTO)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Secciones)
                .Ignore(dest => dest.Articulos);

            TypeAdapterConfig<Departamento, DepartamentoResponseDto>.NewConfig()
                .Map(dest => dest.NUMDPTO, src => src.NUMDPTO)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

            // Seccion
            TypeAdapterConfig<SeccionDto, Seccion>.NewConfig()
                .Map(dest => dest.NUMDPTO, src => src.NUMDPTO)
                .Map(dest => dest.NUMSECCION, src => src.NUMSECCION)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Departamento)
                .Ignore(dest => dest.Familias);

            TypeAdapterConfig<Seccion, SeccionResponseDto>.NewConfig()
                .Map(dest => dest.NUMDPTO, src => src.NUMDPTO)
                .Map(dest => dest.NUMSECCION, src => src.NUMSECCION)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

            // Familia
            TypeAdapterConfig<FamiliaDto, Familia>.NewConfig()
                .Map(dest => dest.NUMDPTO, src => src.NUMDPTO)
                .Map(dest => dest.NUMSECCION, src => src.NUMSECCION)
                .Map(dest => dest.NUMFAMILIA, src => src.NUMFAMILIA)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Seccion);

            TypeAdapterConfig<Familia, FamiliaResponseDto>.NewConfig()
                .Map(dest => dest.NUMDPTO, src => src.NUMDPTO)
                .Map(dest => dest.NUMSECCION, src => src.NUMSECCION)
                .Map(dest => dest.NUMFAMILIA, src => src.NUMFAMILIA)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

            // Cliente
            TypeAdapterConfig<ClienteDto, Cliente>.NewConfig()
                .Map(dest => dest.CODCLIENTE, src => src.CODCLIENTE)
                .Map(dest => dest.CODCONTABLE, src => src.CODCONTABLE)
                .Map(dest => dest.NOMBRECLIENTE, src => src.NOMBRECLIENTE)
                .Map(dest => dest.NOMBRECOMERCIAL, src => src.NOMBRECOMERCIAL)
                .Map(dest => dest.CIF, src => src.CIF)
                .Map(dest => dest.ALIAS, src => src.ALIAS)
                .Map(dest => dest.DIRECCION1, src => src.DIRECCION1)
                .Map(dest => dest.POBLACION, src => src.POBLACION)
                .Map(dest => dest.PROVINCIA, src => src.PROVINCIA)
                .Map(dest => dest.PAIS, src => src.PAIS)
                .Map(dest => dest.TELEFONO1, src => src.TELEFONO1)
                .Map(dest => dest.TELEFONO2, src => src.TELEFONO2)
                .Map(dest => dest.E_MAIL, src => src.E_MAIL)
                .Map(dest => dest.RIESGOCONCEDIDO, src => src.RIESGOCONCEDIDO)
                .Map(dest => dest.FACTURARCONIMPUESTO, src => src.FACTURARCONIMPUESTO)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            TypeAdapterConfig<Cliente, ClienteResponseDto>.NewConfig();

            // Vendedor
            TypeAdapterConfig<VendedorDto, Vendedor>.NewConfig()
                .Map(dest => dest.CODVENDEDOR, src => src.CODVENDEDOR)
                .Map(dest => dest.NOMBRE, src => src.NOMBRE)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            TypeAdapterConfig<Vendedor, VendedorResponseDto>.NewConfig();

            // Impuesto
            TypeAdapterConfig<ImpuestoDto, Impuesto>.NewConfig()
                .Map(dest => dest.TIPOIVA, src => src.TIPOIVA)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.IVA, src => src.IVA)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Articulos);

            TypeAdapterConfig<Impuesto, ImpuestoResponseDto>.NewConfig();

            // FormaPago
            TypeAdapterConfig<FormaPagoDto, FormaPago>.NewConfig()
                .Map(dest => dest.CODFORMAPAGO, src => src.CODFORMAPAGO)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.NUMVENCIMIENTOS, src => src.NUMVENCIMIENTOS)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            TypeAdapterConfig<FormaPago, FormaPagoResponseDto>.NewConfig();

            // Tarifa
            TypeAdapterConfig<TarifaDto, Tarifa>.NewConfig()
                .Map(dest => dest.IDTARIFAV, src => src.IDTARIFAV)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.FECHAINI, src => src.FECHAINI)
                .Map(dest => dest.FECHAFIN, src => src.FECHAFIN)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Precios);

            TypeAdapterConfig<Tarifa, TarifaResponseDto>.NewConfig();

            // ========================================
            // CONFIGURACIONES DE ARTÍCULOS
            // ========================================

            // Articulo básico
            TypeAdapterConfig<ArticuloDto, Articulo>.NewConfig()
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.DESCRIPADIC, src => src.DESCRIPADIC)
                .Map(dest => dest.TIPOIMPUESTO, src => src.TIPOIMPUESTO)
                .Map(dest => dest.DPTO, src => src.DPTO)
                .Map(dest => dest.SECCION, src => src.SECCION)
                .Map(dest => dest.FAMILIA, src => src.FAMILIA)
                .Map(dest => dest.UNID1C, src => src.UNID1C)
                .Map(dest => dest.UNID1V, src => src.UNID1V)
                .Map(dest => dest.REFPROVEEDOR, src => src.REFPROVEEDOR)
                .Map(dest => dest.USASTOCKS, src => src.USASTOCKS)
                .Map(dest => dest.IMPUESTOCOMPRA, src => src.IMPUESTOCOMPRA)
                .Map(dest => dest.DESCATALOGADO, src => src.DESCATALOGADO)
                .Map(dest => dest.UDSTRASPASO, src => src.UDSTRASPASO)
                .Map(dest => dest.TIPOARTICULO, src => src.TIPOARTICULO)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Lineas)
                .Ignore(dest => dest.Precios)
                .Ignore(dest => dest.Impuesto)
                .Ignore(dest => dest.Departamento);

            TypeAdapterConfig<Articulo, ArticuloResponseDto>.NewConfig()
                .Ignore(dest => dest.Impuesto)
                .Ignore(dest => dest.Departamento)
                .Ignore(dest => dest.Lineas)
                .Ignore(dest => dest.Precios)
                .Ignore(dest => dest.Estadisticas);

            // Articulo completo SAP (NUEVO)
            TypeAdapterConfig<ArticuloCompletoSAPRequest, ArticuloDto>.NewConfig()
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.DESCRIPADIC, src => src.DESCRIPADIC)
                .Map(dest => dest.TIPOIMPUESTO, src => src.TIPOIMPUESTO)
                .Map(dest => dest.DPTO, src => src.DPTO)
                .Map(dest => dest.SECCION, src => src.SECCION)
                .Map(dest => dest.FAMILIA, src => src.FAMILIA)
                .Map(dest => dest.UNID1C, src => src.UNID1C)
                .Map(dest => dest.UNID1V, src => src.UNID1V)
                .Map(dest => dest.REFPROVEEDOR, src => src.REFPROVEEDOR)
                .Map(dest => dest.USASTOCKS, src => src.USASTOCKS)
                .Map(dest => dest.IMPUESTOCOMPRA, src => src.IMPUESTOCOMPRA)
                .Map(dest => dest.DESCATALOGADO, src => src.DESCATALOGADO)
                .Map(dest => dest.UDSTRASPASO, src => src.UDSTRASPASO)
                .Map(dest => dest.TIPOARTICULO, src => src.TIPOARTICULO);

            // ArticuloLinea
            TypeAdapterConfig<ArticuloLineaDto, ArticuloLinea>.NewConfig()
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.TALLA, src => src.TALLA)
                .Map(dest => dest.COLOR, src => src.COLOR)
                .Map(dest => dest.CODBARRAS, src => src.CODBARRAS)
                .Map(dest => dest.COSTEMEDIO, src => src.COSTEMEDIO)
                .Map(dest => dest.COSTESTOCK, src => src.COSTESTOCK)
                .Map(dest => dest.ULTIMOCOSTE, src => src.ULTIMOCOSTE)
                .Map(dest => dest.CODBARRAS2, src => src.CODBARRAS2)
                .Map(dest => dest.CODBARRAS3, src => src.CODBARRAS3)
                .Map(dest => dest.DESCATALOGADO, src => src.DESCATALOGADO)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Articulo);

            TypeAdapterConfig<ArticuloLinea, ArticuloLineaResponseDto>.NewConfig()
                .Ignore(dest => dest.Articulo)
                .Ignore(dest => dest.Precios)
                .Ignore(dest => dest.Estadisticas);

            // ========================================
            // CONFIGURACIONES DE PRECIOS
            // ========================================

            // Precio básico
            TypeAdapterConfig<PrecioDto, Precio>.NewConfig()
                .Map(dest => dest.IDTARIFAV, src => src.IDTARIFAV)
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.TALLA, src => src.TALLA)
                .Map(dest => dest.COLOR, src => src.COLOR)
                .Map(dest => dest.CODBARRAS, src => src.CODBARRAS)
                .Map(dest => dest.PBRUTO, src => src.PBRUTO)
                .Map(dest => dest.DTO, src => src.DTO)
                .Map(dest => dest.PNETO, src => src.PNETO)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Tarifa)
                .Ignore(dest => dest.Articulo);

            TypeAdapterConfig<Precio, PrecioResponseDto>.NewConfig()
                .Ignore(dest => dest.Tarifa)
                .Ignore(dest => dest.Articulo)
                .Ignore(dest => dest.Estadisticas);

            // ========================================
            // CONFIGURACIONES DE FACTURACIÓN (NUEVO)
            // ========================================

            // Factura
            TypeAdapterConfig<FacturaDto, Factura>.NewConfig()
                .Map(dest => dest.NUMSERIE, src => src.NUMSERIE)
                .Map(dest => dest.NUMFACTURA, src => src.NUMFACTURA)
                .Map(dest => dest.N, src => src.N)
                .Map(dest => dest.FECHA, src => src.FECHA)
                .Map(dest => dest.CODCLIENTE, src => src.CODCLIENTE)
                .Map(dest => dest.CODVENDEDOR, src => src.CODVENDEDOR)
                .Map(dest => dest.TOTALBRUTO, src => src.TOTALBRUTO)
                .Map(dest => dest.TOTALIMPUESTOS, src => src.TOTALIMPUESTOS)
                .Map(dest => dest.TOTDTOCOMERCIAL, src => src.TOTDTOCOMERCIAL)
                .Map(dest => dest.TOTALNETO, src => src.TOTALNETO)
                .Map(dest => dest.TIPODOC, src => src.TIPODOC)
                .Map(dest => dest.FECHACREADO, src => src.FECHACREADO)
                .Map(dest => dest.FECHAMODIFICADO, src => src.FECHAMODIFICADO)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Cliente)
                .Ignore(dest => dest.Vendedor)
                .Ignore(dest => dest.Detalles)
                .Ignore(dest => dest.Pagos);

            TypeAdapterConfig<Factura, FacturaResponseDto>.NewConfig()
                .Ignore(dest => dest.Cliente)
                .Ignore(dest => dest.Vendedor)
                .Ignore(dest => dest.Detalles)
                .Ignore(dest => dest.Pagos)
                .Ignore(dest => dest.Estadisticas);

            // FacturaDetalle
            TypeAdapterConfig<FacturaDetalleDto, FacturaDetalle>.NewConfig()
                .Map(dest => dest.SERIE, src => src.SERIE)
                .Map(dest => dest.NUMERO, src => src.NUMERO)
                .Map(dest => dest.N, src => src.N)
                .Map(dest => dest.LINEA, src => src.LINEA)
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.REFERENCIA, src => src.REFERENCIA)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Map(dest => dest.TALLA, src => src.TALLA)
                .Map(dest => dest.COLOR, src => src.COLOR)
                .Map(dest => dest.TIPOIMPUESTO, src => src.TIPOIMPUESTO)
                .Map(dest => dest.UNIDADESTOTAL, src => src.UNIDADESTOTAL)
                .Map(dest => dest.PRECIO, src => src.PRECIO)
                .Map(dest => dest.DTO, src => src.DTO)
                .Map(dest => dest.TOTAL, src => src.TOTAL)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Factura)
                .Ignore(dest => dest.Articulo)
                .Ignore(dest => dest.Impuesto);

            TypeAdapterConfig<FacturaDetalle, FacturaDetalleResponseDto>.NewConfig()
                .Ignore(dest => dest.Articulo)
                .Ignore(dest => dest.Impuesto);

            // FacturaPago
            TypeAdapterConfig<FacturaPagoDto, FacturaPago>.NewConfig()
                .Map(dest => dest.SERIE, src => src.SERIE)
                .Map(dest => dest.NUMERO, src => src.NUMERO)
                .Map(dest => dest.N, src => src.N)
                .Map(dest => dest.POSICION, src => src.POSICION)
                .Map(dest => dest.CODTIPOPAGO, src => src.CODTIPOPAGO)
                .Map(dest => dest.IMPORTE, src => src.IMPORTE)
                .Map(dest => dest.DESCRIPCION, src => src.DESCRIPCION)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Factura)
                .Ignore(dest => dest.FormaPago);

            TypeAdapterConfig<FacturaPago, FacturaPagoResponseDto>.NewConfig()
                .Ignore(dest => dest.FormaPago);

            // ========================================
            // CONFIGURACIONES INVERSAS (Entidad -> DTO)
            // ========================================

            // Para respuestas GET - mapeo automático pero ignorando navegación
            TypeAdapterConfig<Departamento, Departamento>.NewConfig()
                .Ignore(dest => dest.Secciones)
                .Ignore(dest => dest.Articulos);

            TypeAdapterConfig<Seccion, Seccion>.NewConfig()
                .Ignore(dest => dest.Departamento)
                .Ignore(dest => dest.Familias);

            TypeAdapterConfig<Familia, Familia>.NewConfig()
                .Ignore(dest => dest.Seccion);

            TypeAdapterConfig<Vendedor, Vendedor>.NewConfig();

            TypeAdapterConfig<Impuesto, Impuesto>.NewConfig()
                .Ignore(dest => dest.Articulos);

            TypeAdapterConfig<FormaPago, FormaPago>.NewConfig();

            TypeAdapterConfig<Cliente, Cliente>.NewConfig();

            TypeAdapterConfig<Tarifa, Tarifa>.NewConfig()
                .Ignore(dest => dest.Precios);

            TypeAdapterConfig<Articulo, Articulo>.NewConfig()
                .Ignore(dest => dest.Lineas)
                .Ignore(dest => dest.Precios)
                .Ignore(dest => dest.Impuesto)
                .Ignore(dest => dest.Departamento);

            TypeAdapterConfig<ArticuloLinea, ArticuloLinea>.NewConfig()
                .Ignore(dest => dest.Articulo);

            TypeAdapterConfig<Precio, Precio>.NewConfig()
                .Ignore(dest => dest.Tarifa)
                .Ignore(dest => dest.Articulo);

            TypeAdapterConfig<Factura, Factura>.NewConfig()
                .Ignore(dest => dest.Cliente)
                .Ignore(dest => dest.Vendedor)
                .Ignore(dest => dest.Detalles)
                .Ignore(dest => dest.Pagos);

            TypeAdapterConfig<FacturaDetalle, FacturaDetalle>.NewConfig()
                .Ignore(dest => dest.Factura)
                .Ignore(dest => dest.Articulo)
                .Ignore(dest => dest.Impuesto);

            TypeAdapterConfig<FacturaPago, FacturaPago>.NewConfig()
                .Ignore(dest => dest.Factura)
                .Ignore(dest => dest.FormaPago);
        }
    }
}