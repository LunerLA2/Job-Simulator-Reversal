using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BrainEntry
{
	public enum CauseLogicTypes
	{
		AllTrue = 0,
		AnyTrue = 1,
		OnceAllTrue = 2,
		OnceAnyTrue = 3
	}

	public enum EffectOverrideTypes
	{
		Normal = 0,
		CancelSameTypes = 1,
		DropEverything = 2
	}

	[SerializeField]
	private string folderID = string.Empty;

	[SerializeField]
	private List<BrainCause> causes = new List<BrainCause>();

	[SerializeField]
	private List<BrainEffect> effects = new List<BrainEffect>();

	[SerializeField]
	private CauseLogicTypes logicType;

	[SerializeField]
	private EffectOverrideTypes effectOverrideType;

	public string FolderID
	{
		get
		{
			return folderID;
		}
	}

	public List<BrainCause> Causes
	{
		get
		{
			return causes;
		}
	}

	public List<BrainEffect> Effects
	{
		get
		{
			return effects;
		}
	}

	public CauseLogicTypes LogicType
	{
		get
		{
			return logicType;
		}
	}

	public EffectOverrideTypes EffectOverrideType
	{
		get
		{
			return effectOverrideType;
		}
	}

	public void InternalSetLogicType(CauseLogicTypes type)
	{
		logicType = type;
	}

	public void InternalSetEffectOverrideType(EffectOverrideTypes type)
	{
		effectOverrideType = type;
	}

	public void InternalSetFolderID(string id)
	{
		folderID = id;
	}

	public void PasteContentsFrom(BrainEntry other)
	{
		causes.Clear();
		effects.Clear();
		logicType = other.LogicType;
		for (int i = 0; i < other.Causes.Count; i++)
		{
			causes.Add(new BrainCause(other.Causes[i]));
		}
		for (int j = 0; j < other.Effects.Count; j++)
		{
			effects.Add(new BrainEffect(other.Effects[j]));
		}
	}
}
