using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using SimpleGameEngine.Core.SceneObjects;
using Properties = SimpleGameEngine.Core.Serialization.SerializeExtensions.SceneProperties;
using ScObjProperties = SimpleGameEngine.Core.Serialization.SerializeExtensions.SceneObjectProperties;

namespace SimpleGameEngine.Core
{
    public class Scene
    {
        private BitmapImage _background;
        private Canvas _mainCanvas;

        public Scene()
        {
        }

        public virtual Canvas MainCanvas => _mainCanvas;

        public virtual string Name { get; set; } = "Scene";
        
        public virtual string Description { get; set; }
        
        public virtual MutableSize Size { get; set; } = new MutableSize(850, 500);
        
        public virtual BitmapImage Background
        {
            get => _background;
            set
            {
                _background = value;
                SetBackground(value);
            }
        }

        public virtual List<SceneObject> Objects { get; } = new List<SceneObject>();

        public void EnableScene ()
        {
            GameLoop.RenderEvent += Render;
            foreach (var behaviour in Objects.SelectMany(sceneObject => sceneObject.Behaviours))
                behaviour.EnableBehaviour();
        }

        public void DisableScene()
        {
            GameLoop.RenderEvent -= Render;
            foreach (var behaviour in Objects.SelectMany(sceneObject => sceneObject.Behaviours))
                behaviour.DisableBehaviour();
        }

        public virtual void SetCanvas(Canvas canvas)
        {
            _mainCanvas = canvas;
            if (canvas != null)
            {
                canvas.ClipToBounds = true;
                canvas.Width = Size.Width;
                canvas.Height = Size.Height;
            }
            SetBackground(Background);
            Render();
        }
        
        private void SetBackground(BitmapImage image)
        {
            if (MainCanvas != null)
                MainCanvas.Background = new ImageBrush(image);
        }
        
        public Point ConvertPoint(Point point)
        {
            return new Point
            {
                X = MainCanvas.Width * point.X / 100,
                Y = MainCanvas.Height * point.Y / 100
            };
        }
        
        public virtual void AddSceneObject(SceneObject sceneObject) => Objects.Add(sceneObject);

        public virtual void RemoveObject(SceneObject sceneObject) => Objects.Remove(sceneObject);

        public IEnumerable<SceneObject> GetObjectsByName(string name) => Objects.Where(x => x.Name.Equals(name));
        
        public SceneObject GetObjectByName(string name) => Objects.FirstOrDefault(x => x.Name.Equals(name));

        public virtual void Render()
        {
            if (MainCanvas is null)
            {
                Console.WriteLine("Scene canvas is null");
                return;
            }

            MainCanvas.Children.Clear();
            foreach (var sceneObject in Objects)
            {
                sceneObject.Render();
            }
        }

        public static Scene FromJson(JObject jsObj)
        {
            var scene = new Scene();
            scene.Name = jsObj[Properties.Name].Value<string>();
            scene.Description = jsObj[Properties.Description]?.Value<string>();
            var sizeValues = jsObj[Properties.Size]?.Values<double>().ToArray();
            if (sizeValues != null)
                scene.Size = new MutableSize(sizeValues[0], sizeValues[1]);
            if (!string.IsNullOrWhiteSpace(jsObj[Properties.Background].ToString()))
            {
                var imgPath = jsObj[Properties.Background].Value<string>();
                scene.Background = new BitmapImage(new Uri(imgPath));
            }

            var sceneObjTypes = InheritedClassMethods.GetInheritedClasses<SceneObject>();
            foreach (var obj in jsObj[Properties.Objects])
            {
                var type = sceneObjTypes.First(x => x.Name.Equals(obj[ScObjProperties.Type].Value<string>()));
                var fromJsonMethod = type.GetMethods().First(x => x.Name == "FromJson");
                if (fromJsonMethod == null) 
                    continue;
                var sceneObj = (SceneObject)fromJsonMethod.Invoke(null, new object[] {obj});
                sceneObj.SetParent(scene);
            }
            return scene;
        }

    }


}