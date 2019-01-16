using UnityEngine;
using System;
using System.Collections.Generic;

public class CubeCoordinates : MonoBehaviour
{
    private Dictionary<string, Dictionary<Vector3, Coordinate>> _coordinateContainers = new Dictionary<string, Dictionary<Vector3, Coordinate>>();

    private GameObject _group;

    private float _gameScale = 1.0f;
    private float _coordinateRadius = 1.0f;

    private float _coordinateWidth = 0.0f;
    private float _coordinateHeight = 0.0f;

    private float _spacingVertical = 0.0f;
    private float _spacingHorizontal = 0.0f;

    private Vector3[] _cubeDirections =
    {
        new Vector3(1.0f, -1.0f, 0.0f),
        new Vector3(1.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 1.0f, -1.0f),
        new Vector3(-1.0f, 1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 1.0f),
        new Vector3(0.0f, -1.0f, 1.0f)
    };

    private Vector3[] _cubeDiagonalDirections =
    {
        new Vector3( 2.0f, -1.0f, -1.0f),
        new Vector3( 1.0f, 1.0f, -2.0f),
        new Vector3( -1.0f, 2.0f, -1.0f),
        new Vector3(-2.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, -1.0f, 2.0f),
        new Vector3(1.0f, -2.0f, 1.0f)
    };

    private void Awake()
    {
        CalculateCoordinateDimensions();
    }

    // Uses gamescale to calculate spacings & dimensions
    public void CalculateCoordinateDimensions()
    {
        _coordinateRadius = _coordinateRadius * _gameScale;

        _coordinateWidth = _coordinateRadius * 2;
        _spacingHorizontal = _coordinateWidth * 0.75f;

        _coordinateHeight = (Mathf.Sqrt(3) / 2.0f) * _coordinateWidth;
        _spacingVertical = _coordinateHeight / 2.0f;

        HexMeshCreator.Instance.SetRadius(_coordinateRadius);
    }

