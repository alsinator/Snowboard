using Snowboard.Views;

namespace Snowboard.Controllers.BoarderStates
{
    public class IdleState : BoarderStateBase
    {

        private BoarderController boarder;

        public override void OnEnter()
        {
            boarder = StateMachine as BoarderController;
            boarder.SetAnim(BoarderView.Anims.Run);
            boarder.SetImpulse(0f);
            boarder.ActivateSnowParticles(false);
        }

        public override void OnUpdate()
        {
            boarder.PlaceAtGround();
        }
    }
}