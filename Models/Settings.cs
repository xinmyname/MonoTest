using System.Runtime.Serialization;

namespace MonoTest.Models
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public string DatabasePath { get; set; }

        [DataMember]
        public string HostUrl { get; set; }
    }
}