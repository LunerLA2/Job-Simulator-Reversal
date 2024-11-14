using UnityEngine;

public class SteamVR_Teleporter : MonoBehaviour
{
	public enum TeleportType
	{
		TeleportTypeUseTerrain = 0,
		TeleportTypeUseCollider = 1,
		TeleportTypeUseZeroY = 2
	}

	public bool teleportOnClick;

	public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;

	private Transform reference;

	private void Start()
	{
		Transform component = Object.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
		reference = component.parent.parent;
		if (GetComponent<SteamVR_TrackedController>() == null)
		{
			Debug.LogError("SteamVR_Teleporter must be on a SteamVR_TrackedController");
			return;
		}
		GetComponent<SteamVR_TrackedController>().TriggerClicked += DoClick;
		if (teleportType == TeleportType.TeleportTypeUseTerrain)
		{
			reference.position = new Vector3(reference.position.x, Terrain.activeTerrain.SampleHeight(reference.position), reference.position.z);
		}
	}

	private void Update()
	{
	}

	private void DoClick(object sender, ClickedEventArgs e)
	{
		if (teleportOnClick)
		{
			float y = reference.position.y;
			Plane plane = new Plane(Vector3.up, 0f - y);
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			bool flag = false;
			float enter = 0f;
			if (teleportType == TeleportType.TeleportTypeUseCollider)
			{
				TerrainCollider component = Terrain.activeTerrain.GetComponent<TerrainCollider>();
				RaycastHit hitInfo;
				flag = component.Raycast(ray, out hitInfo, 1000f);
				enter = hitInfo.distance;
			}
			else if (teleportType == TeleportType.TeleportTypeUseCollider)
			{
				RaycastHit hitInfo2;
				Physics.Raycast(ray, out hitInfo2);
				enter = hitInfo2.distance;
			}
			else
			{
				flag = plane.Raycast(ray, out enter);
			}
			if (flag)
			{
				Vector3 position = ray.origin + ray.direction * enter - new Vector3(reference.GetChild(0).localPosition.x, 0f, reference.GetChild(0).localPosition.z);
				reference.position = position;
			}
		}
	}
}
