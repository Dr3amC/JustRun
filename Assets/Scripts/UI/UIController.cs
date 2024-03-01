using Core.Signals;
using Route;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    // Простейший UI на кликах и обработчиках событий шины сигналов
    public class UIController : MonoBehaviour
    {
        [SerializeField] private RouteController _routeController;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private GameObject _description;

        [Inject] private SignalBus _signalBus;

        private void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartClick);
            _pauseButton.onClick.AddListener(OnPauseClick);
            _continueButton.onClick.AddListener(OnContinueClick);
            _restartButton.onClick.AddListener(OnRestartClick);
            
            _signalBus.Subscribe<GameStartSignal>(OnGameStarted);
            _signalBus.Subscribe<GameFinishedSignal>(OnGameFinished);
            _signalBus.Subscribe<GamePausedSignal>(OnGamePaused);
            _signalBus.Subscribe<GameContinueSignal>(OnGameContinued);
            _signalBus.Subscribe<GameRestartedSignal>(OnGameRestarted);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            
            _signalBus.Unsubscribe<GameStartSignal>(OnGameStarted);
            _signalBus.Unsubscribe<GameFinishedSignal>(OnGameFinished);
            _signalBus.Unsubscribe<GamePausedSignal>(OnGamePaused);
            _signalBus.Unsubscribe<GameContinueSignal>(OnGameContinued);
            _signalBus.Unsubscribe<GameRestartedSignal>(OnGameRestarted);
        }

        private void OnStartClick()
        {
            _routeController.StartGame();
        }

        private void OnPauseClick()
        {
            _routeController.PauseGame();
        }

        private void OnContinueClick()
        {
            _routeController.ContinueGame();
        }

        private void OnRestartClick()
        {
            _routeController.RestartGame();
        }

        private void OnGameStarted(GameStartSignal _)
        {
            _pauseButton.gameObject.SetActive(true);
            _startButton.gameObject.SetActive(false);
            _description.SetActive(false);
        }

        private void OnGameFinished(GameFinishedSignal _)
        {
            _pauseButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
        }
        
        private void OnGamePaused(GamePausedSignal _)
        {
            _pauseButton.gameObject.SetActive(false);
            _continueButton.gameObject.SetActive(true);
        }

        private void OnGameContinued(GameContinueSignal _)
        {
            _pauseButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(false);
            _continueButton.gameObject.SetActive(false);
        }

        private void OnGameRestarted(GameRestartedSignal _)
        {
            _pauseButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(false);
        }
    }
}
