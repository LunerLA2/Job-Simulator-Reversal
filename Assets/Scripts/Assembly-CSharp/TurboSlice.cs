using System;
using System.Collections.Generic;
using NobleMuffins.MuffinSlicer;
using UnityEngine;

public class TurboSlice : MonoBehaviour
{
	private struct SplitAction
	{
		public const short nullIndex = -1;

		public const byte TO_FRONT = 1;

		public const byte TO_BACK = 2;

		public const byte INTERSECT = 4;

		public byte flags;

		public int cloneOf;

		public int index0;

		public int index1;

		public int realIndex;

		public float intersectionResult;

		public SplitAction(bool _toFront, bool _toBack, int _index0)
		{
			flags = 0;
			if (_toFront)
			{
				flags |= 1;
			}
			if (_toBack)
			{
				flags |= 2;
			}
			index0 = _index0;
			index1 = -1;
			cloneOf = -1;
			realIndex = index0;
			intersectionResult = 0f;
		}

		public SplitAction(int _index0, int _index1)
		{
			flags = 7;
			index0 = _index0;
			index1 = _index1;
			cloneOf = -1;
			realIndex = -1;
			intersectionResult = 0f;
		}
	}

	[Serializable]
	public class InfillConfiguration
	{
		public Material material;

		public Rect regionForInfill;
	}

	private class MeshCache
	{
		public readonly float creationTime = Time.time;

		public TurboList<Vector3> vertices;

		public TurboList<Vector3> normals;

		public TurboList<Vector2> coords;

		public TurboList<Vector2> coords2;

		public int[][] indices;

		public Material[] mats;
	}

	private const float factorOfSafetyGeometry = 4.5f;

	private const float factorOfSafetyIndices = 0.9f;

	public static Action<Vector3[], Vector2[], Vector3[], int[]> infillGeometryReceivers;

	private readonly Dictionary<Mesh, MeshCache> meshCaches = new Dictionary<Mesh, MeshCache>();

	private readonly Queue<Mesh> meshDeletionQueue = new Queue<Mesh>();

	private static TurboSlice _instance;

