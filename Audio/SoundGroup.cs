using System.Collections.Generic;
using UnityEngine;

namespace KatAudio
{
    [System.Serializable]
    public class SoundGroup
    {
        public string GroupID;
        public List<AudioClip> Clips;
    }
}
