using System;
using UnityEngine;

public abstract class ObjectCustomizerBase : MonoBehaviour
{
	public virtual Type PresetType
	{
		get
		{
			return typeof(ObjectPreset);
		}
	}

	public virtual void Customize()
	{
	}
}
