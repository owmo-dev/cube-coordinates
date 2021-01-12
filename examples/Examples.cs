using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CubeCoordinates;
using System.Linq;

public class Examples : MonoBehaviour
{
    private Coordinates coordinates;

    private void Awake()
    {
        Coordinates.Instance.SetCoordinateType(Coordinate.Type.GenerateMesh);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RandomMap();
        else if ( Input.GetKeyDown(KeyCode.H) )
            HappyFace();
    }

    private void RandomMap()
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

            if (Random.Range(0.0f, 100.0f) < 30.0f)
                cubes.RemoveAt(i);
        }

        // Create the Coordinate objects, they're necessary for `GetExpand` to work
        Coordinates.Instance.CreateCoordinates(cubes);

        // Get the origin Coordinate
        Coordinate origin = Coordinates.Instance.GetContainer().GetCoordinate(Vector3.zero);

        // Use `GetExpand` to find all Coordinates connected to Vector3.zero, removing others
        Coordinates.Instance.GetContainer().RemoveCoordinates(
            Coordinates.Instance.BooleanDifference(
                Coordinates.Instance.GetContainer().GetAllCoordinates(),
                Coordinates.Instance.GetExpand(origin, 10)
            )
        );

        // Build instantiates the GameObjects, connecting them to Coordinate objects
        Coordinates.Instance.Build();

        int steps = 10;
        bool found = false;
        do
        {
            if (steps <= 0)
                return;
            
            // Get a Ring (circle) of Coordinates at "steps" from origin, from existing Coordinates
            List<Coordinate> ring = Coordinates.Instance.GetRing(origin, steps);
            if ( ring.Count > 0)
            {
                // Get a Path of Coordinates from 'origin' to random Coordinate in Ring results
                List<Coordinate> path = Coordinates.Instance.GetPath(
                    origin,
                    ring[Random.Range(0, ring.Count-1)]
                );

                if (path.Count >= 2)
                {
                    // Add Coordinates to a custom "path" container
                    Coordinates.Instance.GetContainer("path").AddCoordinates(path);
                    found = true;
                }
            }

            // Reduce steps if Ring results in no Coordinates
            steps--;
        }
        while (!found);

        // Get the path Coordinates from the Container and add some color
        foreach (Coordinate c in Coordinates.Instance.GetContainer("path").GetAllCoordinates())
            c.go.GetComponent<Renderer>().material.color = Color.red;
    }

    private void HappyFace()
    {
        Coordinates.Instance.Clear();

        // Clear all Coordinates and GameObjects
        Coordinates.Instance.Clear();

        // Cube coordinate radius of 10 from Vector3.zero
        List<Vector3> cubes = Cubes.GetNeighbors(Vector3.zero, 10);

        // Cube coordinate circles for the "eyes"
        List<Vector3> eye_L = Cubes.GetNeighbors(new Vector3(-4,5,-1), 2);
        List<Vector3> eye_R = Cubes.GetNeighbors(new Vector3(4,1,-5), 2);
        

        // Subtract a smaller circle from a larger
        List<Vector3> mouth = Cubes.BooleanDifference(
            Cubes.GetNeighbors(new Vector3(0,1,-1), 8),
            Cubes.GetNeighbors(new Vector3(0,2,-2), 7)
        );

        // Subtract two lines, to "fill in" the gaps in the first subtraction
        mouth = Cubes.BooleanDifference(
            mouth,
            Cubes.GetLine(new Vector3(8,2,-10), new Vector3(8,-10,2))
        );
        mouth = Cubes.BooleanDifference(
            mouth,
            Cubes.GetLine(new Vector3(-8,10,-2), new Vector3(-8,-2,10))
        );

        // Fille in & remove to make the mouth curved
        mouth.Add(new Vector3(-1,-4,5));
        mouth.Add(new Vector3(1,-5,4));
        mouth.Add(new Vector3(0,-5,5));
        mouth.Remove(new Vector3(0,-7,7));

        // Remove eyes and mouth from all cubes
        cubes = cubes.Except(eye_L).ToList();
        cubes = cubes.Except(eye_R).ToList();
        cubes = cubes.Except(mouth).ToList();

        // Create Coordinates and Build
        Coordinates.Instance.CreateCoordinates(cubes);
        Coordinates.Instance.Build();
    }
}
