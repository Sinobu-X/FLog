using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLog;
using FLog.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FLogWebApplicationTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly Logger _logger = LogManager.GetLogger(typeof(ValuesController));

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(){
            _logger.Info("Test Get");
            return new string[]{"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id){
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value){
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value){
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id){
        }
    }
}