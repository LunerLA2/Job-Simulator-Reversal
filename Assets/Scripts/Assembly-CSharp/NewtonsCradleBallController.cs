using System.Collections.Generic;
using UnityEngine;

public class NewtonsCradleBallController : MonoBehaviour
{
	public const float MAGIC_ENERGY_TRANSFER_RATIO = 1.3f;

	private List<NewtonsCradleBallController> touchingBalls;

	private SpringJoint[] joints;

	private bool transferredEnergy;

	private Rigidbody rb;

	private Collider col;

	private float neutralStretchDist;

	private Vector3 top;

	private Vector3 anchor0;

	private Vector3 anchor1;

	public Rigidbody Rigidbody
	{
		get
		{
			return rb;
		}
	}

	public Vector3 Top
	{
		get
		{
			return top;
		}
	}

	public Vector3 Anchor0
	{
		get
		{
			return anchor0;
		}
	}

	public Vector3 Anchor1
	{
		get
		{
			return anchor1;
		}
	}

	private void Awake()
	{
		touchingBalls = new List<NewtonsCradleBallController>();
		joints = GetComponents<SpringJoint>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		top = base.transform.TransformPoint(new Vector3(0f, 0.5f, 0f));
		anchor0 = base.transform.parent.TransformPoint(joints[0].connectedAnchor);
		anchor1 = base.transform.parent.TransformPoint(joints[1].connectedAnchor);
		neutralStretchDist = Vector3.Distance((anchor0 + anchor1) / 2f, top);
	}

	private void Update()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.y = 0f;
		base.transform.localEulerAngles = localEulerAngles;
		top = base.transform.TransformPoint(new Vector3(0f, 0.5f, 0f));
		anchor0 = base.transform.parent.TransformPoint(joints[0].connectedAnchor);
		anchor1 = base.transform.parent.TransformPoint(joints[1].connectedAnchor);
		float num = Vector3.Distance((anchor0 + anchor1) / 2f, top);
		col.enabled = num <= neutralStretchDist * 1.25f;
	}

	private void FixedUpdate()
	{
		transferredEnergy = false;
	}

	public void TransferEnergy(NewtonsCradleBallController incomingBall, Vector3 energy)
	{
		if (transferredEnergy)
		{
			return;
		}
		transferredEnergy = true;
		rb.velocity = Vector3.zero;
		float num = Vector3.Dot(incomingBall.transform.position - base.transform.position, base.transform.forward);
		for (int i = 0; i < touchingBalls.Count; i++)
		{
			NewtonsCradleBallController newtonsCradleBallController = touchingBalls[i];
			if (!(newtonsCradleBallController == this) && !(newtonsCradleBallController == incomingBall))
			{
				float num2 = Vector3.Dot(newtonsCradleBallController.transform.position - base.transform.position, base.transform.forward);
				if (num * num2 < 0f)
				{
					rb.velocity = Vector3.zero;
					newtonsCradleBallController.TransferEnergy(this, energy);
					return;
				}
			}
		}
		rb.velocity = Vector3.Project(energy, base.transform.parent.forward) * 1.3f;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.rigidbody == null)
		{
			return;
		}
		NewtonsCradleBallController component = collision.rigidbody.GetComponent<NewtonsCradleBallController>();
		if (component == null)
		{
			return;
		}
		Vector3 velocity = component.rb.velocity;
		float num = Vector3.Dot(component.transform.position - base.transform.position, base.transform.forward);
		for (int i = 0; i < touchingBalls.Count; i++)
		{
			NewtonsCradleBallController newtonsCradleBallController = touchingBalls[i];
			if (!(newtonsCradleBallController == this) && !(newtonsCradleBallController == component))
			{
				float num2 = Vector3.Dot(newtonsCradleBallController.transform.position - base.transform.position, base.transform.forward);
				if (num * num2 < 0f)
				{
					component.rb.velocity = Vector3.zero;
					component.transform.position = base.transform.position + (component.transform.position - base.transform.position).normalized * base.transform.localScale.z;
					newtonsCradleBallController.TransferEnergy(this, velocity);
					rb.velocity = Vector3.zero;
					break;
				}
			}
		}
		if (!touchingBalls.Contains(component))
		{
			touchingBalls.Add(component);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!(collision.rigidbody == null))
		{
			NewtonsCradleBallController component = collision.rigidbody.GetComponent<NewtonsCradleBallController>();
			if (!(component == null))
			{
				touchingBalls.Remove(component);
			}
		}
	}
}
