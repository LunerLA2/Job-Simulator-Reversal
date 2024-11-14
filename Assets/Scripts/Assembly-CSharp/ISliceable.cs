using UnityEngine;

public interface ISliceable
{
	GameObject[] Slice(Vector3 positionInWorldSpace, Vector3 normalInWorldSpace);
}
