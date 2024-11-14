using UnityEngine;

namespace OwlchemyVR
{
	public class EditorHmdDevice : MonoBehaviour, IHmdDevice
	{
		public float MovementSpeed;

		public float LookSensitivity;

		private PlayerController playerController;

		private Vector3 movement;

		private Vector3 look;

		private Transform headTransform;

		private void Awake()
		{
		}

		private void OnEnable()
		{
			playerController = Object.FindObjectOfType<PlayerController>();
			headTransform = playerController.Hmd.transform;
			playerController.ResetView();
			Cursor.lockState = CursorLockMode.None;
		}

		private void OnDisable()
		{
		}

		public HmdState GetSuggestedHmdState(HMD hmd)
		{
			HmdState hmdState = new HmdState();
			hmdState.Position = headTransform.parent.InverseTransformPoint(headTransform.position + headTransform.TransformDirection(movement) * MovementSpeed * Time.deltaTime);
			hmdState.EulerAngles = headTransform.eulerAngles + look * LookSensitivity;
			return hmdState;
		}

		private void Update()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			movement = Vector3.zero;
			if (Input.GetKey(KeyCode.W))
			{
				movement += Vector3.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				movement += Vector3.back;
			}
			if (Input.GetKey(KeyCode.D))
			{
				movement += Vector3.right;
			}
			if (Input.GetKey(KeyCode.A))
			{
				movement += Vector3.left;
			}
			if (Input.GetKeyDown(KeyCode.V))
			{
				playerController.ResetView();
			}
			if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt))
			{
				look = new Vector3(0f - Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
			}
			else
			{
				look = Vector3.zero;
			}
		}
	}
}
