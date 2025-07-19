using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Enemy
{
    public class AllEnemyController: MonoBehaviour
    {
        [SerializeField] private List<Enemy> enemies;
        [SerializeField] private UnityEvent onWin;
        [SerializeField] private UnityEvent onLoose;
        public static AllEnemyController Instance;

        private void Awake()
        {
            Instance = this;
        }
        public void CheckWin()
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    onLoose.Invoke();
                } 
            }    
            onWin.Invoke();
        }

        public List<Enemy> GetEnemies()
        {
            return enemies;
        }
    }
}