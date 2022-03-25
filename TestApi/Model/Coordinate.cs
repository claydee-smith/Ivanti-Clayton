namespace TestApi.Model
{
    /// <summary>
    /// The coordinate location for a given point on the graph
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// The row coordinate on the graph
        /// </summary>
        public int row { get; set; }

        /// <summary>
        /// The column coordinate on the graph
        /// </summary>
        public int column { get; set; }
    }
}
