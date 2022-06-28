using Microsoft.EntityFrameworkCore;
using TestTaskApi.Data.Entity;
using TestTaskApi.Data.EntityConfiguration;

namespace TestTaskApi.Data
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext()
        {
            
        }
        public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AddressEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PersonEntityConfiguration());
        }
    }
}
