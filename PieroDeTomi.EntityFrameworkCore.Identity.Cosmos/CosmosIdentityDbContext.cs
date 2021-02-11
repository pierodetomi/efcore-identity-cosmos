using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.EntityConfigurations;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos
{
    public class CosmosIdentityDbContext<TUserEntity> : ApiAuthorizationDbContext<TUserEntity> where TUserEntity : IdentityUser
    {
        public CosmosIdentityDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserEntityTypeConfiguration<TUserEntity> { });
            builder.ApplyConfiguration(new UserRoleEntityTypeConfiguration { });
            builder.ApplyConfiguration(new RoleEntityTypeConfiguration { });
            builder.ApplyConfiguration(new RoleClaimEntityTypeConfiguration { });
            builder.ApplyConfiguration(new UserClaimEntityTypeConfiguration { });
            builder.ApplyConfiguration(new UserLoginEntityTypeConfiguration { });
            builder.ApplyConfiguration(new UserTokensEntityTypeConfiguration { });
            builder.ApplyConfiguration(new DeviceFlowCodesEntityTypeConfiguration { });
            builder.ApplyConfiguration(new PersistedGrantEntityTypeConfiguration { });
        }
    }
}