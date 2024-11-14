using System.Collections;
using UnityEngine;

namespace OwlchemyVR
{
	public class MorpheusHMDAndInputController : MasterHMDAndInputController
	{
		public const float DESIRED_RENDER_SCALE = 1.4f;

		[SerializeField]
		private Transform controllerTransformR;

		[SerializeField]
		private Transform controllerTransformL;

		private int trackedController1Index;

		private int trackedController2Index = 1;

		private int moveController1Index;

		private int moveController2Index = 1;

		private bool isMoveControllersHandlesRegistered;

		public int TrackedController1Index
		{
			get
			{
				return trackedController1Index;
			}
		}

		public int TrackedController2Index
		{
			get
			{
				return trackedController2Index;
			}
		}

		public override MonoBehaviour[] TrackingScripts
		{
			get
			{
				return null;
			}
		}

		public bool IsMoveControllersHandlesRegistered
		{
			get
			{
				return isMoveControllersHandlesRegistered;
			}
		}

		public override void Start()
		{
			trackedHmdTransform = Camera.main.transform;
			StartCoroutine(WaitForControllersAndSetupWhenReady());
		}

		private IEnumerator WaitForControllersAndSetupWhenReady()
		{
			yield return null;
		}

		private void SetupMorpheusHandControllers(Vector3 controller1Pos, Vector3 controller2Pos)
		{
			Transform transform = Object.Instantiate(controllerPrefab).transform;
			Transform transform2 = Object.Instantiate(controllerPrefab).transform;
			transform.SetParent(controllerTransformR, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform2.SetParent(controllerTransformL, false);
			transform2.localPosition = Vector3.zero;
			transform2.localRotation = Quaternion.identity;
			ControllerConfigurator component = transform.GetComponent<ControllerConfigurator>();
			controllersConfigs.Add(component);
			ControllerConfigurator component2 = transform2.GetComponent<ControllerConfigurator>();
			controllersConfigs.Add(component2);
			component.SetHandedness(false);
			component2.SetHandedness(true);
			rightHandController = component.GetComponent<InteractionHandController>();
			leftHandController = component2.GetComponent<InteractionHandController>();
			if (DoDotProductHandDetermination(trackedHmdTransform, controller1Pos, controller2Pos))
			{
				component2.GetComponent<Morpheus_IndividualController>().Setup(trackedController1Index, moveController1Index);
				component.GetComponent<Morpheus_IndividualController>().Setup(trackedController2Index, moveController2Index);
			}
			else
			{
				component2.GetComponent<Morpheus_IndividualController>().Setup(trackedController2Index, moveController2Index);
				component.GetComponent<Morpheus_IndividualController>().Setup(trackedController1Index, moveController1Index);
			}
		}

		public void RedoMorpheusHandedness()
		{
		}

		private bool DoDotProductHandDetermination(Transform hmdTransform, Vector3 leftControllerPos, Vector3 rightControllerPos)
		{
			float num = Vector3.Dot((hmdTransform.localPosition - leftControllerPos).normalized, hmdTransform.right);
			float num2 = Vector3.Dot((hmdTransform.localPosition - rightControllerPos).normalized, hmdTransform.right);
			if (num > 0f && num2 < 0f)
			{
				return true;
			}
			if (num < 0f && num2 > 0f)
			{
				return false;
			}
			Debug.LogWarning("Handedness failed, hmdposition:" + hmdTransform.localPosition);
			return true;
		}

		public override void Update()
		{
			base.Update();
		}

		private void ResetControllerTracking()
		{
			UnregisterMoveControllers();
			RegisterMoveControllers();
		}

		private void RegisterMoveControllers()
		{
		}

		private void UnregisterMoveControllers()
		{
		}
	}
}
