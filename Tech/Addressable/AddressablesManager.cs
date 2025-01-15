using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tech.Logger;
using Tech.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[DefaultExecutionOrder(-1000)]
public class AddressablesManager : SingletonPersistent<AddressablesManager>
{
	private readonly Dictionary<object, AsyncOperationHandle> _dicAsset = new ();

	protected override void Awake()
	{
		base.Awake();
		Addressables.InitializeAsync();
	}
	public async Task<T> LoadAssetAsync<T>(object key, Action onFailed = null) where T : class
	{
		if (_dicAsset.TryGetValue(key, out var value))
		{
			return value.Result as T;
		}
		try
		{
			if (key is IEnumerable enumerable)
			{
				Addressables.LoadAssetAsync<T>(enumerable);
			}
			var opHandle =  Addressables.LoadAssetAsync<T>(key);
			await opHandle.Task;
			if (opHandle.Status == AsyncOperationStatus.Succeeded)
			{
				_dicAsset.Add(key, opHandle);
				return (T)opHandle.Result;
			}
		}
		catch (Exception e)
		{
			// ignored
		}

		LogCommon.LogWarning($"Load Asset Failed: {key}");
		onFailed?.Invoke();
		return default;
	}
	public async Task<List<T>> LoadAssetsAsync<T>(object key, Action onFailed = null)
	{
		if (_dicAsset.TryGetValue(key, out var value))
		{
			return value.Result as List<T>;
		}
		try
		{
			var opHandle = Addressables.LoadAssetsAsync<T>(key, null);
			await opHandle.Task;
			if (opHandle.Status == AsyncOperationStatus.Succeeded)
			{
				_dicAsset.Add(key, opHandle);
				return (List<T>)opHandle.Result;
			}
		}
		catch (Exception e)
		{
			// ignored
		}

		LogCommon.LogWarning($"Load Asset Failed: {key}");
		onFailed?.Invoke();
		return default;
	}
	public void RemoveAsset(object key)
	{
		if(!_dicAsset.TryGetValue(key, out var value)) return;
		Addressables.ReleaseInstance(value);
		_dicAsset.Remove(key);
	}
	public bool TryGetAssetInCache<T>(string key, out T result) where T : class
	{
		if (_dicAsset.TryGetValue(key, out var opHandle))
		{
			result = opHandle.Result as T;
			return true;
		}
		result = default;
		return false;
	}

	public async Task<GameObject> InstantiateAsync(object key, Transform parent = null)
	{
		var opHandle = Addressables.InstantiateAsync(key, parent);
		await opHandle.Task;
		_dicAsset.Add(key, opHandle);
		return opHandle.Result;
	}
}
