using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestTaskApi.Data.Entity;
using TestTaskApi.Data.Repository;

namespace TestTaskApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        protected readonly IPersonRepository PersonRepository;
        protected readonly IJsonParser JsonParser;

        public PersonController(IPersonRepository personRepository, IJsonParser jsonParser)
        {
            PersonRepository = personRepository;
            JsonParser = jsonParser;
        }

        [HttpPost("/save")]
        public async Task<long> Save([FromBody]string json)
        {
            Person person = JsonParser.Deserialize(json);
            long id = await PersonRepository.Save(person);
            return id;
        }

        [HttpPost("/getAll")]
        public async Task<string> GetAll([FromBody]GetAllRequest request)
        {
            var list =  await PersonRepository.List(request);
            return JsonParser.Serialize(list);
        }



    }
}
