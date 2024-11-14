using System;
using UnityEngine;

public class RigidbodyRemover : MonoBehaviour
{
	[SerializeField]
	private Rigidbody primaryRigidbody;

	public Action<RigidbodyRemover> OnRigidbodyAdded;

	public Action<RigidbodyRemover> OnRigidbodyRemoved;

	private RigidbodyBackupStoreObject primaryRigidbodyBackup = new RigidbodyBackupStoreObject();

	private bool hasBeenRemoved;

	public RigidbodyBackupStoreObject PrimaryRigidbodyBackup
	{
		get
		{
			return primaryRigidbodyBackup;
		}
	}

	public bool HasBeenRemoved
	{
		get
		{
			return hasBeenRemoved;
		}
	}

	public Rigidbody PrimaryRigidbody
	{
		get
		{
			return primaryRigidbody;
		}
	}

	public void RemoveRigidbodies()
	{
		if (base.enabled && !hasBeenRemoved)
		{
			primaryRigidbodyBackup.BackupRigidbody(PrimaryRigidbody);
			UnityEngine.Object.Destroy(PrimaryRigidbody);
			hasBeenRemoved = true;
			if (OnRigidbodyRemoved != null)
			{
				OnRigidbodyRemoved(this);
			}
		}
	}

	public void RestoreRigidbodies()
	{
		if (!base.enabled || !hasBeenRemoved)
		{
			return;
		}
		primaryRigidbody = primaryRigidbodyBackup.RigidbodyGameObject.AddComponent<Rigidbody>();
		if (PrimaryRigidbody != null)
		{
			primaryRigidbodyBackup.RestoreRigidbody(PrimaryRigidbody);
			hasBeenRemoved = false;
		}
		else
		{
			Debug.LogWarning("Nuclear option for dealing with Rigidbody being destroy and then readded in the same frame, you should probably be using OnReleasedWasNotSwappedHands");
			primaryRigidbody = primaryRigidbodyBackup.RigidbodyGameObject.GetComponent<Rigidbody>();
			if (PrimaryRigidbody != null)
			{
				UnityEngine.Object.DestroyImmediate(PrimaryRigidbody);
				primaryRigidbody = primaryRigidbodyBackup.RigidbodyGameObject.AddComponent<Rigidbody>();
				if (PrimaryRigidbody != null)
				{
					primaryRigidbodyBackup.RestoreRigidbody(PrimaryRigidbody);
					hasBeenRemoved = false;
				}
				else
				{
					Debug.LogWarning("Even the nuclear option 2 didn't work :(");
					hasBeenRemoved = true;
				}
			}
			else
			{
				Debug.LogWarning("Even the nulcear option 1 didn't work :(");
				hasBeenRemoved = true;
			}
		}
		if (!hasBeenRemoved && OnRigidbodyAdded != null)
		{
			OnRigidbodyAdded(this);
		}
	}
}
