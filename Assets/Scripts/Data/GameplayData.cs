using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameplayData", menuName = "ScriptableObjects/GameplayData", order = 0)]
    public class GameplayData : ScriptableObject
    {
        [Header("Main")]
        [SerializeField] private float _defaultGameSpeed;
        [SerializeField] private Vector3 _defaultCharacterPosition;

        [Header("Boosters")] 
        [SerializeField] private float _flySpeed;
        [SerializeField] private float _flyDuration;
        [SerializeField] private float _speedUpSpeed;
        [SerializeField] private float _speedUpDuration;
        [SerializeField] private float _slowDownSpeed;
        [SerializeField] private float _slowDownDuration;
        [SerializeField] private float _delayToDisableInvincibility;
        [SerializeField] private int _maxBoostersSpawned;
        [SerializeField] private int _minIndexToSpawnBoosters;
        [SerializeField] private float _timeBetweenBoosters;

        public float DefaultGameSpeed => _defaultGameSpeed;
        public Vector3 DefaultCharacterPosition => _defaultCharacterPosition;
        public float FlySpeed => _flySpeed;
        public float FlyDuration => _flyDuration;
        public float SpeedUpSpeed => _speedUpSpeed;
        public float SpeedUpDuration => _speedUpDuration;
        public float SlowDownSpeed => _slowDownSpeed;
        public float SlowDownDuration => _slowDownDuration;
        public float DelayToDisableInvincibility => _delayToDisableInvincibility;
        public int MaxBoostersSpawned => _maxBoostersSpawned;
        public int MinIndexToSpawnBoosters => _minIndexToSpawnBoosters;
        public float TimeBetweenBoosters => _timeBetweenBoosters;
    }
}