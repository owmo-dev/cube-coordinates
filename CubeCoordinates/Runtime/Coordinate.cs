using UnityEngine;

namespace CubeCoordinates
{
    public class Coordinate : MonoBehaviour
    {
        public enum Type { Empty, Prefab, GenerateMesh }

        private Vector3 _cube = Vector3.zero;
        public Vector3 cube { get { return _cube; } }

        private Vector3 _position = Vector3.zero;
        public Vector3 position { get { return _position; } }

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
            gameObject.transform.position = _position;
        }
    }
}