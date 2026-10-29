using ApiBase.Application.ApplicationGuid;
using ApiBase.Domain.Interfaces;
using ApiBase.Infra.Options;
using Application.Abstractions;
using Application.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Infra.Queries
{
    // Read side (the "Q" of CQRS): ApiBase turns the query string (filter, sort, page, fields)
    // into SQL and projects the entities into the views. No domain rules run here.
    // DefaultFilter applies ownership: a Seller user only ever sees their own rows,
    // whatever filter the client sends (the seller id comes from the token, never from the request).

    public interface ISellerQueries : IApplicationGuid<SellerView>
    {
        Task<List<SellerOptionView>> GetActiveOptionsAsync(CancellationToken cancellationToken = default);
    }

    public class SellerQueries : ApplicationGuid<Seller, IRepositoryBase<Seller>, SellerView>, ISellerQueries
    {
        public SellerQueries(IUnitOfWork unitOfWork, IRepositoryBase<Seller> repository, ILogger<SellerQueries> logger, IOptions<ApiBaseOptions> options)
            : base(unitOfWork, repository, logger, options) { }

        public Task<List<SellerOptionView>> GetActiveOptionsAsync(CancellationToken cancellationToken = default)
        {
            return Repository.Get()
                .Where(s => s.IsActive)
                .OrderBy(s => s.FullName)
                .Select(s => new SellerOptionView { Id = s.Id, FullName = s.FullName })
                .ToListAsync(cancellationToken);
        }
    }

    public class InvoiceQueries : ApplicationGuid<Invoice, IRepositoryBase<Invoice>, InvoiceView>
    {
        private readonly ICurrentUser _currentUser;

        public InvoiceQueries(IUnitOfWork unitOfWork, IRepositoryBase<Invoice> repository, ILogger<InvoiceQueries> logger, IOptions<ApiBaseOptions> options, ICurrentUser currentUser)
            : base(unitOfWork, repository, logger, options)
        {
            _currentUser = currentUser;
        }

        public override Expression<Func<Invoice, bool>> DefaultFilter()
        {
            if (_currentUser.IsAdmin)
            {
                return invoice => true;
            }

            // Deny by default: a user without a seller id matches no row.
            Guid sellerId = _currentUser.SellerId ?? Guid.Empty;
            return invoice => invoice.SellerId == sellerId;
        }
    }

    public class CommissionQueries : ApplicationGuid<Commission, IRepositoryBase<Commission>, CommissionView>
    {
        private readonly ICurrentUser _currentUser;

        public CommissionQueries(IUnitOfWork unitOfWork, IRepositoryBase<Commission> repository, ILogger<CommissionQueries> logger, IOptions<ApiBaseOptions> options, ICurrentUser currentUser)
            : base(unitOfWork, repository, logger, options)
        {
            _currentUser = currentUser;
        }

        public override Expression<Func<Commission, bool>> DefaultFilter()
        {
            if (_currentUser.IsAdmin)
            {
                return commission => true;
            }

            Guid sellerId = _currentUser.SellerId ?? Guid.Empty;
            return commission => commission.Invoice!.SellerId == sellerId;
        }
    }
}
