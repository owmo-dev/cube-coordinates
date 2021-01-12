using System.Collections.Generic;
using UnityEngine;

namespace CubeCoordinates
{
    /// <summary>
    /// Generates hexagonal meshes useful for debugging and prototyping purposes
    /// </summary>
    public class MeshCreator : MonoBehaviour
    {
        private static MeshCreator _instance;

        private static readonly object Lock = new object();

        public static MeshCreator Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance != null) return _instance;

                    GameObject obj =
                        new GameObject("{MonoBehaviour}<{" +
                            typeof (MeshCreator).ToString() +
                            "}>");
                    DontDestroyOnLoad (obj);
                    return _instance = obj.AddComponent<MeshCreator>();
                }
            }
        }

        /// <summary>
        /// Generates a GameObject with hexagonal and outline meshes
        /// </summary>
        /// <param name="radius">Radius of the tile geometry</param>
        /// <returns>GameObject</returns>
        public GameObject CreateGameObject(float radius)
        {
            GameObject go = new GameObject("generated");
            go.hideFlags = HideFlags.HideInHierarchy;
            go.SetActive(false);

            AddBaseMeshToGameObject(go, radius, Color.white);

            GameObject go_outline = new GameObject("outline");
            go_outline.transform.parent = go.transform;

            AddOutlineMeshToGameObject(go_outline, radius, Color.black);

            return go;
        }

        /// <summary>
        /// Generates the base hexagonal mesh and adds it to the specified GameObject
        /// </summary>
        /// <param name="go">GameObject to add mesh to</param>
        /// <param name="radius">Radius of the tile geometry</param>
        /// <param name="color">Color to apply to the geometry</param>
        public void AddBaseMeshToGameObject(
            GameObject go,
            float radius,
            Color color
        )
        {
            Mesh mesh = MeshCreator.Instance.GetHexBase(radius);
            PrepareGameObject (go, mesh, color);
        }

        /// <summary>
        /// Generates the outline mesh and adds it to the specified GameObject
        /// </summary>
        /// <param name="go">GameObject to add mesh to</param>
        /// <param name="radius">Radius of the outline geometry</param>
        /// <param name="color">Color to apply to the geometry</param>
        public void AddOutlineMeshToGameObject(
            GameObject go,
            float radius,
            Color color
        )
        {
            Mesh mesh = MeshCreator.Instance.GetHexOutline(radius);
            PrepareGameObject (go, mesh, color);
        }

        /// <summary>
        /// Prepares a GameObject with MeshRenderer and MeshFilter
        /// </summary>
        /// <param name="go">GameObject to add Components to</param>
        /// <param name="mesh">Mesh to add to GameObject</param>
        /// <param name="color">Color to apply to MeshRenderer</param>
        private void PrepareGameObject(GameObject go, Mesh mesh, Color color)
        {
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.material = new Material(Shader.Find("Standard"));

            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = go.AddComponent<MeshFilter>();
            meshRenderer.material.color = color;
            meshFilter.mesh = mesh;
        }

        /// <summary>
        /// Generates a hexagonal base Mesh
        /// </summary>
        /// <param name="radius">Radius of the hexagonal mesh to create</param>
        /// <returns>Mesh geometry</returns>
        private Mesh GetHexBase(float radius)
        {
            Mesh mesh = new Mesh();
            mesh.name = "hex_base";

            Vector3[] verts = new Vector3[6];

            verts[0] = GetVertex(0, radius);
            verts[1] = GetVertex(1, radius);
            verts[2] = GetVertex(2, radius);
            verts[3] = GetVertex(3, radius);
            verts[4] = GetVertex(4, radius);
            verts[5] = GetVertex(5, radius);

            mesh.vertices = verts;

            int[] triangles = new int[12];

            triangles[0] = 0;
            triangles[1] = 5;
            triangles[2] = 1;
            triangles[3] = 1;
            triangles[4] = 5;
            triangles[5] = 2;
            triangles[6] = 5;
            triangles[7] = 4;
            triangles[8] = 2;
            triangles[9] = 4;
            triangles[10] = 3;
            triangles[11] = 2;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Generates a hexagonal outline Mesh
        /// </summary>
        /// <param name="radius">Radius of the hexagonal outline mesh to create</param>
        /// <returns>Mesh geometry</returns>
        private Mesh GetHexOutline(float radius)
        {
            Mesh mesh = new Mesh();
            mesh.name = "hexTileOutline";

            Vector3[] verts = new Vector3[6];

            verts[0] = GetVertex(0, radius);
            verts[1] = GetVertex(1, radius);
            verts[2] = GetVertex(2, radius);
            verts[3] = GetVertex(3, radius);
            verts[4] = GetVertex(4, radius);
            verts[5] = GetVertex(5, radius);

            Vector3[] v = new Vector3[12];

            float upOffset = radius * 0.005f;
            float innerRadius = radius * 0.8f;

            v[0] = verts[0] + (Vector3.up * upOffset);
            v[1] = verts[1] + (Vector3.up * upOffset);
            v[2] = verts[2] + (Vector3.up * upOffset);
            v[3] = verts[3] + (Vector3.up * upOffset);
            v[4] = verts[4] + (Vector3.up * upOffset);
            v[5] = verts[5] + (Vector3.up * upOffset);
            v[6] = (verts[0] * innerRadius) + (Vector3.up * upOffset);
            v[7] = (verts[1] * innerRadius) + (Vector3.up * upOffset);
            v[8] = (verts[2] * innerRadius) + (Vector3.up * upOffset);
            v[9] = (verts[3] * innerRadius) + (Vector3.up * upOffset);
            v[10] = (verts[4] * innerRadius) + (Vector3.up * upOffset);
            v[11] = (verts[5] * innerRadius) + (Vector3.up * upOffset);

            mesh.vertices = v;

            int[] triangles = new int[36];

            triangles[0] = 0;
            triangles[1] = 5;
            triangles[2] = 11;

            triangles[3] = 0;
            triangles[4] = 11;
            triangles[5] = 6;

            triangles[6] = 1;
            triangles[7] = 0;
            triangles[8] = 6;

            triangles[9] = 1;
            triangles[10] = 6;
            triangles[11] = 7;

            triangles[12] = 2;
            triangles[13] = 1;
            triangles[14] = 7;

            triangles[15] = 2;
            triangles[16] = 7;
            triangles[17] = 8;

            triangles[18] = 2;
            triangles[19] = 8;
            triangles[20] = 9;

            triangles[21] = 2;
            triangles[22] = 9;
            triangles[23] = 3;

            triangles[24] = 3;
            triangles[25] = 9;
            triangles[26] = 10;

            triangles[27] = 3;
            triangles[28] = 10;
            triangles[29] = 4;

            triangles[30] = 4;
            triangles[31] = 10;
            triangles[32] = 11;

            triangles[33] = 4;
            triangles[34] = 11;
            triangles[35] = 5;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Retruns a vertex for mesh generation
        /// </summary>
        /// <param name="i">index</param>
        /// <param name="radius">radius</param>
        /// <returns>Vector3</returns>
        private Vector3 GetVertex(int i, float radius)
        {
            float angle_deg = 60.0f * (float) i;
            float angle_rad = (Mathf.PI / 180.0f) * angle_deg;
            return new Vector3((radius * Mathf.Cos(angle_rad)),
                0.0f,
                (radius * Mathf.Sin(angle_rad)));
        }
    }
}
