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
        private const string INVALID_LOCATION_ERR_MSG = "Triangle row location is not valid.";
        private const string EXCEPTION_NOT_NULL_MSG = "Exception should not be null";

        /// <summary>
        /// Build the grid with the triangle  locations and corresponding coordinates
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            BuildExpectedResults();
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
        /// Test when all coordinates are the same returns an exception
        /// </summary>
        [Test]
        public void AllCoordinatesTheSame()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleLocation(new Coordinate(), new Coordinate(), new Coordinate());
            ValidateBadResponsetForInvalidCoordinates(response);
        }

        /// <summary>
        /// Test an invalid row coordinate returns an exception
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
        /// Test an invalid column coordinate returns an exception
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
        /// Test an invalid row/column combination coordinate returns an exception
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
        /// loop thru triangle coordinaetes
        /// - get the each set of coordinates
        /// - get the expected location
        /// - call api
        /// - assert locations equal
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
        /// Test the an invalid location row returns an exception
        /// </summary>
        [Test]
        public void InvalidRowLocation()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleCoordinates("Z", 1);
            ValidateBadResponseForInvalidlocations(response, "Triangle row location is not valid.");
        }

        /// <summary>
        /// Test the a location column < 0 returns an exception
        /// </summary>
        [Test]
        public void InvalidColumnLocationTooSmall()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleCoordinates("A", -1);
            ValidateBadResponseForInvalidlocations(response, "Triangle column location is not valid.");
        }

        /// <summary>
        /// Test the a location column > 12 returns an exception
        /// </summary>
        [Test]
        public void InvalidColumnLocationTooBig()
        {
            var triangleController = new TriangleController();
            var response = triangleController.GetTriangleCoordinates("A", 13);
            ValidateBadResponseForInvalidlocations(response, "Triangle column location is not valid.");
        }

        private void ValidateBadResponseForInvalidlocations(ActionResult<IEnumerable<Coordinate>> actionResult, string errorMessageExpected)
        {
            Assert.IsNotNull(actionResult, "Response should not be null");
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult.Result, "Response should be Bad");
            var errorMessageReturned = ((BadRequestObjectResult)actionResult.Result).Value;
            Assert.AreEqual(errorMessageExpected, errorMessageReturned, "wrong error message returned.");
        }

        #endregion return coordinates tests

        /// <summary>
        /// Method to assert correct exception thrown
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        private void AssertException(Exception exception, string message)
        {
            Assert.IsNotNull(exception, EXCEPTION_NOT_NULL_MSG);
            Assert.AreEqual(exception.Message, message);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private IEnumerable<CoordinateStruct> GetCoordinatePositions(string row, int column)
        {
            int topRowLocation = GetTopRowCoordinateLocation(row);
            int bottomRowLocation = GetBottomRowCoordinateLocation(topRowLocation);
            int righttColumnLocation = GetRightCoordinateLocation(column);
            int leftColumnLocation = GetLeftCoordinateLocation(righttColumnLocation);

            CoordinateStruct coordinate1 = new CoordinateStruct();
            coordinate1.row = topRowLocation;
            coordinate1.column = leftColumnLocation;

            CoordinateStruct coordinate2 = new CoordinateStruct();
            coordinate2.row = bottomRowLocation;
            coordinate2.column = righttColumnLocation;

            CoordinateStruct coordinate3 = new CoordinateStruct();
            if (IsIntegerEven(column))
            {
                coordinate3.row = topRowLocation;
                coordinate3.column = righttColumnLocation;
            }
            else
            {
                coordinate3.row = bottomRowLocation;
                coordinate3.column = leftColumnLocation;
            }
            //hyp 1 is always top and left positions
            //hyp 2 is always bottom and  right
            //coord 3 is based on the column
            //  if even then top and right positions
            //    else bottom ande left positions

            return new List<CoordinateStruct> { coordinate1, coordinate2, coordinate3 };
        }

        private string GetRow(IEnumerable<int> rowCoordinates)
        {
            var duplicateRowCoordinate = GetDuplicateCoordinate(rowCoordinates);
            var singleRowCoordinate = rowCoordinates.Where(r => r != duplicateRowCoordinate).First();

            int rowIndex;
            if (duplicateRowCoordinate < singleRowCoordinate)
            {
                rowIndex = (singleRowCoordinate / HEIGHT);
            }
            else
            {
                rowIndex = (duplicateRowCoordinate / HEIGHT);
            }

            return _rows[rowIndex - 1];
        }

        private int GetColumn(IEnumerable<int> columnCoordinate)
        {
            var duplicateRowCoordinate = GetDuplicateCoordinate(columnCoordinate);
            var singleColumnCoordinate = columnCoordinate.Where(r => r != duplicateRowCoordinate).First();

            var largestCoordinate = columnCoordinate.Max();
            var largestColumn = largestCoordinate / 5;

            if (duplicateRowCoordinate > singleColumnCoordinate)
            {
                return largestColumn;
            }
            else
            {
                return largestColumn - 1;
            }
        }

        private int GetDuplicateCoordinate(IEnumerable<int> coordinates)
        {
            return coordinates.GroupBy(c => c)
                .Select(c => new { Coordinate = c.Key, Count = c.Count() })
                .OrderByDescending(g => g.Count)
                .First()
                .Coordinate;
        }
        private int GetTopRowCoordinateLocation(string row)
        {
            var x = Array.FindIndex(_rows, r => r == row);
            return (x * HEIGHT);
        }

        private int GetBottomRowCoordinateLocation(int coordinateLocation)
        {
            return coordinateLocation + HEIGHT;
        }

        private int GetLeftCoordinateLocation(int rightPosition)
        {
            return rightPosition - (WIDTH * 2);
        }

        private int GetRightCoordinateLocation(int column)
        {
            if (IsIntegerEven(column))
            {
                return (column * WIDTH);
            }
            else
            {
                return ((column + 1) * WIDTH);
            }
        }

        private bool IsIntegerEven(int number)
        {
            return number % 2 == 0;
        }

        private struct CoordinateStruct
        {
            public int row;
            public int column;
        }

        private void BuildExpectedResults()
        {
            int rightColumnCoordinate = 0;
            int leftColumnCoordinate = 0;

            foreach (string row in _rows)
            {
                for (int i = 1; i < 13; i++)
                {
                    Coordinate coordinate1 = new Coordinate();
                    Coordinate coordinate2 = new Coordinate();
                    Coordinate coordinate3 = new Coordinate();

                    int topRowCoordinate = GetTopRowCoordinateLocation(row);
                    int bottomRowCoordinate = GetBottomRowCoordinateLocation(topRowCoordinate);
                    rightColumnCoordinate = GetRightCoordinateLocation(i);
                    leftColumnCoordinate = GetLeftCoordinateLocation(rightColumnCoordinate);

                    if (IsIntegerEven(i))
                    {
                        coordinate1.row = topRowCoordinate;
                        coordinate1.column = leftColumnCoordinate;
                        coordinate2.row = topRowCoordinate;
                        coordinate2.column = rightColumnCoordinate;
                        coordinate3.row = bottomRowCoordinate;
                        coordinate3.column = rightColumnCoordinate;   
                    }
                    else
                    {
                        coordinate1.row = topRowCoordinate;
                        coordinate1.column = leftColumnCoordinate;
                        coordinate2.row = bottomRowCoordinate;
                        coordinate2.column = leftColumnCoordinate;
                        coordinate3.row = bottomRowCoordinate;
                        coordinate3.column = rightColumnCoordinate;
                    }

                    _triangleCoordinates.Add(row + i, new List<Coordinate> { coordinate1, coordinate2, coordinate3 });

                }
            }
        }
    }
}