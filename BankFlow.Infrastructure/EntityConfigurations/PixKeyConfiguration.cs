using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankFlow.Infrastructure.EntityConfigurations;

public class PixKeyConfiguration : IEntityTypeConfiguration<PixKey>
{
    public void Configure(EntityTypeBuilder<PixKey> builder)
    {
        builder.HasKey(e => e.Id);
    }
}
