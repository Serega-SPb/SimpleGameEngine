using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleGameEngine.Core.Serialization
{

    public class SerializerManager
    {
        private static SerializerManager _instance;

        public static SerializerManager Instance => _instance ?? (_instance = new SerializerManager());

        private SerializerManager()
        {
        }

        private string GetStrFromInstance<T>(T instance)
        {
            var toDictMethod = SerializeExtensions.GetToDictMethod(instance.GetType());
            if (toDictMethod is null)
            {
                Console.WriteLine($"Method ToDict not implemented for {instance.GetType().Name}");
                return string.Empty;
            }

            var data = toDictMethod.Invoke(instance, new object[] {instance});
            var jsData = JsonConvert.SerializeObject(data, Formatting.Indented);
            return jsData.ToString();
        }
        
        public void Save<T>(T instance, string path)
        {
            var data = GetStrFromInstance(instance);
            File.WriteAllText(path, data);
        }

        public void Save<T>(T instance, Stream stream)
        {
            var writer = new StreamWriter(stream){AutoFlush = true};
            var data = GetStrFromInstance(instance);
            writer.Write(data);
            stream.Position = 0;
        }

        private T GetInstanceFromStr<T>(string data)
        {
            var jsObj = JsonConvert.DeserializeObject(data) as JObject;
            var fromJsonMethod = typeof(T).GetMethods().First(x => x.Name == "FromJson");
            if (fromJsonMethod != null)
                return (T) fromJsonMethod.Invoke(null, new object[] {jsObj});
            return default(T);
        }
        
        public T Load<T>(string path)
        {
            var strData = File.ReadAllText(path);
            return GetInstanceFromStr<T>(strData);
        }

        public T Load<T>(Stream stream)
        {
            var reader = new StreamReader(stream);
            var data = reader.ReadToEnd();
            return GetInstanceFromStr<T>(data);
        }
    }

}