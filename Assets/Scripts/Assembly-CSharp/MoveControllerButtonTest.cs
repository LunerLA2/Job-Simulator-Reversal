using OwlchemyVR;
using UnityEngine;

public class MoveControllerButtonTest : MonoBehaviour
{
	private MeshRenderer mr;

	private HandController leftHand;

	private void Start()
	{
		mr = GetComponent<MeshRenderer>();
		mr.material.color = Color.gray;
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.up * 0.5f);
		if (!(GlobalStorage.Instance.MasterHMDAndInputController.LeftHand == null))
		{
			if (leftHand == null)
			{
				leftHand = GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.HandController;
			}
			if (leftHand.GetButtonDown(HandController.HandControllerButton.AButton))
			{
				mr.material.color = Color.blue;
			}
			else if (leftHand.GetButtonDown(HandController.HandControllerButton.BButton))
			{
				mr.material.color = Color.red;
			}
			else if (leftHand.GetButtonDown(HandController.HandControllerButton.XButton))
			{
				mr.material.color = new Color(1f, 0.5f, 0.5f);
			}
			else if (leftHand.GetButtonDown(HandController.HandControllerButton.YButton))
			{
				mr.material.color = Color.green;
			}
			else if (leftHand.GetButtonDown(HandController.HandControllerButton.Menu))
			{
				mr.material.color = Color.white;
			}
			else if (leftHand.GetButtonDown(HandController.HandControllerButton.Trigger))
			{
				mr.material.color = Color.cyan;
			}
		}
	}
}
