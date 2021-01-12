using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeCoordinates;
using UnityEngine;

public class RandomPath : MonoBehaviour
{
    private Coordinates coordinates;

    private void Awake()
    {
        Coordinates.Instance.SetCoordinateType(Coordinate.Type.GenerateMesh);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) BuildMap();
    }

    private void BuildMap()
    {
        // Clear all Coordinates and GameObjects
        Coordinates.Instance.Clear();

        // Cube coordinate radius of 10 from Vector3.zero
        List<Vector3> cubes = Cubes.GetNeighbors(Vector3.zero, 10);

        // Randomly remove some of the coordinates
        for (int i = cubes.Count - 1; i >= 0; i--)
        {
            // Exclude Vector3.zero, our starting point
            if (cubes[i] == Vector3.zero) continue;

            if (Random.Range(0.0f, 100.0f) < 30.0f) cubes.RemoveAt(i);
        }

        // Create the Coordinate objects, they're necessary for `GetExpand` to work
        Coordinates.Instance.CreateCoordinates (cubes);

        // Get the origin Coordinate
        Coordinate origin =
            Coordinates.Instance.GetContainer().GetCoordinate(Vector3.zero);

        // Use `GetExpand` to find all Coordinates connected to Vector3.zero, removing others
        Coordinates
            .Instance
            .GetContainer()
            .RemoveCoordinates(Coordinates
                .Instance
                .BooleanDifference(Coordinates
                    .Instance
                    .GetContainer()
                    .GetAllCoordinates(),
                Coordinates.Instance.GetExpand(origin, 10)));

        // Build instantiates the GameObjects, connecting them to Coordinate objects
        Coordinates.Instance.Build();

        int steps = 10;
        bool found = false;
        do
        {
            if (steps <= 0) return;

            // Get a Ring (circle) of Coordinates at "steps" from origin, from existing Coordinates
            List<Coordinate> ring = Coordinates.Instance.GetRing(origin, steps);
            if (ring.Count > 0)
            {
                // Get a Path of Coordinates from 'origin' to random Coordinate in Ring results
                List<Coordinate> path =
                    Coordinates
                        .Instance
                        .GetPath(origin, ring[Random.Range(0, ring.Count - 1)]);

                if (path.Count >= 2)
                {
                    // Add Coordinates to a custom "path" container
                    Coordinates
                        .Instance
                        .GetContainer("path")
                        .AddCoordinates(path);
                    found = true;
                }
            }

            // Reduce steps if Ring results in no Coordinates
            steps--;
        }
        while (!found);

        // Get the path Coordinates from the Container and add some color
        foreach (Coordinate
            c
            in
            Coordinates.Instance.GetContainer("path").GetAllCoordinates()
        )
        c.go.GetComponent<Renderer>().material.color = Color.red;
    }
}
