using System.Collections;
using UnityEngine;

public class SpawnedByCollision : MonoBehaviour
{
	public AudioClip impactSoundEffect;

	public float impactSoundVolume = 1f;

	public ImpactResult[] impactResults;

	private Vector3 impactRelativeVelocity;

	private Vector3 collisionNormal;

	private int childrenBeingControlled;

	public void SetImpactDetails(Vector3 impactRelativeVelocity, Vector3 collisionNormal)
	{
		this.impactRelativeVelocity = impactRelativeVelocity;
		this.collisionNormal = collisionNormal;
	}

	private void Start()
	{
		if (impactSoundEffect != null)
		{
			AudioManager.Instance.Play(base.transform.position, impactSoundEffect, impactSoundVolume, 1f);
		}
		for (int i = 0; i < impactResults.Length; i++)
		{
			impactResults[i].Do(impactRelativeVelocity, collisionNormal);
			if (impactResults[i].SpawnAsAdditionalObject)
			{
				StartCoroutine(SpawnAsAdditionalObject(impactResults[i].GameObjectTarget));
				childrenBeingControlled++;
			}
		}
		CheckIfParentGameObjectIsStillNeeded();
	}

	private IEnumerator SpawnAsAdditionalObject(GameObject obj)
	{
		yield return null;
		GameObject spawned = (GameObject)Object.Instantiate(rotation: Quaternion.FromToRotation(Vector3.up, collisionNormal), original: obj, position: base.transform.position);
		spawned.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		Vector3 originalScale = spawned.transform.localScale;
		float percentageScaled = 0.1f;
		while (percentageScaled < 1f)
		{
			percentageScaled += Time.deltaTime * 3f;
			if (spawned != null)
			{
				spawned.transform.localScale = originalScale * percentageScaled;
			}
			yield return null;
			if (percentageScaled >= 1f && spawned != null)
			{
				spawned.transform.localScale = originalScale;
			}
		}
		childrenBeingControlled--;
		CheckIfParentGameObjectIsStillNeeded();
	}

	private void CheckIfParentGameObjectIsStillNeeded()
	{
		if (childrenBeingControlled == 0 && base.transform.childCount == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
