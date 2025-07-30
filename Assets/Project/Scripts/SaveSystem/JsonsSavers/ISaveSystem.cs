namespace JsonSave
{
    public interface ISaveSystem
    {
        public void Save<T>(T saveData,string key);
        public T Load<T>(string key);
    }
}