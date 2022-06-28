using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
               var repository = new PersonRepository(context);

               Address ad = new Address() {AddressLine = "Chornobylska Street 17", City = "Kyiv"};
               Person pe = new Person() {Address = ad, FirstName = "Aleksandra", LastName = "Chumachenko"};

               var r = repository.Get(2).Result;
               Console.WriteLine(r.Id+" "+r.FirstName+" "+r.AddressId+" "+r.LastName+" "+r.Address);
               var rr = repository.List(null).Result;

               var er = repository.List(new GetAllRequest(){FirstName = "Aleksandra"}).Result;
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
