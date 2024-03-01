using UnityEngine;
using Zenject;

namespace Route.Boosters
{
    // Класс для бустера, который размещается на игровом поле. Используется пул объектов
    public class BoosterObject : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material[] _boosterMaterials;

        public BoosterType BoosterType {get; private set; }

        public void SetupBooster(BoosterType boosterType)
        {
            BoosterType = boosterType;
            _meshRenderer.material = _boosterMaterials[(int) boosterType];
        }

        public class Pool : MonoMemoryPool<Transform, BoosterObject>
        {
            protected override void Reinitialize(Transform parent, BoosterObject item)
            {
                base.Reinitialize(parent, item);
                item.transform.SetParent(parent);
            }
        }
    }
}