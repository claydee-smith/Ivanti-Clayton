using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TestApi.Controllers;
using System.Linq;

namespace NuNitTestProject
{
    public class Tests
    {
        private readonly string[] rows = new string[] {"A", "B", "C", "D", "E", "F"};

        private const int WIDTH = 5;
        private const int HEIGHT = 10;

        /*
         * 
         */
        
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            var controller = new JcsController();
            //controller.Request = new HttpRequestMessage();
            var x = controller.GetJcs();
            Assert.Pass();
        }
        #region return location

        [Test]
        public void ReturnLocationTest()
        {
            List<int> rows = new List<int> { 30, 20, 30 };
            var x = GetRow(rows);
            Assert.AreEqual("C", x);

            List<int> columns = new List<int> { 50, 50, 40 };
            var y = GetColumn(columns);
            Assert.AreEqual(10, y);

            Coordinate coordinate1 = new Coordinate { column = 0, row = 0 };
            Coordinate coordinate2 = new Coordinate { column = 0, row = 10 };
            Coordinate coordinate3 = new Coordinate { column = 10, row = 10 };
            List<Coordinate> coordinates = new List<Coordinate> { coordinate1 , coordinate2 , coordinate3 };

            var location = GetLocation(coordinates);
            Assert.AreEqual("A1", location);
        }

        private string GetLocation(IEnumerable<Coordinate> coordinates)
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

            return rows[rowIndex - 1];
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
            IEnumerable<Coordinate> coordinates = GetCoordinatePositions("B", 1);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 10), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 20), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 10 && c.row == 20), "should have found coordinate");
            }


            coordinates = GetCoordinatePositions("E", 8);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 30 && c.row == 40), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 40 && c.row == 40), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 40 && c.row == 50), "should have found coordinate");
            }

            coordinates = GetCoordinatePositions("A", 1);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 0), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 10), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 10 && c.row == 10), "should have found coordinate");
            }

            coordinates = GetCoordinatePositions("F", 12);
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
        private IEnumerable<Coordinate> GetCoordinatePositions(string row, int column)
        {
            int topRowLocation = GetTopRowCoordinateLocation(row);
            int bottomRowLocation = GetBottomRowCoordinateLocation(topRowLocation);
            int righttColumnLocation = GetRightCoordinateLocation(column);
            int leftColumnLocation = GetLeftCoordinateLocation(righttColumnLocation);

            //topRowLocation = topRowLocation == -1 ? 0 : topRowLocation;
            //leftColumnLocation = leftColumnLocation == -1 ? 0 : leftColumnLocation;

            Coordinate coordinate1 = new Coordinate();
            coordinate1.row = topRowLocation;
            coordinate1.column = leftColumnLocation;

            Coordinate coordinate2 = new Coordinate();
            coordinate2.row = bottomRowLocation;
            coordinate2.column = righttColumnLocation;

            Coordinate coordinate3 = new Coordinate();
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

            return new List<Coordinate> { coordinate1, coordinate2, coordinate3 };
        }

        private int GetTopRowCoordinateLocation(string row)
        {
            var x = Array.FindIndex(rows, r => r == row);
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

        private struct Coordinate
        {
            public int row;
            public int column;
        }
        #endregion return coordinates
    }
}