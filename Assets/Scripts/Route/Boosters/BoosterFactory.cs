using System;
using Zenject;

namespace Route.Boosters
{
    public class BoosterFactory : PlaceholderFactory<BoosterType, BoosterController, IBooster>
    {
        
    }
    
    public class CustomBoosterFactory : IFactory<BoosterType, BoosterController, IBooster>
    {
        // Создаем бустеры, инъекция через конструктор, прокидываем метод
        public IBooster Create(BoosterType type, BoosterController boosterController)
        {
            return type switch
            {
                BoosterType.Fly => new Booster(boosterController.Fly),
                BoosterType.SpeedUp => new Booster(boosterController.SpeedUp),
                BoosterType.Torque => new Booster(boosterController.SlowDown),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Wrong booster type")
            };
        }
    }
}