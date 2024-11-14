using System;
using UnityEngine;

public abstract class ObjectCustomizer<T> : ObjectCustomizerBase where T : ObjectPreset
{
	[SerializeField]
	[HideInInspector]
	private bool customized;

	public override Type PresetType
	{
		get
		{
			return typeof(T);
		}
	}

	public virtual void Awake()
	{
		if (!customized)
		{
			Customize();
			customized = true;
		}
	}

	public override void Customize()
	{
		T objectPreset = ObjectCustomizationManager.Instance.GetObjectPreset(this);
		if (objectPreset != null)
		{
			Customize(objectPreset);
		}
	}

	public virtual void Customize(T preset)
	{
	}
}
