using UnityEngine;

public class GarageDoorController : MonoBehaviour
{
	public delegate void DoorMovement();

	private const string doorOpen = "GarageDoorOpen";

	private Animator anim;

	[SerializeField]
	private bool startOpen;

	public bool isOpen { get; private set; }

	public event DoorMovement OnDoorOpen;

	public event DoorMovement OnDoorClose;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		if (startOpen)
		{
			OpenDoor();
		}
	}

	public void OpenDoor()
	{
		if (this.OnDoorOpen != null)
		{
			this.OnDoorOpen();
		}
		anim.SetBool("GarageDoorOpen", true);
		isOpen = true;
	}

	public void CloseDoor()
	{
		if (this.OnDoorClose != null)
		{
			this.OnDoorClose();
		}
		anim.SetBool("GarageDoorOpen", false);
		isOpen = false;
	}
}
