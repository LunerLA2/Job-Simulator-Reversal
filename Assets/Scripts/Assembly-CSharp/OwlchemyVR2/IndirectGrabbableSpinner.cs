using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

namespace OwlchemyVR2
{
	[RequireComponent(typeof(GrabbableItem))]
	public class IndirectGrabbableSpinner : MonoBehaviour
	{
		private const float MAX_OFFSET = 0.15f;

		private Transform fidgetRoot;

		private float previousAngle;

		private bool previousAngleHasBeenInitialized;

		public float overallSpinSpeed;

		private float avgAngleDelta;

		private float releaseFactor = 1.85f;

		private bool isGrabbed;

		private List<float> angleDeltas = new List<float>();

		private List<float> angleDeltasSorted = new List<float>();

		private float sortedAvg;

		[SerializeField]
		private float breakDistanceToleranceMultiplier = 1f;

		[SerializeField]
		private HapticTransformInfo hapticInfo;

		protected GrabbableItem grabbableItem;

		private Vector3 handPosRelToSelfAtGrab;

		private float angleAtGrab;

		private Transform handTransform;

		public GrabbableItem Grabbable
		{
			get
			{
				if (grabbableItem == null)
				{
					grabbableItem = GetComponent<GrabbableItem>();
					if (grabbableItem == null)
					{
						Debug.LogError("Couldn't find its required OwlchemyVR.GrabbableItem: " + base.gameObject.name, base.gameObject);
					}
				}
				return grabbableItem;
			}
		}

		protected virtual void Awake()
		{
			grabbableItem = GetComponent<GrabbableItem>();
			if (grabbableItem == null)
			{
				Debug.LogError("Couldn't find its required OwlchemyVR.GrabbableItem: " + base.gameObject.name, base.gameObject);
			}
			hapticInfo.ManualAwake();
			fidgetRoot = base.transform.parent;
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnEnable()
		{
			GrabbableItem obj = grabbableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem obj2 = grabbableItem;
			obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
			GrabbableItem obj3 = grabbableItem;
			obj3.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(obj3.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Released));
		}

		protected virtual void OnDisable()
		{
			GrabbableItem obj = grabbableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem obj2 = grabbableItem;
			obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
			GrabbableItem obj3 = grabbableItem;
			obj3.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(obj3.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Released));
		}

		private void Grabbed(GrabbableItem item)
		{
			handTransform = grabbableItem.CurrInteractableHand.GetGrabPointTransform();
			handPosRelToSelfAtGrab = fidgetRoot.InverseTransformPoint(handTransform.position);
			angleAtGrab = Mathf.Atan2(handPosRelToSelfAtGrab.z, handPosRelToSelfAtGrab.x) * 57.29578f;
			isGrabbed = true;
		}

		private void Released(GrabbableItem item)
		{
			isGrabbed = false;
			overallSpinSpeed = avgAngleDelta * releaseFactor;
		}

		private void Update()
		{
			if (!isGrabbed)
			{
				overallSpinSpeed = SlowDownCurve(overallSpinSpeed);
				base.transform.Rotate(0f, overallSpinSpeed, 0f);
			}
		}

		private void GrabbedUpdate(GrabbableItem item)
		{
			if (handTransform == null)
			{
				return;
			}
			Vector3 vector = fidgetRoot.InverseTransformPoint(handTransform.position);
			if ((vector - handPosRelToSelfAtGrab).magnitude > 0.15f * breakDistanceToleranceMultiplier)
			{
				grabbableItem.CurrInteractableHand.TryRelease(false);
				return;
			}
			float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
			num *= -1f;
			if (!previousAngleHasBeenInitialized)
			{
				previousAngle = num;
				previousAngleHasBeenInitialized = true;
			}
			float num2 = num - previousAngle;
			if (Mathf.Abs(num2) < 50f)
			{
				angleDeltas.Add(num2);
			}
			if (angleDeltas.Count > 8)
			{
				angleDeltas.RemoveAt(0);
			}
			avgAngleDelta = 0f;
			for (int i = 0; i < angleDeltas.Count; i++)
			{
				avgAngleDelta += angleDeltas[i];
			}
			base.transform.Rotate(0f, avgAngleDelta / (float)angleDeltas.Count, 0f);
			avgAngleDelta = FindBestAngleDelta();
			previousAngle = num;
			if (hapticInfo != null)
			{
				hapticInfo.ManualUpdate();
			}
		}

		private float FindBestAngleDelta()
		{
			angleDeltasSorted = new List<float>(angleDeltas);
			angleDeltasSorted.Sort();
			sortedAvg = 0f;
			for (int i = 2; i < angleDeltasSorted.Count; i++)
			{
				sortedAvg += angleDeltasSorted[i];
			}
			return sortedAvg / 6f;
		}

		private float SlowDownCurve(float speed)
		{
			bool flag = speed < 0f;
			speed = Mathf.Abs(speed);
			if (speed <= 0f)
			{
				return 0f;
			}
			speed = ((speed > 20f) ? (speed * 0.9994f) : ((speed > 10f) ? (speed * 0.9993f) : ((speed > 5f) ? (speed * 0.9991f) : ((speed > 1f) ? (speed * 0.997f) : ((speed > 0.5f) ? (speed * 0.994f) : ((speed > 0.1f) ? (speed * 0.98f) : ((!(speed > 0.01f)) ? (speed * 0.8f) : (speed * 0.9f))))))));
			return (!flag) ? speed : (speed * -1f);
		}
	}
}
