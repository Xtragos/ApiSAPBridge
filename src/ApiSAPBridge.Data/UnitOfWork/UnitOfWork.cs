using Microsoft.EntityFrameworkCore.Storage;
using ApiSAPBridge.Data.Repositories;
using ApiSAPBridge.Models;
using ApiSAPBridge.Models.Entities;

namespace ApiSAPBridge.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiSAPBridgeDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApiSAPBridgeDbContext context)
        {
            _context = context;

            Departamentos = new GenericRepository<Departamento>(_context);
            Secciones = new GenericRepository<Seccion>(_context);
            Familias = new GenericRepository<Familia>(_context);
            Vendedores = new GenericRepository<Vendedor>(_context);
            Impuestos = new GenericRepository<Impuesto>(_context);
            FormasPago = new GenericRepository<FormaPago>(_context);
            Clientes = new GenericRepository<Cliente>(_context);
            Tarifas = new GenericRepository<Tarifa>(_context);
            Articulos = new GenericRepository<Articulo>(_context);
            ArticuloLineas = new GenericRepository<ArticuloLinea>(_context);
            Precios = new GenericRepository<Precio>(_context);
            ApiLogs = new GenericRepository<ApiLog>(_context);
        }

        public IGenericRepository<Departamento> Departamentos { get; }
        public IGenericRepository<Seccion> Secciones { get; }
        public IGenericRepository<Familia> Familias { get; }
        public IGenericRepository<Vendedor> Vendedores { get; }
        public IGenericRepository<Impuesto> Impuestos { get; }
        public IGenericRepository<FormaPago> FormasPago { get; }
        public IGenericRepository<Cliente> Clientes { get; }
        public IGenericRepository<Tarifa> Tarifas { get; }
        public IGenericRepository<Articulo> Articulos { get; }
        public IGenericRepository<ArticuloLinea> ArticuloLineas { get; }
        public IGenericRepository<Precio> Precios { get; }
        public IGenericRepository<ApiLog> ApiLogs { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}