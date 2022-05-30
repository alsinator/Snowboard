using UnityEngine;
using System;

namespace Snowboard.Views
{
    public class BoarderView : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private ParticleSystem particles;

        public Material boarderSkin
        {
            set
            {
                sprite.material = value;
            }
        }


        public Action<Collider2D> OnCollision;


        public enum Anims
        {
            Run,
            Jump,
            Grab,
            Fall
        }


        public void SetAnim(Anims animation)
        {
            animator.SetTrigger(animation.ToString());
        }

        public void ActivateSnowParticles(bool activate)
        {
            if (activate)
            {
                particles.Play();
            }
            else
            {
                particles.Stop();
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            OnCollision?.Invoke(collision);
        }

    }
}