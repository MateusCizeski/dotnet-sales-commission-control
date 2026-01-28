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
            await _context.SaveChangesAsync();
        }

        public async Task<Comissao?> GetByIdAsync(Guid id)
        {
            return await _context.Comissoes.FindAsync(id);
        }

        public async Task<Comissao?> GetByInvoiceIdAsync(Guid invoiceId)
        {
            return await _context.Comissoes.FirstOrDefaultAsync(c => c.InvoiceId == invoiceId);
        }

        public async Task<IReadOnlyList<Comissao>> GetByVendedorIdAsync(Guid vendedorId)
        {
            return await _context.Comissoes.Include(c => c.Invoice).Where(c => c.Invoice.VendedorId == vendedorId).ToListAsync();
        }

        public async Task<IReadOnlyList<Comissao>> GetPagasAsync()
        {
            return await _context.Comissoes.Where(c => c.Status == StatusComissao.Paga).ToListAsync();
        }

        public async Task<IReadOnlyList<Comissao>> GetPendentesAsync()
        {
            return await _context.Comissoes.Where(c => c.Status == StatusComissao.Pendente).ToListAsync();
        }

        public async Task UpdateAsync(Comissao comissao)
        {
            _context.Comissoes.Update(comissao);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteComissaoParaVendedor(Guid vendedorId)
        {
            return await _context.Comissoes.AnyAsync(c => c.Invoice.VendedorId == vendedorId);
        }
    }
}
