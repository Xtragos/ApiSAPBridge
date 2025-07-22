using ApiSAPBridge.Data.Repositories;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.Entities;

namespace ApiSAPBridge.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Departamento> Departamentos { get; }
        IGenericRepository<Seccion> Secciones { get; }
        IGenericRepository<Familia> Familias { get; }
        IGenericRepository<Vendedor> Vendedores { get; }
        IGenericRepository<Impuesto> Impuestos { get; }
        IGenericRepository<FormaPago> FormasPago { get; }
        IGenericRepository<Cliente> Clientes { get; }
        IGenericRepository<Tarifa> Tarifas { get; }
        IGenericRepository<Articulo> Articulos { get; }
        IGenericRepository<ArticuloLinea> ArticuloLineas { get; }
        IGenericRepository<Precio> Precios { get; }
        IGenericRepository<ApiLog> ApiLogs { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}