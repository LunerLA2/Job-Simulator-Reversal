using UnityEngine;

public class SimpleMovableBrainControlledObject : BrainControlledObject
{
	public override void MoveToPosition(string positionName, float moveDuration)
	{
		UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(positionName);
		if (objectByName != null)
		{
			if (moveDuration > 0f)
			{
				Go.to(base.transform, moveDuration, new GoTweenConfig().position(objectByName.transform.position).setEaseType(GoEaseType.QuadInOut));
			}
			else
			{
				base.transform.position = objectByName.transform.position;
			}
		}
	}

	public override void LookAt(BrainEffect.LookAtTypes lookAtType, string optionalObjectName = "", float optionalWorldAngle = 0f)
	{
		switch (lookAtType)
		{
		case BrainEffect.LookAtTypes.Bot:
			break;
		case BrainEffect.LookAtTypes.Nothing:
			break;
		case BrainEffect.LookAtTypes.Object:
			break;
		case BrainEffect.LookAtTypes.Player:
			break;
		case BrainEffect.LookAtTypes.WorldAngle:
		{
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(base.transform.eulerAngles.x, optionalWorldAngle, base.transform.eulerAngles.z);
			Go.to(base.transform, 1f, new GoTweenConfig().setEaseType(GoEaseType.QuadInOut).rotation(identity));
			break;
		}
		}
	}
}
