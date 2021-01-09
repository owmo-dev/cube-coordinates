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
                return instance ?? ( instance = new GameObject("CubeCoordinates.Coordinates").AddComponent<Coordinates>() );
            }
        }

        private GameObject _coordinateGroup;
        private GameObject _coordinateGameObject = null;
        private Coordinate.Type _coordinateType = Coordinate.Type.None;
        private Dictionary<string, Container> _containers = new Dictionary<string, Container>();

        private float _scale = 1.0f;
        public float scale { get { return _scale; } }

        private float _radius = 1.0f;
        public float radius { get { return _radius; } }

        private float _spacingX = 0.0f;
        public float spacingX { get { return _spacingX; } }

        private float _spacingZ = 0.0f;
        public float spacingZ { get { return _spacingZ; } }

        private void Awake()
        {
            _coordinateGroup = new GameObject("(Group) Coordinates");
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
            _coordinateGroup = new GameObject("(Group) Coordinates");
        }

        public void SetCoordinateType( Coordinate.Type type, GameObject gameObject = null )
        {
            _coordinateType = type;

            if (_coordinateGameObject != null) Destroy(_coordinateGameObject);

            switch (_coordinateType)
            {
                case Coordinate.Type.None:
                    _coordinateGameObject = null;
                    break;
                case Coordinate.Type.Prefab:
                    _coordinateGameObject = gameObject;
                    break;
                case Coordinate.Type.GenerateMesh:
                    _coordinateGameObject =
                        MeshCreator.Instance.CreateGameObject(_radius);
                    break;
            }
        }

        public void PrepareCustomBase(List<Vector3> cubes)
        {
            CreateCoordinates (cubes);
        }

        public void PrepareRadialBase(int r)
        {
            CreateCoordinates(
                Cubes.GetNeighbors(Vector3.zero, r)
            );
        }

        private void CreateCoordinates(List<Vector3> cubes)
        {
            Container container = GetContainer();
            List<Coordinate> coordinates = new List<Coordinate>();

            foreach (Vector3 cube in cubes)
            {
                if (container.GetCoordinate(cube) != null) continue;
                Coordinate coordinate = new Coordinate(cube, Cubes.ConvertCubeToWorldPosition(cube, _spacingX, _spacingZ));
                coordinates.Add (coordinate);
            }

            container.AddCoordinates (coordinates);
        }

        public void Build()
        {
            if (_coordinateType == Coordinate.Type.None) return;

            List<Coordinate> coordinates = GetContainer().GetAllCoordinates();
            foreach (Coordinate coordinate in coordinates)
            {
                string label = "Coordinate [" +  coordinate.cube.x + "," + coordinate.cube.y + "," + coordinate.cube.z + "]";
                GameObject go = (GameObject)Instantiate(_coordinateGameObject, coordinate.position, Quaternion.identity);
                go.name = label;
                go.transform.parent = _coordinateGroup.transform;
                go.SetActive(true);
                coordinate.SetGameObject(go);
            }
        }

        public Container GetContainer(string key = "all")
        {
            Container container;
            if (!_containers.TryGetValue(key, out container))
            {
                CreateContainer(key);
                _containers.TryGetValue(key, out container);
            }
            return container;
        }

        private void CreateContainer(string key)
        {
            if (!_containers.ContainsKey(key)) _containers.Add(key, new Container(key));
        }

        public Coordinate GetNeighbor( Coordinate origin, int direction, int distance, string container_label = "all" )
        {
            return GetContainer(container_label).GetCoordinate(
                Cubes.GetNeighbor(origin.cube, direction, distance)
            );
        }

        public List<Coordinate> GetNeighbors( Coordinate origin, int steps, string container_label = "all" )
        {
            return GetContainer(container_label).GetCoordinates(
                Cubes.GetNeighbors(origin.cube, steps)
            );
        }

        public Coordinate GetDiagonalNeighbor( Coordinate origin, int direction, int distance, string container_label = "all" )
        {
            return GetContainer(container_label).GetCoordinate(
                Cubes.GetDiagonalNeighbor(origin.cube, direction, distance)
            );
        }

        public List<Coordinate> GetDiagonalNeighbors( Coordinate origin, int steps, string container_label = "all" )
        {
            List<Coordinate> results = new List<Coordinate>();
            Container container = GetContainer(container_label);
            for (int i = 0; i < 6; i++)
            {
                Coordinate coordinate = GetDiagonalNeighbor(origin, i, steps, container_label);
                if (coordinate != null) results.Add(coordinate);
            }
            return results;
        }

        public List<Coordinate> GetLine(Coordinate a, Coordinate b, string container_label = "all")
        {
            return GetContainer(container_label).GetCoordinates(
                Cubes.GetLine(a.cube, b.cube)
            );
        }

        public Coordinate GetPointOnLne( Coordinate a, Coordinate b, int distance, string container_label = "all" )
        {
            return GetContainer(container_label).GetCoordinate(
                Cubes.GetPointOnLine(a.cube, b.cube, distance)
            );
        }

        public List<Coordinate> GetRing(Coordinate origin, int distance, string container_label = "all")
        {
            return GetContainer(container_label).GetCoordinates(
                Cubes.GetRing(origin.cube, distance)
            );
        }

        public List<Coordinate> GetSpiral(Coordinate origin, int steps, string container_label = "all")
        {
            return GetContainer(container_label).GetCoordinates(
                Cubes.GetSpiral(origin.cube, steps)
            );
        }

        public List<Coordinate> GetExpand(Coordinate origin, int steps, string container_label = "all")
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
                    foreach ( Coordinate c in GetNeighbors(current, 1, container_label) )
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

        public List<Coordinate> GetPath( Coordinate origin, Coordinate target, string container_label = "all" )
        {
            if (origin == target) new List<Coordinate>();

            Container container = GetContainer(container_label);
            List<Vector3> openSet = new List<Vector3>();
            List<Vector3> closedSet = new List<Vector3>();
            openSet.Add(origin.cube);

            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
            cameFrom.Add(origin.cube, Vector3.zero);

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
                    if ( coordinate.fCost < currentCoordinate.fCost || coordinate.fCost == currentCoordinate.fCost && coordinate.hCost < currentCoordinate.hCost )
                    {
                        current = openSet[i];
                        currentCoordinate = container.GetCoordinate(current);
                    }
                }

                openSet.Remove (current);
                closedSet.Add (current);

                if (current == target.cube) break;

                List<Coordinate> neighbors = GetNeighbors(container.GetCoordinate(current), 1, container_label);

                foreach ( Vector3 neighbor in neighbors.Select(x => x.cube).ToList() )
                {
                    coordinate = container.GetCoordinate(neighbor);
                    if (coordinate == null || closedSet.Contains(neighbor))
                        continue;

                    newCost = currentCoordinate.gCost + Cubes.GetDistanceBetweenTwoCubes(current, neighbor);
                    neighborCoordinate = container.GetCoordinate(neighbor);

                    if ( newCost < neighborCoordinate.gCost || !openSet.Contains(neighbor) )
                    {
                        neighborCoordinate.gCost = newCost;
                        neighborCoordinate.hCost = Cubes.GetDistanceBetweenTwoCubes(current, neighbor);
                        cameFrom.Add (neighbor, current);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            List<Vector3> path = new List<Vector3>();

            current = target.cube;
            path.Add(target.cube);

            while (current != origin.cube)
            {
                cameFrom.TryGetValue(current, out current);
                path.Add (current);
            }

            path.Reverse();

            return container.GetCoordinates(path);
        }
    }
}
