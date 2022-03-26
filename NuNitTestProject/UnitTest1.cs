using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TestApi.Controllers;
using System.Linq;
using TestApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace NuNitTestProject
{
    public class Tests
    {
        private readonly string[] _rows = new string[] {"A", "B", "C", "D", "E", "F"};
        private Dictionary<string, List<Coordinate>>  _triangleCoordinates = new Dictionary<string, List<Coordinate>>();

        private const int WIDTH = 5;
        private const int HEIGHT = 10;
        private const string INVALID_COORDINATE_ERR_MSG = "One or more coordinates is invalid.";
        private const string EXCEPTION_NOT_NULL_MSG = "Exception should not be null";

        /// <summary>
        /// Build the grid of expected results for the triangle locations with corresponding coordinates
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            ManuallyBuildExpectedResults();
        }
        
#region return location tests

        /// <summary>
        /// Test passing in all the triangle coordinates to get the triangle location
        /// loop thru triangle coordinaetes
        /// - get the each set of coordinates
        /// - get the expected location
        /// - call api
        /// - assert locations equal
        /// </summary>
        [Test]
        public void ReturnLocationTest()
        {
            var triangleController = new TriangleController();
            foreach (var triangleLocation in _triangleCoordinates)
            {
                string column = triangleLocation.Key.Substring(0, 1);
                int row = int.Parse(triangleLocation.Key.Substring(1, triangleLocation.Key.Length-1));

                string expectedLocation = column + row;
                Coordinate coordinate1 = triangleLocation.Value[0];
                Coordinate coordinate2 = triangleLocation.Value[1];
                Coordinate coordinate3 = triangleLocation.Value[2];

                var response = triangleController.GetTriangleLocation(coordinate1, coordinate2, coordinate3);
                Assert.IsNotNull(response, "Response should not be null");
                Assert.IsInstanceOf<OkObjectResult>(response.Result, "Rsponse should be Ok");

                var location = ((OkObjectResult)response.Result).Value;
                Assert.AreEqual(expectedLocation, location, "locations for triangle should be equal");
            }
        }

        /// <summary>
        /// Test when all coordinates are the same returns a BadResponse
        /// </summary>
        [Test]
        public void AllCoordinatesTheSame()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleLocation(new Coordinate(), new Coordinate(), new Coordinate());
            ValidateBadResponsetForInvalidCoordinates(response);
        }

        /// <summary>
        /// Test an invalid row coordinate returns a BadResponse
        /// </summary>
        [Test]
        public void InvalidRowCoordinate()
        {
            var triangleController = new TriangleController();

            Coordinate coordinate = new Coordinate { column = 10, row = 1 };
            var response = triangleController.GetTriangleLocation(coordinate, new Coordinate(), new Coordinate());
            ValidateBadResponsetForInvalidCoordinates(response);
        }

        /// <summary>
        /// Test an invalid column coordinate returns a BadResponse
        /// </summary>
        [Test]
        public void InvalidColumnCoordinate()
        {
            var triangleController = new TriangleController();

            Coordinate coordinate = new Coordinate { column = 1, row = 10 };
            var response = triangleController.GetTriangleLocation(coordinate, new Coordinate(), new Coordinate());
            ValidateBadResponsetForInvalidCoordinates(response);
        }

        /// <summary>
        /// Test an invalid row/column combination coordinate returns a BadResponse
        /// </summary>
        [Test]
        public void InvalidCoordinateCombination()
        {
            var triangleController = new TriangleController();

            Coordinate coordinate1 = new Coordinate { column = 0, row = 0 };
            Coordinate coordinate2 = new Coordinate { column = 50, row = 50 };
            var response = triangleController.GetTriangleLocation(coordinate1, coordinate2, new Coordinate());
            ValidateBadResponsetForInvalidCoordinates(response);
        }

        /// <summary>
        /// Commmon method to validate the resonse is bad and the expected error message is returned
        /// </summary>
        /// <param name="actionResult"></param>
        private void ValidateBadResponsetForInvalidCoordinates(ActionResult<string> actionResult)
        {
            Assert.IsNotNull(actionResult, "Response should not be null");
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult.Result, "Response should be Bad");
            var errorMessage = ((BadRequestObjectResult)actionResult.Result).Value;
            Assert.AreEqual(errorMessage, INVALID_COORDINATE_ERR_MSG, "wrong error message returned.");
        }
        #endregion return location tests

        #region return coordinates tests

        /// <summary>
        /// Test passing in all the triangle locations to get the triangle coordinates
        /// loop thru triangle locations
        /// - get the expected coordinates
        /// - call api
        /// - assert coordinates equal
        /// </summary>
        [Test]
        public void ReturnCoordinatesTest()
        {
            var triangleController = new TriangleController();

            foreach (var triangleLocation in _triangleCoordinates)
            {
                string column = triangleLocation.Key.Substring(0, 1);
                int row = int.Parse(triangleLocation.Key.Substring(1, 1));

                var response = triangleController.GetTriangleCoordinates(column, row);
                Assert.IsNotNull(response, "Response should not be null");
                Assert.IsInstanceOf<OkObjectResult>(response.Result, "Rsponse should be Ok");

                var coordinates = ((OkObjectResult)response.Result).Value as IEnumerable<Coordinate>;
                Assert.IsNotNull(coordinates, "Coordinates should not null");

                foreach (var coordinate in coordinates)
                {
                    Assert.IsNotNull(coordinate, $"coordinates for {column + row } should not be null");
                    var expectedCoordinates = _triangleCoordinates[column + row];
                    foreach (var expectedCoordinate in expectedCoordinates)
                    {
                        Assert.IsTrue(expectedCoordinates.Any(e => e.row == coordinate.row && e.column == coordinate.column), $"Should have found coordinate for {column + row }");
                    }
                }
            }
        }

        /// <summary>
        /// Test the an invalid location row returns a BadResponse
        /// </summary>
        [Test]
        public void InvalidRowLocation()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleCoordinates("Z", 1);
            ValidateBadResponseForInvalidlocations(response, "Triangle row location is not valid.");
        }

        /// <summary>
        /// Test the a location column < 0 returns a BadResponse
        /// </summary>
        [Test]
        public void InvalidColumnLocationTooSmall()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleCoordinates("A", -1);
            ValidateBadResponseForInvalidlocations(response, "Triangle column location is not valid.");
        }

        /// <summary>
        /// Test the a location column > 12 returns a BadResponse
        /// </summary>
        [Test]
        public void InvalidColumnLocationTooBig()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleCoordinates("A", 13);
            ValidateBadResponseForInvalidlocations(response, "Triangle column location is not valid.");
        }

        /// <summary>
        /// Common method to validate the resonse is bad and the expected error message is returned
        /// </summary>
        /// <param name="actionResult"></param>
        /// <param name="errorMessageExpected"></param>
        private void ValidateBadResponseForInvalidlocations(ActionResult<IEnumerable<Coordinate>> actionResult, string errorMessageExpected)
        {
            Assert.IsNotNull(actionResult, "Response should not be null");
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult.Result, "Response should be Bad");
            var errorMessageReturned = ((BadRequestObjectResult)actionResult.Result).Value;
            Assert.AreEqual(errorMessageExpected, errorMessageReturned, "wrong error message returned.");
        }

        #endregion return coordinates tests

        /// <summary>
        /// Manualy build the expected Location and corresponding coordinates
        /// </summary>
        private void ManuallyBuildExpectedResults()
        {
            AddOddColumnCoordinate("A1", 0, 0);
            AddOddColumnCoordinate("A3", 0, 10);
            AddOddColumnCoordinate("A5", 0, 20);
            AddOddColumnCoordinate("A7", 0, 30);
            AddOddColumnCoordinate("A9", 0, 40);
            AddOddColumnCoordinate("A11", 0, 50);

            AddEvenColumnCoordinate("A2", 0, 0);
            AddEvenColumnCoordinate("A4", 0, 10);
            AddEvenColumnCoordinate("A6", 0, 20);
            AddEvenColumnCoordinate("A8", 0, 30);
            AddEvenColumnCoordinate("A10", 0, 40);
            AddEvenColumnCoordinate("A12", 0, 50);

            AddOddColumnCoordinate("B1", 10, 0);
            AddOddColumnCoordinate("B3", 10, 10);
            AddOddColumnCoordinate("B5", 10, 20);
            AddOddColumnCoordinate("B7", 10, 30);
            AddOddColumnCoordinate("B9", 10, 40);
            AddOddColumnCoordinate("B11", 10, 50);

            AddEvenColumnCoordinate("B2", 10, 0);
            AddEvenColumnCoordinate("B4", 10, 10);
            AddEvenColumnCoordinate("B6", 10, 20);
            AddEvenColumnCoordinate("B8", 10, 30);
            AddEvenColumnCoordinate("B10", 10, 40);
            AddEvenColumnCoordinate("B12", 10, 50);

            AddOddColumnCoordinate("C1", 20, 0);
            AddOddColumnCoordinate("C3", 20, 10);
            AddOddColumnCoordinate("C5", 20, 20);
            AddOddColumnCoordinate("C7", 20, 30);
            AddOddColumnCoordinate("C9", 20, 40);
            AddOddColumnCoordinate("C11", 20, 50);

            AddEvenColumnCoordinate("C2", 20, 0);
            AddEvenColumnCoordinate("C4", 20, 10);
            AddEvenColumnCoordinate("C6", 20, 20);
            AddEvenColumnCoordinate("C8", 20, 30);
            AddEvenColumnCoordinate("C10", 20, 40);
            AddEvenColumnCoordinate("C12", 20, 50);

            AddOddColumnCoordinate("D1", 30, 0);
            AddOddColumnCoordinate("D3", 30, 10);
            AddOddColumnCoordinate("D5", 30, 20);
            AddOddColumnCoordinate("D7", 30, 30);
            AddOddColumnCoordinate("D9", 30, 40);
            AddOddColumnCoordinate("D11", 30, 50);

            AddEvenColumnCoordinate("D2", 30, 0);
            AddEvenColumnCoordinate("D4", 30, 10);
            AddEvenColumnCoordinate("D6", 30, 20);
            AddEvenColumnCoordinate("D8", 30, 30);
            AddEvenColumnCoordinate("D10", 30, 40);
            AddEvenColumnCoordinate("D12", 30, 50);

            AddOddColumnCoordinate("E1", 40, 0);
            AddOddColumnCoordinate("E3", 40, 10);
            AddOddColumnCoordinate("E5", 40, 20);
            AddOddColumnCoordinate("E7", 40, 30);
            AddOddColumnCoordinate("E9", 40, 40);
            AddOddColumnCoordinate("E11", 40, 50);

            AddEvenColumnCoordinate("E2", 40, 0);
            AddEvenColumnCoordinate("E4", 40, 10);
            AddEvenColumnCoordinate("E6", 40, 20);
            AddEvenColumnCoordinate("E8", 40, 30);
            AddEvenColumnCoordinate("E10", 40, 40);
            AddEvenColumnCoordinate("E12", 40, 50);

            AddOddColumnCoordinate("F1", 50, 0);
            AddOddColumnCoordinate("F3", 50, 10);
            AddOddColumnCoordinate("F5", 50, 20);
            AddOddColumnCoordinate("F7", 50, 30);
            AddOddColumnCoordinate("F9", 50, 40);
            AddOddColumnCoordinate("F11", 50, 50);

            AddEvenColumnCoordinate("F2", 50, 0);
            AddEvenColumnCoordinate("F4", 50, 10);
            AddEvenColumnCoordinate("F6", 50, 20);
            AddEvenColumnCoordinate("F8", 50, 30);
            AddEvenColumnCoordinate("F10", 50, 40);
            AddEvenColumnCoordinate("F12", 50, 50);

        }

        /// <summary>
        /// Add the coordinates for odd columns
        /// </summary>
        /// <param name="location"></param>
        /// <param name="topRowLocation"></param>
        /// <param name="leftColumnlocation"></param>
        private void AddOddColumnCoordinate(string location, int topRowLocation, int leftColumnlocation)
        {
            Coordinate coordinate1 = new Coordinate { row = topRowLocation, column = leftColumnlocation };
            Coordinate coordinate2 = new Coordinate { row = topRowLocation + HEIGHT, column = leftColumnlocation };
            Coordinate coordinate3 = new Coordinate { row = topRowLocation + HEIGHT, column = leftColumnlocation + HEIGHT };
            _triangleCoordinates.Add(location, new List<Coordinate> { coordinate1, coordinate2, coordinate3 });
        }

        /// <summary>
        /// Add the coordinates for even columns
        /// </summary>
        /// <param name="location"></param>
        /// <param name="topRowLocation"></param>
        /// <param name="leftColumnlocation"></param>
        private void AddEvenColumnCoordinate(string location, int topRowLocation, int leftColumnlocation)
        {
            Coordinate coordinate1 = new Coordinate { row = topRowLocation, column = leftColumnlocation };
            Coordinate coordinate2 = new Coordinate { row = topRowLocation, column = leftColumnlocation + HEIGHT};
            Coordinate coordinate3 = new Coordinate { row = topRowLocation + HEIGHT, column = leftColumnlocation + HEIGHT };
            _triangleCoordinates.Add(location, new List<Coordinate> { coordinate1, coordinate2, coordinate3 });
        }
    }
}