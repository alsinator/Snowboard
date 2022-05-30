using UnityEngine;

namespace Snowboard.Controllers
{
    public class CameraController : GameActor
    {

        private readonly Camera mainCamera;

        private readonly BoarderController boarder;

        private const float cameraHeight = 0.3f;
        private const float cameraForward = 3.5f;
        private const float cameraSmoothing = 8f;
        private const float cameraWindow = 2f;


        private bool smoothing;


        public CameraController(Camera mainCamera, BoarderController boarder)
        {
            this.mainCamera = mainCamera;
            this.boarder = boarder;
        }



        public override void Update()
        {

            var pos = boarder.Ground;
            if (!boarder.OnGround)
            {
                float diff = boarder.Position.y - pos.y;
                if (diff > cameraWindow)
                {
                    pos.y += diff - cameraWindow;
                }
            }

            var target = pos + Vector3.right * cameraForward;
            target += Vector3.up * cameraHeight;
            target.z = -10f;

            if (smoothing)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target, cameraSmoothing * Time.deltaTime);
            }
            else
            {
                mainCamera.transform.position = target;
            }
        }

        public override void OnGameStart()
        {
            smoothing = true;
        }

        public override void OnGameReset()
        {
            smoothing = false;
        }

    }
}