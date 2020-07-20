using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Helpers.SceneManagement;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Singleton

        private static LevelManager _levelManagerInstance;

        public static LevelManager Get
        {
            get
            {
                if (_levelManagerInstance != null)
                {
                    return _levelManagerInstance;
                }
                
                Debug.LogError($"Object of type {typeof(LevelManager)} was called to early");
                return null;
            }
        }

        #endregion
        
        /// <summary>
        /// Player transform that is accessible to enemies.
        /// </summary>
        [HideInInspector] public Transform PlayerTransform;

        /// <summary>
        /// Enemy count that is alive in current scene
        /// </summary>
        private readonly List<Enemy> _enemies = new List<Enemy>();
        
        
        
        private void Awake()
        {
            // Assign the reference to singleton instance.
            _levelManagerInstance = this;
        }

        public void RegisterNewEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
            
            // Update UI.
            LevelUiManager.Get.IncreaseEnemyCount();
        }

        public void RegisterEnemyDeath(Enemy enemy)
        {
            _enemies.Remove(enemy);
            NotifyEnemies();
            
            // Update UI.
            LevelUiManager.Get.IncreaseKilledEnemyCount();
            
            if (_enemies.Count <= 0)
            {
                CustomSceneManager.Get.NextLevel();
            }
        }
        
        /// <summary>
        /// Simulates slow motion effect. 
        /// </summary>
        public void LevelLost()
        {
            StartCoroutine(SwitchLevel());
            
            Time.timeScale = 0.2f;
        }
        
        public void NotifyEnemies()
        {
            foreach (var enemy in _enemies)
            {
                enemy.IsChasing = true;
            }
        }

        private IEnumerator SwitchLevel()
        {
            yield return new WaitForSeconds(0.2f);
            
            // Setting timeScale to default value. 
            Time.timeScale = 1f;
            
            CustomSceneManager.Get.LoadScene(UnitySceneName.MainMenu);
        }
    }
}