
using Snowboard.Controllers.BoarderStates;
using Snowboard.Views;
using UnityEngine;

namespace Snowboard.Controllers
{
    public class BoarderController : StateMachine
    {

        public bool AiControlled;

        public Vector3 Position;

        public Vector3 Direction;

        public Vector3 Ground;

        public float Angle;

        public bool OnGround;

        public bool Jumping;


        private BoarderView view;

        private MountainController mountain;

        private float speedMultiplier;

        private float currentSpeed;

        private float impulse;

        private float boostTimer;



        public BoarderController(bool aiControlled, float speedMultiplier, BoarderView view, MountainController mountain)
        {
            AiControlled = aiControlled;
            this.speedMultiplier = speedMultiplier;
            this.view = view;
            this.mountain = mountain;

            Direction = Vector3.right;

            view.OnCollision += OnCollision;

            if (aiControlled)
            {
                InitAi();
            }

            ChangeState(new IdleState());
        }

        private void InitAi()
        {
            Position.x = Random.Range(1f, 4f);
            view.GetComponent<BoxCollider2D>().enabled = false;
        }

        public override void Update()
        {
            base.Update();

            // Update view positions
            view.transform.position = Position;
            view.transform.localEulerAngles = new Vector3(0f, 0f, Angle);

            if (!AiControlled)
            {
                mountain.UpdatePlayerPosition(Position);
            }
        }

        public void UpdatePhysics()
        {
            Position += currentSpeed * Time.deltaTime * Direction;
            Direction += Vector3.down * (BoarderProperties.Gravity * Time.deltaTime);

            Ground = mountain.GetGroundAt(Position);
            if (Ground == Vector3.zero)
            {
                // Invalid ground, probably outside of bounds
                Ground = Position + Vector3.down;
                Direction = Vector3.right;
                Angle = 0f;

                return;
            }

            var frontPos = mountain.GetGroundAt(Position + Vector3.right * 0.1f);

            bool nowOnGround = Ground.y >= Position.y - 0.05f && !Jumping;

            if (nowOnGround && !OnGround)
            {
                currentSpeed = Direction.magnitude;
            }

            OnGround = nowOnGround;

            if (OnGround)
            {
                Position = Ground;
                Direction = (frontPos - Ground).normalized;
                Angle = Vector3.Angle(Direction, Vector3.right);
                if (Direction.y < 0f)
                {
                    Angle *= -1;
                }
                UpdateSpeed();
            }
        }

        private void UpdateSpeed()
        {

            var speed = impulse * (1f - Direction.y * 2f);

            if (boostTimer > Time.time)
            {
                speed *= BoarderProperties.BoostImpulse;
            }

            if (currentSpeed < speed)
            {
                currentSpeed = speed;
            }
            else
            {
                var friction = impulse == 0 ? BoarderProperties.FrictionStop : BoarderProperties.FrictionRun;
                currentSpeed = Mathf.MoveTowards(currentSpeed, speed, friction * Time.deltaTime);
            }

            if (impulse > 0f)
            {
                currentSpeed = Mathf.Clamp(currentSpeed, impulse * 0.75f, impulse * 10f);
            }
        }


        public void PlaceAtGround()
        {
            Position = mountain.GetGroundAt(Position);

            var frontPos = mountain.GetGroundAt(Position + Vector3.right * 0.1f);

            Angle = -Vector3.Angle((frontPos - Position).normalized, Vector3.right);

            Ground = Position;

            OnGround = true;
        }

        public void SetImpulse(float force)
        {
            impulse = force * speedMultiplier;
        }

        public void ActivateBoost()
        {
            boostTimer = boostTimer < Time.time ? Time.time + BoarderProperties.BoostTime : boostTimer + BoarderProperties.BoostTime;
        }

        public void OnCollision(Collider2D collider)
        {
            if (AiControlled || boostTimer > Time.time + 2f)
            {
                // Inmune if just boosted
                return;
            }

            if (currentState is BoarderStateBase state)
            {
                state.OnCollision(collider);
            }
        }

        public override void OnGameStart()
        {
            if (AiControlled)
            {
                ChangeState(new AIRunState());
            }
            else
            {
                ChangeState(new RunState());
            }
        }

        public override void OnGameEnd()
        {
            SetImpulse(0);
        }

        public override void OnGameReset()
        {
            Position = Vector3.zero;
            Direction = Vector3.right;
            currentSpeed = 0f;

            if (AiControlled)
            {
                InitAi();
            }
            ChangeState(new IdleState());
        }

        public void SetAnim(BoarderView.Anims animation)
        {
            view.SetAnim(animation);
        }

        public void ActivateSnowParticles(bool activate)
        {
            if (!AiControlled)
            {
                view.ActivateSnowParticles(activate);
            }
        }
    }
}