using UnityEngine;
using Zenject;

// Прегада, используется пул объектов
public class Obstacle : MonoBehaviour
{
    public class Pool : MonoMemoryPool<Transform, Obstacle>
    {
        protected override void Reinitialize(Transform parent, Obstacle item)
        {
            base.Reinitialize(parent, item);
            item.transform.SetParent(parent);
        }
    }
}
