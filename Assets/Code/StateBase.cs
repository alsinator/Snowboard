namespace Snowboard
{
    public class StateBase
    {

        public StateMachine StateMachine { protected get; set; }
        public bool IsFinished { get; protected set; }

        public virtual void OnEnter()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnExit()
        {

        }
    }
}