using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.EntityConfigurations;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyIdentityMappings<TUserEntity>(this ModelBuilder builder) where TUserEntity : IdentityUser
        {
            builder.ApplyConfiguration(new UserEntityTypeConfiguration<TUserEntity> { });
            builder.ApplyConfiguration(new UserRoleEntityTypeConfiguration { });
            builder.ApplyConfiguration(new RoleEntityTypeConfiguration { });
            builder.ApplyConfiguration(new RoleClaimEntityTypeConfiguration { });
            builder.ApplyConfiguration(new UserClaimEntityTypeConfiguration { });
            builder.ApplyConfiguration(new UserLoginEntityTypeConfiguration { });
            builder.ApplyConfiguration(new UserTokensEntityTypeConfiguration { });
            builder.ApplyConfiguration(new DeviceFlowCodesEntityTypeConfiguration { });
            builder.ApplyConfiguration(new PersistedGrantEntityTypeConfiguration { });

            return builder;
        }
    }
}