using Microsoft.AspNetCore.Mvc;
using System.Web;
using TestApi.Model;
using TestApi.Utilities;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TriangleController : ControllerBase
    {
        [HttpGet("GetTriangleCoordinates")]
        public ActionResult<IEnumerable<Coordinate>> GetTriangleCoordinates(string row, int column)
        {
            try
            {
                return Ok(TrianglePositioning.GetTriangleCoordinatesByLocation(row, column));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTriangleLocation")]
        public ActionResult<string> GetTriangleLocation(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
        {
            try
            {
                return Ok(TrianglePositioning.GetTriangleLocationByCoordinates(coordinate1, coordinate2, coordinate3));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
    }
}