using UnityEngine;

namespace Helpers.SceneManagement
{
    /// <summary>
    /// This class is used only for buttons
    /// </summary>
    public class CustomSceneLoaderForButtons : MonoBehaviour
    {
        [SerializeField] private UnitySceneName _unitySceneName;
        
        public void LoadScene()
        {
            if (_unitySceneName == UnitySceneName.None)
            {
                Debug.LogError("A button has unsigned Scene Value.");
                return;
            }
            
            CustomSceneManager.Get.LoadScene(_unitySceneName);
        }
        
        public void ResetProgress()
        {
            if (_unitySceneName == UnitySceneName.None)
            {
                Debug.LogError("A button has unsigned Scene Value.");
                return;
            }

            CustomSceneManager.Get.ResetLevelIndex();
            CustomSceneManager.Get.LoadScene(UnitySceneName.MainMenu);
        }
        
        public void LoadNextLevel()
        {
            CustomSceneManager.Get.LoadLevel();
        }
    }
}