using Apitransac.Common.Exceptions;
using Apitransac.Data;
using Apitransac.DTOs.Auth;
using Apitransac.Models;
using Apitransac.Models.Configuration;
using Apitransac.Services.Jwt;
using Apitransac.Services.Password;
using Apitransac.Services.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace Apitransac.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;


        public AuthService(ApplicationDbContext context, IPasswordHasherService passwordHasher, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }


        public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser is not null)
            {
                throw new ConflictException("El email ya está registrado.");
            }
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            var userRole = await _context.Roles.FirstOrDefaultAsync
                (
                    r => r.Name == "User" && r.Status == RoleStatus.Active
                );

            if (userRole is null)
            {
                throw new ConflictException("El rol User no está configurado.");
            }

            user.UserRoles.Add(new UserRole
            {
                RoleId = userRole.Id,
                AssignedAt = DateTime.UtcNow
            });


            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = new List<string> { userRole.Name }
            };
        }


        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync( u => u.Email == request.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException( "Credenciales inválidas.");
            }

            if (user is null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }
            if (user.Status != UserStatus.Active)
            {
                throw new UnauthorizedAccessException("El usuario no está activo.");
            }

            var passwordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            user.LastLoginAt = DateTime.UtcNow;

            //await _context.SaveChangesAsync();

            var roles = user.UserRoles
                .Where(ur => ur.Role.Status == RoleStatus.Active)
                .Select(ur => ur.Role.Name).ToList();

            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

            var accessTokenExpiration = _jwtTokenService.GetAccessTokenExpiration();

            var refreshTokenResult = _refreshTokenService.CreateRefreshToken(user.Id);


            //aqui puiede fallar
            _context.RefreshTokens.Add(refreshTokenResult.Entity);
            await _context.SaveChangesAsync();


            var refreshToken = _refreshTokenService.GenerateToken();

            var refreshTokenHash = _refreshTokenService.HashToken(refreshToken);


            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenResult.Token,
                AccessTokenExpiresAt = accessTokenExpiration,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Status = user.Status.ToString(),
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    Roles = roles
                }
            };
        }
    }
}
