using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Route.Boosters;
using UnityEngine;
using Zenject;

namespace Route
{
    // Спавним все тайлы, препятствия, бустеры.
    public class RouteSpawner
    {
        [Inject] private RouteTile.Pool _tilePool;
        [Inject] private Obstacle.Pool _obstaclePool;
        [Inject] private BoosterObject.Pool _boostersPool;
        [Inject] private GameplayData _gameplayData;

        private readonly List<RouteTile> _routeTiles = new();
        private float _currentTilesDistance;
        private int _tilesSpawned;
        private float _lastTimeBoosterSpawned;
        private int _boosterSpawnedCount;

        private const int TotalTilesCount = 10;

        public void InitTiles()
        {
            for (var i = 0; i < TotalTilesCount; i++)
            {
                // Первую преграду ставим на втором тайле
                SpawnNewTile(i, i > 1);
            }
        }

        public void TrySwapTiles(float speed)
        {
            if (_routeTiles.Count == 0)
            {
                return;
            }
            
            _currentTilesDistance += speed;
            
            if (_currentTilesDistance < _routeTiles[0].Length)
            {
                return;
            }
            
            DespawnTile(0);
            SpawnNewTile(_tilesSpawned, true);
            _currentTilesDistance = 0;
        }

        public void ClearAll()
        {
            foreach (var tile in _routeTiles)
            {
                if (tile.Obstacle != null)
                {
                    _obstaclePool.Despawn(tile.Obstacle);
                }
                
                if (tile.Booster != null)
                {
                    _boostersPool.Despawn(tile.Booster);
                }
                
                _tilePool.Despawn(tile);
            }
            
            _obstaclePool.Clear();
            _tilePool.Clear();
            _boostersPool.Clear();
            _routeTiles.Clear();
            _currentTilesDistance = 0;
            _tilesSpawned = 0;
            _boosterSpawnedCount = 0;
        }
        
        public void DespawnBooster(BoosterObject booster)
        {
            _boostersPool.Despawn(booster);
            _boosterSpawnedCount--;

            foreach (var tile in _routeTiles.Where(tile => tile.Booster != null && tile.Booster.GetInstanceID() == booster.GetInstanceID()))
            {
                tile.Booster = null;
            }
        }

        private void SpawnNewTile(int index, bool spawnObstacle)
        {
            var tile = _tilePool.Spawn();
            tile.transform.position = new Vector3(0, 0, -tile.Length * index);
            _routeTiles.Add(tile);
            _tilesSpawned++;

            // Защита от спавна бустеров при первом запуске или рестарте
            if (index > _gameplayData.MinIndexToSpawnBoosters)
            {
                SpawnRandomBooster(tile);
            }
            
            if (spawnObstacle)
            {
                SpawnObstacle(tile);
            }
        }

        private void SpawnObstacle(RouteTile tile)
        {
            var obstacle = _obstaclePool.Spawn(tile.transform);
            var localPosition = obstacle.transform.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y, -tile.GetPositionForObstacle());
            obstacle.transform.localPosition = localPosition;
            tile.Obstacle = obstacle;
        }

        private void SpawnRandomBooster(RouteTile tile)
        {
            if (_boosterSpawnedCount >= _gameplayData.MaxBoostersSpawned)
            {
                return;
            }

            if (_boosterSpawnedCount == 0)
            {
                SpawnBooster();
            }
            else
            {
                if (Time.realtimeSinceStartup - _lastTimeBoosterSpawned > _gameplayData.TimeBetweenBoosters)
                {
                    SpawnBooster();
                }
            }

            void SpawnBooster()
            {
                var booster = _boostersPool.Spawn(tile.transform);
                var localPosition = booster.transform.localPosition;
                localPosition = new Vector3(localPosition.x, localPosition.y, -tile.GetPositionForBooster());
                booster.transform.localPosition = localPosition;
                booster.SetupBooster(GetRandomType());
                tile.Booster = booster;
                
                _lastTimeBoosterSpawned = Time.realtimeSinceStartup;
                _boosterSpawnedCount++;
            }
        }

        private void DespawnTile(int index)
        {
            if (_routeTiles[index].Obstacle != null)
            {
                _obstaclePool.Despawn(_routeTiles[index].Obstacle);
            }

            if (_routeTiles[index].Booster != null)
            {
                _boostersPool.Despawn(_routeTiles[index].Booster);
                _boosterSpawnedCount--;
            }
            
            _tilePool.Despawn(_routeTiles[index]);
            _routeTiles.RemoveAt(index);
        }

        private BoosterType GetRandomType()
        {
            var values = Enum.GetValues(typeof(BoosterType));
            var random = new System.Random();
            return (BoosterType) values.GetValue(random.Next(values.Length));
        }
    }
}