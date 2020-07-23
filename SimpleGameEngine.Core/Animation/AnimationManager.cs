using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace SimpleGameEngine.Core.Animation
{
    public class AnimationManager
    {
        private static AnimationManager _instance;

        public static AnimationManager Instance => _instance ?? (_instance = new AnimationManager());

        
        private const string Extension = ".anim";
        private const string ConfigDir = "Configs";

        private AnimationManager()
        {
            LoadAnimations();
        }
        
        public Dictionary<string, Lazy<Animation>> AnimationsCache { get; } = new Dictionary<string, Lazy<Animation>>();
        
        private void LoadAnimations()
        {
            if (!Directory.Exists(ConfigDir))
            {
                Console.WriteLine("Config dir not found");
                return;
            }

            foreach (var file in Directory.GetFiles(ConfigDir, $"*{Extension}", SearchOption.AllDirectories))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                AnimationsCache.Add(filename, new Lazy<Animation>(()=>InitAnimation(file)));
            }
        }

        public Animation GetAnimationByName(string name)
        {
            if (!AnimationsCache.ContainsKey(name))
                throw new KeyNotFoundException($"{GetType().Name}: Animation with {name} not found");
            return AnimationsCache[name].Value;
        }

        private Animation InitAnimation(string configFile)
        {
            try
            {
                var xdoc = XDocument.Load(configFile);
                var name = xdoc.Root.Attribute("Name")?.Value ?? configFile.Replace(".anim", "");
                var delay = Convert.ToInt32(xdoc.Root.Attribute("DelayTime").Value);
                var slides = new List<BitmapImage>();
                foreach (var xElement in xdoc.Root.Element("Slides").Elements())
                {
                    var part = xElement.Attribute("Path").Value.TrimStart('\\');
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, part); 
                    var slide = new BitmapImage(new Uri(path, UriKind.Absolute));
                    slides.Add(slide);
                }
                return new Animation(name, slides, delay);
            }
            catch (Exception e)
            {
                Console.WriteLine($"AnimationInit: Incorrect .anim file - {configFile}");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}