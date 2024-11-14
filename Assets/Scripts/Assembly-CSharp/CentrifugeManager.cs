using System.Collections;
using UnityEngine;

public class CentrifugeManager : MonoBehaviour
{
	private GameObject pillBottle;

	[SerializeField]
	private Transform pillSpawnPosition;

	[SerializeField]
	private Transform pillBottleSpawnPosition;

	[SerializeField]
	private GameObject pillPrefab;

	[SerializeField]
	private GameObject pillBottlePrefab;

	[SerializeField]
	private ParticleSystem activeParticleSystem;

	[SerializeField]
	private Animation activeAnim;

	[SerializeField]
	private Transform handle;

	[SerializeField]
	private Transform closedHandlePosition;

	[SerializeField]
	private float distance = 0.05f;

	private bool isOpen;

	private bool buttonPress;

	private void Awake()
	{
		InvokeRepeating("ManageMyPillBottles", 0f, 10f);
		pillBottle = Object.Instantiate(pillBottlePrefab, pillBottleSpawnPosition.position, Quaternion.identity) as GameObject;
	}

	private void Update()
	{
		isOpen = Vector3.Distance(closedHandlePosition.position, handle.position) > distance;
	}

	private void OnTriggerStay(Collider col)
	{
		if (!isOpen && buttonPress)
		{
			Object.Destroy(col.gameObject);
			buttonPress = false;
		}
	}

	public void ButtonPressed()
	{
		if (!isOpen)
		{
			activeParticleSystem.Play();
			activeAnim.Play();
			StartCoroutine(SpawnPillWaitTime());
			buttonPress = true;
		}
	}

	private void ManageMyPillBottles()
	{
		if (Vector3.Distance(pillBottleSpawnPosition.position, pillBottle.transform.position) > 2f && Physics.CheckSphere(pillBottleSpawnPosition.position, 0.05f))
		{
			pillBottle = Object.Instantiate(pillBottlePrefab, pillBottleSpawnPosition.position, Quaternion.identity) as GameObject;
		}
	}

	private IEnumerator SpawnPillWaitTime()
	{
		yield return new WaitForSeconds(activeAnim.clip.length);
		SpawnMyPill();
	}

	public void SpawnMyPill()
	{
		Object.Instantiate(pillPrefab, pillSpawnPosition.position, Quaternion.identity);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(closedHandlePosition.position, distance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(handle.position, distance);
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawWireSphere(pillSpawnPosition.position, 0.05f);
		Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
		Gizmos.DrawWireSphere(pillBottleSpawnPosition.position, 0.05f);
	}
}