	public static TurboSlice instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject();
				_instance = gameObject.AddComponent<TurboSlice>();
			}
			return _instance;
		}
	}

	private void handleHierarchy(Transform root, Dictionary<string, bool> presenceByName, Dictionary<string, Transform> originalsByName)
	{
		List<Transform> list = new List<Transform>(presenceByName.Count);
		concatenateHierarchy(root, list);
		foreach (Transform item in list)
		{
			GameObject gameObject = item.gameObject;
			string key = item.name;
			bool flag = presenceByName.ContainsKey(key) && presenceByName[key];
			flag &= originalsByName[key].gameObject.activeSelf;
			gameObject.SetActive(flag);
		}
		foreach (Transform item2 in list)
		{
			string key2 = item2.name;
			if (originalsByName.ContainsKey(key2))
			{
				Transform transform = originalsByName[key2];
				item2.localPosition = transform.localPosition;
				item2.localRotation = transform.localRotation;
				item2.localScale = transform.localScale;
			}
		}
	}

	private void determinePresence(Transform root, Vector4 plane, out Dictionary<string, Transform> transformByName, out Dictionary<string, bool> frontPresence, out Dictionary<string, bool> backPresence)
	{
		List<Transform> list = new List<Transform>();
		concatenateHierarchy(root, list);
		Vector3[] array = new Vector3[list.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = list[i].position;
		}
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = worldToLocalMatrix.MultiplyPoint3x4(array[j]);
		}
		PlaneTriResult[] array2 = new PlaneTriResult[array.Length];
		for (int k = 0; k < array.Length; k++)
		{
			array2[k] = MuffinSliceCommon.getSidePlane(ref array[k], ref plane);
		}
		transformByName = new Dictionary<string, Transform>();
		frontPresence = new Dictionary<string, bool>();
		backPresence = new Dictionary<string, bool>();
		bool flag = false;
		for (int l = 0; l < array2.Length; l++)
		{
			Transform transform = list[l];
			string key = transform.name;
			if (transformByName.ContainsKey(key))
			{
				flag = true;
			}
			transformByName[key] = transform;
			frontPresence[key] = array2[l] == PlaneTriResult.PTR_FRONT;
			backPresence[key] = array2[l] == PlaneTriResult.PTR_BACK;
		}
		if (flag)
		{
			Debug.LogWarning("Sliceable has children with non-unique names. Behaviour is undefined!");
		}
	}

	private void concatenateHierarchy(Transform t, List<Transform> results)
	{
		foreach (Transform item in t)
		{
			results.Add(item);
			concatenateHierarchy(item, results);
		}
	}

	private void createResultObjects(GameObject go, Sliceable sliceable, bool forceCloning, Vector4 plane, out GameObject frontObject, out GameObject backObject)
	{
		Transform transform = go.transform;
		Dictionary<string, Transform> transformByName;
		Dictionary<string, bool> frontPresence;
		Dictionary<string, bool> backPresence;
		determinePresence(transform, plane, out transformByName, out frontPresence, out backPresence);
		bool flag;
		bool flag2;
		if (sliceable.alternatePrefab == null)
		{
			flag = false;
			flag2 = false;
		}
		else if (sliceable.alwaysCloneFromAlternate)
		{
			flag = true;
			flag2 = true;
		}
		else
		{
			flag = sliceable.cloneAlternate(frontPresence);
			flag2 = sliceable.cloneAlternate(backPresence);
		}
		UnityEngine.Object original = ((!flag) ? go : sliceable.alternatePrefab);
		UnityEngine.Object original2 = ((!flag2) ? go : sliceable.alternatePrefab);
		frontObject = (GameObject)UnityEngine.Object.Instantiate(original);
		backObject = (GameObject)UnityEngine.Object.Instantiate(original2);
		handleHierarchy(frontObject.transform, frontPresence, transformByName);
		handleHierarchy(backObject.transform, backPresence, transformByName);
		Transform parent = transform.parent;
		Vector3 localPosition = transform.localPosition;
		Vector3 localScale = transform.localScale;
		Quaternion localRotation = transform.localRotation;
		frontObject.transform.parent = parent;
		frontObject.transform.localPosition = localPosition;
		frontObject.transform.localScale = localScale;
		backObject.transform.parent = parent;
		backObject.transform.localPosition = localPosition;
		backObject.transform.localScale = localScale;
		frontObject.transform.localRotation = localRotation;
		backObject.transform.localRotation = localRotation;
		frontObject.layer = go.layer;
		backObject.layer = go.layer;
		Rigidbody component = go.GetComponent<Rigidbody>();
		if (component != null)
		{
			Rigidbody component2 = frontObject.GetComponent<Rigidbody>();
			Rigidbody component3 = backObject.GetComponent<Rigidbody>();
			if (component2 != null)
			{
				component2.angularVelocity = component.angularVelocity;
				component2.velocity = component.velocity;
			}
			if (component3 != null)
			{
				component3.angularVelocity = component.angularVelocity;
				component3.velocity = component.velocity;
			}
		}
	}

	public GameObject[] splitByPlane(GameObject go, Vector4 planeInLocalSpace, bool destroyOriginal)
	{
		return _splitByPlane(go, planeInLocalSpace, destroyOriginal);
	}

	public GameObject[] splitByLine(GameObject target, Camera camera, Vector3 _start, Vector3 _end)
	{
		return splitByLine(target, camera, _start, _end, true);
	}

	public GameObject[] splitByLine(GameObject target, Camera camera, Vector3 _start, Vector3 _end, bool destroyOriginal)
	{
		Vector3 vector = camera.transform.worldToLocalMatrix.MultiplyPoint3x4(target.transform.position);
		_start.z = vector.z;
		_end.z = vector.z;
		Vector3 position = (_start + _end) / 2f;
		position.z *= 2f;
		Vector3 vector2 = camera.ScreenToWorldPoint(_start);
		Vector3 vector3 = camera.ScreenToWorldPoint(position);
		Vector3 vector4 = camera.ScreenToWorldPoint(_end);
		return splitByTriangle(target, new Vector3[3] { vector2, vector3, vector4 }, destroyOriginal);
	}

	public GameObject[] splitByTriangle(GameObject target, Vector3[] triangleInWorldSpace, bool destroyOriginal)
	{
		Vector3[] array = new Vector3[3];
		Matrix4x4 worldToLocalMatrix = target.transform.worldToLocalMatrix;
		array[0] = worldToLocalMatrix.MultiplyPoint3x4(triangleInWorldSpace[0]);
		array[1] = worldToLocalMatrix.MultiplyPoint3x4(triangleInWorldSpace[1]);
		array[2] = worldToLocalMatrix.MultiplyPoint3x4(triangleInWorldSpace[2]);
		Vector4 zero = Vector4.zero;
		zero.x = array[0].y * (array[1].z - array[2].z) + array[1].y * (array[2].z - array[0].z) + array[2].y * (array[0].z - array[1].z);
		zero.y = array[0].z * (array[1].x - array[2].x) + array[1].z * (array[2].x - array[0].x) + array[2].z * (array[0].x - array[1].x);
		zero.z = array[0].x * (array[1].y - array[2].y) + array[1].x * (array[2].y - array[0].y) + array[2].x * (array[0].y - array[1].y);
		zero.w = 0f - (array[0].x * (array[1].y * array[2].z - array[2].y * array[1].z) + array[1].x * (array[2].y * array[0].z - array[0].y * array[2].z) + array[2].x * (array[0].y * array[1].z - array[1].y * array[0].z));
		return _splitByPlane(target, zero, destroyOriginal);
	}

	public GameObject[] shatter(GameObject go, int steps)
	{
		Sliceable component = go.GetComponent<Sliceable>();
		List<GameObject> list = new List<GameObject>();
		list.Add(go);
		List<GameObject> list2 = list;
		for (int i = 0; i < steps; i++)
		{
			list2 = new List<GameObject>(list.Count * 2);
			Vector4 planeInLocalSpace = UnityEngine.Random.insideUnitSphere;
			foreach (GameObject item in list)
			{
				list2.AddRange(_splitByPlane(item, planeInLocalSpace, true, false));
			}
			list = list2;
		}
		GameObject[] array = list2.ToArray();
		if (component != null)
		{
			component.handleSlice(array);
		}
		return array;
	}

	private GameObject[] _splitByPlane(GameObject go, Vector4 planeInLocalSpace, bool destroyOriginal, bool callHandlers = true)
	{
		bool flag = true;
		Sliceable sliceable = ensureSliceable(go);
		if (sliceable == null)
		{
			return null;
		}
		if (!sliceable.currentlySliceable)
		{
			return new GameObject[1] { go };
		}
		InfillConfiguration[] array = ((sliceable.infillers.Length <= 0) ? new InfillConfiguration[0] : sliceable.infillers);
		MeshCache meshCache = null;
		Mesh mesh = getMesh(sliceable);
		if (!(mesh == null))
		{
			meshCache = ((!meshCaches.ContainsKey(mesh)) ? cacheFromGameObject(sliceable, true) : meshCaches[mesh]);
		}
		if (meshCache == null)
		{
			return new GameObject[1] { go };
		}
		int num = meshCache.indices.Length;
		TurboList<int>[] array2 = new TurboList<int>[num];
		TurboList<int>[] array3 = new TurboList<int>[num];
		PlaneTriResult[] array4 = new PlaneTriResult[meshCache.vertices.Count];
		Vector3[] array5 = meshCache.vertices.array;
		for (int i = 0; i < array4.Length; i++)
		{
			array4[i] = MuffinSliceCommon.getSidePlane(ref array5[i], ref planeInLocalSpace);
		}
		for (int j = 0; j < num; j++)
		{
			int capacity = Mathf.RoundToInt((float)meshCache.indices[j].Length * 0.9f);
			array2[j] = new TurboList<int>(capacity);
			array3[j] = new TurboList<int>(capacity);
			int[] array6 = meshCache.indices[j];
			TurboList<int> turboList = array2[j];
			TurboList<int> turboList2 = array3[j];
			TurboList<int> turboList3 = new TurboList<int>(capacity);
			int[] array7 = new int[3];
			int num2 = 0;
			while (num2 < array6.Length)
			{
				array7[0] = array6[num2++];
				array7[1] = array6[num2++];
				array7[2] = array6[num2++];
				PlaneTriResult planeTriResult = array4[array7[0]];
				PlaneTriResult planeTriResult2 = array4[array7[1]];
				PlaneTriResult planeTriResult3 = array4[array7[2]];
				if (planeTriResult == planeTriResult2 && planeTriResult == planeTriResult3)
				{
					if (planeTriResult == PlaneTriResult.PTR_FRONT)
					{
						turboList.AddArray(array7);
					}
					else
					{
						turboList2.AddArray(array7);
					}
				}
				else
				{
					turboList3.AddArray(array7);
				}
			}
			InfillConfiguration infill = null;
			if (j < meshCache.mats.Length)
			{
				InfillConfiguration[] array8 = array;
				foreach (InfillConfiguration infillConfiguration in array8)
				{
					infill = infillConfiguration;
				}
			}
			if (turboList3.Count > 0)
			{
				splitTriangles(planeInLocalSpace, turboList3.ToArray(), meshCache, infill, turboList, turboList2);
			}
		}
		bool flag2 = true;
		for (int l = 0; l < meshCache.indices.Length; l++)
		{
			flag2 &= array2[l].Count == 0 || array3[l].Count == 0;
		}
		GameObject[] array9;
		if (flag2)
		{
			array9 = new GameObject[1] { go };
		}
		else
		{
			MeshCache meshCache2 = new MeshCache();
			meshCache2.vertices = meshCache.vertices;
			if (sliceable.channelNormals)
			{
				meshCache2.normals = meshCache.normals;
			}
			meshCache2.coords = meshCache.coords;
			meshCache2.coords2 = meshCache.coords2;
			meshCache2.mats = meshCache.mats;
			MeshCache meshCache3 = new MeshCache();
			meshCache3.vertices = meshCache.vertices;
			if (sliceable.channelNormals)
			{
				meshCache3.normals = meshCache.normals;
			}
			meshCache3.coords = meshCache.coords;
			meshCache3.coords2 = meshCache.coords2;
			meshCache3.mats = meshCache.mats;
			meshCache2.indices = new int[num][];
			meshCache3.indices = new int[num][];
			for (int m = 0; m < num; m++)
			{
				meshCache2.indices[m] = array2[m].ToArray();
				meshCache3.indices[m] = array3[m].ToArray();
			}
			Vector3[] normals = null;
			Vector3[] normals2 = null;
			Vector2[] uv = null;
			Vector2[] uv2 = null;
			int[][] array10 = new int[num][];
			int[][] array11 = new int[num][];
			int capacity2 = 0;
			int capacity3 = 0;
			TurboList<Vector3> turboList4 = null;
			TurboList<Vector3> turboList5 = null;
			TurboList<Vector3> turboList6 = null;
			TurboList<Vector3> turboList7 = null;
			TurboList<Vector2> turboList8 = null;
			TurboList<Vector2> turboList9 = null;
			TurboList<Vector2> turboList10 = null;
			TurboList<Vector2> turboList11 = null;
			turboList4 = new TurboList<Vector3>(capacity2);
			turboList5 = new TurboList<Vector3>(capacity3);
			if (sliceable.channelNormals)
			{
				turboList6 = new TurboList<Vector3>(capacity2);
				turboList7 = new TurboList<Vector3>(capacity3);
			}
			turboList8 = new TurboList<Vector2>(capacity2);
			turboList9 = new TurboList<Vector2>(capacity3);
			if (sliceable.channelUV2)
			{
				turboList10 = new TurboList<Vector2>(capacity2);
				turboList11 = new TurboList<Vector2>(capacity3);
			}
			int count = meshCache.vertices.Count;
			int[] transferTable = new int[count];
			int[] transferTable2 = new int[count];
			for (int n = 0; n < transferTable.Length; n++)
			{
				transferTable[n] = -1;
			}
			for (int num3 = 0; num3 < transferTable2.Length; num3++)
			{
				transferTable2[num3] = -1;
			}
			for (int num4 = 0; num4 < num; num4++)
			{
				perfectSubset(array2[num4], meshCache.vertices, meshCache.normals, meshCache.coords, meshCache.coords2, out array10[num4], turboList4, turboList6, turboList8, turboList10, ref transferTable);
			}
			for (int num5 = 0; num5 < num; num5++)
			{
				perfectSubset(array3[num5], meshCache.vertices, meshCache.normals, meshCache.coords, meshCache.coords2, out array11[num5], turboList5, turboList7, turboList9, turboList11, ref transferTable2);
			}
			Vector3[] vertices = turboList4.ToArray();
			Vector3[] vertices2 = turboList5.ToArray();
			if (sliceable.channelNormals)
			{
				normals = turboList6.ToArray();
				normals2 = turboList7.ToArray();
			}
			Vector2[] array12 = turboList8.ToArray();
			Vector2[] array13 = turboList9.ToArray();
			if (sliceable.channelUV2)
			{
				uv = turboList10.ToArray();
				uv2 = turboList11.ToArray();
			}
			Mesh mesh2 = new Mesh();
			Mesh mesh3 = new Mesh();
			GameObject frontObject;
			GameObject backObject;
			createResultObjects(go, sliceable, false, planeInLocalSpace, out frontObject, out backObject);
			ensureSliceable(frontObject);
			ensureSliceable(backObject);
			setMesh(frontObject.GetComponent<Sliceable>(), mesh2);
			setMesh(backObject.GetComponent<Sliceable>(), mesh3);
			mesh2.vertices = vertices;
			mesh3.vertices = vertices2;
			if (sliceable.channelTangents)
			{
				int[] triangles = concatenateIndexArrays(array10);
				int[] triangles2 = concatenateIndexArrays(array11);
				Vector4[] tangents;
				RealculateTangents(vertices, normals, array12, triangles, out tangents);
				Vector4[] tangents2;
				RealculateTangents(vertices2, normals2, array13, triangles2, out tangents2);
				mesh2.tangents = tangents;
				mesh3.tangents = tangents2;
			}
			if (sliceable.channelNormals)
			{
				mesh2.normals = normals;
				mesh3.normals = normals2;
			}
			mesh2.uv = array12;
			mesh3.uv = array13;
			if (sliceable.channelUV2)
			{
				mesh2.uv2 = uv;
				mesh3.uv2 = uv2;
			}
			mesh2.subMeshCount = num;
			mesh3.subMeshCount = num;
			for (int num6 = 0; num6 < num; num6++)
			{
				mesh2.SetTriangles(array10[num6], num6);
				mesh3.SetTriangles(array11[num6], num6);
			}
			TSCallbackOnDestroy tSCallbackOnDestroy = frontObject.GetComponent<TSCallbackOnDestroy>();
			TSCallbackOnDestroy tSCallbackOnDestroy2 = backObject.GetComponent<TSCallbackOnDestroy>();
			if (tSCallbackOnDestroy == null)
			{
				tSCallbackOnDestroy = frontObject.AddComponent<TSCallbackOnDestroy>();
			}
			if (tSCallbackOnDestroy2 == null)
			{
				tSCallbackOnDestroy2 = backObject.AddComponent<TSCallbackOnDestroy>();
			}
			tSCallbackOnDestroy.callWithMeshOnDestroy = releaseMesh;
			tSCallbackOnDestroy.mesh = mesh2;
			tSCallbackOnDestroy2.callWithMeshOnDestroy = releaseMesh;
			tSCallbackOnDestroy2.mesh = mesh3;
			meshCaches[mesh2] = meshCache2;
			meshCaches[mesh3] = meshCache3;
			flag = true;
			array9 = new GameObject[2] { frontObject, backObject };
			if (sliceable != null && sliceable.refreshColliders)
			{
				GameObject[] array14 = array9;
				foreach (GameObject gameObject in array14)
				{
					Collider collider = gameObject.GetComponent<Collider>();
					if (collider != null)
					{
						bool isTrigger = collider.isTrigger;
						if (collider is BoxCollider)
						{
							UnityEngine.Object.DestroyImmediate(collider);
							collider = gameObject.AddComponent<BoxCollider>();
						}
						else if (collider is SphereCollider)
						{
							UnityEngine.Object.DestroyImmediate(collider);
							collider = gameObject.AddComponent<SphereCollider>();
						}
						else if (collider is MeshCollider)
						{
							MeshCollider meshCollider = (MeshCollider)collider;
							Mesh sharedMesh = ((!(gameObject == frontObject)) ? mesh3 : mesh2);
							meshCollider.sharedMesh = sharedMesh;
						}
						collider.isTrigger = isTrigger;
					}
				}
			}
			if (callHandlers && sliceable != null)
			{
				sliceable.handleSlice(array9);
			}
			if (destroyOriginal)
			{
				UnityEngine.Object.Destroy(go);
			}
		}
		if (flag)
		{
			return array9;
		}
		return null;
	}

	private static void splitTriangles(Vector4 plane, int[] sourceIndices, MeshCache meshCache, InfillConfiguration infill, TurboList<int> frontIndices, TurboList<int> backIndices)
	{
		bool flag = infill != null;
		bool flag2 = meshCache.normals != null;
		bool flag3 = meshCache.coords2 != null;
		Vector3[] array = meshCache.vertices.array;
		Vector3[] array2 = null;
		if (flag2)
		{
			array2 = meshCache.normals.array;
		}
		Vector2[] array3 = meshCache.coords.array;
		Vector2[] array4 = null;
		if (flag3)
		{
			array4 = meshCache.coords2.array;
		}
		float[] array5 = new float[sourceIndices.Length];
		for (int i = 0; i < array5.Length; i++)
		{
			array5[i] = MuffinSliceCommon.classifyPoint(ref plane, ref array[sourceIndices[i]]);
		}
		int num = sourceIndices.Length / 3;
		int capacity = num * 5;
		List<SplitAction> list = new List<SplitAction>(capacity);
		short[] array6 = new short[num];
		short[] array7 = new short[num];
		short num2 = 0;
		short num3 = 0;
		for (int j = 0; j < sourceIndices.Length; j += 3)
		{
			int[] array8 = new int[3]
			{
				sourceIndices[j],
				sourceIndices[j + 1],
				sourceIndices[j + 2]
			};
			float[] array9 = new float[3]
			{
				array5[j],
				array5[j + 1],
				array5[j + 2]
			};
			short num4 = 2;
			short num5 = 0;
			short num6 = 0;
			for (short num7 = 0; num7 < 3; num7++)
			{
				float num8 = array9[num4];
				float num9 = array9[num7];
				if (num9 > 0f)
				{
					if (num8 < 0f)
					{
						list.Add(new SplitAction(array8[num4], array8[num7]));
						num5++;
						num6++;
					}
					list.Add(new SplitAction(true, false, array8[num7]));
					num5++;
				}
				else if (num9 < 0f)
				{
					if (num8 > 0f)
					{
						list.Add(new SplitAction(array8[num4], array8[num7]));
						num5++;
						num6++;
					}
					list.Add(new SplitAction(false, true, array8[num7]));
					num6++;
				}
				else
				{
					list.Add(new SplitAction(false, true, array8[num7]));
					num5++;
					num6++;
				}
				num4 = num7;
			}
			int num10 = j / 3;
			array6[num10] = num5;
			array7[num10] = num6;
			num2 += num5;
			num3 += num6;
		}
		int num11 = 0;
		foreach (SplitAction item in list)
		{
			if ((item.flags & 4) == 4)
			{
				num11++;
			}
		}
		SplitAction[] array10 = new SplitAction[num11];
		int[] array11 = new int[num11];
		int num12 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			SplitAction splitAction = list[k];
			if ((splitAction.flags & 4) == 4)
			{
				array10[num12] = splitAction;
				array11[num12] = k;
				num12++;
			}
		}
		for (int l = 0; l < array10.Length; l++)
		{
			SplitAction splitAction2 = array10[l];
			if (splitAction2.index0 > splitAction2.index1)
			{
				int index = splitAction2.index0;
				splitAction2.index0 = splitAction2.index1;
				splitAction2.index1 = index;
			}
			for (int m = 0; m < l; m++)
			{
				SplitAction splitAction3 = array10[m];
				if (splitAction2.index0 == splitAction3.index0 && splitAction2.index1 == splitAction3.index1)
				{
					splitAction2.cloneOf = m;
				}
			}
			array10[l] = splitAction2;
		}
		for (int n = 0; n < array10.Length; n++)
		{
			SplitAction splitAction4 = array10[n];
			if (splitAction4.cloneOf == -1)
			{
				Vector3 p = array[splitAction4.index0];
				Vector3 p2 = array[splitAction4.index1];
				splitAction4.intersectionResult = MuffinSliceCommon.intersectCommon(ref p2, ref p, ref plane);
				array10[n] = splitAction4;
			}
		}
		int count = meshCache.vertices.Count;
		int num13 = 0;
		int[] array12 = new int[array10.Length];
		int num14 = 0;
		for (int num15 = 0; num15 < array10.Length; num15++)
		{
			SplitAction splitAction5 = array10[num15];
			int num16 = ((splitAction5.cloneOf != -1) ? array12[splitAction5.cloneOf] : num14++);
			splitAction5.realIndex = count + num16;
			array12[num15] = num16;
			array10[num15] = splitAction5;
		}
		num13 = num14;
		int num17 = num13 * ((!flag) ? 1 : 3);
		Vector3[] array13 = new Vector3[num17];
		Vector3[] array14 = null;
		if (flag2)
		{
			array14 = new Vector3[num17];
		}
		Vector2[] array15 = new Vector2[num17];
		Vector2[] array16 = new Vector2[num17];
		int num18 = 0;
		SplitAction[] array17 = array10;
		for (int num19 = 0; num19 < array17.Length; num19++)
		{
			SplitAction splitAction6 = array17[num19];
			if (splitAction6.cloneOf == -1)
			{
				Vector3 b = array[splitAction6.index0];
				Vector3 a = array[splitAction6.index1];
				array13[num18] = Vector3.Lerp(a, b, splitAction6.intersectionResult);
				num18++;
			}
		}
		if (flag2)
		{
			int num20 = 0;
			SplitAction[] array18 = array10;
			for (int num21 = 0; num21 < array18.Length; num21++)
			{
				SplitAction splitAction7 = array18[num21];
				if (splitAction7.cloneOf == -1)
				{
					Vector3 b2 = array2[splitAction7.index0];
					Vector3 a2 = array2[splitAction7.index1];
					array14[num20] = Vector3.Lerp(a2, b2, splitAction7.intersectionResult);
					num20++;
				}
			}
		}
		int num22 = 0;
		SplitAction[] array19 = array10;
		for (int num23 = 0; num23 < array19.Length; num23++)
		{
			SplitAction splitAction8 = array19[num23];
			if (splitAction8.cloneOf == -1)
			{
				Vector2 b3 = array3[splitAction8.index0];
				Vector2 a3 = array3[splitAction8.index1];
				array15[num22] = Vector2.Lerp(a3, b3, splitAction8.intersectionResult);
				num22++;
			}
		}
		if (flag3)
		{
			int num24 = 0;
			SplitAction[] array20 = array10;
			for (int num25 = 0; num25 < array20.Length; num25++)
			{
				SplitAction splitAction9 = array20[num25];
				if (splitAction9.cloneOf == -1)
				{
					Vector2 b4 = array4[splitAction9.index0];
					Vector2 a4 = array4[splitAction9.index1];
					array16[num24] = Vector2.Lerp(a4, b4, splitAction9.intersectionResult);
					num24++;
				}
			}
		}
		Vector2[] array21 = new Vector2[0];
		int num26 = 0;
		int num27 = 0;
		if (flag)
		{
			array21 = new Vector2[num17];
			if (!Mathf.Approximately(plane.x, 0f) || !Mathf.Approximately(plane.y, 0f))
			{
				Vector3 forward = Vector3.forward;
				Vector3 normalized = new Vector3(plane.x, plane.y, plane.z).normalized;
				Vector3 normalized2 = Vector3.Cross(forward, normalized).normalized;
				Vector3 vector = Vector3.Cross(normalized2, forward);
				float num28 = Vector3.Dot(normalized, forward);
				float num29 = Vector3.Dot(normalized, vector);
				Matrix4x4 identity = Matrix4x4.identity;
				identity.SetRow(0, forward);
				identity.SetRow(1, vector);
				identity.SetRow(2, normalized2);
				Matrix4x4 inverse = identity.inverse;
				Matrix4x4 identity2 = Matrix4x4.identity;
				identity2.SetRow(0, new Vector4(num28, num29, 0f, 0f));
				identity2.SetRow(1, new Vector4(0f - num29, num28, 0f, 0f));
				Matrix4x4 matrix4x = inverse * identity2 * identity;
				for (int num30 = 0; num30 < array13.Length; num30++)
				{
					array21[num30] = matrix4x.MultiplyPoint3x4(array13[num30]);
				}
			}
			else
			{
				for (int num31 = 0; num31 < array13.Length; num31++)
				{
					array21[num31] = new Vector2(array13[num31].x, 0f - array13[num31].y);
				}
			}
			for (int num32 = 1; num32 < array12.Length; num32++)
			{
				int num33 = array12[num32];
				Vector2 vector2 = array21[num33];
				for (int num34 = 0; num34 < num32; num34++)
				{
					int num35 = array12[num34];
					Vector2 vector3 = array21[num35];
					Vector2 vector4 = vector3 - vector2;
					if (Mathf.Abs(vector4.x) < 1E-06f && Mathf.Abs(vector4.y) < 1E-06f)
					{
						array12[num32] = num35;
					}
				}
			}
			float num36 = 0f;
			float num37 = 0f;
			for (int num38 = 0; num38 < array21.Length; num38++)
			{
				Vector2 vector5 = array21[num38];
				vector5.x = Mathf.Abs(vector5.x);
				vector5.y = Mathf.Abs(vector5.y);
				if (vector5.x > num36)
				{
					num36 = vector5.x;
				}
				if (vector5.y > num37)
				{
					num37 = vector5.y;
				}
			}
			num36 = 0.5f / num36;
			num37 = 0.5f / num37;
			Rect regionForInfill = infill.regionForInfill;
			for (int num39 = 0; num39 < array21.Length; num39++)
			{
				Vector2 vector6 = array21[num39];
				vector6.x *= num36;
				vector6.y *= num37;
				vector6.x += 0.5f;
				vector6.y += 0.5f;
				vector6.x *= regionForInfill.width;
				vector6.y *= regionForInfill.height;
				vector6.x += regionForInfill.x;
				vector6.y += regionForInfill.y;
				array21[num39] = vector6;
			}
			num26 = num13;
			num27 = num13 * 2;
			Array.Copy(array13, 0, array13, num26, num13);
			Array.Copy(array13, 0, array13, num27, num13);
			Array.Copy(array21, 0, array15, num26, num13);
			Array.Copy(array21, 0, array15, num27, num13);
			if (flag3)
			{
				for (int num40 = 0; num40 < num13; num40++)
				{
					array16[num40 + num26] = Vector2.zero;
				}
				for (int num41 = 0; num41 < num13; num41++)
				{
					array16[num41 + num27] = Vector2.zero;
				}
			}
			if (flag2)
			{
				Vector3 vector7 = (Vector3)plane * -1f;
				vector7.Normalize();
				for (int num42 = num26; num42 < num27; num42++)
				{
					array14[num42] = vector7;
				}
				Vector3 vector8 = plane;
				vector8.Normalize();
				for (int num43 = num27; num43 < array14.Length; num43++)
				{
					array14[num43] = vector8;
				}
			}
		}
		int[] array22 = new int[num2];
		int[] array23 = new int[num3];
		for (int num44 = 0; num44 < array10.Length; num44++)
		{
			int index2 = array11[num44];
			list[index2] = array10[num44];
		}
		int num45 = 0;
		int num46 = 0;
		foreach (SplitAction item2 in list)
		{
			if ((item2.flags & 1) == 1)
			{
				array22[num45] = item2.realIndex;
				num45++;
			}
			if ((item2.flags & 2) == 2)
			{
				array23[num46] = item2.realIndex;
				num46++;
			}
		}
		int num47 = 0;
		int[] array24 = new int[3];
		int[] array25 = new int[6];
		short[] array26 = array6;
		foreach (short num49 in array26)
		{
			switch (num49)
			{
			case 3:
				array24[0] = array22[num47];
				array24[1] = array22[num47 + 1];
				array24[2] = array22[num47 + 2];
				frontIndices.AddArray(array24);
				break;
			case 4:
				array25[0] = array22[num47];
				array25[1] = array22[num47 + 1];
				array25[2] = array22[num47 + 3];
				array25[3] = array22[num47 + 1];
				array25[4] = array22[num47 + 2];
				array25[5] = array22[num47 + 3];
				frontIndices.AddArray(array25);
				break;
			}
			num47 += num49;
		}
		num47 = 0;
		short[] array27 = array7;
		foreach (short num51 in array27)
		{
			switch (num51)
			{
			case 3:
				array24[0] = array23[num47];
				array24[1] = array23[num47 + 1];
				array24[2] = array23[num47 + 2];
				backIndices.AddArray(array24);
				break;
			case 4:
				array25[0] = array23[num47];
				array25[1] = array23[num47 + 1];
				array25[2] = array23[num47 + 3];
				array25[3] = array23[num47 + 1];
				array25[4] = array23[num47 + 2];
				array25[5] = array23[num47 + 3];
				backIndices.AddArray(array25);
				break;
			}
			num47 += num51;
		}
		meshCache.vertices.AddArray(array13);
		if (flag2)
		{
			meshCache.normals.AddArray(array14);
		}
		meshCache.coords.AddArray(array15);
		if (flag3)
		{
			meshCache.coords2.AddArray(array16);
		}
		if (!flag)
		{
			return;
		}
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<List<int>> list4 = new List<List<int>>();
		List<int> list5 = new List<int>();
		int num52 = -1;
		do
		{
			for (int num53 = 0; num53 < array12.Length; num53++)
			{
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				if (num52 < 0)
				{
					flag4 = !list5.Contains(num53);
				}
				else if (num52 == num53)
				{
					flag5 = true;
				}
				else
				{
					bool flag7 = array12[num53] == array12[num52];
					if (flag7 && list2.Contains(num53))
					{
						list4.Add(list3);
						flag6 = true;
					}
					else
					{
						flag4 = flag7;
					}
				}
				if (flag4)
				{
					int num54 = ((num53 % 2 != 1) ? (num53 + 1) : (num53 - 1));
					int[] collection = new int[2] { num53, num54 };
					list2.AddRange(collection);
					list5.AddRange(collection);
					list3.Add(num54);
					num52 = num54;
					num53 = num54;
				}
				else if (flag5)
				{
					flag6 = true;
				}
				if (flag6)
				{
					list2.Clear();
					list3 = new List<int>();
					num52 = -1;
				}
			}
		}
		while (list2.Count > 0);
		foreach (List<int> item3 in list4)
		{
			Vector2[] array28 = new Vector2[item3.Count];
			for (int num55 = 0; num55 < array28.Length; num55++)
			{
				int num56 = array12[item3[num55]];
				array28[num55] = array21[num56];
			}
			List<Vector2> list6 = new List<Vector2>(array28.Length);
			List<int> list7 = new List<int>(item3.Count);
			for (int num57 = 0; num57 < item3.Count; num57++)
			{
				bool flag8 = false;
				int num58 = item3[num57];
				int num59 = array12[num58];
				for (int num60 = 0; num60 < list7.Count; num60++)
				{
					if (flag8)
					{
						break;
					}
					int num61 = list7[num60];
					int num62 = array12[num61];
					flag8 = num59 == num62;
				}
				if (!flag8)
				{
					list6.Add(array28[num57]);
					list7.Add(item3[num57]);
				}
			}
			int[] result;
			if (!Triangulation.triangulate(list6.ToArray(), out result))
			{
				continue;
			}
			int[] array29 = new int[result.Length];
			int[] array30 = new int[result.Length];
			for (int num63 = 0; num63 < result.Length; num63++)
			{
				int num64 = list7[result[num63]];
				int num65 = array12[num64];
				array29[num63] = num65 + num26 + count;
				array30[num63] = num65 + num27 + count;
			}
			for (int num66 = 0; num66 < result.Length; num66 += 3)
			{
				int num67 = array29[num66];
				array29[num66] = array29[num66 + 2];
				array29[num66 + 2] = num67;
			}
			frontIndices.AddArray(array29);
			backIndices.AddArray(array30);
			if (infillGeometryReceivers != null)
			{
				int[] array31 = new int[result.Length];
				for (int num68 = 0; num68 < result.Length; num68++)
				{
					int num69 = list7[result[num68]];
					int num70 = array12[num69];
					array31[num68] = num70;
				}
				infillGeometryReceivers(array13, array15, array14, array31);
			}
		}
	}

	private void perfectSubset(TurboList<int> _sourceIndices, TurboList<Vector3> _sourceVertices, TurboList<Vector3> _sourceNormals, TurboList<Vector2> _sourceUVs, TurboList<Vector2> _sourceUV2s, out int[] targetIndices, TurboList<Vector3> targetVertices, TurboList<Vector3> targetNormals, TurboList<Vector2> targetUVs, TurboList<Vector2> targetUV2s, ref int[] transferTable)
	{
		int[] array = _sourceIndices.array;
		Vector3[] array2 = _sourceVertices.array;
		Vector2[] array3 = _sourceUVs.array;
		Vector2[] array4 = ((_sourceUV2s != null) ? _sourceUV2s.array : null);
		Vector3[] array5 = null;
		if (_sourceNormals != null)
		{
			array5 = _sourceNormals.array;
		}
		targetIndices = new int[_sourceIndices.Count];
		int num = targetVertices.Count;
		for (int i = 0; i < _sourceIndices.Count; i++)
		{
			int num2 = array[i];
			int num3 = transferTable[num2];
			if (num3 == -1)
			{
				num3 = num;
				transferTable[num2] = num3;
				num++;
			}
			targetIndices[i] = num3;
		}
		targetVertices.EnsureCapacity(num);
		if (targetNormals != null)
		{
			targetNormals.EnsureCapacity(num);
		}
		targetUVs.EnsureCapacity(num);
		if (targetUV2s != null)
		{
			targetUV2s.EnsureCapacity(num);
		}
		targetVertices.Count = num;
		if (targetNormals != null)
		{
			targetNormals.Count = num;
		}
		targetUVs.Count = num;
		for (int j = 0; j < transferTable.Length; j++)
		{
			int num4 = transferTable[j];
			if (num4 != -1)
			{
				targetVertices.array[num4] = array2[j];
			}
		}
		if (targetNormals != null)
		{
			for (int k = 0; k < transferTable.Length; k++)
			{
				int num5 = transferTable[k];
				if (num5 != -1)
				{
					targetNormals.array[num5] = array5[k];
				}
			}
		}
		for (int l = 0; l < transferTable.Length; l++)
		{
			int num6 = transferTable[l];
			if (num6 != -1)
			{
				targetUVs.array[num6] = array3[l];
			}
		}
		if (targetUV2s == null)
		{
			return;
		}
		for (int m = 0; m < transferTable.Length; m++)
		{
			int num7 = transferTable[m];
			if (num7 != -1)
			{
				targetUV2s.array[num7] = array4[m];
			}
		}
	}

	private MeshCache cacheFromGameObject(Sliceable sliceable, bool includeRoomForGrowth)
	{
		Renderer meshRenderer = getMeshRenderer(sliceable);
		Mesh mesh = getMesh(sliceable);
		int capacity = ((!includeRoomForGrowth) ? mesh.vertexCount : Mathf.RoundToInt((float)mesh.vertexCount * 4.5f));
		MeshCache meshCache = new MeshCache();
		meshCache.vertices = new TurboList<Vector3>(capacity);
		if (sliceable.channelNormals)
		{
			meshCache.normals = new TurboList<Vector3>(capacity);
		}
		meshCache.coords = new TurboList<Vector2>(capacity);
		if (sliceable.channelUV2)
		{
			meshCache.coords2 = new TurboList<Vector2>(capacity);
		}
		meshCache.indices = new int[mesh.subMeshCount][];
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			meshCache.indices[i] = mesh.GetTriangles(i);
		}
		meshCache.vertices.AddArray(mesh.vertices);
		if (sliceable.channelNormals)
		{
			meshCache.normals.AddArray(mesh.normals);
		}
		meshCache.coords.AddArray(mesh.uv);
		if (sliceable.channelUV2)
		{
			meshCache.coords2.AddArray(mesh.uv2);
		}
		if (meshRenderer != null)
		{
			if (meshRenderer.sharedMaterials == null)
			{
				meshCache.mats = new Material[1];
				meshCache.mats[0] = meshRenderer.sharedMaterial;
			}
			else
			{
				meshCache.mats = meshRenderer.sharedMaterials;
			}
		}
		else
		{
			Debug.LogError("Object '" + sliceable.name + "' has no renderer");
		}
		return meshCache;
	}

	private static Renderer getMeshRenderer(Sliceable s)
	{
		GameObject meshHolder = getMeshHolder(s);
		if (meshHolder != null)
		{
			return meshHolder.GetComponent(typeof(Renderer)) as Renderer;
		}
		return null;
	}

	private Mesh getMesh(Sliceable s)
	{
		GameObject meshHolder = getMeshHolder(s);
		Renderer component = meshHolder.GetComponent<Renderer>();
		Mesh mesh = null;
		if (component is MeshRenderer)
		{
			mesh = meshHolder.GetComponent<MeshFilter>().mesh;
		}
		else if (component is SkinnedMeshRenderer)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = component as SkinnedMeshRenderer;
			mesh = new Mesh();
			skinnedMeshRenderer.BakeMesh(mesh);
			meshDeletionQueue.Enqueue(mesh);
		}
		return mesh;
	}

	private static void setMesh(Sliceable s, Mesh mesh)
	{
		if ((bool)s)
		{
			GameObject meshHolder = getMeshHolder(s);
			Renderer component = meshHolder.GetComponent<Renderer>();
			MeshFilter meshFilter = null;
			if (component is MeshRenderer)
			{
				meshFilter = meshHolder.GetComponent<MeshFilter>();
			}
			else if (component is SkinnedMeshRenderer)
			{
				meshHolder = (s.explicitlySelectedMeshHolder = s.gameObject);
				Material[] sharedMaterials = component.sharedMaterials;
				UnityEngine.Object.DestroyImmediate(component);
				component = meshHolder.AddComponent<MeshRenderer>();
				component.sharedMaterials = sharedMaterials;
				meshFilter = meshHolder.AddComponent<MeshFilter>();
			}
			if (meshFilter != null)
			{
				meshFilter.mesh = mesh;
			}
		}
	}

	private static GameObject getMeshHolder(Sliceable s)
	{
		if (!s)
		{
			return null;
		}
		if (s.explicitlySelectedMeshHolder != null)
		{
			return s.explicitlySelectedMeshHolder;
		}
		MeshFilter component = s.GetComponent<MeshFilter>();
		if (component != null)
		{
			return component.gameObject;
		}
		return null;
	}

	private static Sliceable ensureSliceable(GameObject go)
	{
		Sliceable sliceable = go.GetComponent<Sliceable>();
		if (sliceable == null)
		{
			Debug.LogWarning("Turbo Slicer was given an object (" + go.name + ") with no Sliceable; improvising.");
			sliceable = go.AddComponent<Sliceable>();
			sliceable.refreshColliders = true;
			sliceable.currentlySliceable = true;
		}
		return sliceable;
	}

	private int[] concatenateIndexArrays(int[][] arrays)
	{
		int num = 0;
		for (int i = 0; i < arrays.Length; i++)
		{
			num += arrays[i].Length;
		}
		int[] array = new int[num];
		int destinationIndex = 0;
		for (int j = 0; j < arrays.Length; j++)
		{
			Array.Copy(arrays[j], 0, array, destinationIndex, arrays[j].Length);
		}
		return array;
	}

	private void RealculateTangents(Vector3[] vertices, Vector3[] normals, Vector2[] coords, int[] triangles, out Vector4[] tangents)
	{
		int num = vertices.Length;
		int num2 = triangles.Length / 3;
		tangents = new Vector4[num];
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		int num3 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num4 = triangles[num3];
			int num5 = triangles[num3 + 1];
			int num6 = triangles[num3 + 2];
			Vector3 vector = vertices[num4];
			Vector3 vector2 = vertices[num5];
			Vector3 vector3 = vertices[num6];
			Vector2 vector4 = coords[num4];
			Vector2 vector5 = coords[num5];
			Vector2 vector6 = coords[num6];
			float num7 = vector2.x - vector.x;
			float num8 = vector3.x - vector.x;
			float num9 = vector2.y - vector.y;
			float num10 = vector3.y - vector.y;
			float num11 = vector2.z - vector.z;
			float num12 = vector3.z - vector.z;
			float num13 = vector5.x - vector4.x;
			float num14 = vector6.x - vector4.x;
			float num15 = vector5.y - vector4.y;
			float num16 = vector6.y - vector4.y;
			float num17 = 1f / (num13 * num16 - num14 * num15);
			Vector3 vector7 = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
			Vector3 vector8 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
			array[num4] += vector7;
			array[num5] += vector7;
			array[num6] += vector7;
			array2[num4] += vector8;
			array2[num5] += vector8;
			array2[num6] += vector8;
			num3 += 3;
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 normal = normals[j];
			Vector3 tangent = array[j];
			Vector3.OrthoNormalize(ref normal, ref tangent);
			tangents[j].x = tangent.x;
			tangents[j].y = tangent.y;
			tangents[j].z = tangent.z;
			tangents[j].w = ((!(Vector3.Dot(Vector3.Cross(normal, tangent), array2[j]) < 0f)) ? 1f : (-1f));
		}
	}

	private void Start()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void releaseMesh(Mesh m)
	{
		if (meshCaches != null && meshCaches.ContainsKey(m))
		{
			meshCaches.Remove(m);
		}
		UnityEngine.Object.DestroyImmediate(m);
	}

	private void Update()
	{
		float time = Time.time;
		Queue<Mesh> queue = new Queue<Mesh>();
		foreach (KeyValuePair<Mesh, MeshCache> meshCache in meshCaches)
		{
			float num = time - meshCache.Value.creationTime;
			if (num > 5f)
			{
				queue.Enqueue(meshCache.Key);
			}
		}
		while (queue.Count > 0)
		{
			Mesh key = queue.Dequeue();
			meshCaches.Remove(key);
		}
		while (meshDeletionQueue.Count > 0)
		{
			Mesh m = meshDeletionQueue.Dequeue();
			releaseMesh(m);
		}
	}
}
