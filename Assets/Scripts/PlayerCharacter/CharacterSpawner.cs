namespace PlayerCharacter
{
    // Создание персонажа, фабрика используется для того, чтобы в будущем можно было добавлять еще персонажей
    public class CharacterSpawner
    {
        private readonly Character.Factory _characterFactory;
        
        public CharacterSpawner(Character.Factory characterFactory)
        {
            _characterFactory = characterFactory;
        }

        public Character CreateCharacter()
        {
            return _characterFactory.Create();
        }
    }
}
