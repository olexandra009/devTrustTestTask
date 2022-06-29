using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTaskApi.Data.Entity;

namespace TestTaskApi.Data.Repository
{
    public interface IPersonRepository
    {
        Task<long> Save(Person person);
        Task<List<Person>> List(GetAllRequest filter);
      
    }

    public class PersonRepository : IPersonRepository
    {
        protected readonly PersonDbContext DbContext;
        private readonly IAddressRepository _addressRepository;

        public PersonRepository(PersonDbContext dbContext, IAddressRepository addressRepository)
        {
            DbContext = dbContext;
            _addressRepository = addressRepository;
        }

        public async Task<long> Save(Person person)
        {
            long resultId = person.Id;
            if (resultId == 0)
            {
                var res = await Create(person);
                resultId = res.Id;
            }
            else
            {
                await Update(person);
            }
            return resultId;
        }

        private async Task<Person> Create(Person person)
        {
            var address = person.Address;
            var addressId = await _addressRepository.GetAddressId(address);
            if (addressId != -1)
            {
                person.Address = null;
                person.AddressId = addressId;
            }
            DbContext.Set<Person>().Add(person);
            await DbContext.SaveChangesAsync();
            return person;
        }

        private async Task<Person> Update(Person person)
        {
            DbContext.Attach(person);
            DbContext.Entry(person).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return person;
        }

        protected async Task<Person> Get(long id)
        {
            var keys = new object[] {id};
            return await DbContext.Set<Person>().FindAsync(keys);
        }


        protected async Task DeleteRange(List<long> ids)
        {
            var query = await DbContext.Set<Person>().Where(p => ids.Contains(p.Id)).ToListAsync();
            DbContext.Set<Person>().RemoveRange(query);
            await DbContext.SaveChangesAsync();

        }

        public async Task<List<Person>> List(GetAllRequest filter)
        {
            var query = 
                DbContext.Set<Person>()
                .Join(DbContext.Set<Address>(), person => person.AddressId, address => address.Id,
                    (person, address) => new {person, address});

            if (filter != null)
            {
                if (!String.IsNullOrEmpty(filter.LastName))
                    query = query.Where(@t => @t.person.LastName == filter.LastName);
                if (!String.IsNullOrEmpty(filter.FirstName))
                    query = query.Where(@t => @t.person.FirstName == filter.FirstName);
                if (!String.IsNullOrEmpty(filter.City))
                    query = query.Where(@t => @t.address.City == filter.City);
            }

            var result = await query.ToListAsync();

            List<Person> persons = new List<Person>();
            foreach (var pa in result)
            {
                    var temp = pa.person;
                    temp.Address = pa.address;
                    persons.Add(temp);
            }

            return persons;
        }
     
    }
}

