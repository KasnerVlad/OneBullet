using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Scripts.Pool
{
    public class BulletGiver
    {

        private readonly List<GameObject> activeObjects = new List<GameObject>();
        private readonly List<GameObject> hidenObjects = new List<GameObject>();
        private readonly List<GameObject> cleardObjects = new List<GameObject>();
        private IObjectPool<GameObject> pool;
        private Vector3 pos;
        private Quaternion rot;
        private GameObject _prefab;
        public BulletGiver()
        {
            pool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, maxSize:int.MaxValue, collectionCheck:true);
        }
        private GameObject CreateFunc()
        {
            return GameObject.Instantiate(_prefab, pos, rot);;
        }
        private void ActionOnGet(GameObject go)
        {
            activeObjects.Add(go);
            hidenObjects.Remove(go);
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.SetActive(true);
            Debug.Log("Object Geted");
        }
        private void ActionOnRelease(GameObject go)
        {
            activeObjects.Remove(go);
            hidenObjects.Add(go);
            go.SetActive(false);
            go.transform.position = Vector3.zero;
        }
        private void ActionOnDestroy(GameObject go)
        {
            cleardObjects.Add(go);
            activeObjects.Remove(go);
            hidenObjects.Remove(go);
            go.SetActive(false);
            go.transform.position = Vector3.zero;
        }

        public GameObject GetGameObjectFromPool() {Debug.Log("Obj Geted"); return pool.Get(); }
        public void ReturnGameObjectToPool(GameObject go) {Debug.Log("Obj Released"); pool.Release(go);} 
        public void ClearPool() {Debug.Log("Pool Cleared"); pool.Clear(); }
        public void SetNeededPrefab(GameObject prefab) { _prefab = prefab; }
        public void SetPosition(Vector3 pos) { this.pos = pos; }
        public void SetRotation(Quaternion rot) { this.rot = rot; }
        public List<GameObject> GetActiveObjects() { return new List<GameObject>(activeObjects); }
        
        
    }
}