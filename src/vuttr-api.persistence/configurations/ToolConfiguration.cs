using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vuttr_api.domain.entities;

namespace vuttr_api.persistence;

public class ToolConfiguration : IEntityTypeConfiguration<Tool>
{
    public void Configure(EntityTypeBuilder<Tool> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Link).HasMaxLength(50).IsRequired();
        builder.HasMany(x => x.Tags).WithOne().HasForeignKey(x => x.ToolId);
    }
}