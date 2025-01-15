using System;
using UnityEngine;

namespace KatAudio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioChild : MonoBehaviour
    {
        [NonSerialized] public AudioSource Source;
        [NonSerialized] public Transform FollowTarget;
        private void Awake()
        {
            Source = GetComponent<AudioSource>();
        }

        private void Update()
        {
            CheckingPlaying();
            FollowingTarget();
        }

        private void FollowingTarget()
        {
            if(!FollowTarget) return;
            transform.position = FollowTarget.position;
        }

        private void CheckingPlaying()
        {
            if(Source.isPlaying) return;
            gameObject.SetActive(false);
        }
    }
}
