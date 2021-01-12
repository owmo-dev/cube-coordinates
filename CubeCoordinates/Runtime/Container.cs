using System.Collections.Generic;
using UnityEngine;

namespace CubeCoordinates
{
    /// <summary>
    /// Used to manage lists of Coordinate instances for a given label
    /// </summary>
    public class Container
    {
        private string _label;

        public string label
        {
            get
            {
                return label;
            }
        }

        private Dictionary<Vector3, Coordinate> _contents;

        /// <summary>
        /// Constructor for Container which accepts a label and prepares the internal Dictionary
        /// </summary>
        /// <param name="label">Label string to name the Container</param>
        public Container(string label)
        {
            _label = label;
            _contents = new Dictionary<Vector3, Coordinate>();
        }

        /// <summary>
        /// Add a Coordinate instance to the Container
        /// </summary>
        /// <param name="coordinate">Coordinate</param>
        public void AddCoordinates(Coordinate coordinate)
        {
            AddCoordinates(new List<Coordinate> { coordinate });
        }

        /// <summary>
        /// Add a Coordinate List instances to the Container
        /// </summary>
        /// <param name="coordinates">Coordinate List</param>
        public void AddCoordinates(List<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
            if (!_contents.ContainsKey(coordinate.cube))
                _contents.Add(coordinate.cube, coordinate);
        }

        /// <summary>
        /// Remove a Coordinate instance from the Container
        /// </summary>
        /// <param name="coordinate">Coordinate</param>
        public void RemoveCoordinates(Coordinate coordinate)
        {
            RemoveCoordinates(new List<Coordinate> { coordinate });
        }

        /// <summary>
        /// Remove a Coordinate List from the Container
        /// </summary>
        /// <param name="coordinates">Coordinate List</param>
        public void RemoveCoordinates(List<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
            if (_contents.ContainsKey(coordinate.cube))
                _contents.Remove(coordinate.cube);
        }

        /// <summary>
        /// Remove all Coordinate instances from the Container
        /// </summary>
        public void RemoveAllCoordinates()
        {
            _contents.Clear();
        }

        /// <summary>
        /// Gets a Coordinate matching the cube coordinate supplied, if found
        /// </summary>
        /// <param name="cube">Vector3 cube coordinate</param>
        /// <returns>Coordinate instance if found, null otherwise</returns>
        public Coordinate GetCoordinate(Vector3 cube)
        {
            Coordinate coordinate = null;
            _contents.TryGetValue(cube, out coordinate);
            return coordinate;
        }

        /// <summary>
        /// Gets a Coordinate matching the closest rounded cube coordinate to the world transform position (Coordinate may not exist at rounded cube coordinate)
        /// </summary>
        /// <param name="position">Vector3 transform position in world space</param>
        /// <returns>Coordinate instance if found</returns>
        public Coordinate GetCoordinateFromWorldPosition(Vector3 position)
        {
            Vector3 cube = Cubes.ConvertWorldPositionToCube(position);
            return GetCoordinate(cube);
        }

        /// <summary>
        /// Get a Coordinate List that match any of the supplied cube coordinates
        /// </summary>
        /// <param name="cubes">Vector3 List of cube coordinates</param>
        /// <returns>List of Coordinate instances found</returns>
        public List<Coordinate> GetCoordinates(List<Vector3> cubes)
        {
            List<Coordinate> results = new List<Coordinate>();
            foreach (Vector3 cube in cubes)
            {
                Coordinate coordinate = GetCoordinate(cube);
                if (coordinate != null) results.Add(coordinate);
            }
            return results;
        }

        /// <summary>
        /// Gets a Coordinate List of all Coordinate instances in the Container
        /// </summary>
        /// <returns>List of Coordinate instances</returns>
        public List<Coordinate> GetAllCoordinates()
        {
            return new List<Coordinate>(_contents.Values);
        }

        /// <summary>
        /// Gets a Vector3 cube coordinate List for all Coordinate instances in the Container
        /// </summary>
        /// <returns>List of Vector3 cube coordinates</returns>
        public List<Vector3> GetAllCubes()
        {
            return new List<Vector3>(_contents.Keys);
        }
    }
}
