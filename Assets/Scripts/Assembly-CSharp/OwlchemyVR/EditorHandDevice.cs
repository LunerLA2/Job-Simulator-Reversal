using UnityEngine;

namespace OwlchemyVR
{
	public class EditorHandDevice : MonoBehaviour, IHandDevice
	{
		public float MovementSpeed;

		public float RollSensitivity;

		private Transform headTransform;

		private Transform lhTransform;

		private Transform rhTransform;

		private Vector3 rhPosFromHead;

		private Vector3 lhPosFromHead;

		private Quaternion rhRotFromHead;

		private Quaternion lhRotFromHead;

		private PlayerController playerController;

		private InteractionHandController rhInteractionHandController;

		private InteractionHandController lhInteractionHandController;

		private Hand activeHand;

		private bool rhGrabbing;

		private bool lhGrabbing;

		private int touchLayerMask;

		private void Awake()
		{
			touchLayerMask = LayerMaskHelper.EverythingBut(LayerMask.NameToLayer("Hand"), LayerMask.NameToLayer("HandGrabTrigger"));
		}

		private void OnEnable()
		{
			playerController = Object.FindObjectOfType<PlayerController>();
			activeHand = playerController.RightHand;
			headTransform = playerController.Hmd.transform;
			rhTransform = playerController.RightHand.transform;
			lhTransform = playerController.LeftHand.transform;
			rhInteractionHandController = rhTransform.GetComponent<InteractionHandController>();
			lhInteractionHandController = lhTransform.GetComponent<InteractionHandController>();
			ResetHands();
		}

		private void ResetHands()
		{
			playerController.ResetBothHands();
			rhGrabbing = false;
			lhGrabbing = false;
			ResetDesiredRelativeHandTransformations();
		}

		private void ResetDesiredRelativeHandTransformations()
		{
			rhPosFromHead = headTransform.InverseTransformPoint(rhTransform.localPosition);
			rhRotFromHead = Quaternion.Inverse(headTransform.localRotation) * rhTransform.localRotation;
			lhPosFromHead = headTransform.InverseTransformPoint(lhTransform.localPosition);
			lhRotFromHead = Quaternion.Inverse(headTransform.localRotation) * lhTransform.localRotation;
		}

		private void OnDisable()
		{
		}

		private void Update()
		{
			if (rhGrabbing && !rhInteractionHandController.IsGrabbableCurrInHand)
			{
				rhGrabbing = false;
			}
			if (lhGrabbing && !lhInteractionHandController.IsGrabbableCurrInHand)
			{
				lhGrabbing = false;
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				activeHand = playerController.RightHand;
			}
			else if (Input.GetKeyDown(KeyCode.Q))
			{
				activeHand = playerController.LeftHand;
			}
			if (Input.GetKey(KeyCode.T))
			{
				TouchTargetSurface();
			}
			else if (Input.GetKeyDown(KeyCode.H))
			{
				ResetHands();
			}
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				if (activeHand.Handedness == Handedness.Right)
				{
					rhGrabbing = !rhGrabbing;
				}
				else
				{
					lhGrabbing = !lhGrabbing;
				}
			}
			Vector3 vector = Vector3.zero;
			float num = 0f;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				vector = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
			}
			else if (Input.GetKey(KeyCode.LeftControl))
			{
				vector = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));
			}
			else if (Input.GetKey(KeyCode.LeftAlt))
			{
				num = 0f - Input.GetAxis("Mouse X");
			}
			if (vector != Vector3.zero)
			{
				if (activeHand.Handedness == Handedness.Right)
				{
					rhPosFromHead += vector * MovementSpeed * Time.deltaTime;
				}
				else
				{
					lhPosFromHead += vector * MovementSpeed * Time.deltaTime;
				}
			}
			else if (num != 0f)
			{
				if (activeHand.Handedness == Handedness.Right)
				{
					rhRotFromHead *= Quaternion.Euler(0f, 0f, num * RollSensitivity);
				}
				else
				{
					lhRotFromHead *= Quaternion.Euler(0f, 0f, num * RollSensitivity);
				}
			}
		}

		public HandState GetSuggestedHandState(Hand hand)
		{
			HandState handState = new HandState();
			if (hand.Handedness == Handedness.Right)
			{
				handState.Position = headTransform.TransformPoint(rhPosFromHead);
				handState.EulerAngles = (headTransform.localRotation * rhRotFromHead).eulerAngles;
				handState.CommonButtons = new bool[2]
				{
					rhGrabbing,
					hand == activeHand && Input.GetKey(KeyCode.Mouse1)
				};
			}
			else
			{
				handState.Position = headTransform.TransformPoint(lhPosFromHead);
				handState.EulerAngles = (headTransform.localRotation * lhRotFromHead).eulerAngles;
				handState.CommonButtons = new bool[2]
				{
					lhGrabbing,
					hand == activeHand && Input.GetKey(KeyCode.Mouse1)
				};
			}
			return handState;
		}

		public void TriggerHapticPulse(Hand hand, float pulseRate)
		{
			if (pulseRate != 0f)
			{
				Debug.Log(string.Concat(hand.Handedness, " hand says: Yo! I'm getting a haptic pulse!"));
			}
		}

		private void TouchTargetSurface()
		{
			Ray ray = new Ray(playerController.Hmd.transform.position, playerController.Hmd.transform.forward);
			RaycastHit[] array = Physics.RaycastAll(ray, 2f, touchLayerMask);
			playerController.ResetHand(activeHand.Handedness);
			if (array.Length > 0)
			{
				activeHand.transform.position = array[0].point - ray.direction * 0.2f + playerController.Hmd.transform.up * 0.05f;
			}
			ResetDesiredRelativeHandTransformations();
		}
	}
}
