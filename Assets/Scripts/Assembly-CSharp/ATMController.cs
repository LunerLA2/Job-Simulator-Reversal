using System;
using System.Collections;
using UnityEngine;

public class ATMController : MonoBehaviour
{
	[SerializeField]
	private GameObjectPrefabSpawner spawner;

	[SerializeField]
	private RigidbodyEnterExitCollisionEvents collisionEvents;

	[SerializeField]
	private AudioClip freakoutHitClip;

	[SerializeField]
	private MeshRenderer[] freakoutBlack;

	[SerializeField]
	private AudioClip[] freakoutRandomClip;

	private ElementSequence<AudioClip> freakoutRandomClipSequence;

	private bool isFreakingOut;

	[SerializeField]
	private int amountToSpawn = 30;

	private void Awake()
	{
		freakoutRandomClipSequence = new ElementSequence<AudioClip>(freakoutRandomClip);
	}

	private void OnEnable()
	{
		RigidbodyEnterExitCollisionEvents rigidbodyEnterExitCollisionEvents = collisionEvents;
		rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision = (Action<RigidbodyCollisionInfo>)Delegate.Combine(rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision, new Action<RigidbodyCollisionInfo>(RigidbodyEntered));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitCollisionEvents rigidbodyEnterExitCollisionEvents = collisionEvents;
		rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision = (Action<RigidbodyCollisionInfo>)Delegate.Remove(rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision, new Action<RigidbodyCollisionInfo>(RigidbodyEntered));
	}

	private void RigidbodyEntered(RigidbodyCollisionInfo info)
	{
		Debug.Log(string.Concat("ATM was hit at: ", info.Rigidbody.velocity, "By: ", info.Rigidbody.name));
		if (!isFreakingOut)
		{
			StartCoroutine(FreakOut());
		}
	}

	private IEnumerator FreakOut()
	{
		isFreakingOut = true;
		AudioManager.Instance.Play(base.transform.position, freakoutHitClip, 1f, 1f);
		int spawnCount = 0;
		while (spawnCount < amountToSpawn)
		{
			spawnCount++;
			yield return new WaitForSeconds(0.2f);
			spawner.SpawnPrefab();
			GameObject instance = spawner.LastSpawnedPrefabGO;
			Rigidbody rb = instance.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.AddForce(base.transform.forward * UnityEngine.Random.Range(120, 300) + Vector3.Lerp(-base.transform.right, base.transform.right, UnityEngine.Random.Range(0.4f, 0.6f)) * UnityEngine.Random.Range(100, 200), ForceMode.Acceleration);
			}
			else
			{
				Debug.LogWarning("ATM ejected a prefab without a rigidbody");
			}
			if (UnityEngine.Random.Range(0, 3) == 1)
			{
				AudioManager.Instance.Play(base.transform.position, freakoutRandomClipSequence.GetNext(), 1f, 1f);
				for (int j = 0; j < freakoutBlack.Length; j++)
				{
					freakoutBlack[j].enabled = true;
				}
			}
			else
			{
				for (int k = 0; k < freakoutBlack.Length; k++)
				{
					freakoutBlack[k].enabled = false;
				}
			}
		}
		isFreakingOut = false;
		for (int i = 0; i < freakoutBlack.Length; i++)
		{
			freakoutBlack[i].enabled = false;
		}
	}
}
