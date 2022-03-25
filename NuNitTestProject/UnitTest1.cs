using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TestApi.Controllers;
using System.Linq;
using TestApi.Model;

namespace NuNitTestProject
{
    public class Tests
    {
        private readonly string[] _rows = new string[] {"A", "B", "C", "D", "E", "F"};
        private Dictionary<string, List<Coordinate>>  _triangleCoordinates = new Dictionary<string, List<Coordinate>>();

        private const int WIDTH = 5;
        private const int HEIGHT = 10;

        [SetUp]
        public void Setup()
        {
            BuildExpectedResults();
        }

        [Test]
        public void Test1()
        {
            var triangleController = new TriangleController();
            var coordinates = triangleController.GetTriangleCoordinates("A", 1);

            Coordinate coordinate1 = new Coordinate { column = 0, row = 0 };
            Coordinate coordinate2 = new Coordinate { column = 0, row = 10 };
            Coordinate coordinate3 = new Coordinate { column = 10, row = 10 };
            var location = triangleController.GetTriangleLocation(coordinate1, coordinate2, coordinate3);
            Assert.AreEqual("A1", location, "location should equal");

            coordinate1 = new Coordinate { column = 50, row = 50 };
            coordinate2 = new Coordinate { column = 60, row = 50 };
            coordinate3 = new Coordinate { column = 60, row = 60 };
            location = triangleController.GetTriangleLocation(coordinate1, coordinate2, coordinate3);
            Assert.AreEqual("F12", location, "location should equal");

            coordinate1 = new Coordinate { column = 30, row = 30 };
            coordinate2 = new Coordinate { column = 40, row = 30 };
            coordinate3 = new Coordinate { column = 40, row = 40 };
            location = triangleController.GetTriangleLocation(coordinate1, coordinate2, coordinate3);
            Assert.AreEqual("D8", location, "location should equal");

        }

        [Test]
        public void ReturnLocationTest()
        {
            /*
             * loop thru triangle coordinaetes
             * - get the each set of coordinates
             * - get the expected location
             * - call api
             * - assert locations equal
             */
            var triangleController = new TriangleController();

            foreach (var triangleLocation in _triangleCoordinates)
            {
                string column = triangleLocation.Key.Substring(0, 1);
                int row = int.Parse(triangleLocation.Key.Substring(1, triangleLocation.Key.Length-1));

                string expectedLocation = column + row;
                Coordinate coordinate1 = triangleLocation.Value[0];
                Coordinate coordinate2 = triangleLocation.Value[1];
                Coordinate coordinate3 = triangleLocation.Value[2];

                var location = triangleController.GetTriangleLocation(coordinate1,coordinate2, coordinate3);
                Assert.AreEqual(expectedLocation, location, "locations for triangle should be equal");
            }
        }

 #region return location

            [Test]
        public void ReturnCoordinatesTest()
        {
            var triangleController = new TriangleController();

            foreach (var triangleLocation in _triangleCoordinates)
            {
                string column = triangleLocation.Key.Substring(0, 1);
                int row = int.Parse(triangleLocation.Key.Substring(1, 1));

                var coordinates = triangleController.GetTriangleCoordinates(column, row);
                Assert.IsNotNull(coordinates, "coordinates for triangle should not be null");

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


            //List<int> rows = new List<int> { 30, 20, 30 };

            //var x = GetRow(rows);
            //Assert.AreEqual("C", x);

            //List<int> columns = new List<int> { 50, 50, 40 };
            //var y = GetColumn(columns);
            //Assert.AreEqual(10, y);

            //CoordinateStruct coordinate1 = new CoordinateStruct { column = 0, row = 0 };
            //CoordinateStruct coordinate2 = new CoordinateStruct { column = 0, row = 10 };
            //CoordinateStruct coordinate3 = new CoordinateStruct { column = 10, row = 10 };
            //List<CoordinateStruct> coordinates = new List<CoordinateStruct> { coordinate1 , coordinate2 , coordinate3 };

            //var location = GetLocation(coordinates);
            //Assert.AreEqual("A1", location);
        }

        private string GetLocation(IEnumerable<CoordinateStruct> coordinates)
        {
            var rows = coordinates.Select(c => c.row);
            var columns = coordinates.Select(c => c.column);

            var row = GetRow(rows);
            var column = GetColumn(columns);

            return row + column;
        }

        private string GetRow(IEnumerable<int> rowCoordinates)
        {
            var duplicateRowCoordinate = GetDuplicateCoordinate(rowCoordinates);
            var singleRowCoordinate = rowCoordinates.Where(r => r != duplicateRowCoordinate).First();

            int rowIndex;
            if (duplicateRowCoordinate < singleRowCoordinate)
            {
                rowIndex = (singleRowCoordinate / 10);
            }
            else
            {
                rowIndex = (duplicateRowCoordinate / 10);
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
        #endregion return location

        #region return coordinates
        [Test]
        public void Test2var ()
        {
            var triangleController = new TriangleController();
            //IEnumerable<CoordinateStruct> coordinates = GetCoordinatePositions("B", 1);
            var coordinates = triangleController.GetTriangleCoordinates("B", 1);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 10), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 20), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 10 && c.row == 20), "should have found coordinate");
            }


            coordinates = triangleController.GetTriangleCoordinates("E", 8);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 30 && c.row == 40), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 40 && c.row == 40), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 40 && c.row == 50), "should have found coordinate");
            }

            coordinates = triangleController.GetTriangleCoordinates("A", 1);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 0), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 10), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 10 && c.row == 10), "should have found coordinate");
            }

            coordinates = triangleController.GetTriangleCoordinates("F", 12);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 50 && c.row == 50), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 60 && c.row == 50), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 60 && c.row == 60), "should have found coordinate");
            }
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

            //topRowLocation = topRowLocation == -1 ? 0 : topRowLocation;
            //leftColumnLocation = leftColumnLocation == -1 ? 0 : leftColumnLocation;

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

        private int GetTopRowCoordinateLocation(string row)
        {
            var x = Array.FindIndex(_rows, r => r == row);
            return (x * 10);
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
        #endregion return coordinates

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