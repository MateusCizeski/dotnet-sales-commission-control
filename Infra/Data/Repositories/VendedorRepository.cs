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
        }

        public async Task<IReadOnlyList<Vendedor>> GetAllAsync()
        {
            return await _context.Vendedores.AsNoTracking().ToListAsync();
        }

        public async Task<Vendedor?> GetByIdAsync(Guid id)
        {
            return await _context.Vendedores.FirstOrDefaultAsync(v => v.Id == id);
        }

        public Task RemoveAsync(Vendedor vendedor)
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
    }
}
