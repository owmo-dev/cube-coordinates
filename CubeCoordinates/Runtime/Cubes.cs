using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CubeCoordinates
{
    public static class Cubes
    {
        public static Vector3[]
            directions =
            {
                new Vector3(1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 1.0f),
                new Vector3(0.0f, -1.0f, 1.0f)
            };

        public static Vector3[]
            diagonals =
            {
                new Vector3(2.0f, -1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, -2.0f),
                new Vector3(-1.0f, 2.0f, -1.0f),
                new Vector3(-2.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, 2.0f),
                new Vector3(1.0f, -2.0f, 1.0f)
            };

        public static Vector3
        GetNeighbor(Vector3 origin, int direction, int distance)
        {
            return origin + directions[direction] * (float) distance;
        }

        public static List<Vector3> GetNeighbors(Vector3 origin, int steps)
        {
            List<Vector3> results = new List<Vector3>();

            for (
                int x = (int)(origin.x - steps);
                x <= (int)(origin.x + steps);
                x++
            )
            for (
                int y = (int)(origin.y - steps);
                y <= (int)(origin.y + steps);
                y++
            )
            for (
                int z = (int)(origin.z - steps);
                z <= (int)(origin.z + steps);
                z++
            )
            if ((x + y + z) == 0) results.Add(new Vector3(x, y, z));

            return results;
        }

        public static Vector3
        GetDiagonalNeighbor(Vector3 origin, int direction, int distance)
        {
            return origin + diagonals[direction] * (float) distance;
        }

        public static List<Vector3>
        GetDiagonalNeighbors(Vector3 origin, int distance)
        {
            List<Vector3> results = new List<Vector3>();
            for (int i = 0; i < 6; i++)
            for (int s = 1; s <= distance; s++)
            results.Add(GetDiagonalNeighbor(origin, i, s));
            return results;
        }

        public static List<Vector3> GetLine(Vector3 a, Vector3 b)
        {
            List<Vector3> results = new List<Vector3>();
            float d = GetDistanceBetweenTwoCubes(a, b);
            for (int i = 0; i <= d; i++)
            results
                .Add(RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / d) * i))));
            results.Add (a);
            return results;
        }

        public static Vector3 GetPointOnLine(Vector3 a, Vector3 b, int distance)
        {
            float d = GetDistanceBetweenTwoCubes(a, b);
            return RoundCube(GetLerpBetweenTwoCubes(a,
            b,
            ((1.0f / d) * distance)));
        }

        public static List<Vector3> GetRing(Vector3 origin, int distance)
        {
            List<Vector3> results = new List<Vector3>();
            Vector3 current = origin + directions[4] * (float) distance;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    results.Add (current);
                    current += directions[i];
                }
            }
            return results;
        }

        public static List<Vector3> GetSpiral(Vector3 origin, int steps)
        {
            List<Vector3> results = new List<Vector3>();
            results.Add (origin);
            for (int i = 0; i <= steps; i++)
            results.AddRange(GetRing(origin, i));
            return results;
        }

        public static List<Vector3>
        BooleanCombine(List<Vector3> a, List<Vector3> b)
        {
            List<Vector3> results = new List<Vector3>();
            results.AddRange (a);
            foreach (Vector3 vb in b) if (!a.Contains(vb)) results.Add(vb);
            return results;
        }

        public static List<Vector3>
        BooleanDifference(List<Vector3> a, List<Vector3> b)
        {
            List<Vector3> results = new List<Vector3>();
            results.AddRange (a);
            foreach (Vector3 vb in b) if (a.Contains(vb)) results.Remove(vb);
            return results;
        }

        public static List<Vector3>
        BooleanIntersect(List<Vector3> a, List<Vector3> b)
        {
            List<Vector3> results = new List<Vector3>();
            foreach (Vector3 va in a)
            foreach (Vector3 vb in b) if (va == vb) results.Add(va);
            return results;
        }

        public static List<Vector3>
        BooleanExclude(List<Vector3> a, List<Vector3> b)
        {
            return BooleanDifference(BooleanCombine(a, b),
            BooleanIntersect(a, b));
        }

        public static Vector3 RoundCube(Vector3 cube)
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

        public static Vector3 RotateCubeCoordinatesRight(Vector3 cube)
        {
            return new Vector3(-cube.z, -cube.x, -cube.y);
        }

        public static Vector3 RotateCubeCoordinatesLeft(Vector3 cube)
        {
            return new Vector3(-cube.y, -cube.z, -cube.x);
        }

        public static Vector2 ConvertCubeToAxial(Vector3 cube)
        {
            return new Vector2(cube.x, cube.z);
        }

        public static Vector3 ConvertAxialToCube(Vector2 axial)
        {
            return new Vector3(axial.x, (-axial.x - axial.y), axial.y);
        }

        public static Vector3 ConvertAxialToWorldPosition(Vector2 axial)
        {
            return new Vector3(axial.x * Coordinates.Instance.spacingX,
                0.0f,
                -(
                (axial.x * Coordinates.Instance.spacingZ) +
                (axial.y * Coordinates.Instance.spacingZ * 2.0f)
                ));
        }

        public static Vector3 ConvertCubeToWorldPosition(Vector3 cube)
        {
            return new Vector3(cube.x * Coordinates.Instance.spacingX,
                0.0f,
                -(
                (cube.x * Coordinates.Instance.spacingZ) +
                (cube.z * Coordinates.Instance.spacingZ * 2.0f)
                ));
        }

        public static Vector2 ConvertWorldPositionToAxial(Vector3 cube)
        {
            float q = (cube.x * (2.0f / 3.0f)) / Coordinates.Instance.radius;
            float r =
                ((-cube.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * cube.z)) /
                Coordinates.Instance.radius;
            return RoundAxial(new Vector2(q, r));
        }

        public static Vector3 ConvertWorldPositionToCube(Vector3 cube)
        {
            return ConvertAxialToCube(ConvertWorldPositionToAxial(cube));
        }

        public static Vector2 RoundAxial(Vector2 axial)
        {
            return RoundCube(ConvertAxialToCube(axial));
        }

        public static float GetDistanceBetweenTwoCubes(Vector3 a, Vector3 b)
        {
            return Mathf
                .Max(Mathf.Abs(a.x - b.x),
                Mathf.Abs(a.y - b.y),
                Mathf.Abs(a.z - b.z));
        }

        public static Vector3
        GetLerpBetweenTwoCubes(Vector3 a, Vector3 b, float t)
        {
            Vector3 cube =
                new Vector3(a.x + (b.x - a.x) * t,
                    a.y + (b.y - a.y) * t,
                    a.z + (b.z - a.z) * t);
            return cube;
        }
    }
}
