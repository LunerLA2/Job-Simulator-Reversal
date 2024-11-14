using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public static class GameObjectComponentInfoBuilder
	{
		private static Dictionary<string, List<string>> systemExclusions;

		public static GameObjectInfoObj BuildGameObjectInfoObj(GameObject go)
		{
			CheckAndInit();
			List<ComponentInfoObject> list = new List<ComponentInfoObject>();
			Component[] components = go.GetComponents<Component>();
			Component[] array = components;
			foreach (Component component in array)
			{
				List<string> exclusionsForType = GetExclusionsForType(component.GetType());
				list.Add(new ComponentInfoObject(component, GetProperties(component, exclusionsForType), GetFields(component, exclusionsForType)));
			}
			return new GameObjectInfoObj(go, list);
		}

		private static List<FieldInfo> GetFields(Component component, List<string> propertiesToIgnore)
		{
			List<FieldInfo> list = new List<FieldInfo>();
			FieldInfo[] fields = component.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsPublic && !propertiesToIgnore.Contains(fieldInfo.Name) && !Attribute.IsDefined(fieldInfo, typeof(HideInInspector)))
				{
					list.Add(fieldInfo);
				}
			}
			return list;
		}

		private static List<PropertyInfo> GetProperties(Component component, List<string> propertiesToIgnore)
		{
			List<PropertyInfo> list = new List<PropertyInfo>();
			PropertyInfo[] properties = component.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertiesToIgnore.Contains(propertyInfo.Name) && !Attribute.IsDefined(propertyInfo, typeof(HideInInspector)))
				{
					MethodInfo setMethod = propertyInfo.GetSetMethod();
					if (setMethod != null && setMethod.IsPublic)
					{
						list.Add(propertyInfo);
					}
				}
			}
			return list;
		}

		private static List<string> GetExclusionsForType(Type type)
		{
			List<string> list = new List<string>();
			List<string> classNames = GetClassNames(type);
			foreach (string item in classNames)
			{
				if (systemExclusions.ContainsKey(item))
				{
					list.AddRange(systemExclusions[item]);
				}
			}
			return list;
		}

		private static List<string> GetClassNames(Type type)
		{
			List<string> list = new List<string>();
			list.Add(type.ToString());
			while (type.BaseType != null)
			{
				list.Add(type.BaseType.ToString());
				type = type.BaseType;
			}
			return list;
		}

		private static void CheckAndInit()
		{
			if (systemExclusions == null)
			{
				LoadSystemExclusions();
			}
		}

		private static void LoadSystemExclusions()
		{
			systemExclusions = new Dictionary<string, List<string>>();
			systemExclusions.Add("UnityEngine.Transform", new List<string> { "position", "eulerAngles", "localEulerAngles", "right", "up", "forward", "rotation", "parent" });
			systemExclusions.Add("UnityEngine.Renderer", new List<string> { "lightmapIndex", "lightmapTilingOffset", "material", "materials" });
			systemExclusions.Add("UnityEngine.MeshFilter", new List<string> { "mesh" });
			systemExclusions.Add("UnityEngine.Collider", new List<string> { "material" });
			systemExclusions.Add("UnityEngine.MonoBehaviour", new List<string> { "useGUILayout", "enabled", "name", "tag", "hideFlags" });
			systemExclusions.Add("UnityEngine.ParticleEmitter", new List<string> { "particles" });
			systemExclusions.Add("UnityEngine.ParticleRenderer", new List<string> { "animatedTextureCount", "uvTiles", "widthCurve", "heightCurve", "rotationCurve" });
			systemExclusions.Add("UnityEngine.Rigidbody", new List<string>
			{
				"velocity", "angularVelocity", "centerOfMass", "inertiaTensorRotation", "inertiaTensor", "useConeFriction", "position", "rotation", "solverIterationCount", "sleepVelocity",
				"sleepAngularVelocity", "maxAngularVelocity"
			});
			systemExclusions.Add("UnityEngine.CharacterController", new List<string> { "detectCollisions", "isTrigger", "sharedMaterial" });
			systemExclusions.Add("UnityEngine.BoxCollider", new List<string> { "extents" });
			systemExclusions.Add("UnityEngine.MeshCollider", new List<string> { "mesh" });
			systemExclusions.Add("UnityEngine.WheelCollider", new List<string> { "motorTorque", "brakeTorque", "steerAngle", "isTrigger", "sharedMaterial" });
			systemExclusions.Add("UnityEngine.FixedJoint", new List<string> { "axis", "anchor" });
			systemExclusions.Add("UnityEngine.SpringJoint", new List<string> { "axis" });
			systemExclusions.Add("UnityEngine.CharacterJoint", new List<string> { "targetRotation", "targetAngularVelocity", "rotationDrive" });
			systemExclusions.Add("UnityEngine.AudioListener", new List<string> { "volume", "pause", "velocityUpdateMode" });
			systemExclusions.Add("UnityEngine.AudioSource", new List<string> { "time", "timeSamples", "ignoreListenerVolume", "velocityUpdateMode", "minVolume", "maxVolume", "rolloffFactor" });
			systemExclusions.Add("UnityEngine.AudioReverbFilter", new List<string> { "roomRolloff" });
			systemExclusions.Add("UnityEngine.Camera", new List<string>
			{
				"fov", "near", "far", "aspect", "orthograhic", "pixelRect", "aspect", "worldToCameraMatrix", "projectionMatrix", "layerCullDistances",
				"depthTextureMode"
			});
			systemExclusions.Add("UnityEngine.Projector", new List<string> { "isOrthoGraphic", "orthoGraphicSize" });
			systemExclusions.Add("UnityEngine.GUIText", new List<string> { "material" });
			systemExclusions.Add("UnityEngine.Animation", new List<string> { "wrapMode", "animateOnlyIfVisible" });
			systemExclusions.Add("UnityEngine.Tree", new List<string> { "data" });
		}
	}
}
