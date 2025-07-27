using System;
using System.IO;
using UnityEngine;
namespace JsonSave
{
    public class JsonSaveSystem : ISaveSystem
    {
        public void Save<T>(T saveData, string path) {
            try
            {
                var json = JsonUtility.ToJson(saveData);
                using (var writer = new StreamWriter(path)) { writer.Write(json); }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public T Load<T>(string path) {
            try
            {
                if (File.Exists(path)) {
                    string json = "";
                    using (var reader = new StreamReader(path))
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
    public interface ISaveSystem
    {
        public void Save<T>(T saveData,string path);
        public T Load<T>(string path);
    }
}
