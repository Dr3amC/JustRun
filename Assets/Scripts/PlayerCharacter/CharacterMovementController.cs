using System;
using System.Threading;
using Core.Signals;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace PlayerCharacter
{
    // Контроллер передвижения, голый c#, чтобы не использовать монобех, внедрение зависимостей через конструктор
    // Отвечается за анимации персонажа, скорость движения, реагирует на инпуты
    public class CharacterMovementController : IDisposable
    {
        private const float LaneOffset = -2.25f;
        private const float LaneChangeTime = 0.3f;
        private const float JumpDuration = 0.8f;
        
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsFlying = Animator.StringToHash("IsFlying");
        
        private readonly Animator _animator;
        private readonly Transform _transform;
        private readonly BoxCollider _boxCollider;
        private readonly SignalBus _signalBus;
        private readonly GameplayData _gameplayData;
        
        private float _currentSpeed;
        private CancellationTokenSource _cts;
        private int _currentLane = 1;
        private bool _isCharacterRunning;
        private Tween _currentTween;
        private bool _isCharacterFlying;
        
        public CharacterMovementController(
            Animator animator, 
            Transform transform, 
            BoxCollider boxCollider, 
            GameplayData gameplayData, 
            SignalBus signalBus)
        {
            _animator = animator;
            _transform = transform;
            _boxCollider = boxCollider;
            _gameplayData = gameplayData;
            _signalBus = signalBus;
            _signalBus.Subscribe<ObstacleTriggerSignal>(OnTriggerSignal);
        }
        
        public void StartRunning(float speed)
        {
            if (_isCharacterRunning)
            {
                return;
            }
            
            _currentSpeed = speed;
            _isCharacterRunning = true;
            _animator.SetBool(IsRunning, true);
            
            MoveCharacter(speed)
                .SuppressCancellationThrow()
                .Forget();
        }
        
        public void ChangeLane(int direction)
        {
            if (!_isCharacterRunning)
            {
                return;
            }
            
            var targetLane = _currentLane + direction;

            if (targetLane < 0 || targetLane > 2)
            {
                return;
            }
            
            _currentTween = _transform.DOLocalMoveX((targetLane - 1) * LaneOffset, LaneChangeTime);
            _currentLane = targetLane;
        }
        
        public void Jump()
        {
            if (!_isCharacterRunning || _isCharacterFlying)
            {
                return;
            }
            
            var newPosition = _transform.localPosition + new Vector3(0, 0, -_currentSpeed);
            _currentTween = _transform.DOLocalJump(newPosition, 1.5f, 1, JumpDuration)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    _animator.SetBool(IsJumping, true);
                    StopRunning();
                })
                .OnComplete(() =>
                {
                    _animator.SetBool(IsJumping, false);
                    StartRunning(_currentSpeed);
                })
                .OnKill(() =>
                {
                    _animator.SetBool(IsJumping, false);
                });
        }

        public void StartFlying()
        {
            _isCharacterFlying = true;
            _transform.DOLocalMoveY(5f, 1f)
                .OnStart(() => _animator.SetBool(IsFlying, true));
        }

        public void StopFlying()
        {
            _transform.DOLocalMoveY(_gameplayData.DefaultCharacterPosition.y, 1f)
                .OnComplete(() =>
                {
                    _animator.SetBool(IsFlying, false);
                    _isCharacterFlying = false;
                });
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _signalBus.Unsubscribe<ObstacleTriggerSignal>(OnTriggerSignal);
        }

        public void UpdateAnimationSpeed(float speed)
        {
            _animator.speed = speed / _gameplayData.DefaultGameSpeed;
            _currentSpeed = speed;
            
            MoveCharacter(speed)
                .SuppressCancellationThrow()
                .Forget();
        }

        public async UniTask MakeCharacterInvincible(bool isInvincible, float delay = 0)
        {
            if (delay > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay))
                    .SuppressCancellationThrow();
            }
            
            _boxCollider.enabled = !isInvincible;
        }
        
        private async UniTask MoveCharacter(float speed)
        {
            RenewCts();
            
            while (!_cts.IsCancellationRequested)
            {
                _transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
                await UniTask.WaitForFixedUpdate(_cts.Token);
            }
        }
        
        private void StopRunning()
        {
            _isCharacterRunning = false;
            _cts?.Cancel();
            _animator.SetBool(IsRunning, false);
        }
        
        private void RenewCts()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }

        private void KillCurrentTween()
        {
            _currentTween?.Kill();
            _currentTween = null;
        }

        private void OnTriggerSignal(ObstacleTriggerSignal signal)
        {
            KillCurrentTween();
            StopRunning();
            _currentLane = 1;
        }
    }
}