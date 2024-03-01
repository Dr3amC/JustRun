using Route.Boosters;
using UnityEngine;
using Zenject;

namespace Route
{
    public class RouteTile : MonoBehaviour
    {
        [SerializeField] private BoxCollider _boxCollider;

        public float Length => _boxCollider.size.x;
        
        public Obstacle Obstacle { get; set; }
        
        public BoosterObject Booster { get; set; }

        // Используется только четверть длины тайла, чтобы помеха оказалсь где-то посередине тайла,
        //  но с небольшим смещением
        public float GetPositionForObstacle()
        {
            var quarter = Length / 4;
            return Random.Range(-quarter, quarter);
        }

        // Бустер размещаем ближе к концу тайла
        public float GetPositionForBooster() => Length / 2 - 1f;
        
        public class Pool : MonoMemoryPool<RouteTile>
        {
            private readonly Transform _parent;
            
            public Pool(Transform parent)
            {
                _parent = parent;
            }
            
            protected override void Reinitialize(RouteTile item)
            {
                base.Reinitialize(item);
                item.transform.SetParent(_parent);
            }

            protected override void OnDespawned(RouteTile item)
            {
                base.OnDespawned(item);
                item.Booster = null;
                item.Obstacle = null;
            }
        }
    }
}
