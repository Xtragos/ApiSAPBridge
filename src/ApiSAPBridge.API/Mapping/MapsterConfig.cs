using Mapster;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.DTOs;

namespace ApiSAPBridge.API.Mapping
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            // Configuración automática - Mapster mapea automáticamente propiedades con el mismo nombre
            // Solo necesitamos configuraciones especiales aquí

            // Configuración para ArticuloCompleto DTO
            TypeAdapterConfig<ArticuloCompletoDto, Articulo>.NewConfig()
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

            // Configuración para ArticuloLinea DTO
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

            // Configuración para Precio DTO
            TypeAdapterConfig<PrecioDto, Precio>.NewConfig()
                .Map(dest => dest.IDTARIFAV, src => src.IDTARIFAV)
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.CODBARRAS, src => src.CODBARRAS)
                .Map(dest => dest.TALLA, src => src.TALLA)
                .Map(dest => dest.COLOR, src => src.COLOR)
                .Map(dest => dest.PBRUTO, src => src.PBRUTO)
                .Map(dest => dest.DTO, src => src.DTO)
                .Map(dest => dest.PNETO, src => src.PNETO)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Tarifa)
                .Ignore(dest => dest.Articulo);

            // Configuración para PrecioBatchItem DTO
            TypeAdapterConfig<PrecioBatchItemDto, Precio>.NewConfig()
                .Map(dest => dest.CODARTICULO, src => src.CODARTICULO)
                .Map(dest => dest.CODBARRAS, src => src.CODBARRAS)
                .Map(dest => dest.TALLA, src => src.TALLA)
                .Map(dest => dest.COLOR, src => src.COLOR)
                .Map(dest => dest.PBRUTO, src => src.PBRUTO)
                .Map(dest => dest.DTO, src => src.DTO)
                .Map(dest => dest.PNETO, src => src.PNETO)
                .Ignore(dest => dest.IDTARIFAV) // Se asigna por separado
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Tarifa)
                .Ignore(dest => dest.Articulo);

            // Configuraciones de respuesta (entidad a DTO para GET)
            TypeAdapterConfig<Departamento, Departamento>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Secciones)
                .Ignore(dest => dest.Articulos);

            TypeAdapterConfig<Seccion, Seccion>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Departamento)
                .Ignore(dest => dest.Familias);

            TypeAdapterConfig<Familia, Familia>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Seccion);

            TypeAdapterConfig<Vendedor, Vendedor>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            TypeAdapterConfig<Impuesto, Impuesto>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Articulos);

            TypeAdapterConfig<FormaPago, FormaPago>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            TypeAdapterConfig<Cliente, Cliente>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            TypeAdapterConfig<Tarifa, Tarifa>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Precios);

            TypeAdapterConfig<Articulo, Articulo>.NewConfig()
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Lineas)
                .Ignore(dest => dest.Precios)
                .Ignore(dest => dest.Impuesto)
                .Ignore(dest => dest.Departamento);
        }
    }
}
