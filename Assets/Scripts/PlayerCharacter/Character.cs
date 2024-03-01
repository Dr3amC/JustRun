using Data;
using UnityEngine;
using Zenject;

namespace PlayerCharacter
{
    // Класс персонажа, который хранит в себе ссылку на контроллер передвижения и пробрасывается в нее зависимости
    public class Character : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private BoxCollider _boxCollider;
        
        [Inject] private Camera _camera;
        [Inject] private SignalBus _signalBus;
        [Inject] private GameplayData _gameplayData;
        
        public CharacterMovementController CharacterMovementController { get; private set; }

        private void Awake()
        {
            CharacterMovementController = new CharacterMovementController(_animator, transform, _boxCollider, _gameplayData, _signalBus);
            _camera.transform.SetParent(transform, true);
            
        }
        
        private void OnDestroy()
        {
            CharacterMovementController.Dispose();
        }

        public class Factory : PlaceholderFactory<Character>
        {
            
        }
    }
}