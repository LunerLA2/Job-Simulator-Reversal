using OwlchemyVR;
using UnityEngine;

public class PopsiclePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private PopsicleController popsiclePrefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData data;

	[SerializeField]
	private WorldItemData genericData;

	public override MonoBehaviour GetPrefab()
	{
		return popsiclePrefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			data = genericData;
		}
		(spawnedPrefab as PopsicleController).Setup(material, data);
	}
}
