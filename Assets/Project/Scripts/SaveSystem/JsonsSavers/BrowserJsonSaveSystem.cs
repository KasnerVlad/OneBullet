using System;
using System.IO;
using JsonSave;
using UnityEngine;

namespace Project.Scripts.SaveSystem
{
    public class BrowserJsonSaveSystem : ISaveSystem
    {
        public void Save<T>(T data,string key)
        {
            try
            {
                var json = JsonUtility.ToJson(data);
#if UNITY_WEBGL && !UNITY_EDITOR
                BrowserInterop.SaveToLocalStorage(key, data);
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public T Load<T>(string key)
        {
            try
            {
                string json = " ";
#if UNITY_WEBGL && !UNITY_EDITOR
                json = BrowserInterop.LoadFromLocalStorage(key);
#endif   
                if (string.IsNullOrEmpty(json)) {
                    return Activator.CreateInstance<T>();
                }
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return Activator.CreateInstance<T>();
        }
    }
}