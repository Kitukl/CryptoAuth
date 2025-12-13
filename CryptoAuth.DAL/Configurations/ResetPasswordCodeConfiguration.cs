using CryptoAuth.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAuth.DAL.Configurations;

public class ResetPasswordCodeConfiguration : IEntityTypeConfiguration<ResetPasswordCode>
{
    public void Configure(EntityTypeBuilder<ResetPasswordCode> builder)
    {
        builder.HasKey(t => t.Code);
    }
}