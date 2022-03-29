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
        /// <summary>
        /// Get the coordinates for a triangle based on a given row and column
        /// </summary>
        /// <param name="row">row location for the triangle, must be a letter 'A-F'</param>
        /// <param name="column">column location for the triangle, must be a multiple of ten number 1-60</param>
        /// <returns>The three coordinate locations of the triangle</returns>
        [HttpGet("GetTriangleCoordinatesByLocation")]
        public ActionResult<IEnumerable<Coordinate>> GetTriangleCoordinatesByLocation(string row, int column)
        {
            try
            {
                return Ok(TrianglePositioning.GetTriangleCoordinatesByLocation(row.ToUpper(), column));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the triangle based on the coordinates passed in
        /// </summary>
        /// <param name="coordinates">The set of three coordinates for the triangle.  Each set of coordinates must have a row and column value.  
        /// The row must be must be a letter 'A-F'.  The column must be a multiple of ten number 1-60</param>
        /// <returns></returns>
        [HttpPost("GetTriangleLocationByCoordinates")]
        public ActionResult<string> GetTriangleLocationByCoordinates(IEnumerable<Coordinate> coordinates)
        {
            if (coordinates == null || coordinates.Count() != 3)
            {
                return BadRequest("Must provide exactly three coordinates.");
            }

            try
            {
                return Ok(TrianglePositioning.GetTriangleLocationByCoordinates(coordinates));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}