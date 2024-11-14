using UnityEngine;

public class TiedUpBot : MonoBehaviour
{
	[SerializeField]
	private CustomBot customBot;

	[SerializeField]
	private AudioClip escapingVO;

	private Vector3 sideRotation = new Vector3(0f, 0f, -90f);

	private void Awake()
	{
		customBot.transform.localEulerAngles = sideRotation;
	}

	public void TrunkGrabbedEvent()
	{
		customBot.transform.localEulerAngles = sideRotation;
	}
}
