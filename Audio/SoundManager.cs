using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Tech.Pooling;
using Tech.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace KatAudio
{
	public class SoundManager : SingletonPersistent<SoundManager>
	{
		private GameObject _emptyObj;
		private AudioMixer _audioMixer;
		private AudioMixerGroup _masterGroup;
		private AudioMixerGroup _bgGroup;
		private AudioMixerGroup _sfxGroup;
		private readonly List<AudioSource> _instanceAudioSource = new();
		private Pools _audioSourcePool;
		private bool _isSound;
		
		[SerializeField]
		private List<SoundGroup> _soundGroups = new();

		protected override void Awake()
		{
			base.Awake();
			CreateAudioChildPrefab();
			LoadAudioMixer();
		}

		private void Start()
		{
			LoadVolume();
		}

		private void CreateAudioChildPrefab()
		{
			_emptyObj = new GameObject("Audio Child")
			{
				transform =
				{
					parent = transform
				}
			};
			_audioSourcePool = new Pools(_emptyObj);
		}
		private void LoadAudioMixer()
		{
			_audioMixer = Resources.Load<AudioMixer>("Audio Mixer");
			var groups = _audioMixer.FindMatchingGroups("Master");
			_masterGroup = groups[0];
			_bgGroup = groups[1];
			_sfxGroup = groups[2];
		}

		public async Task LoadSoundGroupAsync(string key)
		{
			foreach (var soundGroup in _soundGroups)
			{
				if(soundGroup.GroupID == key) return;
			}
			
			while (!AddressablesManager.Instance)
				await Task.Delay(100);

			_soundGroups ??= new List<SoundGroup>();

			var clips = await AddressablesManager
				.Instance.LoadAssetsAsync<AudioClip>(key);
			
			if(clips == null) return;
			
			_soundGroups.Add(new SoundGroup()
			{
				GroupID = key,
				Clips = clips
			});
		}

		public void ReleaseSoundGroup(string key, Action onReleaseComplete = null)
		{
			if(!AddressablesManager.IsExist) return;
			
			for (int i = _soundGroups.Count - 1; i >= 0; i--)
			{
				if (_soundGroups[i].GroupID == key)
				{
					_soundGroups.RemoveAt(i);
					AddressablesManager.Instance.RemoveAsset(key);
					return;
				}
			}
			onReleaseComplete?.Invoke();
		}
		
		private void LoadVolume()
		{
			SetSoundVolume(SoundVolumeConstant.MusicVolume, PlayerPrefs.GetFloat(SoundVolumeConstant.MusicVolume, 1));
			SetSoundVolume(SoundVolumeConstant.MasterVolume, PlayerPrefs.GetFloat(SoundVolumeConstant.MasterVolume, 1));
			SetSoundVolume(SoundVolumeConstant.FXVolume, PlayerPrefs.GetFloat(SoundVolumeConstant.FXVolume, 1));
		}

		public void PlaySound(Sound sound)
		{
			var audioClip = GetClipFromLibrary(sound.ClipName);
			if (!audioClip) return;

			var audioObj = _audioSourcePool.GetPool();
			audioObj.transform.parent = transform;
			if (!audioObj.TryGetComponent(out AudioChild audioChild))
			{
				audioChild = audioObj.AddComponent<AudioChild>();
			}

			audioChild.FollowTarget = sound.FollowTarget;
			var audioSource = audioChild.Source;
			audioSource.clip = audioClip;
			audioSource.volume = sound.Volume;
			audioSource.pitch = sound.Pitch;
			audioSource.loop = sound.Loop;
				
			audioSource.outputAudioMixerGroup = GetMixerGroup(sound.M_SoundType);
			audioSource.playOnAwake = false;
			audioSource.panStereo = sound.StereoPan;
			audioSource.spatialBlend = sound.SpatialBlend;
			audioSource.minDistance = sound.MinDistance;
			audioSource.maxDistance = sound.MaxDistance;
			audioSource.rolloffMode = sound.RolloffMode;
			audioSource.transform.localPosition = Vector3.zero;
			audioSource.Play();
			
			if (!sound.IsFade) return;
			
			sound.Volume = 0f;
			audioSource.DOFade(sound.Volume, sound.FadeDuration);
		}

		public void StopSound(AudioClip audioClip, bool stopAll, bool stopImmediately = false)
		{
			for (int i = 0; i < _instanceAudioSource.Count; i++)
			{
				if (_instanceAudioSource[i].clip != audioClip ||
				    !_instanceAudioSource[i].gameObject.activeSelf) continue;

				if (stopImmediately)
				{
					_instanceAudioSource[i].gameObject.SetActive(false);
					if (!stopAll) return;
					continue;
				}

				_instanceAudioSource[i].loop = false;
				if (!stopAll) return;
			}
		}

		public void StopAllSound()
		{
			foreach (var audioSource in _instanceAudioSource)
			{
				if (!audioSource.isPlaying) continue;
				audioSource.Stop();
			}
		}

		private AudioClip GetClipFromLibrary(string clipName)
		{
			foreach (var group in _soundGroups)
			{
				foreach (var clip in group.Clips)
				{
					if(clip.name == clipName) return clip;
				}
			}
			return null;
		}

		private AudioMixerGroup GetMixerGroup(SoundType type)
		{
			switch (type)
			{
				case SoundType.Master:
					return _masterGroup;
				case SoundType.BGM:
					return _bgGroup;
				case SoundType.SFX:
					return _sfxGroup;
				default:
					return null;
			}
		}

		public static string GetVolumeParameter(SoundType type)
		{
			switch (type)
			{
				case SoundType.Master:
					return SoundVolumeConstant.MasterVolume;
				case SoundType.BGM:
					return SoundVolumeConstant.MusicVolume;
				case SoundType.SFX:
					return SoundVolumeConstant.FXVolume;
				default:
					return null;
			}
		}
		
		public void SetSoundVolume(SoundType soundType, float value)
		{
			SetSoundVolume(GetVolumeParameter(soundType), value);
		}
		
		public void SetSoundVolume(string parameter, float value)
		{
			value = Math.Clamp(value, 0.0001f, 0.999f);
			_audioMixer.SetFloat(parameter, (float)Math.Log10(value) * 20f);
		}

		public void SetSound(bool value)
		{
			_isSound = value;
			if (!_isSound) _instanceAudioSource.ForEach(x => x.Stop());
		}
	}
}