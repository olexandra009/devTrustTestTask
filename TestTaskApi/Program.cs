using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TestTaskApi.Data;
using TestTaskApi.Data.Entity;
using TestTaskApi.Data.Repository;

namespace TestTaskApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var host =  CreateHostBuilder(args).Build();
           using (var scope = host.Services.CreateScope())
           {
               var services = scope.ServiceProvider;
               var context = services.GetRequiredService<PersonDbContext>();
               var addressRepository = new AddressRepository(context);
               var repository = new PersonRepository(context, addressRepository);

               //var personIds = new List<long> {5};
               //repository.DeleteRange(ids: personIds).Wait();

               //var addressIds = new List<long> { 2, 3 };
               //addressRepository.DeleteRange(ids: addressIds).Wait();

               Address ad = new Address() {Id=1, AddressLine = "Chornobylska Street 17", City = "Kyiv"};
               Person pe = new Person() {Id=1, AddressId = 1, Address = ad, FirstName = "Oleksandra", LastName = "Chumachenko"};

               var parser = new SimpleJsonParser();


               var serialization = parser.Serialize(pe);

               Console.WriteLine(serialization);

               var person = parser.Deserialize(serialization);

               var personTest = parser.Deserialize("{firstName: ‘Ivan’, lastName: ‘Petrov’, address: {city: ‘Kiev’, addressLine: prospect “Peremogy” 28/7,}");

                // var addressList = addressRepository.List().Result;
                // foreach (var address in addressList)
                // {
                //     Console.WriteLine(address.Id+": "+address.City+" "+address.AddressLine);
                // }
                // Console.WriteLine();

                // var rr = repository.List(null).Result;
                // foreach (var person in rr)
                // {
                //     Console.WriteLine(person.Id + ": " + person.FirstName+" "+person.LastName+" "+person.AddressId+" "+person.Address.City+" "+person.Address.AddressLine);
                // }
                // Console.WriteLine();
                // Console.WriteLine();
                // //Person per = repository.Get(4).Result;
                // //per.Address = ad;
                // //per.Address.AddressLine = "Chornobylska Street 18";

                //// var id = repository.Save(per).Result;

                // //Console.WriteLine(id);

                // addressList = addressRepository.List().Result;
                // foreach (var address in addressList)
                // {
                //     Console.WriteLine(address.Id+": "+address.City+" "+address.AddressLine);
                // }
                // Console.WriteLine();

                // rr = repository.List(null).Result;
                // foreach (var person in rr)
                // {
                //     Console.WriteLine(person.Id + ": " + person.FirstName+" "+person.LastName+" "+person.AddressId+" "+person.Address.City+" "+person.Address.AddressLine);
                // }
                // Console.WriteLine();

            }

           host.Run();


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
