using Microsoft.AspNetCore.Mvc;
using TestApi.Model;
using TestApi.Utilities;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TriangleController : ControllerBase
    {
        [HttpGet("GetTriangleCoordinates")]
        public IEnumerable<Coordinate> GetTriangleCoordinates(string row, int column)
        {
            return TrianglePositioning.GetTriangleCoordinatesByLocation(row, column);
        }

        [HttpGet("GetTriangleLocation")]
        public string GetTriangleLocation(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
        {
            return TrianglePositioning.GetTriangleLocationByCoordinates(coordinate1, coordinate2, coordinate3);
        }
    }
}