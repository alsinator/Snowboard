using Snowboard.Views;
using UnityEngine;

namespace Snowboard.Controllers.BoarderStates
{
    public class JumpState : BoarderStateBase
    {
        private BoarderController boarder;

        private float accumulatedAngle;

        public override void OnEnter()
        {
            boarder = StateMachine as BoarderController;
            boarder.SetAnim(BoarderView.Anims.Jump);

            boarder.Direction += Vector3.up * BoarderProperties.JumpImpulse;
            boarder.Jumping = true;

            boarder.ActivateSnowParticles(false);
        }

        public override void OnUpdate()
        {

            if (Input.GetButton("Fire1"))
            {
                boarder.Angle += 180f * Time.deltaTime;
                accumulatedAngle += 180f * Time.deltaTime;
                boarder.SetAnim(BoarderView.Anims.Grab);
            }
            else
            {
                boarder.SetAnim(BoarderView.Anims.Jump);
            }

            if (boarder.Direction.y < 0f)
            {
                boarder.Jumping = false;
            }

            float angle = boarder.Angle % 360f;
            if (angle > 180f)
            {
                angle -= 360f;
            }

            boarder.UpdatePhysics();

            if (boarder.OnGround)
            {
                float diff = Mathf.Abs(boarder.Angle - angle);
                if (diff < BoarderProperties.FallTolerance)
                {
                    if (accumulatedAngle > 330f)
                    {
                        boarder.ActivateBoost();
                    }

                    boarder.ChangeState(new RunState());
                }
                else
                {
                    boarder.ChangeState(new FallState());
                }
            }
        }

        public override void OnCollision(Collider2D collider)
        {
            boarder.ChangeState(new FallState());
        }
    }
}