using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Stores
{
    internal class CosmosUserStore<TUserEntity> :
        IUserStore<TUserEntity>,
        IUserRoleStore<TUserEntity>,
        IUserEmailStore<TUserEntity>,
        IUserPasswordStore<TUserEntity>,
        IUserPhoneNumberStore<TUserEntity>,
        IUserLoginStore<TUserEntity> where TUserEntity : IdentityUser, new()
    {
        private readonly IRepository _repo;

        public CosmosUserStore(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<IdentityResult> CreateAsync(TUserEntity user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                _repo.Add(user);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUserEntity user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                _repo.Delete(user);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<TUserEntity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (normalizedEmail == null)
                throw new ArgumentNullException(nameof(normalizedEmail));

            var user = await _repo.Table<TUserEntity>()
                .SingleOrDefaultAsync(_ => _.NormalizedEmail == normalizedEmail, cancellationToken: cancellationToken);

            return user;
        }

        public async Task<TUserEntity> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = await _repo.Table<TUserEntity>()
                .WithPartitionKey(userId)
                .SingleOrDefaultAsync(cancellationToken);

            return user;
        }

        public async Task<TUserEntity> FindByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            return await _repo.Table<TUserEntity>().SingleOrDefaultAsync(_ => _.NormalizedUserName == userName);
        }

        public Task<string> GetEmailAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.Email, cancellationToken));
        }

        public Task<bool> GetEmailConfirmedAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.EmailConfirmed, cancellationToken));
        }

        public Task<string> GetNormalizedEmailAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.NormalizedEmail, cancellationToken));
        }

        public Task<string> GetNormalizedUserNameAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.NormalizedUserName, cancellationToken));
        }

        public Task<string> GetPasswordHashAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.PasswordHash, cancellationToken));
        }

        public Task<string> GetPhoneNumberAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.PhoneNumber, cancellationToken));
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.PhoneNumberConfirmed, cancellationToken));
        }

        public Task<string> GetUserIdAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                 GetUserProperty(user, user => user.Id.ToString(), cancellationToken));
        }

        public Task<string> GetUserNameAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => user.UserName, cancellationToken));
        }

        public Task<bool> HasPasswordAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                GetUserProperty(user, user => !string.IsNullOrEmpty(user.PasswordHash), cancellationToken));
        }

        public Task SetEmailAsync(TUserEntity user, string email, CancellationToken cancellationToken)
        {
            SetUserProperty(user, email, (u, m) => u.Email = email, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(TUserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            SetUserProperty(user, confirmed, (u, m) => u.EmailConfirmed = confirmed, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(TUserEntity user, string normalizedEmail, CancellationToken cancellationToken)
        {
            SetUserProperty(user, normalizedEmail, (u, m) => u.NormalizedEmail = normalizedEmail, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TUserEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            SetUserProperty(user, normalizedName, (u, m) => u.NormalizedUserName = normalizedName, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUserEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            SetUserProperty(user, passwordHash, (u, m) => u.PasswordHash = passwordHash, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUserEntity user, string phoneNumber, CancellationToken cancellationToken)
        {
            SetUserProperty(user, phoneNumber, (u, v) => user.PhoneNumber = v, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(TUserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            SetUserProperty(user, confirmed, (u, v) => user.PhoneNumberConfirmed = v, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUserEntity user, string userName, CancellationToken cancellationToken)
        {
            SetUserProperty(user, userName, (u, m) => u.UserName = userName, cancellationToken);
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            try
            {
                _repo.Update(user);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        private T GetUserProperty<T>(TUserEntity user, Func<TUserEntity, T> accessor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return accessor(user);
        }

        private void SetUserProperty<T>(TUserEntity user, T value, Action<TUserEntity, T> setter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (value == null) throw new ArgumentNullException(nameof(value));

            setter(user, value);
        }

        public async Task AddLoginAsync(TUserEntity user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (login == null) throw new ArgumentNullException(nameof(login));

            try
            {
                IdentityUserLogin<string> loginEntity = new IdentityUserLogin<string>
                {
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName
                };

                _repo.Add(loginEntity);
                await _repo.SaveChangesAsync();
            }
            catch { }
        }

        public async Task RemoveLoginAsync(TUserEntity user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (loginProvider == null) throw new ArgumentNullException(nameof(loginProvider));
            if (providerKey == null) throw new ArgumentNullException(nameof(providerKey));

            try
            {
                var login = await _repo.Table<IdentityUserLogin<string>>()
                    .SingleOrDefaultAsync(l =>
                        l.UserId == user.Id &&
                        l.LoginProvider == loginProvider &&
                        l.ProviderKey == providerKey
                    );

                if (login != null)
                {
                    _repo.Delete(login);
                    await _repo.SaveChangesAsync();
                }
            }
            catch { }
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserLoginInfo> res = _repo
                .Table<IdentityUserLogin<string>>()
                .Where(l => l.UserId == user.Id)
                .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, user.UserName)
                {
                    ProviderDisplayName = l.ProviderDisplayName
                })
                .ToList();

            return Task.FromResult(res);
        }

        public async Task<TUserEntity> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (loginProvider == null) throw new ArgumentNullException(nameof(loginProvider));
            if (providerKey == null) throw new ArgumentNullException(nameof(providerKey));

            var userId = (
                await _repo.Table<IdentityUserLogin<string>>().SingleOrDefaultAsync(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey)
            )?.UserId;

            return string.IsNullOrEmpty(userId)
                ? default(TUserEntity)
                : await FindByIdAsync(userId, cancellationToken);
        }

        public async Task AddToRoleAsync(TUserEntity user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            var role = await _repo.Table<IdentityRole>()
                .SingleOrDefaultAsync(_ => _.NormalizedName == roleName, cancellationToken: cancellationToken);

            if (role == null) throw new InvalidOperationException("Role not found.");

            try
            {
                IdentityUserRole<string> userRole = new IdentityUserRole<string>
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };

                _repo.Add(userRole);
                await _repo.SaveChangesAsync();
            }
            catch { }
        }

        public async Task RemoveFromRoleAsync(TUserEntity user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            var role = await _repo.Table<IdentityRole>()
                .SingleOrDefaultAsync(_ => _.NormalizedName == roleName, cancellationToken: cancellationToken);

            if (role != null)
            {
                var userRole = await _repo.Table<IdentityUserRole<string>>().SingleOrDefaultAsync(_ => _.RoleId == role.Id, cancellationToken);
                if (userRole != null)
                {
                    _repo.Delete(userRole);
                    await _repo.SaveChangesAsync();
                }
            }
        }

        public async Task<IList<string>> GetRolesAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var roleIds = await _repo
                .Table<IdentityUserRole<string>>()
                .Where(m => m.UserId == user.Id)
                .Select(m => m.RoleId)
                .ToListAsync(cancellationToken);

            IList<string> res = await _repo
                .Table<IdentityRole>()
                .Where(m => roleIds.Contains(m.Id))
                .Select(m => m.Name)
                .ToListAsync(cancellationToken);

            return res;
        }

        public async Task<bool> IsInRoleAsync(TUserEntity user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            var role = await _repo.Table<IdentityRole>()
                .SingleOrDefaultAsync(_ => _.NormalizedName == roleName, cancellationToken: cancellationToken);

            if (role != null)
            {
                var userRole = await _repo.Table<IdentityUserRole<string>>().SingleOrDefaultAsync(_ => _.RoleId == role.Id, cancellationToken);
                return userRole != null;
            }

            return false;
        }

        public async Task<IList<TUserEntity>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            var role = await _repo.Table<IdentityRole>()
                            .SingleOrDefaultAsync(_ => _.NormalizedName == roleName, cancellationToken: cancellationToken);

            if (role != null)
            {
                var userIds = await _repo.Table<IdentityUserRole<string>>()
                    .Where(m => m.RoleId == role.Id)
                    .Select(m => m.UserId)
                    .ToListAsync(cancellationToken);

                var users = await _repo.Table<TUserEntity>()
                    .Where(m => userIds.Contains(m.Id))
                    .ToListAsync(cancellationToken);

                return users;
            }

            return new List<TUserEntity>();
        }

        public void Dispose()
        { }
    }
}