using OwlchemyVR;
using UnityEngine;

public class AstronomyStationController : KitchenTool
{
	[SerializeField]
	private BasePrefabSpawner blackHoleSpawner;

	[SerializeField]
	private MeshRenderer lightRenderer;

	[SerializeField]
	private Material onMat;

	[SerializeField]
	private Material offMat;

	private GameObject blackHole;

	private bool blackHoleActive;

	private WorldItem blackHoleWorldItem;

	private void Awake()
	{
		blackHole = blackHoleSpawner.LastSpawnedPrefabGO;
		blackHoleWorldItem = blackHole.GetComponent<WorldItem>();
		HideBlackHole();
	}

	public override void OnDismiss()
	{
		if (blackHoleActive)
		{
			HideBlackHole();
		}
		base.OnDismiss();
	}

	public void ToggleBlackHole()
	{
		if (blackHoleActive)
		{
			HideBlackHole();
		}
		else
		{
			ShowBlackHole();
		}
	}

	private void ShowBlackHole()
	{
		blackHoleActive = true;
		blackHole.SetActive(true);
		lightRenderer.material = onMat;
		GameEventsManager.Instance.ItemActionOccurred(blackHoleWorldItem.Data, "OPENED");
	}

	private void HideBlackHole()
	{
		blackHoleActive = false;
		blackHole.SetActive(false);
		lightRenderer.material = offMat;
		GameEventsManager.Instance.ItemActionOccurred(blackHoleWorldItem.Data, "CLOSED");
	}
}
