using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeCoordinates;
using UnityEngine;

public class HappyFace : MonoBehaviour
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
        Coordinates.Instance.Clear();

        // Clear all Coordinates and GameObjects
        Coordinates.Instance.Clear();

        // Cube coordinate radius of 10 from Vector3.zero
        List<Vector3> cubes = Cubes.GetNeighbors(Vector3.zero, 10);

        // Cube coordinate circles for the "eyes"
        List<Vector3> eye_L = Cubes.GetNeighbors(new Vector3(-4, 5, -1), 2);
        List<Vector3> eye_R = Cubes.GetNeighbors(new Vector3(4, 1, -5), 2);

        // Subtract a smaller circle from a larger
        List<Vector3> mouth =
            Cubes
                .BooleanDifference(Cubes.GetNeighbors(new Vector3(0, 1, -1), 8),
                Cubes.GetNeighbors(new Vector3(0, 2, -2), 7));

        // Subtract two lines, to "fill in" the gaps in the first subtraction
        mouth =
            Cubes
                .BooleanDifference(mouth,
                Cubes.GetLine(new Vector3(8, 2, -10), new Vector3(8, -10, 2)));
        mouth =
            Cubes
                .BooleanDifference(mouth,
                Cubes
                    .GetLine(new Vector3(-8, 10, -2), new Vector3(-8, -2, 10)));

        // Fille in & remove to make the mouth curved
        mouth.Add(new Vector3(-1, -4, 5));
        mouth.Add(new Vector3(1, -5, 4));
        mouth.Add(new Vector3(0, -5, 5));
        mouth.Remove(new Vector3(0, -7, 7));

        // Remove eyes and mouth from all cubes
        cubes = cubes.Except(eye_L).ToList();
        cubes = cubes.Except(eye_R).ToList();
        cubes = cubes.Except(mouth).ToList();

        // Create Coordinates and Build
        Coordinates.Instance.CreateCoordinates (cubes);
        Coordinates.Instance.Build();
    }
}
