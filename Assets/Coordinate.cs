using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate : MonoBehaviour
{
    private Vector3 _position = Vector3.zero;
    public Vector3 position { get { return _position; } }

    private Vector3 _cube = Vector3.zero;
    public Vector3 cube { get { return _cube; } }

    public float gCost = 0.0f;
    public float hCost = 0.0f;
    public float fCost
    {
        get { return gCost + hCost; }
    }

    public void Init(Vector3 cube, Vector3 position)
    {
        this._cube = cube;
        this._position = position;
    }
}
