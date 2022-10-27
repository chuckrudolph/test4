using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace testWs.Controllers
{
    public class adderUpController : ApiController
    {
        //// GET: api/addderUp
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}


        [HttpGet]
        public int GetAdd(int x, int y) {
            System.Diagnostics.Trace.WriteLine($"{DateTime.Now.ToString("mm:ss.ffff")} and x={x} and y={y}");
            return x + y;
        }

        [HttpPost]
        public int Add([FromBody]xy test) {
            System.Diagnostics.Trace.WriteLine($"{DateTime.Now.ToString("mm:ss.ffff")} and x={test.x} and y={test.y}");
            return test.x + test.y;
        }

        [HttpPost]
        public int Secure([FromBody] xy test) {
            System.Diagnostics.Trace.WriteLine($"{DateTime.Now.ToString("mm:ss.ffff")} and x={test.x} and y={test.y}");
            return test.x + test.y;
        }

        public class xy
        {
            public int x{ get; set; }
            public int y{ get; set; }
        }
    }
}
