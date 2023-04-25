using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using scorpio.Data;
using scorpio.Models;

namespace scorpio.Controllers
{
    [ApiController]
    [EnableCors("CorsApi")]
    public class ScorpioController : Controller
    {
        private readonly ScorpioDbContext _dbcontext;

        public ScorpioController(ScorpioDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpPost("GetId")]
        public Response<string> MatchWithProvision([FromBody] Dto dto)
        {
            var response = new Response<string>();
            var PrItemID = _dbcontext.provision.Where(p => p.name.ToLower() == dto.PrItemName.ToLower()).Select(p => p.id).FirstOrDefault();
            var vessel_id = _dbcontext.VesselAddedItem.Where(p => p.name.ToLower() == dto.vesselItemName.ToLower()).Select(p => p.id).FirstOrDefault();
            response.PrItemID = PrItemID;
            response.VesselItemId = vessel_id;
            return response;
        }
        [HttpGet("Names")]
        public List<string> ReturnProvision()
        {
            var names = _dbcontext.provision.Where(x => x.from_shore == "True").Select(p => p.name).ToList();
            return names;
        }
        [HttpPost("Querywrite")]
        public IActionResult Post(QueryData queryData)
        {
            var sqlQueries = $"{queryData.QueryOne}\n{queryData.QueryTwo}\n{queryData.QueryThree}\n";
            var folderPath = queryData.vesselnumber;
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, "Scorpio-update-Queries.sql");
            using (var streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.Write(sqlQueries);
            }
            var sqlQueries1 = $"{queryData.QueryFour}\n";
            var filePath1 = Path.Combine(folderPath, "Victualing-update-Queries.sql");
            using (var streamWriter = new StreamWriter(filePath1, true))
            {
                streamWriter.Write(sqlQueries1);
            }
            return Ok(new { Message = "Queries saved to file!" });
        }

    }
}