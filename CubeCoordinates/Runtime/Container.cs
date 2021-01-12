using System.Collections.Generic;
using UnityEngine;

namespace CubeCoordinates
{
    public class Container
    {
        private string _label;
        public string label { get { return label; } }

        private Dictionary<Vector3, Coordinate> _contents;

        public Container(string label)
        {
            _label = label;
            _contents = new Dictionary<Vector3, Coordinate>();
        }

        public void AddCoordinates(Coordinate coordinate)
        {
            AddCoordinates(new List<Coordinate>{coordinate});
        }

        public void AddCoordinates(List<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
                if (!_contents.ContainsKey(coordinate.cube))
                    _contents.Add(coordinate.cube, coordinate);
        }

        public void RemoveCoordinates(Coordinate coordinate)
        {
            RemoveCoordinates(new List<Coordinate>{coordinate});
        }

        public void RemoveCoordinates(List<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
                if (_contents.ContainsKey(coordinate.cube))
                    _contents.Remove(coordinate.cube);
        }

        public void RemoveAllCoordinates()
        {
            _contents.Clear();
        }

        public Coordinate GetCoordinate(Vector3 cube)
        {
            Coordinate coordinate = null;
            _contents.TryGetValue(cube, out coordinate);
            return coordinate;
        }

        public Coordinate GetCoordinateFromWorldPosition(Vector3 position, float radius)
        {
            Vector3 cube = Cubes.ConvertWorldPositionToCube(position, radius);
            return GetCoordinate(cube);
        }

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

        public List<Coordinate> GetAllCoordinates()
        {
            return new List<Coordinate>(_contents.Values);
        }

        public List<Vector3> GetAllCubes()
        {
            return new List<Vector3>(_contents.Keys);
        }
    }
}
