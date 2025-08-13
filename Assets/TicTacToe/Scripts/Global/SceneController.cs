using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TicTacToe.Global
{
    public enum SceneType
    {
        Boot,
        Menu,
        Gameplay
    }
    
    public static class SceneController
    {
        private static readonly Dictionary<SceneType, string> scenesPath = new ()
        {
            { SceneType.Boot, "Boot" },
            { SceneType.Menu, "Menu" },
            { SceneType.Gameplay, "Gameplay" }
        };
        
        public static SceneType currentScene { get; private set; }
        
        private static AsyncOperationHandle<SceneInstance> _currentSceneHandle;
        
        public static void LoadSceneLocal(SceneType scene)
        {
            if (_currentSceneHandle.IsValid())
            {
                Addressables.UnloadSceneAsync(_currentSceneHandle);
            }

            currentScene = scene;
            _currentSceneHandle = Addressables.LoadSceneAsync(scenesPath[scene]);
        }
        
        public static void LoadNetworkScene(SceneType scene)
        {
            currentScene = scene;
            NetworkManager.Singleton.SceneManager.LoadScene(scenesPath[scene], LoadSceneMode.Single);
        }
    }
}