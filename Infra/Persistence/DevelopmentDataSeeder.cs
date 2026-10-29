using ApiBase.Domain.Interfaces;
using Application.Abstractions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Persistence
{
    /// <summary>
    /// Creates the test users for local development (and the integration tests).
    /// Emails and passwords come from configuration ("Seed" section: User Secrets, .env or
    /// environment variables) — never from the source code. Nothing is created when they are missing.
    /// All data is fictitious.
    /// </summary>
    public class DevelopmentDataSeeder
    {
        // Fictitious but valid CPF, for the demo seller only.
        private const string DemoSellerCpf = "52998224725";

        private readonly IUserRepository _userRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<DevelopmentDataSeeder> _logger;

        public DevelopmentDataSeeder(IUserRepository userRepository, ISellerRepository sellerRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork, TimeProvider timeProvider, ILogger<DevelopmentDataSeeder> logger)
        {
            _userRepository = userRepository;
            _sellerRepository = sellerRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task SeedAsync(IConfiguration configuration, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.AnyAsync(cancellationToken))
            {
                return;
            }

            var seed = configuration.GetSection("Seed");
            string? adminEmail = seed["AdminEmail"];
            string? adminPassword = seed["AdminPassword"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                _logger.LogWarning("No users were created: set Seed:AdminEmail and Seed:AdminPassword (User Secrets or environment variables).");
                return;
            }

            await _userRepository.AddAsync(CreateUser(adminEmail, adminPassword, seller: null), cancellationToken);

            string? sellerEmail = seed["SellerEmail"];
            string? sellerPassword = seed["SellerPassword"];

            if (!string.IsNullOrWhiteSpace(sellerEmail) && !string.IsNullOrWhiteSpace(sellerPassword))
            {
                var seller = new Seller("Demo Seller", DemoSellerCpf, sellerEmail, 10, _timeProvider.GetUtcNow().UtcDateTime);
                await _sellerRepository.AddAsync(seller, cancellationToken);
                await _userRepository.AddAsync(CreateUser(sellerEmail, sellerPassword, seller), cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Development users created.");
        }

        private User CreateUser(string email, string password, Seller? seller)
        {
            // The hasher needs a user instance; the hash does not depend on its data.
            var placeholder = User.CreateAdmin(email, "placeholder");
            string hash = _passwordHasher.Hash(placeholder, password);

            return seller == null ? User.CreateAdmin(email, hash) : User.CreateSeller(email, hash, seller);
        }
    }
}
