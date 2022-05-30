using UnityEngine;

namespace Snowboard.Views
{
    public class ParallaxBackground : MonoBehaviour
    {

        public SpriteRenderer[] Backgrounds;

        public Camera Camera;

        public float Parallax;


        private float length;

        private Vector3 cameraPosition;

        private float cameraBound;

        private Transform cameraTransform;

        private void Start()
        {
            cameraTransform = Camera.transform;
            cameraPosition = cameraTransform.position;

            cameraBound = Camera.aspect * Camera.orthographicSize;
            length = Backgrounds[0].bounds.size.x;
        }

        private void LateUpdate()
        {

            var diff = cameraTransform.position - cameraPosition;
            diff.x *= Parallax;

            cameraPosition = cameraTransform.position;


            for (int i = 0; i < Backgrounds.Length; i++)
            {
                var pos = Backgrounds[i].transform.position;
                pos += diff;

                if (Backgrounds.Length > 1)
                {
                    float dist = cameraPosition.x + cameraBound - pos.x;
                    if (dist > length * (Backgrounds.Length - 1))
                    {
                        pos.x += length * Backgrounds.Length;
                    }
                    else if (dist < -length * (Backgrounds.Length - 1))
                    {
                        pos.x -= length * Backgrounds.Length;
                    }
                }

                Backgrounds[i].transform.position = pos;
            }

        }

    }
}