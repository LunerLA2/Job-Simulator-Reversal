using OwlchemyVR;
using UnityEngine;

public class BasicRopeRelease : MonoBehaviour
{
	[SerializeField]
	private PickupableItem[] itemsToDrop;

	[SerializeField]
	private Transform nodeContainer;

	private UltimateRope rope;

	private float highStretchLevelDuration;

	private void Awake()
	{
	}

	private void Start()
	{
		rope = nodeContainer.GetComponent<UltimateRope>();
	}

	private void Update()
	{
		float num = rope.RopeNodes[0].fLength + rope.m_fCurrentExtension;
		float num2 = num * 1.1f;
		float num3 = num * 1.2f;
		float num4 = 0f;
		for (int i = 0; i < rope.RopeNodes[0].linkJoints.Length; i++)
		{
			bool flag = i > 0;
			ConfigurableJoint configurableJoint = rope.RopeNodes[0].linkJoints[i];
			if (flag)
			{
				float num5 = 0f;
				ConfigurableJoint configurableJoint2 = rope.RopeNodes[0].linkJoints[i - 1];
				num5 = Vector3.Distance(configurableJoint.transform.position, configurableJoint2.transform.position);
				num4 += num5;
			}
		}
		bool flag2 = false;
		for (int j = 0; j < itemsToDrop.Length; j++)
		{
			if (itemsToDrop[j].CurrInteractableHand != null)
			{
				flag2 = true;
			}
		}
		if (flag2 && num4 > num2)
		{
			highStretchLevelDuration += Time.deltaTime;
			if (!(highStretchLevelDuration >= 0.2f) && !(num4 > num3))
			{
				return;
			}
			highStretchLevelDuration = 0f;
			for (int k = 0; k < itemsToDrop.Length; k++)
			{
				if (itemsToDrop[k].IsCurrInHand)
				{
					itemsToDrop[k].CurrInteractableHand.TryRelease();
				}
			}
		}
		else
		{
			highStretchLevelDuration = 0f;
		}
	}

	private void ResetRope()
	{
		Debug.LogWarning("Rope Reset");
		rope.Regenerate();
	}
}
