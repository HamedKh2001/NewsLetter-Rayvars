using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Exceptions;
using SharedKernel.Extensions;
using SSO.Application.Common.Settings;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword;
using SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;
using SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser;
using SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken;
using SSO.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IUserRepository _userRepository;
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOptionsMonitor<BearerTokensConfigurationModel> _bearerTokens;
        private readonly IUserContextService _userContextService;
        private readonly ITokenCacheService _tokenCacheService;

        public AuthenticationService(IEncryptionService encryptionService, IUserRepository userRepository, IUserLoginRepository userLoginRepository,
            IGroupRepository groupRepository, IRefreshTokenRepository refreshTokenRepository, IDateTimeService dateTimeService,
            IOptionsMonitor<BearerTokensConfigurationModel> bearerTokens,
            IUserContextService userContextService, ITokenCacheService tokenCacheService)
        {
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _groupRepository = groupRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _dateTimeService = dateTimeService;
            _bearerTokens = bearerTokens;
            _userContextService = userContextService;
            _tokenCacheService = tokenCacheService;
        }

        public async Task<AuthenticateDto> AuthenticateAsync(AuthenticateQuery request, CancellationToken cancellationToken)
        {
            string encPass = _encryptionService.HashPassword(request.Password);
            var user = await _userRepository.GetUserWithRolesAsync(request.UserName, encPass, cancellationToken);
            if (user == null)
                throw new NotFoundException($"No Accounts Registered", request.UserName);

            var token = GenerateJWToken(user, _bearerTokens);
            var refreshToken = GenerateRefreshToken(user, _dateTimeService, _bearerTokens);

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await AddToUserLogin(user, cancellationToken);

            await _tokenCacheService.AddOrUpdateAsync(user.Id, token);

            var result = new AuthenticateDto { Token = token, RefreshToken = refreshToken.Token };
            return result;
        }

        public async Task ChangePasswordAsync(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var encPass = _encryptionService.HashPassword(request.CurrentPassword);
            var user = await _userRepository.GetUserByPasswordAsync(request.UserId, encPass, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(user), request.UserId);

            user.Password = _encryptionService.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task UpdateUserGroupAsync(UpdateUserGroupCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWithRolesAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(user), request.UserId);

            var (deleteGroups, addGroupIds) = ConsistencyUserGroup(user.Groups, request.GroupIds);

            if (addGroupIds.Count > 0)
            {
                var addGroups = await _groupRepository.GetByIdsAsync(addGroupIds, cancellationToken);
                if (addGroupIds.Count != addGroups.Count)
                    throw new BadRequestException("Some GroupIds are Invalid.");
                user.Groups.AddRange(addGroups);
            }

            if (deleteGroups.Count > 0)
                user.Groups.RemoveRange(deleteGroups);

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task LogoutAsync(LogoutUserQuery request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.Identity.GetUserId();

            if (await _userRepository.GetAsync(request.UserId, cancellationToken) == null)
                throw new BadRequestException("Your token is invalid");

            var lastRefreshToken = await _refreshTokenRepository.GetLatestOneAsync(userId, cancellationToken);
            if (lastRefreshToken is null)
                throw new BadRequestException("First login to system.");

            lastRefreshToken.ExpirationDate = _dateTimeService.Now;
            await _refreshTokenRepository.UpdateAsync(lastRefreshToken, cancellationToken);

            await _tokenCacheService.RemoveAsync(userId);
        }

        public async Task<AuthenticateDto> RefreshTokenAsync(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.Identity.GetUserId();

            var user = await _userRepository.GetWithRoleAndRefreshTokensAsync(userId, cancellationToken);
            if (user is null)
                throw new BadRequestException("Your token is invalid");

            var lastRefreshToken = user.RefreshTokens.FirstOrDefault();
            if (lastRefreshToken == null)
                throw new BadRequestException("First login to system.");

            if (lastRefreshToken != null && lastRefreshToken.Token == request.RefreshToken && lastRefreshToken.ExpirationDate >= _dateTimeService.Now)
            {
                var newAccessToken = GenerateJWToken(user, _bearerTokens);

                var refreshToken = GenerateRefreshToken(user, _dateTimeService, _bearerTokens);
                await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
                string newRefreshToken = refreshToken.Token;

                await _tokenCacheService.AddOrUpdateAsync(userId, newAccessToken);
                return new AuthenticateDto { Token = newAccessToken, RefreshToken = newRefreshToken };
            }
            else
                throw new BadRequestException("Invalid Token.");
        }

        #region Privates
        private async Task AddToUserLogin(User user, CancellationToken cancellationToken)
        {
            var requestIp = _userContextService.CurrenConnection.RemoteIpAddress?.ToString();
            string userAgentInfo = _userContextService.UserAgent;
            var userLogin = new UserLogin { UserId = user.Id, IpAddress = requestIp, ExtraInfo = userAgentInfo, CreatedDate = _dateTimeService.Now };
            await _userLoginRepository.CreateAsync(userLogin, cancellationToken);
        }

        private string GenerateJWToken(User user, IOptionsMonitor<BearerTokensConfigurationModel> _bearerTokens)
        {
            List<Claim> claims = AddClaims(user, _bearerTokens);

            AddRoles(user, claims);

            AddRolesAsPermission(user, claims);

            JwtSecurityToken token = CreateToken(_bearerTokens, claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static void AddRolesAsPermission(User user, List<Claim> claims)
        {
            var groupsAsRoles = user.Groups.Where(g => g.IsPermissionBase == true).ToList();
            var rolesAsPermission = user.Groups.Where(g => g.IsPermissionBase == true).SelectMany(g => g.Roles).Where(r => r.Discriminator != "Menu").Distinct().ToList();

            foreach (Group group in groupsAsRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, group.Caption));
            }

            foreach (Role role in rolesAsPermission)
            {
                claims.Add(new Claim("Permission", role.Title));
            }
        }

        private static void AddRoles(User user, List<Claim> claims)
        {
            var roles = user.Groups.Where(g => g.IsPermissionBase == false).SelectMany(g => g.Roles).Where(r => r.Discriminator != "Menu").Distinct().ToList();
            foreach (Role role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role.Title));
        }

        private List<Claim> AddClaims(User user, IOptionsMonitor<BearerTokensConfigurationModel> _bearerTokens)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _bearerTokens.CurrentValue.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Surname, user.LastName??""),
                new Claim(ClaimTypes.GivenName, user.FirstName??""),
            };
        }

        private static JwtSecurityToken CreateToken(IOptionsMonitor<BearerTokensConfigurationModel> _bearerTokens, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_bearerTokens.CurrentValue.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(issuer: _bearerTokens.CurrentValue.Issuer, claims: claims, notBefore: now, expires: now.AddMinutes(_bearerTokens.CurrentValue.AccessTokenExpirationMinutes), signingCredentials: creds);
            return token;
        }

        private static RefreshToken GenerateRefreshToken(User user, IDateTimeService dateTimeService, IOptionsMonitor<BearerTokensConfigurationModel> _bearerTokens)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                User = user,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedDate = dateTimeService.Now,
                ExpirationDate = dateTimeService.Now.AddMinutes(_bearerTokens.CurrentValue.RefreshTokenExpirationMinutes)
            };

            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _bearerTokens.CurrentValue.Issuer,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_bearerTokens.CurrentValue.Key)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.FromMinutes(_bearerTokens.CurrentValue.ClockSkew)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;

        }

        private (List<Group> deleteGroups, List<int> addGroupIds) ConsistencyUserGroup(ICollection<Group> groups, List<int> requestGroupIds)
        {
            var deleteGroups = groups.Where(gg => requestGroupIds.All(g => g != gg.Id)).ToList();
            var addGroupIds = requestGroupIds.Where(gg => groups.All(g => g.Id != gg)).ToList();

            return (deleteGroups, addGroupIds);
        }

        #endregion
    }
}
