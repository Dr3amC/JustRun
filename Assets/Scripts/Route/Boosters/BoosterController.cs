using System;
using Cysharp.Threading.Tasks;
using Data;

namespace Route.Boosters
{
    // Здесь собраны методы активации бустеров, можно дробить еще сильнее
    public class BoosterController
    {
        private readonly RouteController _routeController;
        private readonly GameplayData _gameplayData;

        public BoosterController(RouteController routeController, GameplayData gameplayData)
        {
            _routeController = routeController;
            _gameplayData = gameplayData;
        }
        
        public async UniTask Fly()
        {
            _routeController.Character.CharacterMovementController.MakeCharacterInvincible(true).Forget();
            _routeController.UpdateGameSpeed(_gameplayData.FlySpeed);
            _routeController.Character.CharacterMovementController.StartFlying();
            await UniTask.Delay(TimeSpan.FromSeconds(_gameplayData.FlyDuration));
            _routeController.Character.CharacterMovementController.StopFlying();
            _routeController.UpdateGameSpeed();
            _routeController.Character.CharacterMovementController.MakeCharacterInvincible(false, _gameplayData.DelayToDisableInvincibility).Forget();
        }

        public async UniTask SpeedUp()
        {
            _routeController.Character.CharacterMovementController.MakeCharacterInvincible(true).Forget();
            _routeController.UpdateGameSpeed(_gameplayData.SpeedUpSpeed);
            await UniTask.Delay(TimeSpan.FromSeconds(_gameplayData.SpeedUpDuration));
            _routeController.UpdateGameSpeed();
            _routeController.Character.CharacterMovementController.MakeCharacterInvincible(false, _gameplayData.DelayToDisableInvincibility).Forget();
        }

        public async UniTask SlowDown()
        {
            _routeController.UpdateGameSpeed(_gameplayData.SlowDownSpeed);
            await UniTask.Delay(TimeSpan.FromSeconds(_gameplayData.SlowDownDuration));
            _routeController.UpdateGameSpeed();
        }
    }
}