using System.IO;
using System.Runtime.Serialization.Json;
using MonoTest.Models;

namespace MonoTest.Infrastructure
{
    public class SettingsStore
    {
        private readonly string _settingsPath;
        private readonly DataContractJsonSerializer _serializer;

        public SettingsStore()
        {
            _settingsPath = DocumentPath.For("MonoTest", "settings");
            _serializer = new DataContractJsonSerializer(typeof(Settings));
        }

        public Settings Load()
        {
            Settings settings = null;

            if (File.Exists(_settingsPath))
            {
                using (var stream = new FileStream(_settingsPath, FileMode.Open))
                    settings = (Settings) _serializer.ReadObject(stream);
            }

            return settings;
        }

        public void Save(Settings settings)
        {
            string settingsDir = Path.GetDirectoryName(_settingsPath);

            if (settingsDir == null)
                return;

            Directory.CreateDirectory(settingsDir);

            using (var stream = new FileStream(_settingsPath, FileMode.Create)) 
                _serializer.WriteObject(stream, settings);
        }
    }
}