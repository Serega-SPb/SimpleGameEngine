using System;
using System.Collections.Generic;
using System.IO;
using SimpleGameEngine.Core.Serialization;

namespace SimpleGameEngine.Core
{
    public class SceneManager
    {
        private static SceneManager _instance;

        public static SceneManager Instance => _instance ?? (_instance = new SceneManager());

        private const string Extension = ".scene";
        public const string SceneDir = "Scenes";
        
        private readonly SerializerManager _serializerManager = SerializerManager.Instance;
        
        private SceneManager()
        {
            LoadScenes();
        }
        
        public Scene GameScene { get; set; }
        public Dictionary<string, Func<Scene>> SceneCache { get; } = new Dictionary<string, Func<Scene>>();

        private void LoadScenes()
        {
            SceneCache.Clear();
            if (!Directory.Exists(SceneDir))
            {
                Console.WriteLine("Scene dir not found");
                return;
            }

            foreach (var file in Directory.GetFiles(SceneDir, $"*{Extension}"))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                SceneCache.Add(filename, ()=>InitScene(file));
            }
        }

        public void SaveToFile(Scene scene, string filepath)
        {
            try
            {
                _serializerManager.Save(scene, filepath);
                LoadScenes();
            }
            catch (Exception e)
            {
                Console.WriteLine("SceneManager error:");
                Console.WriteLine(e);
            }
        }
        
        public Scene LoadFromFile(string filepath)
        {
            try
            {
                return InitScene(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine("SceneManager error:");
                Console.WriteLine(e);
                return null;
            }
        }

        public Scene GetSceneByName(string name)
        {
            if (!SceneCache.ContainsKey(name))
                throw new KeyNotFoundException($"{GetType().Name}: Scene with {name} not found");
            return SceneCache[name]();
        }

        private Scene InitScene(string sceneFile) => _serializerManager.Load<Scene>(sceneFile);

        public Scene Copy(Scene source)
        {
            using (var memory = new MemoryStream())
            {
                _serializerManager.Save(source, memory);
                return _serializerManager.Load<Scene>(memory);
            }
        }
        
        public void ChangeScene(Scene current, string nextSceneName)
        {
            var nextScene = GetSceneByName(nextSceneName);
            current.DisableScene();
            nextScene.SetCanvas(current.MainCanvas);
            nextScene.EnableScene();
            if (current.Equals(GameScene))
                GameScene = nextScene;

        }

    }
}