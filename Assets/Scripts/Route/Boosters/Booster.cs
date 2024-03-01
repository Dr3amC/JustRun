using System;
using Cysharp.Threading.Tasks;

namespace Route.Boosters
{
    // Класс бустера, который создается в абстрактной фабрике, чтобы применить его в дальнейшем
    public class Booster : IBooster
    {
        private readonly Func<UniTask> _func;
        
        public Booster(Func<UniTask> func)
        {
            _func = func;
        }
        
        public void ApplyBooster()
        {
            _func()
                .SuppressCancellationThrow()
                .Forget();
        }
    }
}