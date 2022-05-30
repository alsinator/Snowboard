using UnityEngine;

namespace Snowboard.Controllers.BoarderStates
{
    public class BoarderStateBase : StateBase
    {
        public virtual void OnCollision(Collider2D collider) { }
    }
}