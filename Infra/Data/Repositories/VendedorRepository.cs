using Domain.Entities;
using Domain.Interfaces;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class VendedorRepository : IVendedorRepository
    {
        private readonly PortalDbContext _context;

        public VendedorRepository(PortalDbContext context)
        {
            _context = context;
        }

        public async Task Criar(Vendedor vendedor)
        {
            await _context.Vendedores.AddAsync(vendedor);
        }

        public async Task<IReadOnlyList<Vendedor>> Listar()
        {
            return await _context.Vendedores.AsNoTracking().ToListAsync();
        }

        public async Task<Vendedor?> ListarPorId(Guid id)
        {
            return await _context.Vendedores.FirstOrDefaultAsync(v => v.Id == id);
        }

        public Task Remover(Vendedor vendedor)
        {
            _context.Vendedores.Remove(vendedor);
            return Task.CompletedTask;
        }

        public async Task<bool> VerificarCpfExistente(string cpf)
        {
            return await _context.Vendedores.AnyAsync(v => v.Cpf == cpf);
        }

        public async Task<bool> VerificarEmailExistente(string email)
        {
            return await _context.Vendedores.AnyAsync(v => v.Email == email);
        }

        public async Task<(IReadOnlyList<Vendedor>, int)> ListarPaginado(int page, int pageSize)
        {
            var query = _context.Vendedores.AsQueryable();
            var total = await query.CountAsync();
            
            var items = await query
                 .OrderByDescending(i => i.DataCadastro)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            return (items, total);
        }
    }
}
