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

        [HttpPost("GetTriangleLocationByCoordinates")]
        public ActionResult<string> GetTriangleLocationByCoordinates(IEnumerable<Coordinate> coordinates)
        {
            if (coordinates == null || coordinates.Count() != 3)
            {
                return BadRequest("Must provide exactly three coordinates.");
            }

            try
            {
                return Ok(TrianglePositioning.GetTriangleLocationByCoordinates(coordinates.First(), coordinates.ElementAt(1), coordinates.Last()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}