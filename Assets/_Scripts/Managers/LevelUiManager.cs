using System;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class LevelUiManager : MonoBehaviour
    {
        #region Singleton

        private static LevelUiManager _levelUiManagerInstance;

        public static LevelUiManager Get
        {
            get
            {
                if (_levelUiManagerInstance != null)
                {
                    return _levelUiManagerInstance;
                }
                
                Debug.LogError($"Object of type {typeof(LevelUiManager)} was called to early");
                return null;
            }
        }

        #endregion

        /// <summary>
        /// How many enemies where killed in current level
        /// </summary>
        private int _amountOfKilledEnemies;
        
        /// <summary>
        /// Total number of enemies in the level
        /// </summary>
        private int _amountOfAllEnemies;

        [SerializeField] private TextMeshProUGUI _enemyCountText;
        
        private void Awake()
        {
            _levelUiManagerInstance = this;
            
            UpdateEnemyCountText();
        }

        public void IncreaseEnemyCount()
        {
            _amountOfAllEnemies++;

            UpdateEnemyCountText();
        }
        
        public void IncreaseKilledEnemyCount()
        {
            _amountOfKilledEnemies++;

            UpdateEnemyCountText();
        }
        private void UpdateEnemyCountText() => _enemyCountText.text = $"{_amountOfKilledEnemies}/{_amountOfAllEnemies}";
    }
}