using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP.Portal.Users.Module.Data;

public class CartItemConfiguration : IEntityTypeConfiguration<CartMovie>
{
    public void Configure(EntityTypeBuilder<CartMovie> builder)
    {
        builder.ToTable("cart_movie","users");
        builder.Property(item => item.Id)
          .ValueGeneratedNever();
    }
}
