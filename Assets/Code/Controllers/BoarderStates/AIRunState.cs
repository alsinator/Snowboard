using Snowboard.Views;

namespace Snowboard.Controllers.BoarderStates
{
    public class AIRunState : StateBase
    {
        private BoarderController boarder;

        public override void OnEnter()
        {
            boarder = StateMachine as BoarderController;

            boarder.SetAnim(BoarderView.Anims.Run);
            boarder.SetImpulse(BoarderProperties.Speed);
            boarder.ActivateSnowParticles(true);
        }

        public override void OnUpdate()
        {
            boarder.UpdatePhysics();
        }
    }
}