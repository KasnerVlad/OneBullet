using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Project.Scripts.SaveSystem
{
#if UNITY_WEBGL && !UNITY_EDITOR
    public static class BrowserInterop
    {
        

        [DllImport("__Internal")]
        public static extern void SaveToLocalStorage(string key, string data);
        
        [DllImport("__Internal")]
        public static extern string LoadFromLocalStorage(string key);

    }
#endif
}