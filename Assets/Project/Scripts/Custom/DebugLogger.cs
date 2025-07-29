using System.Collections.Generic;
using JsonSave;
using Project.Scripts.SaveSystem;
using UnityEngine;

namespace Project.Scripts.Custom
{
    public class DebugLogger : MonoBehaviour
    {
        public static DebugLogger Instance;
        private LoggerData _loggerData;
        private ISaveSystem _saveSystem;/*
        [SerializeField] private string _logFileName = "log.json";*/
        [SerializeField] private string _logFilePath/* = Application.persistentDataPath + "/" + "log.json"*/;

        private void Awake()
        {
            Instance = this;
            _loggerData = new LoggerData();
            _saveSystem = new PCJsonSaveSystem();
        }
        public void WriteLog(float time, List<Vector3> points)
        {
            Debug.Log("added");
            _loggerData.Add(time, points);
        }


        private void OnDestroy()
        {
            _saveSystem.Save(_loggerData, _logFilePath);
            Debug.Log(JsonUtility.ToJson(_loggerData));
        }
    }
}