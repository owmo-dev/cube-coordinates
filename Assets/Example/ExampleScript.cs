using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    private CubeCoordinates _cubeCoordinates;

    private void Awake()
    {
        _cubeCoordinates = gameObject.GetComponent<CubeCoordinates>();
    }

    private void Start()
    {
        _cubeCoordinates.New(10);

        // Remove 25% of Coordinates except 0,0,0
        foreach (Vector3 cube in _cubeCoordinates.GetCubesFromContainer("all"))
        {
            if (cube == Vector3.zero)
                continue;

            if (Random.Range(0.0f, 100.0f) < 25.0f)
                _cubeCoordinates.RemoveCube(cube);
        }

        // Remove Coordinates not reachable from 0,0,0
        _cubeCoordinates.RemoveCubes(
            _cubeCoordinates.BooleanDifferenceCubes(
                _cubeCoordinates.GetCubesFromContainer("all"),
                _cubeCoordinates.GetReachableCubes(Vector3.zero, 10)
            )
        );

        // Display Coordinates
        _cubeCoordinates.ShowCoordinatesInContainer("all");
    }

    private void Update()
    {
    }
}
