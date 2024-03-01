using Core.Signals;
using Data;
using PlayerCharacter;
using Route;
using Route.Boosters;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private Character _characterPrefab;
        [SerializeField] private RouteTile _tilePrefab;
        [SerializeField] private Transform _tileParent;
        [SerializeField] private Obstacle _obstaclePrefab;
        [SerializeField] private BoosterObject _boosterPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<CharacterSpawner>()
                .AsSingle();
            Container.BindFactory<Character, Character.Factory>()
                .FromComponentInNewPrefab(_characterPrefab);
            
            Container.Bind<RouteSpawner>()
                .AsSingle();
            Container.BindMemoryPool<RouteTile, RouteTile.Pool>()
                .WithFactoryArguments(_tileParent)
                .FromComponentInNewPrefab(_tilePrefab);
            Container.BindMemoryPool<Obstacle, Obstacle.Pool>()
                .FromComponentInNewPrefab(_obstaclePrefab);
            Container.BindMemoryPool<BoosterObject, BoosterObject.Pool>()
                .FromComponentInNewPrefab(_boosterPrefab);
            Container.BindFactory<BoosterType, BoosterController, IBooster, BoosterFactory>()
                .FromFactory<CustomBoosterFactory>();
            
            Container.Bind<Camera>()
                .FromInstance(Camera.main)
                .AsSingle();
            
            Container.Bind<GameplayData>()
                .FromScriptableObjectResource("GameData")
                .AsSingle();

            DeclareSignals();
        }

        private void DeclareSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<ObstacleTriggerSignal>();
            Container.DeclareSignal<ApplyBoosterSignal>();
            Container.DeclareSignal<GameStartSignal>();
            Container.DeclareSignal<GameFinishedSignal>();
            Container.DeclareSignal<GamePausedSignal>();
            Container.DeclareSignal<GameContinueSignal>();
            Container.DeclareSignal<GameRestartedSignal>();
        }
    }
}