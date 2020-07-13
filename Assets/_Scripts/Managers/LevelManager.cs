using System;
using System.Collections;
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
        /// Enemy count that is alive in current scene
        /// </summary>
        [SerializeField] private int _enemyCount;
        
        /// <summary>
        /// Player transform that is accessible to enemies.
        /// </summary>
        public Transform PlayerTransform;
        
        private void Awake()
        {
            // Assign the reference to singleton instance.
            _levelManagerInstance = this;
        }

        public void RegisterNewEnemy()
        {
            _enemyCount++; 
            
            // Update UI.
            LevelUiManager.Get.IncreaseEnemyCount();
        }

        public void RegisterEnemyDeath()
        {
            _enemyCount--;
            
            // Update UI.
            LevelUiManager.Get.IncreaseKilledEnemyCount();
            
            if (_enemyCount <= 0)
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

        private IEnumerator SwitchLevel()
        {
            yield return new WaitForSeconds(0.2f);
            
            // Setting timeScale to default value. 
            Time.timeScale = 1f;
            
            CustomSceneManager.Get.LoadScene(UnitySceneName.MainMenu);
        }
    }
}