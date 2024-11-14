using UnityEngine;

public class ForceLockJoint : MonoBehaviour
{
	[SerializeField]
	public bool lockX;

	[SerializeField]
	public bool lockY;

	[SerializeField]
	public bool lockZ;

	[SerializeField]
	public bool lockXRot;

	[SerializeField]
	public bool lockYRot;

	[SerializeField]
	public bool lockZRot;

	[SerializeField]
	private bool applyAtStart;

	[SerializeField]
	public float tolerance;

	[SerializeField]
	public float angularTolerance;

	[SerializeField]
	private bool forceNeverDoPosition;

	[SerializeField]
	private bool forceNeverDoRotation;

	private Rigidbody r;

	private Vector3 prevLocalPos;

	private Quaternion prevLocalRot;

	private void Awake()
	{
		r = GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		if (!applyAtStart)
		{
			ResetMemory();
		}
	}

	private void Start()
	{
		if (applyAtStart)
		{
			ResetMemory();
		}
	}

	public void ResetMemory()
	{
		ResetPositionMemory();
		ResetRotationMemory();
	}

	public void ResetPositionMemory()
	{
		prevLocalPos = base.transform.localPosition;
	}

	public void ResetRotationMemory()
	{
		prevLocalRot = base.transform.localRotation;
	}

	public void CopyLocksFromJoint(ConfigurableJoint joint)
	{
		lockX = joint.xMotion == ConfigurableJointMotion.Locked;
		lockY = joint.yMotion == ConfigurableJointMotion.Locked;
		lockZ = joint.zMotion == ConfigurableJointMotion.Locked;
		lockXRot = joint.angularXMotion == ConfigurableJointMotion.Locked;
		lockYRot = joint.angularYMotion == ConfigurableJointMotion.Locked;
		lockZRot = joint.angularZMotion == ConfigurableJointMotion.Locked;
	}

	public void LockAll()
	{
		lockX = true;
		lockY = true;
		lockZ = true;
		lockXRot = true;
		lockYRot = true;
		lockZRot = true;
	}

	public void SetupLock(bool lockX, bool lockY, bool lockZ, bool lockXRot, bool lockYRot, bool lockZRot)
	{
		this.lockX = lockX;
		this.lockY = lockY;
		this.lockZ = lockZ;
		this.lockXRot = lockXRot;
		this.lockYRot = lockYRot;
		this.lockZRot = lockZRot;
	}

	private void LateUpdate()
	{
		if (r.IsSleeping() || r.isKinematic)
		{
			return;
		}
		if ((lockX || lockY || lockZ) && !forceNeverDoPosition)
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 vector = localPosition;
			Vector3 vector2 = vector;
			if (lockX)
			{
				if (Mathf.Abs(vector.x - prevLocalPos.x) > tolerance)
				{
					vector.x = prevLocalPos.x;
				}
				vector2.x = prevLocalPos.x;
			}
			if (lockY)
			{
				if (Mathf.Abs(vector.y - prevLocalPos.y) > tolerance)
				{
					vector.y = prevLocalPos.y;
				}
				vector2.y = prevLocalPos.y;
			}
			if (lockZ)
			{
				if (Mathf.Abs(vector.z - prevLocalPos.z) > tolerance)
				{
					vector.z = prevLocalPos.z;
				}
				vector2.z = prevLocalPos.z;
			}
			prevLocalPos = vector2;
			if (vector != localPosition)
			{
				base.transform.localPosition = vector;
				r.MovePosition(base.transform.position);
			}
		}
		else
		{
			prevLocalPos = base.transform.localPosition;
		}
		if ((lockXRot || lockYRot || lockZRot) && !forceNeverDoRotation)
		{
			Quaternion localRotation = base.transform.localRotation;
			Quaternion quaternion = localRotation;
			Vector3 eulerAngles = (Quaternion.Inverse(prevLocalRot) * quaternion).eulerAngles;
			Vector3 euler = eulerAngles;
			if (lockXRot)
			{
				if (Mathf.Abs(eulerAngles.x) > angularTolerance)
				{
					eulerAngles.x = 0f;
				}
				euler.x = 0f;
			}
			if (lockYRot)
			{
				if (Mathf.Abs(eulerAngles.y) > angularTolerance)
				{
					eulerAngles.y = 0f;
				}
				euler.y = 0f;
			}
			if (lockZRot)
			{
				if (Mathf.Abs(eulerAngles.z) > angularTolerance)
				{
					eulerAngles.z = 0f;
				}
				euler.z = 0f;
			}
			quaternion = prevLocalRot * Quaternion.Euler(eulerAngles);
			prevLocalRot *= Quaternion.Euler(euler);
			if (quaternion != localRotation)
			{
				base.transform.localRotation = quaternion;
				r.MoveRotation(base.transform.rotation);
			}
		}
		else
		{
			prevLocalRot = base.transform.localRotation;
		}
	}
}
