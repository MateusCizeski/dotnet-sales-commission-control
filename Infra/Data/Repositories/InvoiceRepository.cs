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

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.AsNoTracking().ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices.Include(i => i.Vendedor).Include(i => i.Comissao).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IReadOnlyList<Invoice>> GetByPeriodoAsync(DateTime inicio, DateTime fim)
        {
            return await _context.Invoices.Where(i => i.DataEmissao >= inicio && i.DataEmissao <= fim).ToListAsync();
        }

        public async Task<IReadOnlyList<Invoice>> GetByVendedorIdAsync(Guid vendedorId)
        {
            return await _context.Invoices.Where(i => i.VendedorId == vendedorId).ToListAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
