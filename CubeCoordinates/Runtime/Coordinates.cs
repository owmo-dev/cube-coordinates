using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CubeCoordinates
{
    public class Coordinates : MonoBehaviour
    {
        private static Coordinates instance;

        public static Coordinates Instance
        {
            get
            {
                return instance
                    ?? (
                    instance =
                        new GameObject("CubeCoordinates.Coordinates")
                            .AddComponent<Coordinates>()
                    );
            }
        }

        private GameObject _coordinateGroup;

        private Coordinate.DisplayType
            _coordinateDisplayType = Coordinate.DisplayType.None;

        private GameObject _coordinateGameObject = null;

        private Dictionary<string, Container>
            _containers = new Dictionary<string, Container>();

        private float _scale = 1.0f;

        private float _radius = 1.0f;

        private float _spacingX = 0.0f;

        private float _spacingZ = 0.0f;

        private Vector3[]
            _directions =
            {
                new Vector3(1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 1.0f),
                new Vector3(0.0f, -1.0f, 1.0f)
            };

        private Vector3[]
            _diagonals =
            {
                new Vector3(2.0f, -1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, -2.0f),
                new Vector3(-1.0f, 2.0f, -1.0f),
                new Vector3(-2.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, 2.0f),
                new Vector3(1.0f, -2.0f, 1.0f)
            };

        private void Awake()
        {
            _coordinateGroup = new GameObject("(Group)Coordinates");
            CalculateDimensions();
        }

        private void CalculateDimensions()
        {
            _radius = _radius * _scale;
            _spacingX = _radius * 2 * 0.75f;
            _spacingZ = ((Mathf.Sqrt(3) / 2.0f) * (_radius * 2) / 2);
        }

        public void Clear()
        {
            _containers.Clear();
            Destroy (_coordinateGroup);
            _coordinateGroup = new GameObject("(Group)Coordinates");
        }

        public void SetDisplayType(
            Coordinate.DisplayType type,
            GameObject gameObject = null
        )
        {
            _coordinateDisplayType = type;

            if (_coordinateGameObject != null) Destroy(_coordinateGameObject);

            switch (_coordinateDisplayType)
            {
                case Coordinate.DisplayType.None:
                    _coordinateGameObject = null;
                    break;
                case Coordinate.DisplayType.Prefab:
                    if (gameObject == null)
                        _coordinateGameObject = null;
                    else
                        _coordinateGameObject = gameObject;
                    break;
                case Coordinate.DisplayType.GenerateMesh:
                    _coordinateGameObject =
                        MeshCreator.Instance.CreateGameObject(_radius);
                    break;
            }
        }

        public void PrepareCustomBase(List<Vector3> cubes)
        {
            CreateCoordinates (cubes);
        }

        public void PrepareRadialBase(int radius)
        {
            List<Vector3> cubes = new List<Vector3>();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        if ((x + y + z) == 0) cubes.Add(new Vector3(x, y, z));
                    }
                }
            }

            CreateCoordinates (cubes);
        }

        private void CreateCoordinates(List<Vector3> cubes)
        {
            Container container = GetContainer();
            List<Coordinate> coordinates = new List<Coordinate>();

            foreach (Vector3 cube in cubes)
            {
                if (container.GetCoordinate(cube) != null) continue;
                Coordinate coordinate =
                    new Coordinate(cube, ConvertCubeToWorldPosition(cube));
                coordinates.Add (coordinate);
            }

            container.AddCoordinates (coordinates);
        }

        public void Build()
        {
            if (_coordinateDisplayType == Coordinate.DisplayType.None) return;

            List<Coordinate> coordinates = GetContainer().GetAllCoordinates();
            foreach (Coordinate coordinate in coordinates)
            {
                string label =
                    "Coordinate [" +
                    coordinate.cube.x +
                    "," +
                    coordinate.cube.y +
                    "," +
                    coordinate.cube.z +
                    "]";
                switch (_coordinateDisplayType)
                {
                    case Coordinate.DisplayType.GenerateMesh:
                        GameObject go =
                            (GameObject)
                            Instantiate(_coordinateGameObject,
                            coordinate.position,
                            Quaternion.identity);
                        go.name = label;
                        go.transform.parent = _coordinateGroup.transform;
                        go.SetActive(true);
                        coordinate.SetGameObject (go);
                        break;
                }
            }
        }

        public Container GetContainer(string key = "all")
        {
            Container container;
            if (!_containers.TryGetValue(key, out container))
            {
                CreateContainer (key);
                _containers.TryGetValue(key, out container);
            }
            return container;
        }

        private void CreateContainer(string key)
        {
            if (!_containers.ContainsKey(key))
                _containers.Add(key, new Container(key));
        }

        public Coordinate
        GetNeighbor(
            Coordinate origin,
            int direction,
            int distance,
            string container_label = "all"
        )
        {
            return GetContainer(container_label)
                .GetCoordinate(origin.cube +
                _directions[direction] * (float) distance);
        }

        public List<Coordinate>
        GetNeighbors(
            Coordinate origin,
            int steps,
            string container_label = "all"
        )
        {
            List<Coordinate> results = new List<Coordinate>();

            Container container = GetContainer(container_label);
            List<Coordinate> coordinates = container.GetAllCoordinates();
            List<Vector3> cubes = container.GetAllCubes();

            for (
                int x = (int)(origin.cube.x - steps);
                x <= (int)(origin.cube.x + steps);
                x++
            )
            {
                for (
                    int y = (int)(origin.cube.y - steps);
                    y <= (int)(origin.cube.y + steps);
                    y++
                )
                {
                    for (
                        int z = (int)(origin.cube.z - steps);
                        z <= (int)(origin.cube.z + steps);
                        z++
                    )
                    {
                        if ((x + y + z) == 0)
                        {
                            Vector3 cube = new Vector3(x, y, z);
                            if (cubes.Contains(cube) && cube != origin.cube)
                            {
                                results
                                    .Add(coordinates.Find(e => e.cube == cube));
                            }
                        }
                    }
                }
            }
            return results;
        }

        public Coordinate
        GetDiagonalNeighbor(
            Coordinate origin,
            int direction,
            int distance,
            string container_label = "all"
        )
        {
            return GetContainer(container_label)
                .GetCoordinate(origin.cube +
                _diagonals[direction] * (float) distance);
        }

        public List<Coordinate>
        GetDiagonalNeighbors(
            Coordinate origin,
            int distance,
            string container_label = "all"
        )
        {
            List<Coordinate> results = new List<Coordinate>();
            Container container = GetContainer(container_label);
            for (int i = 0; i < 6; i++)
            {
                Coordinate coordinate =
                    GetDiagonalNeighbor(origin, i, distance, container_label);
                if (coordinate != null) results.Add(coordinate);
            }
            return results;
        }

        public List<Coordinate>
        BooleanCombine(List<Coordinate> a, List<Coordinate> b)
        {
            List<Coordinate> results = new List<Coordinate>();
            results.AddRange (a);
            foreach (Coordinate c in b)
            if (!results.Contains(c)) results.Add(c);
            return results;
        }

        public List<Coordinate>
        BooleanDifference(List<Coordinate> a, List<Coordinate> b)
        {
            List<Coordinate> results = new List<Coordinate>();
            List<Vector3> va = a.Select(x => x.cube).ToList();
            List<Vector3> vb = b.Select(x => x.cube).ToList();
            foreach (Vector3 cube in vb)
            {
                if (va.Contains(cube)) va.Remove(cube);
            }
            foreach (Coordinate coordinate in a)
            {
                if (va.Contains(coordinate.cube)) results.Add(coordinate);
            }
            return results;
        }

        public List<Coordinate>
        BooleanIntersect(List<Coordinate> a, List<Coordinate> b)
        {
            List<Coordinate> results = new List<Coordinate>();
            foreach (Coordinate ca in a)
            {
                foreach (Coordinate cb in b)
                {
                    if (ca == cb) results.Add(ca);
                }
            }
            return results;
        }

        public List<Coordinate>
        BooleanExclude(List<Coordinate> a, List<Coordinate> b)
        {
            return BooleanDifference(BooleanCombine(a, b),
            BooleanIntersect(a, b));
        }

        public Coordinate
        GetPointOnLne(
            Coordinate a,
            Coordinate b,
            int distance,
            string container_label = "all"
        )
        {
            float d = GetDistanceBetweenTwoCubes(a.cube, b.cube);
            return GetContainer(container_label)
                .GetCoordinate(RoundCube(GetLerpBetweenTwoCubes(a.cube,
                b.cube,
                ((1.0f / d) * distance))));
        }

        public List<Coordinate>
        GetLine(Coordinate a, Coordinate b, string container_label = "all")
        {
            List<Vector3> cubes = new List<Vector3>();
            float distance = GetDistanceBetweenTwoCubes(a.cube, b.cube);
            for (int i = 0; i <= distance; i++)
            {
                cubes
                    .Add(RoundCube(GetLerpBetweenTwoCubes(a.cube,
                    b.cube,
                    ((1.0f / distance) * i))));
            }
            cubes.Add(a.cube);
            return GetContainer(container_label).GetCoordinates(cubes);
        }

        public List<Coordinate>
        GetExpand(Coordinate origin, int steps, string container_label = "all")
        {
            List<Coordinate> results = new List<Coordinate>();
            Container container = GetContainer(container_label);

            List<Vector3> visited = new List<Vector3>();
            visited.Add(origin.cube);

            List<List<Vector3>> fringes = new List<List<Vector3>>();
            fringes.Add(new List<Vector3>());
            fringes[0].Add(origin.cube);

            for (int i = 1; i <= steps; i++)
            {
                fringes.Add(new List<Vector3>());
                foreach (Vector3 v in fringes[i - 1])
                {
                    Coordinate current = container.GetCoordinate(v);
                    foreach (Coordinate
                        c
                        in
                        GetNeighbors(current, 1, container_label)
                    )
                    {
                        if (!visited.Contains(c.cube))
                        {
                            visited.Add(c.cube);
                            fringes[i].Add(c.cube);
                        }
                    }
                }
            }

            foreach (Vector3 v in visited)
            {
                results.Add(container.GetCoordinate(v));
            }

            return results;
        }

        public List<Coordinate>
        GetRing(Coordinate origin, int distance, string container_label = "all")
        {
            List<Coordinate> results = new List<Coordinate>();
            Container container = GetContainer(container_label);

            Vector3 current = origin.cube + _directions[4] * (float) distance;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    Coordinate coordinate = container.GetCoordinate(current);
                    if (coordinate != null) results.Add(coordinate);
                    current = current + _directions[i];
                }
            }

            return results;
        }

        public List<Coordinate>
        GetSpiral(Coordinate origin, int steps, string container_label = "all")
        {
            List<Coordinate> results = new List<Coordinate>();
            Container container = GetContainer(container_label);
            results.Add (origin);
            for (int i = 0; i <= steps; i++)
            results.AddRange(GetRing(origin, i));
            return results;
        }

        /*
        public List<Vector3>
        GetPathBetweenTwoCubes(
            Vector3 origin,
            Vector3 target,
            string container_label = "all"
        )
        {
            if (origin == target) return new List<Vector3>();

            Container container = GetContainer(container_label);

            List<Vector3> openSet = new List<Vector3>();
            List<Vector3> closedSet = new List<Vector3>();
            openSet.Add (origin);

            Dictionary<Vector3, Vector3> cameFrom =
                new Dictionary<Vector3, Vector3>();
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
                    if (
                        coordinate.fCost < currentCoordinate.fCost ||
                        coordinate.fCost == currentCoordinate.fCost &&
                        coordinate.hCost < currentCoordinate.hCost
                    )
                    {
                        current = openSet[i];
                        currentCoordinate = container.GetCoordinate(current);
                    }
                }

                openSet.Remove (current);
                closedSet.Add (current);

                if (current == target) break;

                List<Vector3> neighbors = new List<Vector3>();
                neighbors = GetReachableCubes(current);

                foreach (Vector3 neighbor in neighbors)
                {
                    coordinate = container.GetCoordinate(neighbor);
                    if (coordinate == null || closedSet.Contains(neighbor))
                        continue;

                    newCost =
                        currentCoordinate.gCost +
                        GetDistanceBetweenTwoCubes(current, neighbor);
                    neighborCoordinate = container.GetCoordinate(neighbor);

                    if (
                        newCost < neighborCoordinate.gCost ||
                        !openSet.Contains(neighbor)
                    )
                    {
                        neighborCoordinate.gCost = newCost;
                        neighborCoordinate.hCost =
                            GetDistanceBetweenTwoCubes(current, neighbor);
                        cameFrom.Add (neighbor, current);

                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }

            List<Vector3> path = new List<Vector3>();

            current = target;
            path.Add (target);

            while (current != origin)
            {
                cameFrom.TryGetValue(current, out current);
                path.Add (current);
            }

            path.Reverse();

            return path;
        }
        */

