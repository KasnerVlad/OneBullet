using JsonSave;

namespace Project.Scripts.SaveSystem.SaveSystemLogic
{
    public class SaveSystemPresenter
    {
        private GameData _gameData;
        public GameData GameData => _gameData;
        private readonly ISaveSystem _saveSystem;
        private readonly string _key;
        private readonly SaveSystemView _saveSystemView;
        public SaveSystemPresenter(GameData gameData, ISaveSystem saveSystem, string key)
        {
            _saveSystemView = new SaveSystemView();
            _gameData = gameData;
            _saveSystem = saveSystem;
            _key = key;
        }
        public void UpdateAndSaveMoney(int money)
        {
            _gameData.Moneys = money;
            _saveSystemView.View(_gameData);
            _saveSystem.Save(_gameData, _key);
        }
        public void UpdateAndSaveLevel(int unlockedLevel)
        {
            _gameData.UnlokedLevels = unlockedLevel;
            _saveSystemView.View(_gameData);
            _saveSystem.Save(_gameData, _key);
        }
        
        public void SaveAll(GameData gameData) { _saveSystem.Save(gameData, _key); }
        
        public void Load()
        {
            _gameData = _saveSystem.Load<GameData>(_key);
            _saveSystemView.View(_gameData);
        }
        public void Save()=>_saveSystem.Save(_gameData, _key);
    }
}