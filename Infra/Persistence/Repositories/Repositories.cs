using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories
{
    // Write-side repositories. They only queue changes in the scoped PortalDbContext;
    // ApiBase's UnitOfWork (same scoped instance) saves them.

    public class SellerRepository : ISellerRepository
    {
        private readonly PortalDbContext _context;

        public SellerRepository(PortalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Seller seller, CancellationToken cancellationToken = default)
        {
            await _context.Sellers.AddAsync(seller, cancellationToken);
        }

        public Task<Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Sellers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public void Remove(Seller seller)
        {
            _context.Sellers.Remove(seller);
        }

        public Task<bool> CpfExistsAsync(string cpf, CancellationToken cancellationToken = default)
        {
            return _context.Sellers.AnyAsync(s => s.Cpf == cpf, cancellationToken);
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return _context.Sellers.AnyAsync(s => s.Email == email, cancellationToken);
        }
    }

    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly PortalDbContext _context;

        public InvoiceRepository(PortalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
        {
            // The commission is added together through the navigation (same aggregate).
            await _context.Invoices.AddAsync(invoice, cancellationToken);
        }

        public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Invoices
                .Include(i => i.Commission)
                .Include(i => i.Seller)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }
    }

    public class CommissionRepository : ICommissionRepository
    {
        private readonly PortalDbContext _context;

        public CommissionRepository(PortalDbContext context)
        {
            _context = context;
        }

        public Task<Commission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Commissions.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public Task<bool> ExistsForSellerAsync(Guid sellerId, CancellationToken cancellationToken = default)
        {
            return _context.Commissions.AnyAsync(c => c.Invoice!.SellerId == sellerId, cancellationToken);
        }
    }

    public class UserRepository : IUserRepository
    {
        private readonly PortalDbContext _context;

        public UserRepository(PortalDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }
    }
}
