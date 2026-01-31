using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class ComissaoRepository : IComissaoRepository
    {
        private readonly PortalDbContext _context;

        public ComissaoRepository(PortalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Comissao comissao)
        {
            await _context.Comissoes.AddAsync(comissao);
        }

        public async Task<IReadOnlyList<Comissao>> GetAllAsync()
        {
            return await _context.Comissoes.Include(c => c.Invoice).ThenInclude(i => i.Vendedor).ToListAsync();
        }

        public IQueryable<Comissao> Query()
        {
            return _context.Comissoes.Include(c => c.Invoice).ThenInclude(i => i.Vendedor);
        }

        public async Task<Comissao?> GetByIdAsync(Guid id)
        {
            return await _context.Comissoes.FindAsync(id);
        }

        public async Task<Comissao?> GetByInvoiceIdAsync(Guid invoiceId)
        {
            return await _context.Comissoes.FirstOrDefaultAsync(c => c.InvoiceId == invoiceId);
        }

        public async Task<bool> ExisteComissaoParaVendedor(Guid vendedorId)
        {
            return await _context.Comissoes.AnyAsync(c => c.Invoice.VendedorId == vendedorId);
        }
    }
}
