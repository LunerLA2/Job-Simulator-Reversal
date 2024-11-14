using OwlchemyVR;
using UnityEngine;

public class FramedPhotoPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private FramedPhoto prefab;

	[SerializeField]
	private int photoIndex;

	[SerializeField]
	private WorldItemData worldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as FramedPhoto).SetupFramedPhoto(photoIndex, worldItemData);
	}
}
