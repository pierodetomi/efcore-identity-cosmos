using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Stores
{
    class CosmosUserStore<TUserEntity> : IUserStore<TUserEntity>, IUserEmailStore<TUserEntity>, IUserPasswordStore<TUserEntity> where TUserEntity : IdentityUser, new()
    {
        private readonly IRepository _repo;

        public CosmosUserStore(IRepository repo)
        {
            _repo = repo;
        }

        #region createuser

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

        #endregion

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

        public void Dispose() { }

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

        public async Task<string> GetEmailAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.Email, cancellationToken);
        }

        public async Task<bool> GetEmailConfirmedAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.EmailConfirmed, cancellationToken);
        }

        public async Task<string> GetNormalizedEmailAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.NormalizedEmail, cancellationToken);
        }

        public async Task<string> GetNormalizedUserNameAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.NormalizedUserName, cancellationToken);
        }

        public async Task<string> GetPasswordHashAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.PasswordHash, cancellationToken);
        }

        public async Task<string> GetUserIdAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.Id.ToString(), cancellationToken);
        }

        public async Task<string> GetUserNameAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => user.UserName, cancellationToken);
        }

        public async Task<bool> HasPasswordAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            return GetUserProperty(user, user => !string.IsNullOrEmpty(user.PasswordHash), cancellationToken);
        }

        public async Task SetEmailAsync(TUserEntity user, string email, CancellationToken cancellationToken)
        {
            SetUserProperty(user, email, (u, m) => u.Email = email, cancellationToken);
        }

        public async Task SetEmailConfirmedAsync(TUserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            SetUserProperty(user, confirmed, (u, m) => u.EmailConfirmed = confirmed, cancellationToken);
        }

        public async Task SetNormalizedEmailAsync(TUserEntity user, string normalizedEmail, CancellationToken cancellationToken)
        {
            SetUserProperty(user, normalizedEmail, (u, m) => u.NormalizedEmail = normalizedEmail, cancellationToken);
        }

        public async Task SetNormalizedUserNameAsync(TUserEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            SetUserProperty(user, normalizedName, (u, m) => u.NormalizedUserName = normalizedName, cancellationToken);
        }

        public async Task SetPasswordHashAsync(TUserEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            SetUserProperty(user, passwordHash, (u, m) => u.PasswordHash = passwordHash, cancellationToken);
        }

        public async Task SetUserNameAsync(TUserEntity user, string userName, CancellationToken cancellationToken)
        {
            SetUserProperty(user, userName, (u, m) => u.UserName = userName, cancellationToken);
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
    }
}