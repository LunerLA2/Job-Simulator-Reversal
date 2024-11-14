using System;
using OwlchemyVR;
using UnityEngine;

public class StaplerController : MonoBehaviour
{
	[SerializeField]
	private HingeJoint hinge;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private Transform topTransform;

	[SerializeField]
	private Transform baseTransform;

	[SerializeField]
	private Transform stapleLauncherTransform;

	[SerializeField]
	private float compressionAngle;

	[SerializeField]
	private float decompressionAngle = 6f;

	[SerializeField]
	private float restingHingeTargetAngle = 25f;

	[SerializeField]
	private JointSpring restingJointSettings;

	[SerializeField]
	private JointSpring pickedUpJointSettings;

	[SerializeField]
	private float clickCooldown = 0.15f;

	[SerializeField]
	private StapleController staplePrefab;

	[SerializeField]
	private StapleController crushedStaplePrefab;

	[SerializeField]
	private float stapleLaunchSpeed = 2f;

	private LoopingObjectPool<StapleController> staplePool;

	private LoopingObjectPool<StapleController> stapleCrushedPool;

	[SerializeField]
	private AudioClip[] stapleLaunchClips;

	[SerializeField]
	private AudioSourceHelper audioSource;

	private ElementSequence<AudioClip> stapleLaunchClipSequence;

	[SerializeField]
	private Animation stapleAnimation;

	private Vector3 prevPos;

	[SerializeField]
	private float requiredForwardVelocity = 0.3f;

	private bool wasPrevDirectionNegative;

	private bool isPickedUp;

	private bool isCompressed = true;

	private float clickCooldownLeft;

	private int hapticsRateMicroSec = 650;

	private float hapticsLengthSeconds = 0.02f;

	private HapticInfoObject hapticObject;

	private void Awake()
	{
		stapleLaunchClipSequence = new ElementSequence<AudioClip>(stapleLaunchClips);
		int capacity = ((!VRPlatform.IsLowPerformancePlatform) ? 10 : 5);
		int capacity2 = ((!VRPlatform.IsLowPerformancePlatform) ? 7 : 3);
		staplePool = new LoopingObjectPool<StapleController>(staplePrefab, capacity, GlobalStorage.Instance.ContentRoot);
		stapleCrushedPool = new LoopingObjectPool<StapleController>(crushedStaplePrefab, capacity2, GlobalStorage.Instance.ContentRoot);
		float length = hapticsLengthSeconds;
		hapticObject = new HapticInfoObject(hapticsRateMicroSec, length);
		hapticObject.DeactiveHaptic();
		Released(null);
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
		PickupableItem obj3 = pickupableItem;
		obj3.OnStartedUsing = (Action<GrabbableItem>)Delegate.Combine(obj3.OnStartedUsing, new Action<GrabbableItem>(StartedUsing));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
		PickupableItem obj3 = pickupableItem;
		obj3.OnStartedUsing = (Action<GrabbableItem>)Delegate.Remove(obj3.OnStartedUsing, new Action<GrabbableItem>(StartedUsing));
	}

	private void Update()
	{
		if (clickCooldownLeft > 0f)
		{
			clickCooldownLeft -= Time.deltaTime;
		}
		if (pickupableItem.IsCurrInHand)
		{
			ShakeUpdate();
			return;
		}
		float num = Vector3.Angle(topTransform.forward, baseTransform.forward);
		while (num > 180f)
		{
			num -= 360f;
			Debug.Log("Correcting angle");
		}
		while (num < -180f)
		{
			num += 360f;
			Debug.Log("Correcting angle");
		}
		if (!isCompressed && num <= compressionAngle)
		{
			if (!pickupableItem.IsCurrInHand)
			{
				Click();
			}
			isCompressed = true;
		}
		else if (isCompressed && num >= decompressionAngle)
		{
			isCompressed = false;
		}
	}

	private void ShakeUpdate()
	{
		Vector3 direction = (base.transform.position - prevPos) / Time.deltaTime;
		prevPos = base.transform.position;
		float y = base.transform.InverseTransformDirection(direction).y;
		if (y < 0f)
		{
			wasPrevDirectionNegative = true;
		}
		if (wasPrevDirectionNegative && y > requiredForwardVelocity)
		{
			Click();
			wasPrevDirectionNegative = false;
		}
	}

	private void Grabbed(GrabbableItem item)
	{
		hinge.useSpring = false;
	}

	private void Released(GrabbableItem item)
	{
		hinge.useSpring = true;
		ConfigureSpring(restingHingeTargetAngle);
		if (hapticObject.IsRunning && item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticObject);
		}
	}

	private void StartedUsing(GrabbableItem item)
	{
		Click();
	}

	private void ConfigureSpring(float targetAngle)
	{
		JointSpring spring = hinge.spring;
		spring.targetPosition = targetAngle;
		hinge.spring = spring;
	}

	private void Click()
	{
		if (clickCooldownLeft > 0f)
		{
			return;
		}
		clickCooldownLeft = clickCooldown;
		stapleAnimation.Stop();
		audioSource.SetClip(stapleLaunchClipSequence.GetNext());
		audioSource.Play();
		if (pickupableItem.IsCurrInHand)
		{
			hapticObject.Restart();
			if (!pickupableItem.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
			{
				pickupableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
			}
		}
		stapleAnimation.Play();
		if (pickupableItem.IsCurrInHand)
		{
			StapleController stapleController = staplePool.Fetch(stapleLauncherTransform.position, stapleLauncherTransform.rotation);
			stapleController.ResetState();
			stapleController.GetComponent<Rigidbody>().velocity = stapleController.transform.forward * stapleLaunchSpeed;
		}
		else
		{
			StapleController stapleController2 = stapleCrushedPool.Fetch(stapleLauncherTransform.position, stapleLauncherTransform.rotation);
			stapleController2.ResetState();
		}
	}
}