    // Constructs new set of coordinates from 0,0,0 given a radius
    public void Construct(int radius)
    {
        Clear();
        _group = new GameObject("CubeCoordinates");

        for (int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
                for (int z = -radius; z <= radius; z++)
                    if ((x + y + z) == 0)
                        AddCube(new Vector3(x, y, z));
    }

    // Destroys all coordinates and entries
    private void Clear()
    {
        Destroy(_group);
        ClearAllCoordinateContainers();
    }

    // Creates a Coordinate GameObject for a given cube coordinate
    public void AddCube(Vector3 cube)
    {
        if (GetCoordinateFromContainer(cube, "all") != null)
            return;

        GameObject obj = new GameObject("Coordinate: [" + cube.x + "," + cube.y + "," + cube.z + "]");
        obj.transform.parent = _group.transform;

        Coordinate coordinate = obj.AddComponent<Coordinate>();
        coordinate.Init(
            cube,
            ConvertCubeToWorldPosition(cube)
        );

        AddCoordinateToContainer(coordinate, "all");
    }

    // Creates a set of Coordinate GameObjects for a given list of cube coordinates
    public void AddCubes(List<Vector3> cubes)
    {
        foreach (Vector3 cube in cubes)
            AddCube(cube);
    }

    // Removes and destroys a Coordinate for a given cube coordinate
    public void RemoveCube(Vector3 cube)
    {
        Coordinate coordinate = GetCoordinateFromContainer(cube, "all");
        if (coordinate == null)
            return;

        RemoveCoordinateFromAllContainers(coordinate);
        Destroy(coordinate.gameObject);
    }

    // Removes and destroys a set of Coordinates for a given list of cube coordinates
    public void RemoveCubes(List<Vector3> cubes)
    {
        foreach (Vector3 cube in cubes)
            RemoveCube(cube);
    }

    // Converts a cube coordinate to an axial coordinate
    public Vector2 ConvertCubetoAxial(Vector3 cube)
    {
        return new Vector2(cube.x, cube.z);
    }

    // Converts an axial coordinate to a cube coordinate
    public Vector3 ConvertAxialtoCube(Vector2 axial)
    {
        return new Vector3(axial.x, (-axial.x - axial.y), axial.y);
    }

    // Converts an axial coordinate to a world transform position
    public Vector3 ConvertAxialToWorldPosition(Vector2 axial)
    {
        float x = axial.x * _spacingHorizontal;
        float z = -((axial.x * _spacingVertical) + (axial.y * _spacingVertical * 2.0f));

        return new Vector3(x, 0.0f, z);
    }

    // Converts a cube coordinate to a world transform position
    public Vector3 ConvertCubeToWorldPosition(Vector3 cube)
    {
        float x = cube.x * _spacingHorizontal;
        float y = 0.0f;
        float z = -((cube.x * _spacingVertical) + (cube.z * _spacingVertical * 2.0f));

        return new Vector3(x, y, z);
    }

    // Converts a world transform position to the nearest axial coordinate
    public Vector2 ConvertWorldPositionToAxial(Vector3 wPos)
    {
        float q = (wPos.x * (2.0f / 3.0f)) / _coordinateRadius;
        float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / _coordinateRadius;
        return RoundAxial(new Vector2(q, r));
    }

    // Converts a world transform position to the nearest cube coordinate
    public Vector3 ConvertWorldPositionToCube(Vector3 wPos)
    {
        return ConvertAxialtoCube(ConvertWorldPositionToAxial(wPos));
    }

    // Rounds a given Vector2 to the nearest axial coordinate
    public Vector2 RoundAxial(Vector2 axial)
    {
        return RoundCube(ConvertAxialtoCube(axial));
    }

    // Rounds a given Vector3 to the nearest cube coordinate
    public Vector3 RoundCube(Vector3 coord)
    {
        float rx = Mathf.Round(coord.x);
        float ry = Mathf.Round(coord.y);
        float rz = Mathf.Round(coord.z);

        float x_diff = Mathf.Abs(rx - coord.x);
        float y_diff = Mathf.Abs(ry - coord.y);
        float z_diff = Mathf.Abs(rz - coord.z);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new Vector3(rx, ry, rz);
    }

    // Return a cube vector for a given direction index
    public Vector3 GetCubeDirection(int direction)
    {
        return _cubeDirections[direction];
    }

    // Returns an cube diagonal vector for a given direction index
    public Vector3 GetCubeDiagonalDirection(int direction)
    {
        return _cubeDiagonalDirections[direction];
    }

    // Returns the neighboring cube coordinate for a given direction index at a coordinate distance of 1
    public Vector3 GetNeighborCube(Vector3 cube, int direction)
    {
        return GetNeighborCube(cube, direction, 1);
    }

    // Returns the neighboring cube coordinate for a given direction index at a given coordinate distance
    public Vector3 GetNeighborCube(Vector3 cube, int direction, int distance)
    {
        return cube + (GetCubeDirection(direction) * (float)distance);
    }

    // Returns all neighboring cube coordinates at a coordinate distance of 1
    public List<Vector3> GetNeighborCubes(Vector3 cube)
    {
        return GetNeighborCubes(cube, 1);
    }

    // Returns all neighboring cube coordinates inclusively up to a given coordinate distance
    public List<Vector3> GetNeighborCubes(Vector3 cube, int radius, bool cleanResults = true)
    {
        List<Vector3> cubes = new List<Vector3>();

        for (int x = (int)(cube.x - radius); x <= (int)(cube.x + radius); x++)
            for (int y = (int)(cube.y - radius); y <= (int)(cube.y + radius); y++)
                for (int z = (int)(cube.z - radius); z <= (int)(cube.z + radius); z++)
                    if ((x + y + z) == 0)
                        cubes.Add(new Vector3(x, y, z));

        cubes.Remove(cube);

        if (cleanResults)
            return CleanCubeResults(cubes);
        return cubes;
    }

    // Returns a neighboring diagonal cube coordinate for a given direction index at a coordinate distance of 1
    public Vector3 GetDiagonalNeighborCube(Vector3 cube, int direction)
    {
        return cube + GetCubeDiagonalDirection(direction);
    }

    // Returns a neighboring diagonal cube coordinate for a given direction index at a given coordinate distance
    public Vector3 GetDiagonalNeighborCube(Vector3 cube, int direction, int distance)
    {
        return cube + (GetCubeDiagonalDirection(direction) * (float)distance);
    }

    // Returns all neighboring diagonal cube coordinates at a coordinate distance of 1
    public List<Vector3> GetDiagonalNeighborCubes(Vector3 cube)
    {
        return GetDiagonalNeighborCubes(cube, 1);
    }

    // Returns all neighboring diagonal cube coordinates at a given coordinate distance
    public List<Vector3> GetDiagonalNeighborCubes(Vector3 cube, int distance, bool cleanResults = true)
    {
        List<Vector3> cubes = new List<Vector3>();
        for (int i = 0; i < 6; i++)
            cubes.Add(GetDiagonalNeighborCube(cube, i, distance));
        if (cleanResults)
            return CleanCubeResults(cubes);
        return cubes;
    }

    // Boolean combines two lists of cube coordinates
    public List<Vector3> BooleanCombineCubes(List<Vector3> a, List<Vector3> b)
    {
        List<Vector3> vec = a;
        foreach (Vector3 vb in b)
            if (!a.Contains(vb))
                a.Add(vb);
        return vec;
    }

    // Boolean differences two lists of cube coordinates
    public List<Vector3> BooleanDifferenceCubes(List<Vector3> a, List<Vector3> b)
    {
        List<Vector3> vec = a;
        foreach (Vector3 vb in b)
            if (a.Contains(vb))
                a.Remove(vb);
        return vec;
    }

    // Boolean intersects two lists of cube coordinates
    public List<Vector3> BooleanIntersectionCubes(List<Vector3> a, List<Vector3> b)
    {
        List<Vector3> vec = new List<Vector3>();
        foreach (Vector3 va in a)
            foreach (Vector3 vb in b)
                if (va == vb)
                    vec.Add(va);
        return vec;
    }

    // Boolean excludes two lists of cube coordinates
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
        return vec;
    }

