using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OVRPlayerController : MonoBehaviour
{
	public float Acceleration = 0.1f;

	public float Damping = 0.3f;

	public float BackAndSideDampen = 0.5f;

	public float JumpForce = 0.3f;

	public float RotationAmount = 1.5f;

	public float RotationRatchet = 45f;

	public bool HmdResetsY = true;

	public bool HmdRotatesY = true;

	public float GravityModifier = 0.379f;

	public bool useProfileData = true;

	protected CharacterController Controller;

	protected OVRCameraRig CameraRig;

	private float MoveScale = 1f;

	private Vector3 MoveThrottle = Vector3.zero;

	private float FallSpeed;

	private OVRPose? InitialPose;

	private float InitialYRotation;

	private float MoveScaleMultiplier = 1f;

	private float RotationScaleMultiplier = 1f;

	private bool SkipMouseRotation;

	private bool HaltUpdateMovement;

	private bool prevHatLeft;

	private bool prevHatRight;

	private float SimulationRate = 60f;

	private void Start()
	{
		Vector3 localPosition = CameraRig.transform.localPosition;
		localPosition.z = OVRManager.profile.eyeDepth;
		CameraRig.transform.localPosition = localPosition;
	}

	private void Awake()
	{
		Controller = base.gameObject.GetComponent<CharacterController>();
		if (Controller == null)
		{
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");
		}
		OVRCameraRig[] componentsInChildren = base.gameObject.GetComponentsInChildren<OVRCameraRig>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
		}
		else if (componentsInChildren.Length > 1)
		{
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
		}
		else
		{
			CameraRig = componentsInChildren[0];
		}
		InitialYRotation = base.transform.rotation.eulerAngles.y;
	}

	private void OnEnable()
	{
		OVRManager.display.RecenteredPose += ResetOrientation;
		if (CameraRig != null)
		{
			CameraRig.UpdatedAnchors += UpdateTransform;
		}
	}

	private void OnDisable()
	{
		OVRManager.display.RecenteredPose -= ResetOrientation;
		if (CameraRig != null)
		{
			CameraRig.UpdatedAnchors -= UpdateTransform;
		}
	}

	protected virtual void Update()
	{
		if (useProfileData)
		{
			OVRPose? initialPose = InitialPose;
			if (!initialPose.HasValue)
			{
				InitialPose = new OVRPose
				{
					position = CameraRig.transform.localPosition,
					orientation = CameraRig.transform.localRotation
				};
			}
			Vector3 localPosition = CameraRig.transform.localPosition;
			if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
			{
				localPosition.y = OVRManager.profile.eyeHeight - 0.5f * Controller.height + Controller.center.y;
			}
			else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
			{
				localPosition.y = 0f - 0.5f * Controller.height + Controller.center.y;
			}
			CameraRig.transform.localPosition = localPosition;
		}
		else
		{
			OVRPose? initialPose2 = InitialPose;
			if (initialPose2.HasValue)
			{
				CameraRig.transform.localPosition = InitialPose.Value.position;
				CameraRig.transform.localRotation = InitialPose.Value.orientation;
				InitialPose = null;
			}
		}
		UpdateMovement();
		Vector3 zero = Vector3.zero;
		float num = 1f + Damping * SimulationRate * Time.deltaTime;
		MoveThrottle.x /= num;
		MoveThrottle.y = ((!(MoveThrottle.y > 0f)) ? MoveThrottle.y : (MoveThrottle.y / num));
		MoveThrottle.z /= num;
		zero += MoveThrottle * SimulationRate * Time.deltaTime;
		if (Controller.isGrounded && FallSpeed <= 0f)
		{
			FallSpeed = Physics.gravity.y * (GravityModifier * 0.002f);
		}
		else
		{
			FallSpeed += Physics.gravity.y * (GravityModifier * 0.002f) * SimulationRate * Time.deltaTime;
		}
		zero.y += FallSpeed * SimulationRate * Time.deltaTime;
		float num2 = 0f;
		if (Controller.isGrounded && MoveThrottle.y <= base.transform.lossyScale.y * 0.001f)
		{
			num2 = Mathf.Max(Controller.stepOffset, new Vector3(zero.x, 0f, zero.z).magnitude);
			zero -= num2 * Vector3.up;
		}
		Vector3 vector = Vector3.Scale(Controller.transform.localPosition + zero, new Vector3(1f, 0f, 1f));
		Controller.Move(zero);
		Vector3 vector2 = Vector3.Scale(Controller.transform.localPosition, new Vector3(1f, 0f, 1f));
		if (vector != vector2)
		{
			MoveThrottle += (vector2 - vector) / (SimulationRate * Time.deltaTime);
		}
	}

	public virtual void UpdateMovement()
	{
		if (!HaltUpdateMovement)
		{
			bool flag = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
			bool flag2 = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
			bool flag3 = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
			bool flag4 = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
			bool flag5 = false;
			if (OVRInput.Get(OVRInput.Button.DpadUp))
			{
				flag = true;
				flag5 = true;
			}
			if (OVRInput.Get(OVRInput.Button.DpadDown))
			{
				flag4 = true;
				flag5 = true;
			}
			MoveScale = 1f;
			if ((flag && flag2) || (flag && flag3) || (flag4 && flag2) || (flag4 && flag3))
			{
				MoveScale = 0.70710677f;
			}
			if (!Controller.isGrounded)
			{
				MoveScale = 0f;
			}
			MoveScale *= SimulationRate * Time.deltaTime;
			float num = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
			if (flag5 || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				num *= 2f;
			}
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z = (eulerAngles.x = 0f);
			Quaternion quaternion = Quaternion.Euler(eulerAngles);
			if (flag)
			{
				MoveThrottle += quaternion * (base.transform.lossyScale.z * num * Vector3.forward);
			}
			if (flag4)
			{
				MoveThrottle += quaternion * (base.transform.lossyScale.z * num * BackAndSideDampen * Vector3.back);
			}
			if (flag2)
			{
				MoveThrottle += quaternion * (base.transform.lossyScale.x * num * BackAndSideDampen * Vector3.left);
			}
			if (flag3)
			{
				MoveThrottle += quaternion * (base.transform.lossyScale.x * num * BackAndSideDampen * Vector3.right);
			}
			Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
			bool flag6 = OVRInput.Get(OVRInput.Button.PrimaryShoulder);
			if (flag6 && !prevHatLeft)
			{
				eulerAngles2.y -= RotationRatchet;
			}
			prevHatLeft = flag6;
			bool flag7 = OVRInput.Get(OVRInput.Button.SecondaryShoulder);
			if (flag7 && !prevHatRight)
			{
				eulerAngles2.y += RotationRatchet;
			}
			prevHatRight = flag7;
			if (Input.GetKeyDown(KeyCode.Q))
			{
				eulerAngles2.y -= RotationRatchet;
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				eulerAngles2.y += RotationRatchet;
			}
			float num2 = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;
			if (!SkipMouseRotation)
			{
				eulerAngles2.y += Input.GetAxis("Mouse X") * num2 * 3.25f;
			}
			num = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
			num *= 1f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
			Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
			if (vector.y > 0f)
			{
				MoveThrottle += quaternion * (vector.y * base.transform.lossyScale.z * num * Vector3.forward);
			}
			if (vector.y < 0f)
			{
				MoveThrottle += quaternion * (Mathf.Abs(vector.y) * base.transform.lossyScale.z * num * BackAndSideDampen * Vector3.back);
			}
			if (vector.x < 0f)
			{
				MoveThrottle += quaternion * (Mathf.Abs(vector.x) * base.transform.lossyScale.x * num * BackAndSideDampen * Vector3.left);
			}
			if (vector.x > 0f)
			{
				MoveThrottle += quaternion * (vector.x * base.transform.lossyScale.x * num * BackAndSideDampen * Vector3.right);
			}
			eulerAngles2.y += OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * num2;
			base.transform.rotation = Quaternion.Euler(eulerAngles2);
		}
	}

	public void UpdateTransform(OVRCameraRig rig)
	{
		Transform trackingSpace = CameraRig.trackingSpace;
		Transform centerEyeAnchor = CameraRig.centerEyeAnchor;
		if (HmdRotatesY)
		{
			Vector3 position = trackingSpace.position;
			Quaternion rotation = trackingSpace.rotation;
			base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
			trackingSpace.position = position;
			trackingSpace.rotation = rotation;
		}
	}

	public bool Jump()
	{
		if (!Controller.isGrounded)
		{
			return false;
		}
		MoveThrottle += new Vector3(0f, base.transform.lossyScale.y * JumpForce, 0f);
		return true;
	}

	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
		FallSpeed = 0f;
	}

	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}

	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}

	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}

	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}

	public void GetSkipMouseRotation(ref bool skipMouseRotation)
	{
		skipMouseRotation = SkipMouseRotation;
	}

	public void SetSkipMouseRotation(bool skipMouseRotation)
	{
		SkipMouseRotation = skipMouseRotation;
	}

	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = HaltUpdateMovement;
	}

	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		HaltUpdateMovement = haltUpdateMovement;
	}

	public void ResetOrientation()
	{
		if (HmdResetsY)
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.y = InitialYRotation;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
		}
	}
}
