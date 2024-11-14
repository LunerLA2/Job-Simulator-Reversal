using System.Collections;
using UnityEngine;

public class GroceryBagMachineBrainControlledObject : BrainControlledObject
{
	[SerializeField]
	private GameObjectPrefabSpawner groceryBagSpawner;

	[SerializeField]
	private Animation bagSpawnAnimation;

	[SerializeField]
	private AnimationClip open;

	[SerializeField]
	private AnimationClip close;

	private GroceryBagController lastBagSpawned;

	private Vector3 lastBagSpawnedPosition;

	public override void ScriptedEffect(BrainEffect effect)
	{
		if (effect.TextInfo == "DestroyBag")
		{
			StartCoroutine(DespawnBag());
		}
		else
		{
			SpawnNewBag(effect.TextInfo);
		}
	}

	private void SpawnNewBag(string bagName)
	{
		bagSpawnAnimation.Stop();
		groceryBagSpawner.SpawnPrefabGO();
		GroceryBagController component = groceryBagSpawner.LastSpawnedPrefabGO.GetComponent<GroceryBagController>();
		bagSpawnAnimation.Play(open.name);
		component.gameObject.name = bagName;
		component.gameObject.AddComponent<UniqueObject>();
		lastBagSpawned = component;
	}

	private void Update()
	{
		if ((bool)lastBagSpawned && !bagSpawnAnimation.isPlaying && lastBagSpawned.ItemsLocked)
		{
			bagSpawnAnimation.Play(close.name);
			lastBagSpawned = null;
		}
	}

	private IEnumerator DespawnBag()
	{
		GroceryBagController aboutToDestroy = lastBagSpawned;
		lastBagSpawned = null;
		aboutToDestroy.LockItemsInsideBag(null);
		bagSpawnAnimation[open.name].speed = -1f;
		bagSpawnAnimation[open.name].normalizedTime = 1f;
		bagSpawnAnimation.Play(open.name);
		yield return new WaitForSeconds(open.length);
		yield return null;
		Object.Destroy(aboutToDestroy.gameObject);
		bagSpawnAnimation.Stop();
		bagSpawnAnimation[open.name].speed = 1f;
		bagSpawnAnimation[open.name].normalizedTime = 0f;
	}
}
