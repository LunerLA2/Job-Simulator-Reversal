using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public static class SteamVR_Utils
{
	public class Event
	{
		public delegate void Handler(params object[] args);

		private static Hashtable listeners = new Hashtable();

		public static void Listen(string message, Handler action)
		{
			Handler handler = listeners[message] as Handler;
			if (handler != null)
			{
				listeners[message] = (Handler)Delegate.Combine(handler, action);
			}
			else
			{
				listeners[message] = action;
			}
		}

		public static void Remove(string message, Handler action)
		{
			Handler handler = listeners[message] as Handler;
			if (handler != null)
			{
				listeners[message] = (Handler)Delegate.Remove(handler, action);
			}
		}

		public static void Send(string message, params object[] args)
		{
			Handler handler = listeners[message] as Handler;
			if (handler != null)
			{
				handler(args);
			}
		}
	}

	[Serializable]
	public struct RigidTransform
	{
		public Vector3 pos;

		public Quaternion rot;

		public static RigidTransform identity
		{
			get
			{
				return new RigidTransform(Vector3.zero, Quaternion.identity);
			}
		}

		public RigidTransform(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this.rot = rot;
		}

		public RigidTransform(Transform t)
		{
			pos = t.position;
			rot = t.rotation;
		}

		public RigidTransform(Transform from, Transform to)
		{
			Quaternion quaternion = Quaternion.Inverse(from.rotation);
			rot = quaternion * to.rotation;
			pos = quaternion * (to.position - from.position);
		}

		public RigidTransform(HmdMatrix34_t pose)
		{
			Matrix4x4 matrix = Matrix4x4.identity;
			matrix[0, 0] = pose.m0;
			matrix[0, 1] = pose.m1;
			matrix[0, 2] = 0f - pose.m2;
			matrix[0, 3] = pose.m3;
			matrix[1, 0] = pose.m4;
			matrix[1, 1] = pose.m5;
			matrix[1, 2] = 0f - pose.m6;
			matrix[1, 3] = pose.m7;
			matrix[2, 0] = 0f - pose.m8;
			matrix[2, 1] = 0f - pose.m9;
			matrix[2, 2] = pose.m10;
			matrix[2, 3] = 0f - pose.m11;
			pos = matrix.GetPosition();
			rot = matrix.GetRotation();
		}

		public RigidTransform(HmdMatrix44_t pose)
		{
			Matrix4x4 matrix = Matrix4x4.identity;
			matrix[0, 0] = pose.m0;
			matrix[0, 1] = pose.m1;
			matrix[0, 2] = 0f - pose.m2;
			matrix[0, 3] = pose.m3;
			matrix[1, 0] = pose.m4;
			matrix[1, 1] = pose.m5;
			matrix[1, 2] = 0f - pose.m6;
			matrix[1, 3] = pose.m7;
			matrix[2, 0] = 0f - pose.m8;
			matrix[2, 1] = 0f - pose.m9;
			matrix[2, 2] = pose.m10;
			matrix[2, 3] = 0f - pose.m11;
			matrix[3, 0] = pose.m12;
			matrix[3, 1] = pose.m13;
			matrix[3, 2] = 0f - pose.m14;
			matrix[3, 3] = pose.m15;
			pos = matrix.GetPosition();
			rot = matrix.GetRotation();
		}

		public static RigidTransform FromLocal(Transform t)
		{
			return new RigidTransform(t.localPosition, t.localRotation);
		}

		public HmdMatrix44_t ToHmdMatrix44()
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(pos, rot, Vector3.one);
			HmdMatrix44_t result = default(HmdMatrix44_t);
			result.m0 = matrix4x[0, 0];
			result.m1 = matrix4x[0, 1];
			result.m2 = 0f - matrix4x[0, 2];
			result.m3 = matrix4x[0, 3];
			result.m4 = matrix4x[1, 0];
			result.m5 = matrix4x[1, 1];
			result.m6 = 0f - matrix4x[1, 2];
			result.m7 = matrix4x[1, 3];
			result.m8 = 0f - matrix4x[2, 0];
			result.m9 = 0f - matrix4x[2, 1];
			result.m10 = matrix4x[2, 2];
			result.m11 = 0f - matrix4x[2, 3];
			result.m12 = matrix4x[3, 0];
			result.m13 = matrix4x[3, 1];
			result.m14 = 0f - matrix4x[3, 2];
			result.m15 = matrix4x[3, 3];
			return result;
		}

		public HmdMatrix34_t ToHmdMatrix34()
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(pos, rot, Vector3.one);
			HmdMatrix34_t result = default(HmdMatrix34_t);
			result.m0 = matrix4x[0, 0];
			result.m1 = matrix4x[0, 1];
			result.m2 = 0f - matrix4x[0, 2];
			result.m3 = matrix4x[0, 3];
			result.m4 = matrix4x[1, 0];
			result.m5 = matrix4x[1, 1];
			result.m6 = 0f - matrix4x[1, 2];
			result.m7 = matrix4x[1, 3];
			result.m8 = 0f - matrix4x[2, 0];
			result.m9 = 0f - matrix4x[2, 1];
			result.m10 = matrix4x[2, 2];
			result.m11 = 0f - matrix4x[2, 3];
			return result;
		}

		public override bool Equals(object o)
		{
			if (o is RigidTransform)
			{
				RigidTransform rigidTransform = (RigidTransform)o;
				return pos == rigidTransform.pos && rot == rigidTransform.rot;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return pos.GetHashCode() ^ rot.GetHashCode();
		}

		public void Inverse()
		{
			rot = Quaternion.Inverse(rot);
			pos = -(rot * pos);
		}

		public RigidTransform GetInverse()
		{
			RigidTransform result = new RigidTransform(pos, rot);
			result.Inverse();
			return result;
		}

		public void Multiply(RigidTransform a, RigidTransform b)
		{
			rot = a.rot * b.rot;
			pos = a.pos + a.rot * b.pos;
		}

		public Vector3 InverseTransformPoint(Vector3 point)
		{
			return Quaternion.Inverse(rot) * (point - pos);
		}

		public Vector3 TransformPoint(Vector3 point)
		{
			return pos + rot * point;
		}

		public static RigidTransform Interpolate(RigidTransform a, RigidTransform b, float t)
		{
			return new RigidTransform(Vector3.Lerp(a.pos, b.pos, t), Quaternion.Slerp(a.rot, b.rot, t));
		}

		public void Interpolate(RigidTransform to, float t)
		{
			pos = Lerp(pos, to.pos, t);
			rot = Slerp(rot, to.rot, t);
		}

		public static bool operator ==(RigidTransform a, RigidTransform b)
		{
			return a.pos == b.pos && a.rot == b.rot;
		}

		public static bool operator !=(RigidTransform a, RigidTransform b)
		{
			return a.pos != b.pos || a.rot != b.rot;
		}

		public static RigidTransform operator *(RigidTransform a, RigidTransform b)
		{
			RigidTransform result = default(RigidTransform);
			result.rot = a.rot * b.rot;
			result.pos = a.pos + a.rot * b.pos;
			return result;
		}

		public static Vector3 operator *(RigidTransform t, Vector3 v)
		{
			return t.TransformPoint(v);
		}
	}

	public delegate object SystemFn(CVRSystem system, params object[] args);

	public static Quaternion Slerp(Quaternion A, Quaternion B, float t)
	{
		float num = Mathf.Clamp(A.x * B.x + A.y * B.y + A.z * B.z + A.w * B.w, -1f, 1f);
		if (num < 0f)
		{
			B = new Quaternion(0f - B.x, 0f - B.y, 0f - B.z, 0f - B.w);
			num = 0f - num;
		}
		float num4;
		float num5;
		if (1f - num > 0.0001f)
		{
			float num2 = Mathf.Acos(num);
			float num3 = Mathf.Sin(num2);
			num4 = Mathf.Sin((1f - t) * num2) / num3;
			num5 = Mathf.Sin(t * num2) / num3;
		}
		else
		{
			num4 = 1f - t;
			num5 = t;
		}
		return new Quaternion(num4 * A.x + num5 * B.x, num4 * A.y + num5 * B.y, num4 * A.z + num5 * B.z, num4 * A.w + num5 * B.w);
	}

	public static Vector3 Lerp(Vector3 A, Vector3 B, float t)
	{
		return new Vector3(Lerp(A.x, B.x, t), Lerp(A.y, B.y, t), Lerp(A.z, B.z, t));
	}

	public static float Lerp(float A, float B, float t)
	{
		return A + (B - A) * t;
	}

	public static double Lerp(double A, double B, double t)
	{
		return A + (B - A) * t;
	}

	public static float InverseLerp(Vector3 A, Vector3 B, Vector3 result)
	{
		return Vector3.Dot(result - A, B - A);
	}

	public static float InverseLerp(float A, float B, float result)
	{
		return (result - A) / (B - A);
	}

	public static double InverseLerp(double A, double B, double result)
	{
		return (result - A) / (B - A);
	}

	public static float Saturate(float A)
	{
		return (A < 0f) ? 0f : ((!(A > 1f)) ? A : 1f);
	}

	public static Vector2 Saturate(Vector2 A)
	{
		return new Vector2(Saturate(A.x), Saturate(A.y));
	}

	public static float Abs(float A)
	{
		return (!(A < 0f)) ? A : (0f - A);
	}

	public static Vector2 Abs(Vector2 A)
	{
		return new Vector2(Abs(A.x), Abs(A.y));
	}

	private static float _copysign(float sizeval, float signval)
	{
		return (Mathf.Sign(signval) != 1f) ? (0f - Mathf.Abs(sizeval)) : Mathf.Abs(sizeval);
	}

	public static Quaternion GetRotation(this Matrix4x4 matrix)
	{
		Quaternion result = default(Quaternion);
		result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
		result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
		result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
		result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
		result.x = _copysign(result.x, matrix.m21 - matrix.m12);
		result.y = _copysign(result.y, matrix.m02 - matrix.m20);
		result.z = _copysign(result.z, matrix.m10 - matrix.m01);
		return result;
	}

	public static Vector3 GetPosition(this Matrix4x4 matrix)
	{
		float m = matrix.m03;
		float m2 = matrix.m13;
		float m3 = matrix.m23;
		return new Vector3(m, m2, m3);
	}

	public static Vector3 GetScale(this Matrix4x4 m)
	{
		float x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
		float y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
		float z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
		return new Vector3(x, y, z);
	}

	public static Mesh CreateHiddenAreaMesh(HiddenAreaMesh_t src, VRTextureBounds_t bounds)
	{
		if (src.unTriangleCount == 0)
		{
			return null;
		}
		float[] array = new float[src.unTriangleCount * 3 * 2];
		Marshal.Copy(src.pVertexData, array, 0, array.Length);
		Vector3[] array2 = new Vector3[src.unTriangleCount * 3 + 12];
		int[] array3 = new int[src.unTriangleCount * 3 + 24];
		float num = 2f * bounds.uMin - 1f;
		float num2 = 2f * bounds.uMax - 1f;
		float num3 = 2f * bounds.vMin - 1f;
		float num4 = 2f * bounds.vMax - 1f;
		int i = 0;
		int num5 = 0;
		for (; i < src.unTriangleCount * 3; i++)
		{
			float x = Lerp(num, num2, array[num5++]);
			float y = Lerp(num3, num4, array[num5++]);
			array2[i] = new Vector3(x, y, 0f);
			array3[i] = i;
		}
		int num6 = (int)(src.unTriangleCount * 3);
		int num7 = num6;
		array2[num7++] = new Vector3(-1f, -1f, 0f);
		array2[num7++] = new Vector3(num, -1f, 0f);
		array2[num7++] = new Vector3(-1f, 1f, 0f);
		array2[num7++] = new Vector3(num, 1f, 0f);
		array2[num7++] = new Vector3(num2, -1f, 0f);
		array2[num7++] = new Vector3(1f, -1f, 0f);
		array2[num7++] = new Vector3(num2, 1f, 0f);
		array2[num7++] = new Vector3(1f, 1f, 0f);
		array2[num7++] = new Vector3(num, num3, 0f);
		array2[num7++] = new Vector3(num2, num3, 0f);
		array2[num7++] = new Vector3(num, num4, 0f);
		array2[num7++] = new Vector3(num2, num4, 0f);
		int num8 = num6;
		array3[num8++] = num6;
		array3[num8++] = num6 + 1;
		array3[num8++] = num6 + 2;
		array3[num8++] = num6 + 2;
		array3[num8++] = num6 + 1;
		array3[num8++] = num6 + 3;
		array3[num8++] = num6 + 4;
		array3[num8++] = num6 + 5;
		array3[num8++] = num6 + 6;
		array3[num8++] = num6 + 6;
		array3[num8++] = num6 + 5;
		array3[num8++] = num6 + 7;
		array3[num8++] = num6 + 1;
		array3[num8++] = num6 + 4;
		array3[num8++] = num6 + 8;
		array3[num8++] = num6 + 8;
		array3[num8++] = num6 + 4;
		array3[num8++] = num6 + 9;
		array3[num8++] = num6 + 10;
		array3[num8++] = num6 + 11;
		array3[num8++] = num6 + 3;
		array3[num8++] = num6 + 3;
		array3[num8++] = num6 + 11;
		array3[num8++] = num6 + 6;
		Mesh mesh = new Mesh();
		mesh.vertices = array2;
		mesh.triangles = array3;
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		return mesh;
	}

	public static object CallSystemFn(SystemFn fn, params object[] args)
	{
		bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
		if (flag)
		{
			EVRInitError peError = EVRInitError.None;
			OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Other);
		}
		CVRSystem system = OpenVR.System;
		object result = ((system == null) ? null : fn(system, args));
		if (flag)
		{
			OpenVR.Shutdown();
		}
		return result;
	}

	public static void QueueEventOnRenderThread(int eventID)
	{
	}
}
