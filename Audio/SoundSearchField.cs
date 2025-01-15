using UnityEngine;

namespace KatAudio
{
    public class SoundSearchField : PropertyAttribute
    {
        public string Path;
        public SoundSearchField(string path)
        {
            Path = path;
        }
    }
}