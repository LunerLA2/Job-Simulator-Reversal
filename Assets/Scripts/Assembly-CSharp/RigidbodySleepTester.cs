using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySleepTester : MonoBehaviour
{
	private List<Rigidbody> ropeRigidbodies;

	private List<Rigidbody> allRididbodies = new List<Rigidbody>();

	private void Start()
	{
		ropeRigidbodies = new List<Rigidbody>();
		LogicSimpleRopes[] array = Object.FindObjectsOfType<LogicSimpleRopes>();
		foreach (LogicSimpleRopes logicSimpleRopes in array)
		{
			ropeRigidbodies.AddRange(logicSimpleRopes.GetComponentsInChildren<Rigidbody>());
		}
		StartCoroutine(CheckRigidbodies());
	}

	private IEnumerator CheckRigidbodies()
	{
		while (true)
		{
			yield return null;
			int totalRopePartsNotsleeping = 0;
			foreach (Rigidbody body2 in ropeRigidbodies)
			{
				if (!body2.IsSleeping())
				{
					totalRopePartsNotsleeping++;
				}
			}
			Debug.Log("Total Rope Parts Not Sleeping:" + totalRopePartsNotsleeping);
			allRididbodies.Clear();
			allRididbodies.AddRange(Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[]);
			string bodyNames = string.Empty;
			int totalNotSleeping = 0;
			foreach (Rigidbody body in allRididbodies)
			{
				if (body != null && !body.IsSleeping())
				{
					bodyNames = bodyNames + body.gameObject.name + ", ";
					totalNotSleeping++;
				}
			}
			Debug.Log("Rigidbodies Not Sleeping:" + totalNotSleeping);
			Debug.Log(bodyNames);
			yield return new WaitForSeconds(5f);
		}
	}
}
