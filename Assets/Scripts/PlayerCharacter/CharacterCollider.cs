using Core.Signals;
using Route.Boosters;
using UnityEngine;
using Zenject;

namespace PlayerCharacter
{
    // Класс в котором храниться только коллайдер, отвечается за триггеры
    public class CharacterCollider : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        
        private void OnTriggerEnter(Collider c)
        {
            if (c.TryGetComponent<Obstacle>(out _))
            {
                _signalBus.Fire(new ObstacleTriggerSignal());
            }
            else if (c.TryGetComponent<BoosterObject>(out var booster))
            {
                _signalBus.Fire(new ApplyBoosterSignal(booster.BoosterType, c.gameObject));
            }
        }
    }
}
