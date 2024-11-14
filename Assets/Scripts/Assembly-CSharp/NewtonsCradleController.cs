using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class NewtonsCradleController : MonoBehaviour
{
	public float InitialForce = 10f;

	public float StartDelay = 0.5f;

	[SerializeField]
	private NewtonsCradleBallController[] balls;

	[SerializeField]
	private LineRenderer stringRenderer;

	private PickupableItem pickupable;

	private Vector3[] initialBallPos;

	private Vector3[] ballPosHolder;

	private void Awake()
	{
		ballPosHolder = new Vector3[12];
		for (int i = 0; i < ballPosHolder.Length; i++)
		{
			ballPosHolder[i] = new Vector3(0f, 0f, 0f);
		}
		pickupable = GetComponent<PickupableItem>();
		PickupableItem pickupableItem = pickupable;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = pickupable;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(StartDelay);
		StartSwinging();
	}

	private void StartSwinging()
	{
		Rigidbody rigidbody = balls[0].Rigidbody;
		rigidbody.AddForceAtPosition(-rigidbody.transform.forward * InitialForce, rigidbody.transform.position);
	}

	private void Update()
	{
		ballPosHolder[0] = balls[0].Anchor0;
		ballPosHolder[1] = balls[0].Top;
		ballPosHolder[2] = balls[0].Anchor1;
		ballPosHolder[3] = balls[1].Anchor1;
		ballPosHolder[4] = balls[1].Top;
		ballPosHolder[5] = balls[1].Anchor0;
		ballPosHolder[6] = balls[2].Anchor0;
		ballPosHolder[7] = balls[2].Top;
		ballPosHolder[8] = balls[2].Anchor1;
		ballPosHolder[9] = balls[3].Anchor1;
		ballPosHolder[10] = balls[3].Top;
		ballPosHolder[11] = balls[3].Anchor0;
		stringRenderer.SetPositions(ballPosHolder);
	}

	private void Grabbed(GrabbableItem grabbable)
	{
	}

	private void Released(GrabbableItem grabbable)
	{
	}
}
