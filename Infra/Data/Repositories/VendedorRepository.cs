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

        public async Task AddAsync(Vendedor vendedor)
        {
            await _context.Vendedores.AddAsync(vendedor);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Vendedor>> GetAllAsync()
        {
           return await _context.Vendedores.AsNoTracking().ToListAsync();
        }

        public async Task<Vendedor?> GetByCpfAsync(string cpf)
        {
            return await _context.Vendedores.FirstOrDefaultAsync(v => v.Cpf == cpf);
        }

        public async Task<Vendedor?> GetByEmailAsync(string email)
        {
            return await _context.Vendedores.FirstOrDefaultAsync(v => v.Email == email);
        }

        public async Task<Vendedor?> GetByIdAsync(Guid id)
        {
            return await _context.Vendedores.FindAsync(id);
        }

        public async Task RemoveAsync(Vendedor vendedor)
        {
            _context.Vendedores.Remove(vendedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vendedor vendedor)
        {
            _context.Vendedores.Update(vendedor);
            await _context.SaveChangesAsync();
        }
    }
}
