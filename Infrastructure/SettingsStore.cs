using System;
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

        public Settings Load(Func<Settings> initAction = null)
        {
            Settings settings;

            if (File.Exists(_settingsPath))
            {
                using (var stream = new FileStream(_settingsPath, FileMode.Open))
                    settings = (Settings) _serializer.ReadObject(stream);
            }
            else
            {
                settings = initAction != null
                    ? initAction()
                    : new Settings();

                Save(settings);
            }

            return settings;
        }

        public void Save(Settings settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));

            using (var stream = new FileStream(_settingsPath, FileMode.Create)) 
                _serializer.WriteObject(stream, settings);
        }
    }
}