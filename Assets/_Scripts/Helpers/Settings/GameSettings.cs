using Enemies;
using UnityEngine;

namespace Helpers.Settings
{
    /// <summary>
    /// Globally accessible Game settings or configs.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Helpers/Create GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        #region Singleton

        private static GameSettings _gameSettings;
    
        public static GameSettings Get
        {
            get
            {
                if (_gameSettings != null) return _gameSettings;

                _gameSettings = Resources.Load<GameSettings>(ScriptableObjectLocations.Get.GameSettingsLocation);
            
                return _gameSettings;
            }
        }

        #endregion

        public Enemy.Config EnemyConfig;
    }
}