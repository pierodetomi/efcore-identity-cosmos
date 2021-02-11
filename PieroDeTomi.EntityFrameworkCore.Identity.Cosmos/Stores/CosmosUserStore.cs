using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Stores
{
    class CosmosUserStore<TUserEntity> : IUserStore<TUserEntity>, IUserPasswordStore<TUserEntity> where TUserEntity : IdentityUser, new()
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

            _repo.Add(user);
            await _repo.SaveChangesAsync();

            return IdentityResult.Success;
        }

        #endregion

        public async Task<IdentityResult> DeleteAsync(TUserEntity user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _repo.Delete(user);
            await _repo.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public void Dispose() { }

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

        public Task<string> GetNormalizedUserNameAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(TUserEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(TUserEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);

        }

        public Task SetUserNameAsync(TUserEntity user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TUserEntity user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}