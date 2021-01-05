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

        private bool _isVisible;
        public bool isVisible
        {
            get { return _isVisible; }
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
                    ConvertCubeToWorldPosition(cube)
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

        public Vector2 ConvertCubeToAxial(Vector3 cube)
        {
            return new Vector2(cube.x, cube.z);
        }

        public Vector3 ConvertAxialToCube(Vector2 axial)
        {
            return new Vector3(axial.x, (-axial.x - axial.y), axial.y);
        }

        public Vector3 ConvertAxialToWorldPosition(Vector2 axial)
        {
            float x = axial.x * _spacingX;
            float z = -((axial.x * _spacingZ) + (axial.y * _spacingZ * 2.0f));

            return new Vector3(x, 0.0f, z);
        }

        public Vector3 ConvertCubeToWorldPosition(Vector3 cube)
        {
            return new Vector3(
                cube.x * _spacingX,
                0.0f,
                -((cube.x * _spacingZ) + (cube.z * _spacingZ * 2.0f)));
        }

        public Vector2 ConvertWorldPositionToAxial(Vector3 wPos)
        {
            float q = (wPos.x * (2.0f / 3.0f)) / _radius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / _radius;
            return RoundAxial(new Vector2(q, r));
        }

        public Vector3 ConvertWorldPositionToCube(Vector3 wPos)
        {
            return ConvertAxialToCube(ConvertWorldPositionToAxial(wPos));
        }

        public Vector2 RoundAxial(Vector2 axial)
        {
            return RoundCube(ConvertAxialToCube(axial));
        }

        public Vector3 RoundCube(Vector3 cube)
        {
            float rx = Mathf.Round(cube.x);
            float ry = Mathf.Round(cube.y);
            float rz = Mathf.Round(cube.z);

            float x_diff = Mathf.Abs(rx - cube.x);
            float y_diff = Mathf.Abs(ry - cube.y);
            float z_diff = Mathf.Abs(rz - cube.z);

            if (x_diff > y_diff && x_diff > z_diff)
                rx = -ry - rz;
            else if (y_diff > z_diff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Vector3(rx, ry, rz);
        }

        public Vector3 GetCubeDiagonal(int direction)
        {
            return _diagonals[direction];
        }

        public Vector3 GetNeighborCube(Vector3 cube, int direction)
        {
            return GetNeighborCube(cube, direction, 1);
        }

        public Vector3 GetNeighborCube(Vector3 cube, int direction, int distance)
        {
            return cube + (_directions[direction] * (float)distance);
        }

        public List<Vector3> GetNeighborCubes(Vector3 cube)
        {
            return GetNeighborCubes(cube, 1);
        }

        public List<Vector3> GetNeighborCubes(Vector3 cube, int radius)
        {
            List<Vector3> cubes = new List<Vector3>();
            for (int x = (int)(cube.x - radius); x <= (int)(cube.x + radius); x++)
                for (int y = (int)(cube.y - radius); y <= (int)(cube.y + radius); y++)
                    for (int z = (int)(cube.z - radius); z <= (int)(cube.z + radius); z++)
                        if ((x + y + z) == 0)
                            cubes.Add(new Vector3(x, y, z));

            cubes.Remove(cube);
            return CleanCubeResults(cubes);
        }

        public Vector3 GetDiagonalNeighborCube(Vector3 cube, int direction)
        {
            return cube + _diagonals[direction];
        }

        public Vector3 GetDiagonalNeighborCube(Vector3 cube, int direction, int distance)
        {
            return cube + _diagonals[direction] * (float)distance;
        }

        public List<Vector3> GetDiagonalNeighborCubes(Vector3 cube)
        {
            return CleanCubeResults(GetDiagonalNeighborCubes(cube, 1));
        }

        public List<Vector3> GetDiagonalNeighborCubes(Vector3 cube, int distance)
        {
            List<Vector3> cubes = new List<Vector3>();
            for (int i = 0; i < 6; i++)
                cubes.Add(GetDiagonalNeighborCube(cube, i, distance));
            return CleanCubeResults(cubes);
        }

        public List<Vector3> BooleanCombineCubes(List<Vector3> a, List<Vector3> b)
        {
            List<Vector3> vec = a;
            foreach (Vector3 vb in b)
                if (!a.Contains(vb))
                    a.Add(vb);
            return CleanCubeResults(vec);
        }

        public List<Vector3> BooleanIntersectionCubes(List<Vector3> a, List<Vector3> b)
        {
            List<Vector3> vec = new List<Vector3>();
            foreach (Vector3 va in a)
                foreach (Vector3 vb in b)
                    if (va == vb)
                        vec.Add(va);
            return CleanCubeResults(vec);
        }

        public List<Vector3> BooleanExclusionCubes(List<Vector3> a, List<Vector3> b)
        {
            List<Vector3> vec = new List<Vector3>();
            foreach (Vector3 va in a)
                foreach (Vector3 vb in b)
                    if (va != vb)
                    {
                        vec.Add(va);
                        vec.Add(vb);
                    }
            return CleanCubeResults(vec);
        }

        public Vector3 RotateCubeCoordinatesRight(Vector3 cube)
        {
            return new Vector3(-cube.z, -cube.x, -cube.y);
        }

        public Vector3 RotateCubeCoordinatesLeft(Vector3 cube)
        {
            return new Vector3(-cube.y, -cube.z, -cube.x);
        }

        public float GetDistanceBetweenTwoCubes(Vector3 a, Vector3 b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
        }

        public Vector3 GetLerpBetweenTwoCubes(Vector3 a, Vector3 b, float t)
        {
            Vector3 cube = new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );

            return cube;
        }

        public Vector3 GetPointOnLineBetweenTwoCubes(Vector3 a, Vector3 b, int distance)
        {
            float cubeDistance = GetDistanceBetweenTwoCubes(a, b);
            return RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / cubeDistance) * distance)));
        }

        public List<Vector3> GetLineBetweentwoCubes(Vector3 a, Vector3 b)
        {
            List<Vector3> cubes = new List<Vector3>();
            float cubeDistance = GetDistanceBetweenTwoCubes(a, b);
            for (int i = 0; i <= cubeDistance; i++)
                cubes.Add(RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / cubeDistance) * i))));
            cubes.Add(a);
            return CleanCubeResults(cubes);
        }

        public List<Vector3> GetReachableCubes(Vector3 cube)
        {
            return GetReachableCubes(cube, 1);
        }

        public List<Vector3> GetReachableCubes(Vector3 cube, int radius)
        {
            if (radius == 1)
                return GetReachableCubes(cube);

            Container container = GetContainer("all");
            List<Vector3> visited = new List<Vector3>();
            visited.Add(cube);

            List<List<Vector3>> fringes = new List<List<Vector3>>();
            fringes.Add(new List<Vector3>());
            fringes[0].Add(cube);

            for (int i = 1; i <= radius; i++)
            {
                fringes.Add(new List<Vector3>());
                foreach (Vector3 v in fringes[i - 1])
                {
                    foreach (Vector3 n in GetNeighborCubes(v))
                    {
                        if (!visited.Contains(n))
                        {
                            visited.Add(n);
                            fringes[i].Add(n);
                        }
                    }
                }
            }
            return visited;
        }

        public List<Vector3> GetSpiralCubes(Vector3 cube, int radius)
        {
            List<Vector3> vec = new List<Vector3>();
            Vector3 current = cube + _directions[4] * (float)radius;

            for (int i = 0; i < 6; i++)
                for (int j = 0; j < radius; j++)
                {
                    vec.Add(current);
                    current = GetNeighborCube(current, i);
                }

            return CleanCubeResults(vec);
        }

        public List<Vector3> GetMultiSpiralCubes(Vector3 cube, int radius)
        {
            List<Vector3> cubes = new List<Vector3>();
            cubes.Add(cube);
            for (int i = 0; i <= radius; i++)
                foreach (Vector4 r in GetSpiralCubes(cube, i))
                    cubes.Add(r);
            return cubes;
        }

        public List<Vector3> GetPathBetweenTwoCubes(Vector3 origin, Vector3 target, string containerLabel = "all")
        {
            if (origin == target)
                return new List<Vector3>();

            Container container = GetContainer(containerLabel);

            List<Vector3> openSet = new List<Vector3>();
            List<Vector3> closedSet = new List<Vector3>();
            openSet.Add(origin);

            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
            cameFrom.Add(origin, Vector3.zero);

            Vector3 current = Vector3.zero;
            Coordinate coordinate = null;
            Coordinate currentCoordinate = null;
            Coordinate neighborCoordinate = null;
            float newCost = 0.0f;

            while (openSet.Count > 0)
            {
                current = openSet[0];
                currentCoordinate = container.GetCoordinate(current);

                for (int i = 1; i < openSet.Count; i++)
                {
                    coordinate = container.GetCoordinate(openSet[i]);
                    if (coordinate.fCost < currentCoordinate.fCost || coordinate.fCost == currentCoordinate.fCost && coordinate.hCost < currentCoordinate.hCost)
                    {
                        current = openSet[i];
                        currentCoordinate = container.GetCoordinate(current);
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current == target)
                    break;

                List<Vector3> neighbors = new List<Vector3>();
                neighbors = GetReachableCubes(current);

                foreach (Vector3 neighbor in neighbors)
                {
                    coordinate = container.GetCoordinate(neighbor);
                    if (coordinate == null || closedSet.Contains(neighbor))
                        continue;

                    newCost = currentCoordinate.gCost + GetDistanceBetweenTwoCubes(current, neighbor);
                    neighborCoordinate = container.GetCoordinate(neighbor);

                    if (newCost < neighborCoordinate.gCost || !openSet.Contains(neighbor))
                    {
                        neighborCoordinate.gCost = newCost;
                        neighborCoordinate.hCost = GetDistanceBetweenTwoCubes(current, neighbor);
                        cameFrom.Add(neighbor, current);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }

                }
            }

            List<Vector3> path = new List<Vector3>();

            current = target;
            path.Add(target);

            while (current != origin)
            {
                cameFrom.TryGetValue(current, out current);
                path.Add(current);
            }

            path.Reverse();

            return path;
        }

        public List<Vector3> CleanCubeResults(List<Vector3> cubes)
        {
            Container container = GetContainer("all");
            List<Vector3> r = new List<Vector3>();
            foreach (Vector3 cube in cubes)
                if (container.GetCoordinate(cube) != null)
                    r.Add(cube);
            return r;
        }

    }
}