using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Repositories;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Stores;
using System;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IdentityBuilder AddCosmosIdentity<TDbContext, TUserEntity, TRoleEntity>(
            this IServiceCollection services,
            Action<IdentityOptions> identityOptions,
            Action<DbContextOptionsBuilder> dbContextOptions = null,
            bool addDefaultTokenProviders = false
        )
            where TDbContext : CosmosIdentityDbContext<TUserEntity>
            where TUserEntity : IdentityUser, new()
            where TRoleEntity : IdentityRole, new()
        {
            if (dbContextOptions != null)
                services.AddDbContext<TDbContext>(dbContextOptions);

            var builder = services
                .AddIdentity<TUserEntity, TRoleEntity>(identityOptions)
                .AddEntityFrameworkStores<TDbContext>();

            if (addDefaultTokenProviders)
                builder.AddDefaultTokenProviders();

            // Add custom Identity stores
            services.AddTransient<IUserStore<TUserEntity>, CosmosUserStore<TUserEntity>>();
            services.AddTransient<IRoleStore<TRoleEntity>, CosmosRoleStore<TRoleEntity>>();

            // Add repository service
            services.AddTransient<IRepository, CosmosIdentityRepository<TDbContext, TUserEntity>>();

            return builder;
        }
    }
}