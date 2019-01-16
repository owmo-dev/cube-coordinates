# unity-hexagonal-grids

Collection of cube coordinate functionality for constructing and interacting with hexagonal grids in Unity.

![Constructed Grid](images/grid_build.jpg) | ![A* Path Tracing](images/grid_path.jpg) | ![Reachable Coordinates](images/grid_reachable.jpg)
-|-|-

## Scripts
The repository contains the following scripts within the `Assets` folder:

#### Coordinate.cs
Responsible for the information pertaining to a single `Coordinate` within the `CubeCoordinate` grid; cube coordinate, transform position, A* path costs, meshes, display/hide functionality, etc...

#### CubeCoordinates.cs
Responsible for the construction of a hexagonal grid of `Coordinate` instances, managing named containers of `Coordinate` instances for building interactions with the grid, and performing validated coordinate manipulations.

#### HexMeshCreator.cs
Singleton used to provide each `Coordinate` with an appropriate hexagonal mesh for display.

---
## Example
The repository contains the following example within the `Assets\Example` folder:

#### ExampleScene.unity
The scene contains GameObject named "ExampleCubeCoordinate" within the scene that has the `CubeCoordinate` and `ExampleScript` components added to it.

#### ExampleScript.cs
Sets up an example scene (as seen in the above images) which constructs a grid with some minimal randomization, performs a some of the coordinate manipulations and stores them in coordinate containers to be displayed using the following key presses:

- Return > Constructs the grid
- Backspace > Displays all grid coordinates
- L > Displays a line between two coordinates (non-contiguous if coordinates are missing)
- P > Displays an A* path between two coordinates
- S > Displays a spiral, 3 coordinates away from zero
- R > Displays a reachable set of tiles up to 3 coordinates away from zero

Example usage:

```csharp
// Construct a grid with radius of 10
CubeCoordinates cubeCoordinates;
cubeCoordinates = gameObject.GetComponent<CubeCoordinates>();
cubeCoordinates.Construct(10);

// Randomly sample coordinates and add them to a "random" coordinate container
foreach(Vector3 cube in cubeCoordinates.GetCubesFromContainer("all"))
    if (Random.Range(0.0f, 100.0f) < 25.0f)
        cubeCoordinates.AddCubeToContainer(cube, "random");

// Remove "random" coordinates from the grid and cleanup
cubeCoordinates.RemoveCubes(cubeCoordinates.GetCubesFromContainer("random"));
cubeCoordinates.CleanEmptyCoordinateContainers();

//Find and display a path between two coordinates
List<Vector3> allCubeCoordinates = cubeCoordinates.GetCubesFromContainer("all");

cubeCoordinates.AddCubesToContainer(
    cubeCoordinates.GetPathBetweenTwoCubes(
        allCubeCoordinates[0],
        allCubeCoordinates[allCubeCoordinates.Count - 1],
        "path"
    )
);

cubeCoordinates.HideCoordinatesInContainer("all");
cubeCoordinates.ShowCoordinatesInContainer("path");
```

---

##### Developed by following this guide: **https://www.redblobgames.com/grids/hexagons/**
