using UnityEngine;

namespace CubeCoordinates
{
    public class Coordinate
    {
        public enum DisplayType
        {
            None,
            Prefab,
            GenerateMesh
        }

        private Vector3 _cube = Vector3.zero;

        public Vector3 cube
        {
            get
            {
                return _cube;
            }
        }

        private Vector3 _position = Vector3.zero;

        public Vector3 position
        {
            get
            {
                return _position;
            }
        }

        public float gCost = 0.0f;

        public float hCost = 0.0f;

        public float fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        private GameObject _go;

        public GameObject go
        {
            get
            {
                return _go;
            }
        }

        public Coordinate(Vector3 cube, Vector3 position)
        {
            _cube = cube;
            _position = position;
        }

        public void SetGameObject(GameObject gameObject)
        {
            _go = gameObject;
        }
    }
}
