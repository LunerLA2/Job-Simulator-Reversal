using UnityEngine;

public class SteamVR_IK : MonoBehaviour
{
	public Transform target;

	public Transform start;

	public Transform joint;

	public Transform end;

	public Transform poleVector;

	public Transform upVector;

	public float blendPct = 1f;

	[HideInInspector]
	public Transform startXform;

	[HideInInspector]
	public Transform jointXform;

	[HideInInspector]
	public Transform endXform;

	private void LateUpdate()
	{
		if (blendPct < 0.001f)
		{
			return;
		}
		Vector3 worldUp = ((!upVector) ? Vector3.Cross(end.position - start.position, joint.position - start.position).normalized : upVector.up);
		Vector3 position = target.position;
		Quaternion rotation = target.rotation;
		Vector3 result = joint.position;
		Vector3 forward;
		Vector3 up;
		Solve(start.position, position, poleVector.position, (joint.position - start.position).magnitude, (end.position - joint.position).magnitude, ref result, out forward, out up);
		if (!(up == Vector3.zero))
		{
			Vector3 position2 = start.position;
			Vector3 position3 = joint.position;
			Vector3 position4 = end.position;
			Quaternion localRotation = start.localRotation;
			Quaternion localRotation2 = joint.localRotation;
			Quaternion localRotation3 = end.localRotation;
			Transform parent = start.parent;
			Transform parent2 = joint.parent;
			Transform parent3 = end.parent;
			Vector3 localScale = start.localScale;
			Vector3 localScale2 = joint.localScale;
			Vector3 localScale3 = end.localScale;
			if (startXform == null)
			{
				startXform = new GameObject("startXform").transform;
				startXform.parent = base.transform;
			}
			startXform.position = position2;
			startXform.LookAt(joint, worldUp);
			start.parent = startXform;
			if (jointXform == null)
			{
				jointXform = new GameObject("jointXform").transform;
				jointXform.parent = startXform;
			}
			jointXform.position = position3;
			jointXform.LookAt(end, worldUp);
			joint.parent = jointXform;
			if (endXform == null)
			{
				endXform = new GameObject("endXform").transform;
				endXform.parent = jointXform;
			}
			endXform.position = position4;
			end.parent = endXform;
			startXform.LookAt(result, up);
			jointXform.LookAt(position, up);
			endXform.rotation = rotation;
			start.parent = parent;
			joint.parent = parent2;
			end.parent = parent3;
			end.rotation = rotation;
			if (blendPct < 1f)
			{
				start.localRotation = Quaternion.Slerp(localRotation, start.localRotation, blendPct);
				joint.localRotation = Quaternion.Slerp(localRotation2, joint.localRotation, blendPct);
				end.localRotation = Quaternion.Slerp(localRotation3, end.localRotation, blendPct);
			}
			start.localScale = localScale;
			joint.localScale = localScale2;
			end.localScale = localScale3;
		}
	}

	public static bool Solve(Vector3 start, Vector3 end, Vector3 poleVector, float jointDist, float targetDist, ref Vector3 result, out Vector3 forward, out Vector3 up)
	{
		float num = jointDist + targetDist;
		Vector3 vector = end - start;
		Vector3 normalized = (poleVector - start).normalized;
		float magnitude = vector.magnitude;
		result = start;
		if (magnitude < 0.001f)
		{
			result += normalized * jointDist;
			forward = Vector3.Cross(normalized, Vector3.up);
			up = Vector3.Cross(forward, normalized).normalized;
		}
		else
		{
			forward = vector * (1f / magnitude);
			up = Vector3.Cross(forward, normalized).normalized;
			if (magnitude + 0.001f < num)
			{
				float num2 = (num + magnitude) * 0.5f;
				if (num2 > jointDist + 0.001f && num2 > targetDist + 0.001f)
				{
					float num3 = Mathf.Sqrt(num2 * (num2 - jointDist) * (num2 - targetDist) * (num2 - magnitude));
					float num4 = 2f * num3 / magnitude;
					float num5 = Mathf.Sqrt(jointDist * jointDist - num4 * num4);
					Vector3 vector2 = Vector3.Cross(up, forward);
					result += forward * num5 + vector2 * num4;
					return true;
				}
				result += normalized * jointDist;
			}
			else
			{
				result += forward * jointDist;
			}
		}
		return false;
	}
}
