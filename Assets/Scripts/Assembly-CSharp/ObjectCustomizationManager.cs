using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectCustomizationManager : MonoBehaviour
{
	[Serializable]
	private class CustomizerPresetPair
	{
		public ObjectCustomizerBase Customizer;

		public ObjectPreset Preset;
	}

	private static ObjectCustomizationManager _instance;

	[HideInInspector]
	[SerializeField]
	private List<CustomizerPresetPair> customizerPresetPairs = new List<CustomizerPresetPair>();

	private Dictionary<ObjectCustomizerBase, ObjectPreset> presetMappings;

	public static ObjectCustomizationManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<ObjectCustomizationManager>();
				if (_instance != null)
				{
					_instance.SetupMappings();
				}
				else
				{
					_instance = new GameObject("_ObjectCustomizationManager").AddComponent<ObjectCustomizationManager>();
					_instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			if (_instance == null)
			{
				_instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else if (_instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			SetupMappings();
			return;
		}
		CustomizerPresetPair[] array = customizerPresetPairs.ToArray();
		foreach (CustomizerPresetPair customizerPresetPair in array)
		{
			if (customizerPresetPair == null || customizerPresetPair.Customizer == null || customizerPresetPair.Preset == null)
			{
				customizerPresetPairs.Remove(customizerPresetPair);
			}
		}
	}

	private void SetupMappings()
	{
		if (presetMappings != null)
		{
			return;
		}
		presetMappings = new Dictionary<ObjectCustomizerBase, ObjectPreset>();
		for (int i = 0; i < customizerPresetPairs.Count; i++)
		{
			CustomizerPresetPair customizerPresetPair = customizerPresetPairs[i];
			if (customizerPresetPair.Customizer != null && customizerPresetPair.Preset != null)
			{
				presetMappings[customizerPresetPair.Customizer] = customizerPresetPair.Preset;
			}
		}
	}

	public void SetObjectPreset(ObjectCustomizerBase customizer, ObjectPreset preset)
	{
		presetMappings[customizer] = preset;
	}

	public ObjectPreset GetUntypedObjectPreset(ObjectCustomizerBase customizer)
	{
		ObjectPreset value = null;
		presetMappings.TryGetValue(customizer, out value);
		return value;
	}

	public T GetObjectPreset<T>(ObjectCustomizer<T> customizer) where T : ObjectPreset
	{
		return GetUntypedObjectPreset(customizer) as T;
	}

	public void CustomizeObject(GameObject go)
	{
		ObjectCustomizerBase component = go.GetComponent<ObjectCustomizerBase>();
		if (component != null)
		{
			component.Customize();
		}
	}

	public void CustomizeObject(GameObject go, ObjectPreset preset)
	{
		ObjectCustomizerBase component = go.GetComponent<ObjectCustomizerBase>();
		if (component != null)
		{
			SetObjectPreset(component, preset);
			component.Customize();
		}
	}
}
