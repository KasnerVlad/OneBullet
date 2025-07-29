using JsonSave;
using UnityEngine;

namespace Project.Scripts.SaveSystem.SaveSystemLogic
{
    public class SaveManager : MonoBehaviour
    {
        private ISaveSystem _saveSystem;
        private SaveSystemPresenter _saveSystemPresenter;
        private GameData _gameData;
        [SerializeField] private bool _createNullData;
        private string _key;
        private readonly string _name = "aboba.json";
        private readonly string _browserKey = "aboba";
        public static SaveManager Instance { get; private set; }
        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else 
                Destroy(gameObject);
            #if UNITY_WEBGL && !UNITY_EDITOR
            _saveSystem = new BrowserJsonSaveSystem();
            _key = browserKey; 
            #elif UNITY_EDITOR || UNITY_STANDALONE_WIN
            _saveSystem = new PCJsonSaveSystem();
            _key = Application.persistentDataPath + "/" + _name;
            #endif
        }

        public void AddMoney(int addMoney)
        {
            _saveSystemPresenter.UpdateAndSaveMoney(addMoney);
            
            _gameData = _saveSystemPresenter.GameData;
        }
        public void UnlockLevel(int unlockLevel)
        {
            _saveSystemPresenter.UpdateAndSaveLevel(unlockLevel);
            
            _gameData = _saveSystemPresenter.GameData;
        }

        public int GetUnlockedLevels()
        {
            _gameData = _saveSystemPresenter.GameData;
            return _gameData.UnlokedLevels;
        }

        public int GetMoneys()
        {
            _gameData = _saveSystemPresenter.GameData;
            return _gameData.Moneys;
        }
        private void Start()
        {
            StartGameData();
            _saveSystemPresenter = new SaveSystemPresenter(_gameData, _saveSystem,
                _key
            );
        }
        
        private void OnDestroy()
        {
            _saveSystemPresenter.Save();
        }
        private void StartGameData()
        {
            _gameData = _saveSystem.Load<GameData>(_key);
            if (_gameData==null||_createNullData)
            {
                _gameData = new GameData()
                {
                    Moneys = 0,
                    UnlokedLevels = 1
                };
                _saveSystem.Save(_gameData, _key);
            }
        }
    }
}