    // Rotates a cube coordinate right by one coordinate
    public Vector4 RotateCubeCoordinatesRight(Vector3 cube)
    {
        return new Vector3(-cube.z, -cube.x, -cube.y);
    }

    // Rotates a cube coordinate left by one coordinate
    public Vector4 RotateCubeCoordinatesLeft(Vector3 cube)
    {
        return new Vector3(-cube.y, -cube.z, -cube.x);
    }

    // Calculates the distance between two cube coordinates
    public float GetDistanceBetweenTwoCubes(Vector3 a, Vector3 b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
    }

    // Calculates the lerp interpolation between two cube coordinates given a value between 0.0f and 1.0f
    public Vector3 GetLerpBetweenTwoCubes(Vector3 a, Vector3 b, float t)
    {
        Vector3 cube = new Vector3(
            a.x + (b.x - a.x) * t,
            a.y + (b.y - a.y) * t,
            a.z + (b.z - a.z) * t
        );

        return cube;
    }

    // Returns the cube coordinate a given distance interval between two cube coordinates
    public Vector3 GetPointOnLineBetweenTwoCubes(Vector3 a, Vector3 b, int distance)
    {
        float cubeDistance = GetDistanceBetweenTwoCubes(a, b);
        return RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / cubeDistance) * distance)));
    }

    // Returns a list representing the line of cube coordinates between two given cube coordinates (inclusive)
    public List<Vector3> GetLineBetweenTwoCubes(Vector3 a, Vector3 b, bool cleanResults = true)
    {
        List<Vector3> cubes = new List<Vector3>();
        float cubeDistance = GetDistanceBetweenTwoCubes(a, b);
        for (int i = 0; i <= cubeDistance; i++)
            cubes.Add(RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / cubeDistance) * i))));
        cubes.Add(a);
        if (cleanResults)
            return CleanCubeResults(cubes);
        return cubes;
    }

    // Returns a list of validated neighboring cube coordinates at a evaluation distance of 1
    public List<Vector3> GetReachableCubes(Vector3 cube, bool cleanResults = true)
    {
        List<Vector3> cubes = new List<Vector3>();

        Coordinate originCoordinate = GetCoordinateFromContainer(cube, "all");
        Coordinate currentCoordinate = null;
        Vector3 currentCube = cube;

        for (int i = 0; i < 6; i++)
        {
            currentCube = GetNeighborCube(cube, i);
            currentCoordinate = GetCoordinateFromContainer(currentCube, "all");

            if (currentCoordinate != null)
                cubes.Add(currentCube);
        }

        if (cleanResults)
            return CleanCubeResults(cubes);
        return cubes;
    }

    // Returns a list of validated neighboring cube coordinates at a given evaluation radius
    public List<Vector3> GetReachableCubes(Vector3 cube, int radius, bool cleanResults = true)
    {
        if (radius == 1)
            return GetReachableCubes(cube);

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

        if (cleanResults)
            return CleanCubeResults(visited);
        return visited;
    }

    // Returns an ordered list of cube coordinates following a spiral pattern around a given cube coordinate at a given coordinate distance
    public List<Vector3> GetSpiralCubes(Vector3 cube, int radius, bool cleanResutls = true)
    {
        List<Vector3> vec = new List<Vector3>();
        Vector4 current = cube + GetCubeDirection(4) * (float)radius;

        for (int i = 0; i < 6; i++)
            for (int j = 0; j < radius; j++)
            {
                vec.Add(current);
                current = GetNeighborCube(current, i);
            }
        if (cleanResutls)
            return CleanCubeResults(vec);
        return vec;
    }

    // Returns an ordered list of cube coordinates following a spiral pattern around a given cube coordinate up to a given coordinate distance (inclusive)
    public List<Vector3> GetMultiSpiralCubes(Vector3 cube, int radius)
    {
        List<Vector3> cubes = new List<Vector3>();
        cubes.Add(cube);
        for (int i = 0; i <= radius; i++)
            foreach (Vector4 r in GetSpiralCubes(cube, i))
                cubes.Add(r);
        return cubes;
    }

    // Returns an ordered list of cube coordinates following the A* path results between two given cube coordinates
    public List<Vector3> GetPathBetweenTwoCubes(Vector3 origin, Vector3 target, string container = "all")
    {
        if (origin == target)
            return new List<Vector3>();

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
            currentCoordinate = GetCoordinateFromContainer(current, container);

            for (int i = 1; i < openSet.Count; i++)
            {
                coordinate = GetCoordinateFromContainer(openSet[i], container);
                if (coordinate.fCost < currentCoordinate.fCost || coordinate.fCost == currentCoordinate.fCost && coordinate.hCost < currentCoordinate.hCost)
                {
                    current = openSet[i];
                    currentCoordinate = GetCoordinateFromContainer(current, container);
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
                coordinate = GetCoordinateFromContainer(neighbor, container);
                if (coordinate == null || closedSet.Contains(neighbor))
                    continue;

                newCost = currentCoordinate.gCost + GetDistanceBetweenTwoCubes(current, neighbor);
                neighborCoordinate = GetCoordinateFromContainer(neighbor, container);

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

    // Validates all cube coordinate results against instantiated Coordinate GameObjects
    public List<Vector3> CleanCubeResults(List<Vector3> cubes)
    {
        List<Vector3> r = new List<Vector3>();
        foreach (Vector3 cube in cubes)
            if (GetCoordinateContainer("all").ContainsKey(cube))
                r.Add(cube);
        return r;
    }

    // Returns a coordinate container given a container key
    private Dictionary<Vector3, Coordinate> GetCoordinateContainer(string key)
    {
        Dictionary<Vector3, Coordinate> coordinateContainer;
        if (!_coordinateContainers.TryGetValue(key, out coordinateContainer))
        {
            _coordinateContainers.Add(key, new Dictionary<Vector3, Coordinate>());
            _coordinateContainers.TryGetValue(key, out coordinateContainer);
        }
        return coordinateContainer;
    }

    // Removes empty coordinate containers
    private void CleanEmptyCoordinateContainers()
    {
        List<string> coordinateContainerKeysToRemove = new List<string>();
        Dictionary<Vector3, Coordinate> coordinateContainer;
        foreach (string key in _coordinateContainers.Keys)
        {
            _coordinateContainers.TryGetValue(key, out coordinateContainer);
            if (coordinateContainer.Values.Count == 0)
                coordinateContainerKeysToRemove.Add(key);
        }

        foreach (string key in coordinateContainerKeysToRemove)
            _coordinateContainers.Remove(key);
    }

    // Returns a Coordinate given a cube coordinate and a container key
    public Coordinate GetCoordinateFromContainer(Vector3 cube, string key)
    {
        Coordinate coordinate = null;
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        if (cube == Vector3.zero)
            coordinateContainer.TryGetValue(Vector3.zero, out coordinate);
        else
            coordinateContainer.TryGetValue(cube, out coordinate);
        return coordinate;
    }

    // Returns a list of Coordinates given a container key
    public List<Coordinate> GetCoordinatesFromContainer(string key)
    {
        List<Coordinate> coordinates = new List<Coordinate>();
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        foreach (KeyValuePair<Vector3, Coordinate> entry in coordinateContainer)
            coordinates.Add(entry.Value);
        return coordinates;
    }

    // Returns a list of cube coordinates given a container key
    public List<Vector3> GetCubesFromContainer(string key)
    {
        List<Vector3> cubes = new List<Vector3>();
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        foreach (Vector3 cube in coordinateContainer.Keys)
            cubes.Add(cube);
        return cubes;
    }

    // Adds a given cube coordinate to the given container key
    public void AddCubeToContainer(Vector3 cube, string key)
    {
        AddCoordinateToContainer(GetCoordinateFromContainer(cube, "all"), key);
    }

    // Adds a list of cube coordinates to the given container key
    public void AddCubesToContainer(List<Vector3> cubes, string key)
    {
        foreach (Vector3 cube in cubes)
            AddCoordinateToContainer(GetCoordinateFromContainer(cube, "all"), key);
    }

    // Adds a given Coordinate to the given container key
    public bool AddCoordinateToContainer(Coordinate coordinate, string key)
    {
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        if (!coordinateContainer.ContainsKey(coordinate.cube))
        {
            coordinateContainer.Add(coordinate.cube, coordinate);
            return true;
        }
        return false;
    }

    // Removes a given Coordinate from the given container key
    public void RemoveCoordinateFromContainer(Coordinate coordinate, string key)
    {
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        if (coordinateContainer.ContainsKey(coordinate.cube))
            coordinateContainer.Remove(coordinate.cube);
    }

    // Removes all Coordinates from given container key
    public void RemoveAllCoordinatesInContainer(string key)
    {
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        coordinateContainer.Clear();
    }

    // Removes a given Coordinate from all containers
    public void RemoveCoordinateFromAllContainers(Coordinate coordinate)
    {
        foreach (string key in _coordinateContainers.Keys)
            RemoveCoordinateFromContainer(coordinate, key);
    }

    // Clears all coordinate containers
    public void ClearAllCoordinateContainers()
    {
        _coordinateContainers.Clear();
    }

    // Clears all Coordinates from a given container key
    public void ClearCoordinatesFromContainer(string key)
    {
        Dictionary<Vector3, Coordinate> coordinateContainer = GetCoordinateContainer(key);
        coordinateContainer.Clear();
    }

    // Hides all Coordinates for a given container key
    public void HideCoordinatesInContainer(string key)
    {
        foreach (Coordinate coordinate in GetCoordinatesFromContainer(key))
        {
            coordinate.Hide();
            RemoveCoordinateFromContainer(coordinate, "visible");
        }
    }

    // Shows all Coordinates for a given container key
    public void ShowCoordinatesInContainer(string key, bool bCollider = true)
    {
        foreach (Coordinate coordinate in GetCoordinatesFromContainer(key))
        {
            coordinate.Show(bCollider);
            AddCoordinateToContainer(coordinate, "visible");
        }
    }

    // Hides and Clears all Coordinates for a given container key
    public void HideAndClearCoordinateContainer(string key)
    {
        foreach (Coordinate coordinate in GetCoordinatesFromContainer(key))
            coordinate.Hide();
        ClearCoordinatesFromContainer(key);
    }
}
