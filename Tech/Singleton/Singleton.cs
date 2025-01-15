using System;
using Tech.Logger;
using UnityEditor;
using UnityEngine;

namespace Tech.Singleton
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		public static bool IsExist => _instance != null;
		
		public static T Instance
		{
			get
			{
				if (_instance) return _instance;
				_instance = FindObjectOfType<T>();
				if(_instance) return _instance;
				_instance = new GameObject(typeof(T).Name).AddComponent<T>();
				return null;
			}
		}

		protected virtual void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Destroy(gameObject);
				return;
			}
			_instance = this as T;
		}
	}

	public class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
	{
		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(transform.root);
		}
	}
}
