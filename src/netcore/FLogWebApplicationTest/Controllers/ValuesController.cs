﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLog;
using FLog.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FLogWebApplicationTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger) {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(){
            _logger.LogInformation("Test Get");
            _logger.LogDebug("Debug Test");
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