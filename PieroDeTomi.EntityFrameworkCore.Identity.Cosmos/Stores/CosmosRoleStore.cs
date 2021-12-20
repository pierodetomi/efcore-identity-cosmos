using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Stores
{
    internal class CosmosRoleStore<TRoleEntity> : IRoleStore<TRoleEntity> where TRoleEntity : IdentityRole, new()
    {
        private readonly IRepository _repo;

        public CosmosRoleStore(IRepository repo)
        {
            _repo = repo;
        }

        public Task<IdentityResult> CreateAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<TRoleEntity> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleId) || string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentNullException(nameof(roleId));

            var role = await _repo.Table<TRoleEntity>()
                .SingleOrDefaultAsync(_ => _.Id == roleId, cancellationToken: cancellationToken);

            return role;
        }

        public async Task<TRoleEntity> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedRoleName) || string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            var role = await _repo.Table<TRoleEntity>()
                .SingleOrDefaultAsync(_ => _.NormalizedName == normalizedRoleName, cancellationToken: cancellationToken);

            return role;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(TRoleEntity role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(TRoleEntity role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}