using System.Collections;
using UnityEngine;

public class TransmutableObject : MonoBehaviour
{
	public ChemLabManager.ObjectStates objectState;

	[SerializeField]
	private bool debug;

	[SerializeField]
	private bool flammable = true;

	[SerializeField]
	private bool freezable = true;

	[SerializeField]
	private bool canScale = true;

	[SerializeField]
	private ShatterOnCollision shatterTech;

	[SerializeField]
	private Rigidbody myRigidBody;

	[SerializeField]
	private TransmuteFXContainer FXContainerPrefab;

	[SerializeField]
	private MeshFilter[] meshFilters;

	private TransmuteFXContainer[] FXContainers;

	private float originalBreakThreshhold;

	private void Awake()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			myRigidBody = GetComponent<Rigidbody>();
		}
		if ((bool)shatterTech)
		{
			originalBreakThreshhold = shatterTech.breakThresh;
		}
		FXContainers = new TransmuteFXContainer[meshFilters.Length];
		for (int i = 0; i < meshFilters.Length; i++)
		{
			MeshFilter mesh = meshFilters[i];
			TransmuteFXContainer transmuteFXContainer = Object.Instantiate(FXContainerPrefab);
			transmuteFXContainer.transform.SetParent(base.transform, false);
			transmuteFXContainer.SetMesh(mesh);
			FXContainers[i] = transmuteFXContainer;
		}
	}

	public void RecieveChemical(ChemLabManager.Chemicals chemical)
	{
		switch (chemical)
		{
		case ChemLabManager.Chemicals.Bleach:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			break;
		case ChemLabManager.Chemicals.LiquidNitrogen:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			SetState(ChemLabManager.ObjectStates.Frozen);
			break;
		case ChemLabManager.Chemicals.Embiggening:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			if (canScale)
			{
				StartCoroutine(ScaleningChemical(false));
			}
			break;
		case ChemLabManager.Chemicals.Smallening:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			if (canScale)
			{
				StartCoroutine(ScaleningChemical(true));
			}
			break;
		case ChemLabManager.Chemicals.Red:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			break;
		case ChemLabManager.Chemicals.Green:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			break;
		case ChemLabManager.Chemicals.Blue:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			break;
		case ChemLabManager.Chemicals.Water:
			Debug.Log(base.transform.name + " Recieved Chemical: " + chemical);
			if (objectState == ChemLabManager.ObjectStates.Burning)
			{
				SetState(ChemLabManager.ObjectStates.Basic);
			}
			break;
		}
	}

	private IEnumerator ScaleningChemical(bool shrink)
	{
		myRigidBody.AddForce(Vector3.up, ForceMode.Impulse);
		yield return new WaitForSeconds(0.1f);
		if (shrink)
		{
			base.transform.localScale = base.transform.localScale * 0.8f;
		}
		else
		{
			base.transform.localScale = base.transform.localScale * 1.2f;
		}
	}

	public void SetState(ChemLabManager.ObjectStates targetState)
	{
		Debug.Log("Object Set To " + targetState);
		switch (targetState)
		{
		case ChemLabManager.ObjectStates.Frozen:
			if (freezable)
			{
				ChangeFX(targetState);
				if ((bool)shatterTech)
				{
					shatterTech.breakThresh = 8f;
				}
				else
				{
					Debug.LogWarning("Missing Shatter On Collision on " + base.name);
				}
			}
			break;
		case ChemLabManager.ObjectStates.Burning:
			if (flammable)
			{
				ChangeFX(targetState);
				if ((bool)shatterTech)
				{
					shatterTech.breakThresh = 100f;
				}
				else
				{
					Debug.Log("Missing Shatter On Collision on " + base.name);
				}
			}
			break;
		case ChemLabManager.ObjectStates.Basic:
			ChangeFX(targetState);
			if ((bool)shatterTech)
			{
				shatterTech.breakThresh = originalBreakThreshhold;
			}
			else
			{
				Debug.Log("Missing Shatter On Collision on Object");
			}
			break;
		}
		objectState = targetState;
	}

	private void ChangeFX(ChemLabManager.ObjectStates targetState)
	{
		for (int i = 0; i < FXContainers.Length; i++)
		{
			FXContainers[i].SetState(targetState);
		}
	}
}
