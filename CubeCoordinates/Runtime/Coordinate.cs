using UnityEngine;

namespace CubeCoordinates
{
    /// <summary>
    /// Individual representation of a cube coordinate in the system
    /// </summary>
    public class Coordinate
    {
        public enum Type
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

        /// <summary>
        /// Sets the Coordinate cube coordinate and world position
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="position"></param>
        public Coordinate(Vector3 cube, Vector3 position)
        {
            _cube = cube;
            _position = position;
        }

        /// <summary>
        /// Sets the GameObject which is associated with the Coordinate
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetGameObject(GameObject gameObject)
        {
            _go = gameObject;
        }
    }
}
