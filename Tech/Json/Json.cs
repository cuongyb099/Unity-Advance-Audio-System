using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.Rendering;

namespace Tech.Json
{
    public static class Json 
    {
        private static readonly string _key = "gCjK+DZ/GCYbKIGiAt1qCA==";
        private static readonly string _iv = "47l5QsSe1POo31adQ/u7nQ==";
        
        //Kat Note : If You Want To See Raw File Just Command = new AES(_key, _iv)
        private static IEncryption _encryption = new AES(_key, _iv);
        
        public static void SaveJson<T>(this T data, string path)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            WriteAllText(path, json);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    
        public static async void SaveJsonAsync<T>(this T data, string path, Action saveDone = null)
        {
            await Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                WriteAllText(path, _encryption.Encrypt(json));
            });

            saveDone?.Invoke();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static void LoadJson<T>(string path, out T value)
        {
            if (File.Exists(path))
            {
                string json = ReadAllText(path);
                T data = JsonConvert.DeserializeObject<T>(json);
                value = data;
                return;
            }

            value = default;
        }

        private static void WriteAllText(string path, string text)
        {
            if (_encryption != null)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(_encryption.Encrypt(text)));
                return;
            }

            File.WriteAllText(path, text);
        }
        
        
        public static string ReadAllText(string path)
        {
            if (_encryption != null)
            {
                return _encryption.Decrypt(JsonConvert.DeserializeObject<string>(File.ReadAllText(path)));
            }

            return File.ReadAllText(path);
        }
    }
}