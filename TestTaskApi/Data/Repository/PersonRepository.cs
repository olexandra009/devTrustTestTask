using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TestTaskApi.Data.Entity;
using TestTaskApi.Migrations;

namespace TestTaskApi.Data.Repository
{
    public interface IPersonRepository
    {
        Task<Person> Create(Person person);
        Task<Person> Update(Person person);
        Task<Person> Get(long id);
        Task<List<Person>> List(GetAllRequest filter);
    }

    public class PersonRepository : IPersonRepository
    {
        protected readonly PersonDbContext _dbContext;

        public PersonRepository(PersonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Person> Create(Person person)
        {
            _dbContext.Set<Person>().Add(person);
            await _dbContext.SaveChangesAsync();
            return person;
        }

        public async Task<Person> Update(Person person)
        {
            _dbContext.Attach(person);
            _dbContext.Entry(person).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return person;
        }

        public async Task<Person> Get(long id)
        {
            var keys = new object[] {id};
            return await _dbContext.Set<Person>().FindAsync(keys);
        }

      

        public async Task<List<Person>> List(GetAllRequest filter)
        {
            var query = 
                _dbContext.Set<Person>()
                .Join(_dbContext.Set<Address>(), person => person.AddressId, address => address.Id,
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

