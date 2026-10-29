using ApiBase.Domain.Exceptions;
using ApiBase.Domain.Interfaces;
using Application.Abstractions;
using Application.Requests;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;

namespace Tests.Unit.Application
{
    public class CommissionServiceTests
    {
        private readonly Mock<ICommissionRepository> _commissions = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly CommissionService _service;

        public CommissionServiceTests()
        {
            _service = new CommissionService(_commissions.Object, _unitOfWork.Object, new FixedTimeProvider(TestData.Now));
        }

        [Fact]
        public async Task Pay_UsesTheClockAndCommits()
        {
            var commission = TestData.Invoice().Commission;
            _commissions.Setup(r => r.GetByIdAsync(commission.Id, It.IsAny<CancellationToken>())).ReturnsAsync(commission);

            await _service.PayAsync(commission.Id);

            Assert.Equal(CommissionStatus.Paid, commission.Status);
            Assert.Equal(TestData.Now, commission.PaidAt);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Pay_Unknown_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.PayAsync(Guid.NewGuid()));
        }
    }

    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<ITokenService> _tokens = new();
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _hasher.Setup(h => h.Hash(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed");
            _tokens.Setup(t => t.CreateToken(It.IsAny<User>())).Returns(new AccessToken("token", TestData.Now.AddHours(1)));
            _service = new AuthService(_users.Object, _hasher.Object, _tokens.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var user = User.CreateAdmin("admin@example.com", "hashed");
            _users.Setup(r => r.GetByEmailAsync("admin@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify(user, "right")).Returns(true);

            var response = await _service.LoginAsync(new LoginRequest { Email = " Admin@Example.com ", Password = "right" });

            Assert.NotNull(response);
            Assert.Equal("token", response.AccessToken);
            Assert.Equal("Admin", response.Role);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsNull()
        {
            var user = User.CreateAdmin("admin@example.com", "hashed");
            _users.Setup(r => r.GetByEmailAsync("admin@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify(user, "wrong")).Returns(false);

            Assert.Null(await _service.LoginAsync(new LoginRequest { Email = "admin@example.com", Password = "wrong" }));
            _tokens.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Login_UnknownEmail_StillVerifiesAPasswordAndReturnsNull()
        {
            _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

            Assert.Null(await _service.LoginAsync(new LoginRequest { Email = "nobody@example.com", Password = "x" }));

            // Same work as a wrong password, so the response time does not reveal registered emails.
            _hasher.Verify(h => h.Verify(It.IsAny<User>(), "x"), Times.Once);
        }
    }
}
