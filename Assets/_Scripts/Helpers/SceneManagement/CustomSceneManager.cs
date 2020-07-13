using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Settings;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helpers.SceneManagement
{
    [CreateAssetMenu(fileName = "Custom Scene Manager.asset", menuName = "Helpers/Custom Scene Manager")]
    public class CustomSceneManager : ScriptableObject
    {
        #region Singleton

        private static CustomSceneManager _customSceneManager;
    
        public static CustomSceneManager Get
        {
            get
            {
                if (_customSceneManager != null) return _customSceneManager;

                _customSceneManager = Resources.Load<CustomSceneManager>(ScriptableObjectLocations.Get.CustomSceneManagerLocation);
            
                return _customSceneManager;
            }
        }

        #endregion
        
        /// <summary>
        /// List of game scenes.
        /// </summary>
        [ReorderableList]
        [SerializeField] private List<CustomScene> Scenes;
        
        /// <summary>
        /// List of level scenes.
        /// </summary>
        [ReorderableList]
        [SerializeField] private List<string> Levels;
        
        /// <summary>
        /// Current level Index
        /// </summary>
        [Space(10)][SerializeField] private int _currentLevelIndex;

        [SerializeField] private bool _useTestingLevel;

        public void LoadScene(UnitySceneName selectedUnitySceneName)
        {
            var scene = Scenes.FirstOrDefault(customsScene => customsScene.UnitySceneName == selectedUnitySceneName);

            if (scene == null)
            {
                Debug.LogError($"Scene - {selectedUnitySceneName} was not found.");
                return;
            }
            
            scene.Load();
        }

        /// <summary>
        /// Loads scene, if did not found, callback to method for error management. 
        /// </summary>
        /// <param name="selectedUnitySceneName"></param>
        /// <param name="callback"></param>
        public void LoadScene(UnitySceneName selectedUnitySceneName, Action callback)
        {
            var scene = Scenes.FirstOrDefault(customsScene => customsScene.UnitySceneName == selectedUnitySceneName);

            if (scene != null)
            {
                scene.Load();
                return;
            }

            callback();
        }

        /// <summary>
        /// Loads next level or win screen if there is no levels left.
        /// </summary>
        public void NextLevel()
        {
            _currentLevelIndex++;

            if (IsGameWon())
            {
                LoadScene(UnitySceneName.WinScreen);
                return;
            }
            
            LoadScene(UnitySceneName.MainMenu);
        }
        
        public void LoadLevel()
        {
            // Loads only testing level. Used only in development.
            if (_useTestingLevel)
            {
                LoadScene(UnitySceneName.LevelTest);
                return;
            }

            if (IsGameWon())
            {
                LoadScene(UnitySceneName.WinScreen);
                return;
            }

            var level = Levels[_currentLevelIndex];

            SceneManager.LoadScene(level);
        }
        
        [Button]
        public void ResetLevelIndex() => _currentLevelIndex = 0;

        private bool IsGameWon() => _currentLevelIndex >= Levels.Count;

        /// <summary>
        /// Making easier to read Scriptable object in editor window.
        /// </summary>
        [Button]
        private void CreateSummaries()
        {
            foreach (var scene in Scenes)
            {
                scene.CreateSummary();
            }
        }

        [Serializable]
        public class CustomScene
        {
            // Used to easier understand the tool
            [SerializeField] private string _summary;
            
            public string Name;
            public UnitySceneName UnitySceneName;
        
            /// <summary>
            /// Loads this scene;
            /// </summary>
            public void Load()
            {
                SceneManager.LoadScene(Name);
            }

            public void CreateSummary()
            {
                _summary = $"{Name} mimics - {UnitySceneName}";
            }
        }
    }
}