using Domain.Interfaces;
using Infra.Data.Context;

namespace Infra.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PortalDbContext _context;

        public UnitOfWork(PortalDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
