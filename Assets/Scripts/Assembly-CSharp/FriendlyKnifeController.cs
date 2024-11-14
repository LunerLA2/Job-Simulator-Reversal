using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(GrabbableItem))]
public class FriendlyKnifeController : MonoBehaviour
{
	[SerializeField]
	private float bladeInnerTriggerShift;

	[SerializeField]
	private float bladeOuterTriggerShift;

	[SerializeField]
	private float maxBladeLift;

	[SerializeField]
	private float bladeLiftStrength;

	[SerializeField]
	private AnimationCurve bladeLiftCurve;

	[SerializeField]
	private Transform bladeJointsRoot;

	private Transform[] bladeJoints;

	private BoxCollider[] bladeColliders;

	private RigidbodyEnterExitTriggerEvents[] bladeInnerTriggerZones;

	private RigidbodyEnterExitTriggerEvents[] bladeOuterTriggerZones;

	private GrabbableItem grabbable;

	private void Awake()
	{
		bladeJoints = new Transform[bladeJointsRoot.childCount];
		bladeColliders = new BoxCollider[bladeJoints.Length];
		bladeInnerTriggerZones = new RigidbodyEnterExitTriggerEvents[bladeJoints.Length];
		bladeOuterTriggerZones = new RigidbodyEnterExitTriggerEvents[bladeJoints.Length];
		for (int i = 0; i < bladeJoints.Length; i++)
		{
			bladeJoints[i] = bladeJointsRoot.GetChild(i);
			bladeColliders[i] = bladeJoints[i].GetComponent<BoxCollider>();
			if (i >= 1)
			{
				GameObject gameObject = new GameObject("InnerTrigger");
				gameObject.transform.SetParent(bladeJoints[i].transform, false);
				gameObject.transform.localScale = Vector3.one;
				BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.isTrigger = true;
				boxCollider.size = bladeColliders[i].size;
				boxCollider.center = bladeColliders[i].center + new Vector3(0f, 0f - bladeInnerTriggerShift, 0f);
				bladeInnerTriggerZones[i] = gameObject.AddComponent<RigidbodyEnterExitTriggerEvents>();
				GameObject gameObject2 = new GameObject("OuterTrigger");
				gameObject2.transform.SetParent(bladeJoints[i].transform, false);
				gameObject2.transform.localScale = Vector3.one;
				BoxCollider boxCollider2 = gameObject2.AddComponent<BoxCollider>();
				boxCollider2.isTrigger = true;
				boxCollider2.size = boxCollider.size + new Vector3(0f, bladeOuterTriggerShift, 0f);
				boxCollider2.center = boxCollider.center + new Vector3(0f, (0f - bladeOuterTriggerShift) / 2f, 0f);
				bladeOuterTriggerZones[i] = gameObject2.AddComponent<RigidbodyEnterExitTriggerEvents>();
			}
		}
		grabbable = GetComponent<GrabbableItem>();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		for (int i = 1; i < bladeJoints.Length; i++)
		{
			if (bladeInnerTriggerZones[i].ActiveRigidbodiesTriggerInfo.Count > 0 && grabbable.IsCurrInHand)
			{
				Vector3 localPosition = bladeJoints[i].localPosition;
				float time = (float)i / (float)(bladeJoints.Length - 1);
				float b = Mathf.Lerp(localPosition.y / maxBladeLift, 1f, Time.deltaTime * bladeLiftStrength);
				localPosition.y = Mathf.Min(bladeLiftCurve.Evaluate(time), b) * maxBladeLift;
				bladeJoints[i].localPosition = localPosition;
			}
			else if (bladeOuterTriggerZones[i].ActiveRigidbodiesTriggerInfo.Count == 0)
			{
				Vector3 localPosition2 = bladeJoints[i].localPosition;
				if (localPosition2.y > 0f)
				{
					float b2 = Mathf.MoveTowards(localPosition2.y / maxBladeLift, 0f, Time.deltaTime * bladeLiftStrength);
					localPosition2.y = Mathf.Max(0f, b2) * maxBladeLift;
				}
				bladeJoints[i].localPosition = localPosition2;
			}
			Transform transform = bladeJoints[Mathf.Max(1, i - 1)];
			Transform transform2 = bladeJoints[Mathf.Min(bladeJoints.Length - 1, i + 1)];
			Vector3 vector = transform2.localPosition - transform.localPosition;
			float x = (0f - Mathf.Atan(vector.y / vector.z)) * 180f / (float)Math.PI * 0.3f;
			bladeJoints[i].localEulerAngles = new Vector3(x, 0f, 0f);
		}
	}
}
