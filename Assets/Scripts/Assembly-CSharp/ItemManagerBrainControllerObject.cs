using UnityEngine;

public class ItemManagerBrainControllerObject : BrainControlledObject
{
	[SerializeField]
	private ParticleSystem respawnParticle;

	[SerializeField]
	private AudioSourceHelper audioSource;

	public override void EjectPrefab(GameObject prefab, BotInventoryController.EjectTypes ejectType, string locationName = "", float forceMultiplier = 1f)
	{
		if (ejectType == BotInventoryController.EjectTypes.PhysicsPush || ejectType == BotInventoryController.EjectTypes.HoldOutToPlayer)
		{
			Debug.LogWarning("ItemManagerBrainControlledObject doesn't use EjectTypes." + ejectType.ToString() + ", it will just place the object where it needs to go.");
		}
		UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(locationName);
		if (objectByName != null)
		{
			GameObject gameObject = Object.Instantiate(prefab, objectByName.transform.position, objectByName.transform.rotation) as GameObject;
			if (respawnParticle != null)
			{
				respawnParticle.transform.position = objectByName.transform.position;
				respawnParticle.Play();
			}
			if (audioSource != null)
			{
				audioSource.transform.position = objectByName.transform.position;
				audioSource.enabled = true;
				audioSource.Play();
			}
			BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
			if (component != null)
			{
				gameObject = component.LastSpawnedPrefabGO;
			}
			gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		}
	}

	private void Update()
	{
		if (!audioSource.IsPlaying)
		{
			audioSource.enabled = false;
		}
	}
}
