using Snowboard.Views;
using UnityEngine;

namespace Snowboard.Controllers.BoarderStates
{
    public class FallState : BoarderStateBase
    {
        private BoarderController boarder;

        private float timer;

        private bool canContinue;

        public override void OnEnter()
        {
            boarder = StateMachine as BoarderController;

            boarder.SetAnim(BoarderView.Anims.Fall);
            boarder.SetImpulse(0f);
            boarder.ActivateSnowParticles(true);
            boarder.Jumping = false;

            timer = Time.time + 4f;
        }

        public override void OnUpdate()
        {
            boarder.UpdatePhysics();

            if (timer < Time.time+2f && !canContinue)
            {
                canContinue = true;
                boarder.ActivateSnowParticles(false);
            }


            if (timer < Time.time && canContinue)
            {
                boarder.ChangeState(new RunState());
            }
        }
    }
}