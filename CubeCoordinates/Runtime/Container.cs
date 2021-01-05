using System.Collections.Generic;
using UnityEngine;

namespace CubeCoordinates
{
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

        private bool _isVisible;

        public bool isVisible
        {
            get
            {
                return _isVisible;
            }
        }

        public Container(string label)
        {
            _label = label;
            _contents = new Dictionary<Vector3, Coordinate>();
        }

        public void AddCoordinate(Coordinate coordinate)
        {
            if (!_contents.ContainsKey(coordinate.cube))
                _contents.Add(coordinate.cube, coordinate);
        }

        public void AddCoordinates(List<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
            AddCoordinate(coordinate);
        }

        public void RemoveCoordinate(Coordinate coordinate)
        {
            if (_contents.ContainsKey(coordinate.cube))
                _contents.Remove(coordinate.cube);
        }

        public void RemoveCoordinates(List<Coordinate> coordinates)
        {
            foreach (Coordinate coordinate in coordinates)
            RemoveCoordinate(coordinate);
        }

        public Coordinate GetCoordinate(Vector3 cube)
        {
            Coordinate coordinate = null;
            _contents.TryGetValue(cube, out coordinate);
            return coordinate;
        }

        public List<Coordinate> GetAllCoordinates()
        {
            return new List<Coordinate>(_contents.Values);
        }

        public List<Vector3> GetAllCubes()
        {
            return new List<Vector3>(_contents.Keys);
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public void Hide()
        {
            _isVisible = false;
        }

        public void Show()
        {
            _isVisible = true;
        }
    }
}
