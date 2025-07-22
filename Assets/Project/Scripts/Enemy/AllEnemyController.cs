using System;
using System.Collections.Generic;
using Project.Scripts.Character.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Enemy
{
    public class AllEnemyController: MonoBehaviour
    {
        [SerializeField] private List<Enemy> enemies;
        private readonly List<EnemyManager> _enemyManagers=new List<EnemyManager>();
        [SerializeField] private UnityEvent onWin;
        [SerializeField] private UnityEvent onLoose;
        public static AllEnemyController Instance;
        [SerializeField] private Camera MainCamera;
        public GameObject PlayerControllerEnemy=>GetPlayerControl();
        private void Awake()
        {
            Instance = this;
            foreach (var enemy in enemies)
            {
                EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
                if(enemyManager!=null)
                    _enemyManagers.Add( enemyManager);
            }
        }

        public Camera GetMainCamera()
        {
            return MainCamera;
        }
        public GameObject GetPlayerControl()
        {
            foreach (var enemyManager in _enemyManagers)
            {
                if (enemyManager.IsPlayerControl)
                {
                    return enemyManager.gameObject;
                }
            }
            return null;
        }

        public void SetEnemyManager(EnemyManager enemyManager)
        {
            if (_enemyManagers.Contains(enemyManager))
            {
                foreach (var _enemyManager in _enemyManagers)
                {
                    if (_enemyManager.IsPlayerControl)
                    {
                        _enemyManager.SetPlayerControl(false);
                    }
                }
                enemyManager.SetPlayerControl(true);
            }
        }
        public void CheckWin()
        {
            int enemyCount = 0;
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    enemyCount++;
                    Debug.Log("Count: " + enemyCount);
                } 
            }

            if (enemyCount <= 1)
            {
                Debug.Log("All Enemies Win");
                onWin.Invoke();
            }
                /*
            else 
                onLoose.Invoke();*/
        }

        public List<Enemy> GetEnemies()
        {
            return enemies;
        }
    }
}