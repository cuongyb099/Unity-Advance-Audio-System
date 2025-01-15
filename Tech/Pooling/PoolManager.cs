using System;
using System.Collections.Generic;
using System.Linq;
using Tech.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tech.Pooling
{
	public enum PoolType
	{
		Bullet,
		VFX,
		Enemy,
		UIWorldSpace,
		None
	}

	public class PoolManager : Singleton<PoolManager>
	{
		private readonly Dictionary<GameObject, Pools> _objectPools = new ();
		private readonly Dictionary<PoolType, Transform> _poolsHolder = new(); 
		
		protected override void Awake()
		{
			base.Awake();
			SetupHolder();
		}

		private void SetupHolder()
		{
			GameObject holder = new GameObject("Pool Holder");
			holder.transform.SetParent(transform);
			var child = new Transform[transform.childCount];
			
			for (int i = 0; i < transform.childCount; i++)
			{
				child[i] = transform.GetChild(i);
			}

			foreach (PoolType pool in Enum.GetValues(typeof(PoolType)))
			{
				if (pool == PoolType.None) continue;
				
				var name = pool.ToString();
				
				Transform existTransform = child.FirstOrDefault(x => x.name == name);
				
				if (existTransform)
				{
					_poolsHolder.Add(pool, existTransform);
					continue;
				}
				
				GameObject empty = new (name);
				empty.transform.SetParent(holder.transform);
				_poolsHolder.Add(pool, empty.transform);
			}
		}

		public GameObject SpawnObject(GameObject objectToSpawn, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.None)
		{
			if (!_objectPools.ContainsKey(objectToSpawn))
			{
				_objectPools.Add(objectToSpawn, new Pools(objectToSpawn));
			}

			GameObject spawnableObj = _objectPools[objectToSpawn].GetPool(position, rotation);
			
			if (poolType != PoolType.None)
			{
				spawnableObj.transform.SetParent(GetPoolParent(poolType).transform);
			}
			
			return spawnableObj;
		}

		public GameObject SpawnObject(GameObject objectToSpawn, PoolType poolType = PoolType.None)
		{
			return SpawnObject(objectToSpawn, default, default, poolType);
		}
		
		public T SpawnObject<T>(GameObject objectToSpawn, PoolType poolType = PoolType.None) where T : Component
		{
			return SpawnObject(objectToSpawn, default, Quaternion.identity, poolType).GetComponent<T>();
		}

		public void ClearPool(bool includePersistent)
		{
			if (includePersistent)
			{
				_objectPools.Clear();
				return;
			}
		}
		public GameObject GetPoolParent(PoolType poolType)
		{
			return _poolsHolder[poolType].gameObject;
		}
	}
	[Serializable]
	public class Pools
	{
		private Stack<GameObject> _inActiveObjects = new ();
		private GameObject _baseObject;
		
		public Pools(GameObject obj)
		{
			_baseObject = obj;
		}
		
		// ReSharper disable Unity.PerformanceAnalysis
		public GameObject GetPool(Vector3 position = default, Quaternion rotation = default)
		{
			GameObject tmp;
			
			if (_inActiveObjects.Count > 0)
			{
				tmp = _inActiveObjects.Pop();
				tmp.transform.position = position;
				tmp.transform.rotation = rotation;
				tmp.SetActive(true);
				return tmp;
			}

			tmp = Object.Instantiate(_baseObject, position, rotation);
			tmp.AddComponent<ReturnToPool>().PoolsObjects = this;
			return tmp;
		}

		public void AddToPool(GameObject obj)
		{
			_inActiveObjects.Push(obj);
		}
	}
}