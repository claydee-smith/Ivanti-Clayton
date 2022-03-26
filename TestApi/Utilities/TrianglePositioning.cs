﻿using TestApi.Model;


namespace TestApi.Utilities
{
    public static class TrianglePositioning
    {
        private static readonly string[] _rows = new string[] { "A", "B", "C", "D", "E", "F" };
        private static readonly int[] _locations = new int[] { 0, 10, 20, 30, 40, 50, 60 };

        private const int WIDTH = 5;
        private const int HEIGHT = 10;

        /// <summary>
        /// Get the coordinates for a triangle for a graph based on a location
        /// </summary>
        /// <param name="row">The row for the triangle</param>
        /// <param name="column">The column for the triangle</param>
        /// <returns>An IEnumerable of Coordinates for the triangle</returns>
        public static IEnumerable<Coordinate> GetTriangleCoordinatesByLocation(string row, int column)
        {
            //if (IsLocationValid(row, column) == false)
            //{
            //    throw new ArgumentException("Triangle location is not valid.");
            //}

            IsLocationValid(row, column);

            int topRowLocation = GetTopRowCoordinateLocation(row);
            int bottomRowLocation = GetBottomRowCoordinateLocation(topRowLocation);
            int righttColumnLocation = GetRightCoordinateLocation(column);
            int leftColumnLocation = GetLeftCoordinateLocation(righttColumnLocation);

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

            return new List<Coordinate> { coordinate1, coordinate2, coordinate3 };
        }

        /// <summary>
        /// Get the location of a triangle based on the triangle's three coordinates
        /// </summary>
        /// <param name="coordinate1">First coordinate for the triangle</param>
        /// <param name="coordinate2">Second coordinate for the triangle</param>
        /// <param name="coordinate3">Third coordinate for the triangle</param>
        /// <returns>The location of the triangle</returns>
        public static string GetTriangleLocationByCoordinates(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
        {
            List<Coordinate> coordinates = new List<Coordinate> { coordinate1, coordinate2, coordinate3 };
            AreCoordinatesValid(coordinates);
            return GetLocation(coordinates);
        }

        private static void IsLocationValid(string row, int column)
        {
            if (_rows.Any(r => r == row) == false)
            {
                throw new ArgumentException("Triangle row location is not valid.");
            }

            if ( column <0 || column > 12)
            {
                throw new ArgumentException("Triangle column location is not valid.");
            }

            //return true;
        }

        private static void AreCoordinatesValid(IEnumerable<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
            {
                if (coordinate == null ||
                    _locations.Any(c => c == coordinate.row) == false ||
                    _locations.Any(c => c == coordinate.column) == false ||
                    AreRowAndColumnLocationsValid(coordinates) == false)
                {
                    throw new ArgumentException("One or more coordinates is invalid.");
                }
            }
        }

        private static bool AreRowAndColumnLocationsValid(IEnumerable<Coordinate> coordinates)
        {
            IEnumerable<int> columnPositions = coordinates.Select(c => c.column);
            IEnumerable<int> rowPositions = coordinates.Select(c => c.row);

            if ((AreLocationsValid(columnPositions) && AreLocationsValid(rowPositions)))
            {
                return true;
            }

            return false;
        }

        private static bool AreLocationsValid(IEnumerable<int> positions)
        {
            var duplicateLocation = positions.GroupBy(c => c)
                .Select(c => new { Location = c.Key, Count = c.Count() })
                .OrderByDescending(g => g.Count)
                .First();

            if (duplicateLocation.Count != 2)
            {
                return false;
            }

            int singleLocation = positions.Where(p => p != duplicateLocation.Location).First();
            if (Math.Abs(duplicateLocation.Location - singleLocation) != HEIGHT)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Get the top row coordinate for the triangle
        /// </summary>
        /// <param name="row">The row of the triangle</param>
        /// <returns>The top row coordinate for the triangle</returns>
        private static int GetTopRowCoordinateLocation(string row)
        {
            var x = Array.FindIndex(_rows, r => r == row);
            return (x * HEIGHT);
        }

        /// <summary>
        /// Get the bottom row coordinate for the triangle
        /// </summary>
        /// <param name="coordinateLocation">The coordinate of the top row for the triangle</param>
        /// <returns>The bottom row coordinate for the triangle</returns>
        private static int GetBottomRowCoordinateLocation(int coordinateLocation)
        {
            return coordinateLocation + HEIGHT;
        }

        /// <summary>
        /// Get the left columnCoordinate for the triangle
        /// </summary>
        /// <param name="rightCoordinate">The right coordinate for the triangle</param>
        /// <returns>The left column coordinate for the triangle</returns>
        private static int GetLeftCoordinateLocation(int rightCoordinate)
        {
            return rightCoordinate - (WIDTH * 2);
        }

        /// <summary>
        /// Get the right columnCoordinate for the triangle
        /// </summary>
        /// <param name="column"></param>
        /// <returns>The right column coordinate for the triangle</returns>
        private static int GetRightCoordinateLocation(int column)
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

        /// <summary>
        /// Determines if an integer is even
        /// </summary>
        /// <param name="number">Integer to evaluate</param>
        /// <returns>True if even, false if odd</returns>
        private static bool IsIntegerEven(int number)
        {
            return number % 2 == 0;
        }

        /// <summary>
        /// Get the location of the triangle from it's coordinates
        /// </summary>
        /// <param name="coordinates">Ienumerable of the trinagle's coordinates</param>
        /// <returns>The location on the grid of a triangle</returns>
        private static string GetLocation(IEnumerable<Coordinate> coordinates)
        {
            var rows = coordinates.Select(c => c.row);
            var columns = coordinates.Select(c => c.column);

            var row = GetRow(rows);
            var column = GetColumn(columns);

            return row + column;
        }

        /// <summary>
        /// Get the row location for the triangle
        /// </summary>
        /// <param name="rowCoordinates">IEnumerable of the triangle's row coordinates</param>
        /// <returns>The row of the triangle's location</returns>
        private static string GetRow(IEnumerable<int> rowCoordinates)
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

        /// <summary>
        /// Get the column location for the triangle
        /// </summary>
        /// <param name="columnCoordinate">IEnumerable of the triangle's column coordinates</param>
        /// <returns>The column of the triangle's location</returns>
        private static int GetColumn(IEnumerable<int> columnCoordinate)
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

        /// <summary>
        /// Get the coordinates that are duplicated
        /// </summary>
        /// <param name="coordinates">Ienumerable of integers representing coordinates of the triangle</param>
        /// <returns>The duplicated coordinate</returns>
        private static int GetDuplicateCoordinate(IEnumerable<int> coordinates)
        {
            return coordinates.GroupBy(c => c)
                .Select(c => new { Coordinate = c.Key, Count = c.Count() })
                .OrderByDescending(g => g.Count)
                .First()
                .Coordinate;
        }
    }
}
