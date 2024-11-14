using UnityEngine;

public class RigidbodyBackupStoreObject
{
	private GameObject rigidbodyGameObject;

	private float mass;

	private float drag;

	private float angularDrag;

	private bool useGravity;

	private bool isKinematic;

	private RigidbodyInterpolation interpolation;

	private CollisionDetectionMode collisionDetectionMode;

	private RigidbodyConstraints constraints;

	private bool detectCollisions;

	private bool freezeRotation;

	public GameObject RigidbodyGameObject
	{
		get
		{
			return rigidbodyGameObject;
		}
	}

	public void BackupRigidbody(Rigidbody r)
	{
		rigidbodyGameObject = r.gameObject;
		mass = r.mass;
		drag = r.drag;
		angularDrag = r.angularDrag;
		useGravity = r.useGravity;
		isKinematic = r.isKinematic;
		interpolation = r.interpolation;
		collisionDetectionMode = r.collisionDetectionMode;
		constraints = r.constraints;
		detectCollisions = r.detectCollisions;
		freezeRotation = r.freezeRotation;
	}

	public void RestoreRigidbody(Rigidbody r)
	{
		r.mass = mass;
		r.drag = drag;
		r.angularDrag = angularDrag;
		r.useGravity = useGravity;
		r.isKinematic = isKinematic;
		r.interpolation = interpolation;
		r.collisionDetectionMode = collisionDetectionMode;
		r.constraints = constraints;
		r.detectCollisions = detectCollisions;
		r.freezeRotation = freezeRotation;
	}
}
