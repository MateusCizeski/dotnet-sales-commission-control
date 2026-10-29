using Application.Abstractions;
using Application.Requests;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public interface IAuthService
    {
        /// <summary>Returns the access token, or null when the email or the password is wrong.</summary>
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }

    public class AuthService : IAuthService
    {
        // Used when the email does not exist, so both cases take the same time to answer
        // and the response time does not reveal which emails are registered.
        private static User? _dummyUser;

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);

            if (user == null)
            {
                _passwordHasher.Verify(GetDummyUser(), request.Password);
                return null;
            }

            if (!_passwordHasher.Verify(user, request.Password))
            {
                return null;
            }

            var token = _tokenService.CreateToken(user);
            return new LoginResponse(token.Token, token.ExpiresAt, user.Role.ToString(), user.SellerId);
        }

        /// <summary>A user with a real hash, so verifying it costs the same as verifying a registered user.</summary>
        private User GetDummyUser()
        {
            if (_dummyUser == null)
            {
                var placeholder = User.CreateAdmin("dummy@example.com", "placeholder");
                _dummyUser = User.CreateAdmin("dummy@example.com", _passwordHasher.Hash(placeholder, Guid.NewGuid().ToString()));
            }

            return _dummyUser;
        }
    }
}
