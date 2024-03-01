namespace Route.Boosters
{
    public enum BoosterType
    {
        Fly,
        SpeedUp,
        Torque
    }
    
    public interface IBooster
    {
        public void ApplyBooster();
    }
}
