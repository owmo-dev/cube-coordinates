using UnityEngine;
using System;
using System.Collections.Generic;

namespace CubeCoordinates
{
    public class Coordinate : MonoBehaviour
    {
        private Vector3 _cube = Vector3.zero;
        public Vector3 cube { get { return _cube; } }

        private Vector3 _position = Vector3.zero;
        public Vector3 position { get { return _position; } }

        public float gCost = 0.0f;
        public float hCost = 0.0f;
        public float fCost
        {
            get { return gCost + hCost; }
        }

        public void Init(Vector3 cube, Vector3 position)
        {
            this._cube = cube;
            this._position = position;
            gameObject.transform.position = _position;
        }
    }

    public class Container
    {
        private string _label;
        public string label
        {
            get { return label; }
        }
        private Dictionary<Vector3, Coordinate> _contents;

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
    }

    public class Coordinates : MonoBehaviour
    {
        private GameObject _go;
        private Dictionary<string, Container> _containers = new Dictionary<string, Container>();

        private float _scale = 1.0f;
        private float _radius = 1.0f;
        private float _spacingX = 0.0f;
        private float _spacingZ = 0.0f;

        private Vector3[] _directions = {
            new Vector3(1.0f, -1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, -1.0f),
            new Vector3(0.0f, 1.0f, -1.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(-1.0f, 0.0f, 1.0f),
            new Vector3(0.0f, -1.0f, 1.0f)
        };

        private Vector3[] _diagonals = {
            new Vector3( 2.0f, -1.0f, -1.0f),
            new Vector3( 1.0f, 1.0f, -2.0f),
            new Vector3( -1.0f, 2.0f, -1.0f),
            new Vector3(-2.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 2.0f),
            new Vector3(1.0f, -2.0f, 1.0f)
        };

        private void Awake()
        {
            CalculateDimensions();
        }

        private void CalculateDimensions()
        {
            _radius = _radius * _scale;
            _spacingX = _radius * 2 * 0.75f;
            _spacingZ = (Mathf.Sqrt(3) / 2.0f) * (_radius * 2);
        }

        public void BuildRadial(int radius)
        {
            _go = new GameObject(this.GetType().ToString());

            List<Vector3> cubes = new List<Vector3>();

            for (int x = -radius; x <= radius; x++)
                for (int y = -radius; y <= radius; y++)
                    for (int z = -radius; z <= radius; z++)
                        if ((x + y + z) == 0)
                            cubes.Add(new Vector3(x, y, z));

            CreateCoordinates(cubes);
        }

        public Vector3 ConvertCubeCoordinateToWorldPosition(Vector3 cube)
        {
            return new Vector3(
                cube.x * _spacingX,
                0.0f,
                -((cube.x * _spacingZ) + (cube.z * _spacingZ * 2.0f)));
        }

        private void Clear()
        {
            Destroy(_go);
            _containers.Clear();
        }

        private void CreateCoordinates(List<Vector3> cubes)
        {
            Container container = GetContainer("all");
            List<Coordinate> coordinates = new List<Coordinate>();

            foreach (Vector3 cube in cubes)
            {
                if (container.GetCoordinate(cube) != null)
                    continue;

                GameObject go = new GameObject("Coordinate: [" + cube.x + "," + cube.y + "," + cube.z + "]");
                go.transform.parent = _go.transform;

                Coordinate coordinate = go.AddComponent<Coordinate>();
                coordinate.Init(
                    cube,
                    ConvertCubeCoordinateToWorldPosition(cube)
                );
            }

            container.AddCoordinates(coordinates);
        }

        public void CreateContainer(string key)
        {
            if (!_containers.ContainsKey(key))
                _containers.Add(key, new Container(key));
        }

        public Container GetContainer(string key)
        {
            Container container;
            if (!_containers.TryGetValue(key, out container))
            {
                CreateContainer(key);
                _containers.TryGetValue(key, out container);
            }
            return container;
        }
    }
}