/*using System.Collections.Generic;
using JsonSave;
using UnityEngine;

namespace Project.Scripts.Custom
{
    public class DebugLogger : MonoBehaviour
    {
        public static DebugLogger Instance;
        private LoggerData _loggerData;
        private ISaveSystem _saveSystem;/*
        [SerializeField] private string _logFileName = "log.json";#1#
        [SerializeField] private string _logFilePath/* = Application.persistentDataPath + "/" + "log.json"#1#;

        private void Awake()
        {
            Instance = this;
            _loggerData = new LoggerData();
            _saveSystem = new JsonSaveSystem();
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
}*/