using UnityEngine;

namespace OwlchemyVR
{
	public class PlayerController : MonoBehaviour
	{
		public HMD HeadPrefab;

		public Hand HandPrefab;

		public Vector3 NeutralHeadPosition;

		public Vector3 NeutralRightHandPositionFromHead;

		public Vector3 NeutralLeftHandPositionFromHead;

		private bool isUsingEditorDevices;

		private IHmdDevice overridenHmdDevice;

		private IHandDevice overridenHandDevice;

		public HMD Hmd { get; private set; }

		public Hand RightHand { get; private set; }

		public Hand LeftHand { get; private set; }

		public IHmdDevice HmdDevice { get; private set; }

		public IHandDevice HandDevice { get; private set; }

		public Hand[] Hands { get; private set; }

		private void Awake()
		{
			AddHead();
			AddHand(Handedness.Right);
			AddHand(Handedness.Left);
			Hands = new Hand[2] { LeftHand, RightHand };
			ResetView();
			ResetBothHands();
		}

		private void Start()
		{
			ChangeHmdDevice(GetComponentInChildren<SteamVRHmdDevice>());
			ChangeHandDevice(GetComponentInChildren<SteamVRHandDevice>());
		}

		private void AddHead()
		{
			Hmd = Object.Instantiate(HeadPrefab);
			Hmd.Init(this);
		}

		public Hand GetHand(Handedness handedness)
		{
			return (handedness != Handedness.Right) ? LeftHand : RightHand;
		}

		private void AddHand(Handedness handedness)
		{
			Hand hand = Object.Instantiate(HandPrefab);
			if (handedness == Handedness.Right)
			{
				RightHand = hand;
			}
			else
			{
				LeftHand = hand;
			}
			hand.Init(this, handedness);
		}

		public void ResetView()
		{
			Hmd.transform.position = NeutralHeadPosition;
			Hmd.transform.rotation = Quaternion.identity;
		}

		public void ResetBothHands()
		{
			ResetHand(Handedness.Right);
			ResetHand(Handedness.Left);
		}

		public void ResetHand(Handedness handedness)
		{
			Hand hand = GetHand(handedness);
			hand.transform.localPosition = Hmd.transform.TransformPoint((handedness != Handedness.Right) ? NeutralLeftHandPositionFromHead : NeutralRightHandPositionFromHead);
			hand.transform.localRotation = Hmd.transform.localRotation;
			hand.transform.Rotate(0f, 0f, (handedness != Handedness.Right) ? (-90) : 90);
		}

		private void ChangeHmdDevice(IHmdDevice hmdDevice)
		{
			if (HmdDevice != null)
			{
				(HmdDevice as MonoBehaviour).enabled = false;
			}
			HmdDevice = hmdDevice;
			if (HmdDevice != null)
			{
				(HmdDevice as MonoBehaviour).enabled = true;
			}
		}

		private void ChangeHandDevice(IHandDevice handDevice)
		{
			if (HandDevice != null)
			{
				(HandDevice as MonoBehaviour).enabled = false;
			}
			HandDevice = handDevice;
			if (HandDevice != null)
			{
				(HandDevice as MonoBehaviour).enabled = true;
			}
		}

		private void Update()
		{
			if (HmdDevice != null)
			{
				Hmd.SuggestState(HmdDevice.GetSuggestedHmdState(Hmd));
			}
			if (HandDevice != null)
			{
				RightHand.SuggestState(HandDevice.GetSuggestedHandState(RightHand));
				LeftHand.SuggestState(HandDevice.GetSuggestedHandState(LeftHand));
			}
		}
	}
}
