using System;
using System.IO;
using Project.Scripts.SaveSystem;
using UnityEngine;
namespace JsonSave
{
    public class PCJsonSaveSystem : ISaveSystem
    {
        public void Save<T>(T saveData, string key) {
            try
            {
                var json = JsonUtility.ToJson(saveData);
                using (var writer = new StreamWriter(key)) { writer.Write(json); }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public T Load<T>(string key) {
            try
            {
                if (File.Exists(key)) {
                    string json = "";
                    using (var reader = new StreamReader(key))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null) { json += line; }
                    }
                    if (string.IsNullOrEmpty(json)) {
                        return Activator.CreateInstance<T>();
                    }
                    return JsonUtility.FromJson<T>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return Activator.CreateInstance<T>();
        }
    }

}
