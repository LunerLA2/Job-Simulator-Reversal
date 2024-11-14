using UnityEngine;

public class OVRBoundary
{
	public enum Node
	{
		HandLeft = 3,
		HandRight = 4,
		Head = 9
	}

	public enum BoundaryType
	{
		OuterBoundary = 1,
		PlayArea = 0x100
	}

	public struct BoundaryTestResult
	{
		public bool IsTriggering;

		public float ClosestDistance;

		public Vector3 ClosestPoint;

		public Vector3 ClosestPointNormal;
	}

	public struct BoundaryLookAndFeel
	{
		public Color Color;
	}

	public bool GetConfigured()
	{
		return OVRPlugin.GetBoundaryConfigured();
	}

	public BoundaryTestResult TestNode(Node node, BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult boundaryTestResult = OVRPlugin.TestBoundaryNode((OVRPlugin.Node)node, (OVRPlugin.BoundaryType)boundaryType);
		BoundaryTestResult result = default(BoundaryTestResult);
		result.IsTriggering = boundaryTestResult.IsTriggering == OVRPlugin.Bool.True;
		result.ClosestDistance = boundaryTestResult.ClosestDistance;
		result.ClosestPoint = boundaryTestResult.ClosestPoint.FromFlippedZVector3f();
		result.ClosestPointNormal = boundaryTestResult.ClosestPointNormal.FromFlippedZVector3f();
		return result;
	}

	public BoundaryTestResult TestPoint(Vector3 point, BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult boundaryTestResult = OVRPlugin.TestBoundaryPoint(point.ToFlippedZVector3f(), (OVRPlugin.BoundaryType)boundaryType);
		BoundaryTestResult result = default(BoundaryTestResult);
		result.IsTriggering = boundaryTestResult.IsTriggering == OVRPlugin.Bool.True;
		result.ClosestDistance = boundaryTestResult.ClosestDistance;
		result.ClosestPoint = boundaryTestResult.ClosestPoint.FromFlippedZVector3f();
		result.ClosestPointNormal = boundaryTestResult.ClosestPointNormal.FromFlippedZVector3f();
		return result;
	}

	public void SetLookAndFeel(BoundaryLookAndFeel lookAndFeel)
	{
		OVRPlugin.BoundaryLookAndFeel boundaryLookAndFeel = default(OVRPlugin.BoundaryLookAndFeel);
		boundaryLookAndFeel.Color = lookAndFeel.Color.ToColorf();
		OVRPlugin.BoundaryLookAndFeel boundaryLookAndFeel2 = boundaryLookAndFeel;
		OVRPlugin.SetBoundaryLookAndFeel(boundaryLookAndFeel2);
	}

	public void ResetLookAndFeel()
	{
		OVRPlugin.ResetBoundaryLookAndFeel();
	}

	public Vector3[] GetGeometry(BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryGeometry boundaryGeometry = OVRPlugin.GetBoundaryGeometry((OVRPlugin.BoundaryType)boundaryType);
		Vector3[] array = new Vector3[boundaryGeometry.PointsCount];
		for (int i = 0; i < boundaryGeometry.PointsCount; i++)
		{
			array[i] = boundaryGeometry.Points[i].FromFlippedZVector3f();
		}
		return array;
	}

	public Vector3 GetDimensions(BoundaryType boundaryType)
	{
		return OVRPlugin.GetBoundaryDimensions((OVRPlugin.BoundaryType)boundaryType).FromVector3f();
	}

	public bool GetVisible()
	{
		return OVRPlugin.GetBoundaryVisible();
	}

	public void SetVisible(bool value)
	{
		OVRPlugin.SetBoundaryVisible(value);
	}
}
