using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace TestTaskApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        [HttpPost]
        public Task<long> Save(string json)
        {
            // deserialize string into object of type Person using own deserializer (see more details in Minimal requirements section)
            // DO NOT use 3rd party libraries for deserialization like Json.NET or Microsoft
            // insert or update Person entity in database
            // return entity id
            throw new NotImplementedException();
        }
        [HttpGet]
        public Task<string> GetAll(GetAllRequest request)
        {
            // get Persons entities from database
            // filter by GetAllRequest fields (null or empty fields should be ignored)
            // use your own manually written json serializer to serialize result into string
            throw new NotImplementedException();
        }



    }
}
