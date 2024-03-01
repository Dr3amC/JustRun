using Route.Boosters;
using UnityEngine;

namespace Core.Signals
{
    public class ApplyBoosterSignal
    {
        public BoosterType Type { get; private set; }
        public GameObject GameObject { get; private set; }
        
        public ApplyBoosterSignal(BoosterType boosterType, GameObject go)
        {
            Type = boosterType;
            GameObject = go;
        }
    }
}