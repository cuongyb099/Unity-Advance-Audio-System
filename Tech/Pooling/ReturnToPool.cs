using UnityEngine;

namespace Tech.Pooling
{
    public class ReturnToPool : MonoBehaviour
    {
        public Pools PoolsObjects;

        public void OnDisable()
        {
            PoolsObjects.AddToPool(gameObject);
        }
    }
}