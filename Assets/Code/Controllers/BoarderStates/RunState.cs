using Snowboard.Views;
using UnityEngine;

namespace Snowboard.Controllers.BoarderStates
{
    public class RunState : BoarderStateBase
    {
        private BoarderController boarder;

        private float jumpDelay;

        public override void OnEnter()
        {
            boarder = StateMachine as BoarderController;

            boarder.SetAnim(BoarderView.Anims.Run);
            boarder.SetImpulse(BoarderProperties.Speed);
            boarder.ActivateSnowParticles(true);
            jumpDelay = Time.time + 0.25f;
        }

        public override void OnUpdate()
        {
            boarder.UpdatePhysics();

            if (jumpDelay < Time.time && Input.GetButtonDown("Fire1"))
            {
                boarder.ChangeState(new JumpState());
            }
        }

        public override void OnCollision(Collider2D collider)
        {
            boarder.ChangeState(new FallState());
        }
    }
}