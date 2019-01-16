using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMeshCreator : MonoBehaviour
{
    private static HexMeshCreator _instance;
    private static readonly object Lock = new object();

    public static bool Quitting { get; private set; }

    public static HexMeshCreator Instance
    {
        get
        {
            if (Quitting)
            {
                Debug.LogWarning(typeof(HexMeshCreator).ToString() + ", Instance will not be returned, Application is Quitting.");
                return null;
            }

            lock (Lock)
            {
                if (_instance != null)
                    return _instance;

                var objs = FindObjectsOfType<HexMeshCreator>();
                if (objs.Length > 0)
                {
                    if (objs.Length == 1)
                        return _instance = objs[0];

                    Debug.LogWarning(typeof(HexMeshCreator).ToString() + ", More than 1 Singleton! First intance used, all others destroyed.");

                    for (var i = 1; i < objs.Length; i++)
                        Destroy(objs[i]);
                    return _instance = objs[0];
                }
                GameObject obj = new GameObject(typeof(HexMeshCreator).ToString());
                DontDestroyOnLoad(obj);
                return _instance = obj.AddComponent<HexMeshCreator>();
            }
        }
    }

    private void OnApplicationQuit()
    {
        Quitting = true;
    }

    public enum Type { Tile, Outline }

    private Mesh _mesh;
    private Mesh _outlineMesh;

    private float _radius = 1.0f;
    private float _upOffset = 0.01f;
    private float _innerRadius = 0.9f;

    // Sets the radius and constructs the geometry meshes
    public void SetRadius(float radius)
    {
        _radius = radius;
        _mesh = BuildMesh();
        _outlineMesh = BuildOutlineMesh(_upOffset, _innerRadius);
    }

    // Returns a vertex given an index
    private Vector3 GetVertex(int i)
    {
        float angle_deg = 60.0f * (float)i;
        float angle_rad = (Mathf.PI / 180.0f) * angle_deg;
        return new Vector3((_radius * Mathf.Cos(angle_rad)),
            0.0f,
            (_radius * Mathf.Sin(angle_rad))
        );
    }

    // Builds the tile mesh
    private Mesh BuildMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "hexTile";

        Vector3[] verts = new Vector3[6];

        verts[0] = GetVertex(0);
        verts[1] = GetVertex(1);
        verts[2] = GetVertex(2);
        verts[3] = GetVertex(3);
        verts[4] = GetVertex(4);
        verts[5] = GetVertex(5);

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

    // Builds the outline mesh
    private Mesh BuildOutlineMesh(float upOffset, float innerRadius)
    {
        Mesh mesh = new Mesh();
        mesh.name = "hexTileOutline";

        Vector3[] verts = new Vector3[6];

        verts[0] = GetVertex(0);
        verts[1] = GetVertex(1);
        verts[2] = GetVertex(2);
        verts[3] = GetVertex(3);
        verts[4] = GetVertex(4);
        verts[5] = GetVertex(5);

        Vector3[] v = new Vector3[12];

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

    // Adds the specified mesh to a given GameObject, does not add a MeshCollider
    public void AddToGameObject(GameObject go, Type type)
    {
        AddToGameObject(go, type, false);
    }

    // Adds the specified mesh to a given GameObject and can add a MeshCollider
    public void AddToGameObject(GameObject go, Type type, bool addMeshCollider)
    {
        if (go == null)
            return;

        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

        if (meshRenderer == null)
            meshRenderer = go.AddComponent<MeshRenderer>();

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.material = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = go.GetComponent<MeshFilter>();

        if (meshFilter == null)
            meshFilter = go.AddComponent<MeshFilter>();

        switch (type)
        {
            case Type.Tile:
                meshRenderer.material.color = Color.white;
                meshFilter.mesh = _mesh;
                break;

            case Type.Outline:
                meshRenderer.material.color = Color.black;
                meshFilter.mesh = _outlineMesh;
                break;
        }

        if (addMeshCollider)
        {
            MeshCollider meshCollider = go.GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = go.AddComponent<MeshCollider>();
        }
    }
}
