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

        [Test]
        public void Test2var ()
        {
            IEnumerable<Coordinate> coordinates = GetCoordinatePositions("B", 1);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.row == 9 && c.column == 0), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.row == 19 && c.column == 0), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.row == 19 && c.column == 9), "should have found coordinate");
            }


            coordinates = GetCoordinatePositions("E", 8);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 29 && c.row == 39), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 39 && c.row == 39), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 39 && c.row == 49), "should have found coordinate");
            }

            coordinates = GetCoordinatePositions("A", 1);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 0), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 0 && c.row == 9), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 9 && c.row == 9), "should have found coordinate");
            }

            coordinates = GetCoordinatePositions("F", 12);
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(3, coordinates.Count(), "Coordinate count should be 3");
            foreach (Coordinate coord in coordinates)
            {
                Assert.IsTrue(coordinates.Any(c => c.column == 49 && c.row == 49), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 59 && c.row == 49), "should have found coordinate");
                Assert.IsTrue(coordinates.Any(c => c.column == 59 && c.row == 59), "should have found coordinate");
            }

            //get top
            //var x = GetTopRowCoordinateLocation("B");
            //Assert.AreEqual(9, x);

            //int y = GetBottomRowCoordinateLocation(x);
            //Assert.AreEqual(x+10, y);

            //x = GetRightCoordinateLocation(4);
            //Assert.AreEqual(39, x);

            //y = GetLeftCoordinateLocation(x);
            //Assert.AreEqual(x - 10, y);
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

            topRowLocation = topRowLocation == -1 ? 0 : topRowLocation;
            leftColumnLocation = leftColumnLocation == -1 ? 0 : leftColumnLocation;

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

        /// <summary>
        /// 
        /// </summary
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        //private IEnumerable<Coordinate> CalcHypotenuse1Coordinate(string row, int column)
        //{
        //    int topRowLocation = GetTopRowCoordinateLocation(row);
        //    int bottomRowLocation = GetBottomRowCoordinateLocation(topRowLocation);

        //    return new Coordinates();
        //}

        private int GetTopRowCoordinateLocation(string row)
        {
            var x = Array.FindIndex(rows, r => r == row);
            return (x * 10) - 1;

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
                return (column * WIDTH) - 1;
            }
            else
            {
                return ((column + 1) * WIDTH) - 1;
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
    }
}