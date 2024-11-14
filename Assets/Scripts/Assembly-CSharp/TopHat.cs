using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class TopHat : MonoBehaviour
{
	[SerializeField]
	private PickupableItem item;

	[SerializeField]
	private ParticleSystem spawnPoofParticle;

	[SerializeField]
	private Transform spawnAt;

	[SerializeField]
	private GameObject[] prefabsToSpawn;

	private int spawnIndex;

	private bool spawningIsPossible = true;

	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			Rigidbody attachedRigidbody = other.attachedRigidbody;
			if (attachedRigidbody.gameObject.layer == 10 && spawningIsPossible && item.IsCurrInHand && item.CurrInteractableHand.GetComponent<Rigidbody>() != attachedRigidbody)
			{
				StartCoroutine(SpawnAnObject());
			}
		}
	}

	private IEnumerator SpawnAnObject()
	{
		spawningIsPossible = false;
		Object.Instantiate(prefabsToSpawn[spawnIndex], spawnAt.position, spawnAt.rotation);
		spawnPoofParticle.Play();
		if (spawnIndex < prefabsToSpawn.Length - 1)
		{
			spawnIndex++;
		}
		else
		{
			spawnIndex = 0;
		}
		yield return new WaitForSeconds(1f);
		spawningIsPossible = true;
	}
}