#region private
        private Vector3 RotateCubeCoordinatesRight(Vector3 cube)
        {
            return new Vector3(-cube.z, -cube.x, -cube.y);
        }

        private Vector3 RotateCubeCoordinatesLeft(Vector3 cube)
        {
            return new Vector3(-cube.y, -cube.z, -cube.x);
        }

        private Vector2 ConvertCubeToAxial(Vector3 cube)
        {
            return new Vector2(cube.x, cube.z);
        }

        private Vector3 ConvertAxialToCube(Vector2 axial)
        {
            return new Vector3(axial.x, (-axial.x - axial.y), axial.y);
        }

        private Vector3 ConvertAxialToWorldPosition(Vector2 axial)
        {
            return new Vector3(axial.x * _spacingX,
                0.0f,
                -((axial.x * _spacingZ) + (axial.y * _spacingZ * 2.0f)));
        }

        private Vector3 ConvertCubeToWorldPosition(Vector3 cube)
        {
            return new Vector3(cube.x * _spacingX,
                0.0f,
                -((cube.x * _spacingZ) + (cube.z * _spacingZ * 2.0f)));
        }

        private Vector2 ConvertWorldPositionToAxial(Vector3 wPos)
        {
            float q = (wPos.x * (2.0f / 3.0f)) / _radius;
            float r =
                ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) /
                _radius;
            return RoundAxial(new Vector2(q, r));
        }

        private Vector3 ConvertWorldPositionToCube(Vector3 wPos)
        {
            return ConvertAxialToCube(ConvertWorldPositionToAxial(wPos));
        }

        private Vector2 RoundAxial(Vector2 axial)
        {
            return RoundCube(ConvertAxialToCube(axial));
        }

        private Vector3 RoundCube(Vector3 cube)
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

        private float GetDistanceBetweenTwoCubes(Vector3 a, Vector3 b)
        {
            return Mathf
                .Max(Mathf.Abs(a.x - b.x),
                Mathf.Abs(a.y - b.y),
                Mathf.Abs(a.z - b.z));
        }

        private Vector3 GetLerpBetweenTwoCubes(Vector3 a, Vector3 b, float t)
        {
            Vector3 cube =
                new Vector3(a.x + (b.x - a.x) * t,
                    a.y + (b.y - a.y) * t,
                    a.z + (b.z - a.z) * t);

            return cube;
        }
#endregion
    }
}
