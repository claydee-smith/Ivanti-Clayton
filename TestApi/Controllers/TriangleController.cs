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
        [HttpGet("GetTriangleCoordinatesByLocation")]
        public ActionResult<IEnumerable<Coordinate>> GetTriangleCoordinatesByLocation(string row, int column)
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

        [HttpGet("GetTriangleLocationByCoordinates")]
        public ActionResult<string> GetTriangleLocationByCoordinates(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
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