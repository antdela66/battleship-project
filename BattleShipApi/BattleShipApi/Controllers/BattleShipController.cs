using Microsoft.AspNetCore.Mvc;


namespace BattleShipApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BattleShipController : ControllerBase
    {
        // GET: api/<BattleShipController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BattleShipController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BattleShipController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BattleShipController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BattleShipController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
