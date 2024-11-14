using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSliceHandler : MonoBehaviour
{
	public virtual void handleSlice(GameObject[] results)
	{
	}

	public virtual bool cloneAlternate(Dictionary<string, bool> hierarchyPresence)
	{
		return true;
	}
}
