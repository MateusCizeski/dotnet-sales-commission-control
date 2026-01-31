using Domain.Entities;
using Domain.Interfaces;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly PortalDbContext _context;

        public InvoiceRepository(PortalDbContext context)
        {
            _context = context;
        }

        public async Task Criar(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public Task Editar(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            return Task.CompletedTask;
        }

        public async Task<Invoice?> ListarPorId(Guid id)
        {
            return await _context.Invoices.Include(i => i.Vendedor).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IReadOnlyList<Invoice>> Listar(Guid? vendedorId = null)
        {
            var query = _context.Invoices.Include(i => i.Vendedor).AsQueryable();

            if (vendedorId.HasValue)
            {
                query = query.Where(i => i.VendedorId == vendedorId.Value);
            }

            return await query.ToListAsync();
        }

        public IQueryable<Invoice> Query()
        {
            return _context.Invoices.Include(i => i.Vendedor);
        }

        public async Task<IReadOnlyList<Invoice>> ListarPorPeriodo(DateTime inicio, DateTime fim)
        {
            return await _context.Invoices.Where(i => i.DataEmissao >= inicio && i.DataEmissao <= fim).ToListAsync();
        }

        public async Task<IReadOnlyList<Invoice>> ListarPorVendedor(Guid vendedorId)
        {
            return await _context.Invoices.Where(i => i.VendedorId == vendedorId).ToListAsync();
        }

        public async Task<string?> BuscarUltimoNumeroInvoice()
        {
            var ultimoInvoice = await _context.Invoices.OrderByDescending(i => i.DataEmissao).FirstOrDefaultAsync();

            return ultimoInvoice?.NumeroInvoice;
        }

        public async Task<string> BuscarProximoNumeroInvoice()
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT NEXT VALUE FOR InvoiceNumeroSeq";

            var result = await command.ExecuteScalarAsync();
            var numero = Convert.ToInt64(result);

            return $"INV-{numero:D4}";
        }

        public async Task<(IReadOnlyList<Invoice>, int)> ListarPaginado(Guid? vendedorId, int page, int pageSize)
        {
            var query = _context.Invoices.Include(i => i.Vendedor).AsQueryable();

            if (vendedorId.HasValue)
            {
                query = query.Where(i => i.VendedorId == vendedorId.Value);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.DataEmissao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
