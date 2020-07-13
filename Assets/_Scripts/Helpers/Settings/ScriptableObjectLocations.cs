using UnityEngine;

namespace Helpers.Settings
{
    /// <summary>
    /// Scriptable Object to hold location of other scriptable Objects.
    /// </summary>
    [CreateAssetMenu(fileName = "Scriptable Object Locations.asset", menuName = "Helpers/Create Scriptable Object Locations")]
    public class ScriptableObjectLocations : ScriptableObject
    {
        private const string SettingsPath = "Helpers/Scriptable Object Locations";
        
        #region Singleton
             
             private static ScriptableObjectLocations _scriptableObjectLocations;
         
             public static ScriptableObjectLocations Get
             {
                 get
                 {
                     if (_scriptableObjectLocations != null) return _scriptableObjectLocations;
     
                     _scriptableObjectLocations = Resources.Load<ScriptableObjectLocations>(SettingsPath);
                 
                     return _scriptableObjectLocations;
                 }
             }
             
        #endregion
        
        [Space(10)] public string CustomSceneManagerLocation = "Helpers/Custom Scene Manager";
        [Space(10)] public string GameSettingsLocation = "Helpers/GameManager";
    }
}