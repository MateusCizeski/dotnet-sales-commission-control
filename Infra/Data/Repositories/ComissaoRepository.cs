using Domain.Entities;
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

        public async Task Criar(Comissao comissao)
        {
            await _context.Comissoes.AddAsync(comissao);
        }

        public async Task<IReadOnlyList<Comissao>> Listar()
        {
            return await _context.Comissoes.Include(c => c.Invoice).ThenInclude(i => i.Vendedor).ToListAsync();
        }

        public IQueryable<Comissao> Query()
        {
            return _context.Comissoes.Include(c => c.Invoice).ThenInclude(i => i.Vendedor);
        }

        public async Task<Comissao?> ListarPorId(Guid id)
        {
            return await _context.Comissoes.FindAsync(id);
        }

        public async Task<Comissao?> ListarPorInvoice(Guid invoiceId)
        {
            return await _context.Comissoes.FirstOrDefaultAsync(c => c.InvoiceId == invoiceId);
        }

        public async Task<bool> ExisteComissaoParaVendedor(Guid vendedorId)
        {
            return await _context.Comissoes.AnyAsync(c => c.Invoice.VendedorId == vendedorId);
        }

        public async Task<(IReadOnlyList<Comissao>, int)> ListarPaginado(int page, int pageSize)
        {
            var query = _context.Comissoes.Include(i => i.Invoice).ThenInclude(i => i.Vendedor).AsQueryable();

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
