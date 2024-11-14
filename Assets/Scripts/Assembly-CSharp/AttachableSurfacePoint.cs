using UnityEngine;

public class AttachableSurfacePoint : AttachablePoint
{
	[SerializeField]
	private bool useGuaranteedLocalZPos;

	[SerializeField]
	private float guaranteedLocalZPos;

	public override bool IsOccupied
	{
		get
		{
			return false;
		}
	}

	public override void Attach(AttachableObject o, int index = -1, bool suppressEvents = false, bool suppressEffects = false)
	{
		base.Attach(o, index, suppressEvents, suppressEffects);
		if (useGuaranteedLocalZPos)
		{
			o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, guaranteedLocalZPos);
		}
	}

	public override Vector3 GetPoint(Vector3 relativeTo)
	{
		float num = PointToPlaneDistance(relativeTo, base.transform.position, base.transform.forward);
		Vector3 vector = relativeTo + base.transform.forward * num;
		Debug.DrawLine(relativeTo, vector);
		return vector;
	}

	private float PointToPlaneDistance(Vector3 pointPosition, Vector3 planePosition, Vector3 planeNormal)
	{
		float num = 0f - Vector3.Dot(planeNormal, pointPosition - planePosition);
		float num2 = Vector3.Dot(planeNormal, planeNormal);
		float num3 = num / num2;
		Vector3 b = pointPosition + num3 * planeNormal;
		return Vector3.Distance(pointPosition, b);
	}
}
