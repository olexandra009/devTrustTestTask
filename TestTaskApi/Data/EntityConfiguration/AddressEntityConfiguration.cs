using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestTaskApi.Data.Entity;

namespace TestTaskApi.Data.EntityConfiguration
{
    public class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired();
            builder.Property(a => a.AddressLine).IsRequired();
            builder.Property(a => a.City).IsRequired();

            builder.HasMany<Person>().WithOne(p => p.Address).HasForeignKey(p => p.AddressId);
        }
    }
}
