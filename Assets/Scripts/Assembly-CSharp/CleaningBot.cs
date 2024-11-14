using System;
using OwlchemyVR;
using UnityEngine;

public class CleaningBot : MonoBehaviour
{
	[SerializeField]
	private float movementSpeed = 0.5f;

	[SerializeField]
	private float turnSpeed = 180f;

	[SerializeField]
	private Vector3 hardCodedCleaningBotExit;

	[SerializeField]
	private float staticDistance = 0.2f;

	[SerializeField]
	private Vector3 limitOrigin = Vector3.zero;

	[SerializeField]
	private Vector3 limitSize = Vector3.one;

	[SerializeField]
	private Vector3 psvrLimitOrigin = Vector3.zero;

	[SerializeField]
	private Vector3 psvrLimitSize = Vector3.one;

	[SerializeField]
	private MeshRenderer staticRenderer;

	[SerializeField]
	private AudioClip idleClip;

	[SerializeField]
	private AudioClip drivingClip;

	[SerializeField]
	private AudioClip hitStainClip;

	private JoystickController joystick;

	private CleaningBotControlPanel controlPanel;

	private bool isPoweredOn;

	[SerializeField]
	private GameObject[] objectsToShowWhenPoweredOn;

	[SerializeField]
	private ParticleSystem pfxOnPowerStateChange;

	[SerializeField]
	private AudioClip soundWhenPoweredOn;

	[SerializeField]
	private AudioClip soundWhenPoweredOff;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private TriggerListener triggerListener;

	[SerializeField]
	private WorldItemData validData;

	[SerializeField]
	private Camera renderTextureCamera;

	[SerializeField]
	private float cameraUpdateTime;

	[SerializeField]
	private float cameraUpdateTimePSVR = 0.1f;

	private float lastCameraUpdate;

	private float movementValue;

	private float turnValue;

	[SerializeField]
	private AudioSourceHelper currentClip;

	private void Awake()
	{
		if (cameraUpdateTime > 0f)
		{
			renderTextureCamera.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		TriggerListener obj = triggerListener;
		obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(obj.OnEnter, new Action<TriggerEventInfo>(SomethingEnteredTrigger));
		staticRenderer.material.color = Color.clear;
	}

	private void OnDisable()
	{
		TriggerListener obj = triggerListener;
		obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(obj.OnEnter, new Action<TriggerEventInfo>(SomethingEnteredTrigger));
	}

	public void Setup(JoystickController _joystick, CleaningBotControlPanel _controlPanel)
	{
		joystick = _joystick;
		controlPanel = _controlPanel;
	}

	public void SetPowerState(bool power)
	{
		if (power == isPoweredOn)
		{
			return;
		}
		isPoweredOn = power;
		for (int i = 0; i < objectsToShowWhenPoweredOn.Length; i++)
		{
			objectsToShowWhenPoweredOn[i].SetActive(power);
		}
		if (isPoweredOn)
		{
			if (soundWhenPoweredOn != null)
			{
				AudioManager.Instance.Play(base.transform.position, soundWhenPoweredOn, 1f, 1f);
			}
		}
		else if (soundWhenPoweredOff != null)
		{
			AudioManager.Instance.Play(base.transform.position, soundWhenPoweredOff, 1f, 1f);
		}
		if (pfxOnPowerStateChange != null)
		{
			pfxOnPowerStateChange.Play();
		}
	}

	private void Update()
	{
		if (!isPoweredOn)
		{
			return;
		}
		movementValue = joystick.JoystickDirection.y;
		turnValue = joystick.JoystickDirection.x;
		MotorAudio();
		bool flag = false;
		if (controlPanel != null)
		{
			flag = controlPanel.ScreenIsVisible;
		}
		if (!flag)
		{
			return;
		}
		float num = 0f;
		if (cameraUpdateTime > 0f)
		{
			num = cameraUpdateTime;
			if (Time.time - lastCameraUpdate >= num)
			{
				renderTextureCamera.Render();
				lastCameraUpdate = Time.time;
			}
		}
	}

	private void LateUpdate()
	{
		if (isPoweredOn)
		{
			if (base.transform.position.x < limitOrigin.x - limitSize.x / 2f)
			{
				base.transform.SetGlobalPositionXOnly(limitOrigin.x - limitSize.x / 2f);
			}
			if (base.transform.position.x > limitOrigin.x + limitSize.x / 2f)
			{
				base.transform.SetGlobalPositionXOnly(limitOrigin.x + limitSize.x / 2f);
			}
			if (base.transform.position.z < limitOrigin.z - limitSize.z / 2f)
			{
				base.transform.SetGlobalPositionZOnly(limitOrigin.z - limitSize.z / 2f);
			}
			if (base.transform.position.z > limitOrigin.z + limitSize.z / 2f)
			{
				base.transform.SetGlobalPositionZOnly(limitOrigin.z + limitSize.z / 2f);
			}
		}
	}

	private void MotorAudio()
	{
		if (Mathf.Abs(movementValue) < 0.1f && Mathf.Abs(turnValue) < 0.1f)
		{
			if (currentClip.GetClip() == drivingClip)
			{
				currentClip.SetClip(idleClip);
				currentClip.Play();
			}
		}
		else if (currentClip.GetClip() == idleClip)
		{
			currentClip.SetClip(drivingClip);
			currentClip.Play();
		}
	}

	private void FixedUpdate()
	{
		Move();
		Turn();
	}

	private void Move()
	{
		Vector3 vector = base.transform.forward * movementValue * movementSpeed * Time.deltaTime;
		if (!((rb.position + vector).x <= hardCodedCleaningBotExit.x))
		{
			if (Vector3.Distance(hardCodedCleaningBotExit, base.transform.position) < staticDistance)
			{
				float num = Vector3.Distance(hardCodedCleaningBotExit, base.transform.position);
				float t = num / staticDistance;
				staticRenderer.material.color = Color.Lerp(Color.white, Color.clear, t);
			}
			rb.MovePosition(rb.position + vector);
		}
	}

	private void Turn()
	{
		rb.angularVelocity = Vector3.zero;
		float y = turnValue * turnSpeed * Time.deltaTime;
		Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
		rb.MoveRotation(rb.rotation * quaternion);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(hardCodedCleaningBotExit, staticDistance);
		Gizmos.DrawWireCube(limitOrigin, limitSize);
		Gizmos.DrawWireCube(psvrLimitOrigin, psvrLimitSize);
	}

	private void SomethingEnteredTrigger(TriggerEventInfo info)
	{
		if (info.other.attachedRigidbody == null)
		{
			return;
		}
		WorldItem component = info.other.attachedRigidbody.GetComponent<WorldItem>();
		if (component != null && component.Data == validData)
		{
			GameEventsManager.Instance.ItemActionOccurred(component.Data, "USED");
			FloorStainController component2 = info.other.gameObject.GetComponent<FloorStainController>();
			if ((bool)component2)
			{
				component2.Hit();
				AudioManager.Instance.Play(base.transform.position, hitStainClip, 1f, 1f);
			}
		}
	}
}
