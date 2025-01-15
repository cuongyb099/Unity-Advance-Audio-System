using Tech.Pooling;
using UnityEngine;

namespace KatAudio
{
    [System.Serializable]
    public class Sound
    {
        public string ClipName;
        public SoundType M_SoundType = SoundType.SFX;
        public float Volume = 1f;
        public float Pitch = 1f;
        public float StereoPan = 0f;
        public float SpatialBlend = 0f;
        public float MinDistance;
        public float MaxDistance;
        public bool Loop;
        public bool IsFade;
        public float FadeDuration;
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
        public Transform FollowTarget;
        private static GenericPool<Sound> _pools = new();
        //Sound Pooling Class
        public static Sound GetNewSound()
            => _pools.Get().ReNew();

        public Sound SetClip(string clipName)
        {
            ClipName = clipName;
            return this;
        }

        public Sound SetLoop(bool isLoop)
        {
            Loop = isLoop;
            return this;
        }

        public Sound SetVolumeType(SoundType soundType)
        {
            M_SoundType = soundType;
            return this;
        }

        public Sound SetDistance(float min, float max)
        {
            MinDistance = min;
            MaxDistance = max;
            return this;
        }

        public Sound SetVolume(float volume)
        {
            Volume = volume;
            return this;
        }

        public Sound SetPitch(float pitch)
        {
            Pitch = pitch;
            return this;
        }

        public Sound SetStereoPan(float stereoPan)
        {
            StereoPan = stereoPan;
            return this;
        }

        public Sound SetSpatialBlend(float spatialBlend)
        {
            SpatialBlend = spatialBlend;
            return this;
        }

        public Sound SetFollowTarget(Transform target)
        {
            FollowTarget = target;
            return this;
        }

        public Sound SetFade(float duration)
        {
            FadeDuration = duration;
            return this;
        }
        
        public Sound ReNew()
        {
            Volume = 1f;
            ClipName = string.Empty;
            IsFade = false;
            FadeDuration = 0f;
            Pitch = 1f;
            StereoPan = 0f;
            SpatialBlend = 0f;
            MinDistance = 1f;
            MaxDistance = 500f;
            Loop = false;
            M_SoundType = SoundType.SFX;
            RolloffMode = AudioRolloffMode.Logarithmic;
            FollowTarget = null;
            return this;
        }

        public void Play()
        {
            SoundManager.Instance.PlaySound(this);
            _pools.Return(this);
        }
    }
}