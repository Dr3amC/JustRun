using System;
using Core.Signals;
using Cysharp.Threading.Tasks;
using Data;
using PlayerCharacter;
using Route.Boosters;
using UnityEngine;
using Zenject;

namespace Route
{
    // Основной игровой контроллер, где игра создается трасса, персонаж, обрабатываются основные события игры
    public class RouteController : MonoBehaviour
    {
        [Inject] private GameplayData _gameplayData;
        [Inject] private CharacterSpawner _characterSpawner;
        [Inject] private RouteSpawner _routeSpawner;
        [Inject] private SignalBus _signalBus;
        [Inject] private BoosterFactory _boosterFactory;

        private BoosterController _boosterController;
        private bool _isRunning;
        private float _currentGameSpeed;
        
        public Character Character { get; private set; }

        private void OnEnable()
        {
            _signalBus.Subscribe<ObstacleTriggerSignal>(OnObstacleTrigger);
            _signalBus.Subscribe<ApplyBoosterSignal>(OnApplyBooster);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<ObstacleTriggerSignal>(OnObstacleTrigger);
            _signalBus.Unsubscribe<ApplyBoosterSignal>(OnApplyBooster);
        }

        private void Start()
        {
            _boosterController = new BoosterController(this, _gameplayData);
        }

        public void StartGame()
        {
            _signalBus.Fire<GameStartSignal>();
            InitRoute();
            InitCharacter();
            StartRun()
                .SuppressCancellationThrow()
                .Forget();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            _signalBus.Fire<GamePausedSignal>();
        }

        public void ContinueGame()
        {
            Time.timeScale = 1;
            _signalBus.Fire<GameContinueSignal>();
        }

        public void RestartGame()
        {
            _signalBus.Fire<GameRestartedSignal>();
            
            Time.timeScale = 1;
            StartRun()
                .SuppressCancellationThrow()
                .Forget();
        }
        
        public void UpdateGameSpeed(float newSpeed = -1f)
        {
            if (newSpeed < 0)
            {
                newSpeed = _gameplayData.DefaultGameSpeed;
            }
            
            Character.CharacterMovementController.UpdateAnimationSpeed(newSpeed);
            _currentGameSpeed = newSpeed;
        }
        
        private void InitRoute()
        {
            _currentGameSpeed = _gameplayData.DefaultGameSpeed;
            _routeSpawner.InitTiles();
        }
        
        private void InitCharacter()
        {
            Character = _characterSpawner.CreateCharacter();
            Character.transform.position = _gameplayData.DefaultCharacterPosition;
            Character.transform.SetParent(transform);
        }

        private async UniTask StartRun()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            UpdateGameSpeed(_currentGameSpeed);
            Character.CharacterMovementController.StartRunning(_currentGameSpeed);
            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }
            
            var speed = _currentGameSpeed * Time.deltaTime;
            _routeSpawner.TrySwapTiles(speed);
        }
        
        private void OnObstacleTrigger(ObstacleTriggerSignal signal)
        {
            _isRunning = false;
            ResetRoute();
            ResetPlayer();
            _signalBus.Fire<GameFinishedSignal>();
        }

        private void ResetRoute()
        {
            _currentGameSpeed = _gameplayData.DefaultGameSpeed;
            _routeSpawner.ClearAll();
            InitRoute();
        }

        private void ResetPlayer()
        {
            Character.transform.position = _gameplayData.DefaultCharacterPosition;
        }

        private void OnApplyBooster(ApplyBoosterSignal boosterSignal)
        {
            _boosterFactory.Create(boosterSignal.Type, _boosterController).ApplyBooster();
            _routeSpawner.DespawnBooster(boosterSignal.GameObject.GetComponent<BoosterObject>());
        }
    }
}
