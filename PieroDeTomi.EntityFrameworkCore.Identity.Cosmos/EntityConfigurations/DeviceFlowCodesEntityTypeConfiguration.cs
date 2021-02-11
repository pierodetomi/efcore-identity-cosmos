using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.EntityConfigurations
{
    public class DeviceFlowCodesEntityTypeConfiguration : IEntityTypeConfiguration<DeviceFlowCodes>
    {
        private readonly string _tableName;

        public DeviceFlowCodesEntityTypeConfiguration(string tableName = "Identity_DeviceFlowCodes")
        {
            _tableName = tableName;
        }

        public void Configure(EntityTypeBuilder<DeviceFlowCodes> builder)
        {
            builder
                .UseETagConcurrency()
                .HasPartitionKey(_ => _.SessionId);

            builder.ToContainer(_tableName);
        }
    }
}