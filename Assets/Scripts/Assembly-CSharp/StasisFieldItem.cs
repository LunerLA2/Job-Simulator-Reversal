using System.Collections.Generic;
using UnityEngine;

public class StasisFieldItem : MonoBehaviour
{
	private class RigidbodyInfo
	{
		public Rigidbody rb;

		public bool wasGravityEnabled;

		public float oldDrag;

		public float oldAngularDrag;
	}

	private Rigidbody rootRb;

	private List<RigidbodyInfo> rbInfos;

	private bool isInStasis;

	private float stasisTime;

	public bool IsInStasis
	{
		get
		{
			return isInStasis;
		}
	}

	private void Awake()
	{
		rbInfos = new List<RigidbodyInfo>();
	}

	public void ActivateStasis(float drag, float angularDrag)
	{
		Debug.Log("added");
		if (!isInStasis)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				rbInfos.Add(new RigidbodyInfo
				{
					rb = rigidbody,
					oldDrag = rigidbody.drag,
					oldAngularDrag = rigidbody.angularDrag,
					wasGravityEnabled = rigidbody.useGravity
				});
				rigidbody.drag = drag;
				rigidbody.angularDrag = angularDrag;
				rigidbody.useGravity = false;
				rigidbody.isKinematic = false;
			}
			stasisTime = Time.time;
			isInStasis = true;
		}
	}

	public void DeactivateStasis(bool permanent = false)
	{
		if (!isInStasis)
		{
			return;
		}
		for (int i = 0; i < rbInfos.Count; i++)
		{
			RigidbodyInfo rigidbodyInfo = rbInfos[i];
			Debug.Log(rigidbodyInfo.rb);
			if (rigidbodyInfo.rb != null)
			{
				rigidbodyInfo.rb.drag = rigidbodyInfo.oldDrag;
				rigidbodyInfo.rb.angularDrag = rigidbodyInfo.oldAngularDrag;
				rigidbodyInfo.rb.useGravity = rigidbodyInfo.wasGravityEnabled;
			}
		}
		rbInfos.Clear();
		isInStasis = false;
		if (permanent)
		{
			Object.Destroy(this);
		}
	}

	private void FixedUpdate()
	{
		if (!isInStasis)
		{
			return;
		}
		for (int i = 0; i < rbInfos.Count; i++)
		{
			RigidbodyInfo rigidbodyInfo = rbInfos[i];
			if (rigidbodyInfo.rb != null && !rigidbodyInfo.rb.isKinematic)
			{
				rigidbodyInfo.rb.AddForce(new Vector3(0f, Mathf.Sin((Time.time - stasisTime) * 3f) * 0.3f, 0f));
			}
		}
	}
}
