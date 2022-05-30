using UnityEngine;

namespace Snowboard
{
    public class PoolObject : MonoBehaviour
    {
        public string Id;

        public Transform PoolParent { get; set; }
        public bool Pooled { get; set; }

        public void Despawn()
        {
            Pooled = true;
            gameObject.SetActive(false);
            transform.SetParent(PoolParent);
        }

    }
}