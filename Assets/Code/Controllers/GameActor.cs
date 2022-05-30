namespace Snowboard.Controllers
{
    public abstract class GameActor
    {

        public bool ReadyToRemove { get; private set; }

        public abstract void Update();

        protected void Remove()
        {
            ReadyToRemove = true;
        }

        public virtual void OnGameStart() { }

        public virtual void OnGameEnd() { }

        public virtual void OnGameReset() { }
    }
}