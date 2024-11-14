using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class RopePersistManager
{
	private class RopeData
	{
		public class TransformInfo
		{
			public GameObject goObject;

			public string strObjectName;

			public Transform tfParent;

			public Vector3 v3LocalPosition;

			public Quaternion qLocalOrientation;

			public Vector3 v3LocalScale;

			public bool bLinkMarkedKinematic;

			public bool bExtensibleKinematic;
		}

		public UltimateRope m_rope;

		public bool m_bDeleted;

		public Dictionary<string, object> m_hashFieldName2Value;

		public bool m_bSkin;

		public Vector3[] m_av3SkinVertices;

		public Vector2[] m_av2SkinMapping;

		public Vector4[] m_av4SkinTangents;

		public BoneWeight[] m_aSkinBoneWeights;

		public int[] m_anSkinTrianglesRope;

		public int[] m_anSkinTrianglesSections;

		public Matrix4x4[] m_amtxSkinBindPoses;

		public TransformInfo m_transformInfoRope;

		public TransformInfo[] m_aLinkTransformInfo;

		public TransformInfo m_transformInfoStart;

		public TransformInfo[] m_transformInfoSegments;

		public bool[][] m_aaJointsProcessed;

		public bool[][] m_aaJointsBroken;

		public RopeData(UltimateRope rope)
		{
			m_rope = rope;
			m_hashFieldName2Value = new Dictionary<string, object>();
			m_aLinkTransformInfo = new TransformInfo[rope.TotalLinks];
			m_transformInfoSegments = new TransformInfo[rope.RopeNodes.Count];
			m_bSkin = rope.GetComponent<SkinnedMeshRenderer>() != null;
			if (m_bSkin)
			{
				SkinnedMeshRenderer component = rope.GetComponent<SkinnedMeshRenderer>();
				Mesh sharedMesh = component.sharedMesh;
				int vertexCount = component.sharedMesh.vertexCount;
				int num = component.sharedMesh.GetTriangles(0).Length;
				int num2 = component.sharedMesh.GetTriangles(1).Length;
				m_av3SkinVertices = new Vector3[vertexCount];
				m_av2SkinMapping = new Vector2[vertexCount];
				m_av4SkinTangents = ((sharedMesh.tangents == null) ? null : new Vector4[sharedMesh.tangents.Length]);
				m_aSkinBoneWeights = new BoneWeight[vertexCount];
				m_anSkinTrianglesRope = new int[num];
				m_anSkinTrianglesSections = new int[num2];
				m_amtxSkinBindPoses = new Matrix4x4[component.sharedMesh.bindposes.Length];
				MakeSkinDeepCopy(sharedMesh.vertices, sharedMesh.uv, sharedMesh.tangents, sharedMesh.boneWeights, sharedMesh.GetTriangles(0), sharedMesh.GetTriangles(1), sharedMesh.bindposes, m_av3SkinVertices, m_av2SkinMapping, m_av4SkinTangents, m_aSkinBoneWeights, m_anSkinTrianglesRope, m_anSkinTrianglesSections, m_amtxSkinBindPoses);
			}
		}

		public static void MakeSkinDeepCopy(Vector3[] av3VerticesSource, Vector2[] av2MappingSource, Vector4[] av4TangentsSource, BoneWeight[] aBoneWeightsSource, int[] anTrianglesRopeSource, int[] anTrianglesSectionsSource, Matrix4x4[] aBindPosesSource, Vector3[] av3VerticesDestiny, Vector2[] av2MappingDestiny, Vector4[] av4TangentsDestiny, BoneWeight[] aBoneWeightsDestiny, int[] anTrianglesRopeDestiny, int[] anTrianglesSectionsDestiny, Matrix4x4[] aBindPosesDestiny)
		{
			int num = av3VerticesSource.Length;
			for (int i = 0; i < num; i++)
			{
				av3VerticesDestiny[i] = av3VerticesSource[i];
				av2MappingDestiny[i] = av2MappingSource[i];
				if (av4TangentsDestiny != null && av4TangentsSource != null && av4TangentsDestiny.Length == num && av4TangentsSource.Length == num)
				{
					av4TangentsDestiny[i] = av4TangentsSource[i];
				}
				aBoneWeightsDestiny[i] = aBoneWeightsSource[i];
			}
			for (int j = 0; j < anTrianglesRopeDestiny.Length; j++)
			{
				anTrianglesRopeDestiny[j] = anTrianglesRopeSource[j];
			}
			for (int k = 0; k < anTrianglesSectionsDestiny.Length; k++)
			{
				anTrianglesSectionsDestiny[k] = anTrianglesSectionsSource[k];
			}
			for (int l = 0; l < aBindPosesSource.Length; l++)
			{
				aBindPosesDestiny[l] = aBindPosesSource[l];
			}
		}
	}

	private static Dictionary<int, RopeData> s_hashInstanceID2RopeData;

	static RopePersistManager()
	{
		s_hashInstanceID2RopeData = new Dictionary<int, RopeData>();
	}

	public static void StorePersistentData(UltimateRope rope)
	{
		RopeData ropeData = new RopeData(rope);
		FieldInfo[] fields = rope.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (Attribute.IsDefined(fieldInfo, typeof(RopePersistAttribute)))
			{
				ropeData.m_hashFieldName2Value.Add(fieldInfo.Name, fieldInfo.GetValue(rope));
			}
		}
		if (rope.Deleted)
		{
			ropeData.m_bDeleted = true;
		}
		else
		{
			ropeData.m_aaJointsBroken = new bool[rope.RopeNodes.Count][];
			ropeData.m_aaJointsProcessed = new bool[rope.RopeNodes.Count][];
			ropeData.m_transformInfoRope = ComputeTransformInfo(rope, rope.gameObject, (!(rope.transform.parent != null)) ? null : rope.transform.parent.gameObject);
			if (rope.RopeStart != null)
			{
				ropeData.m_transformInfoStart = ComputeTransformInfo(rope, rope.RopeStart, (!(rope.RopeStart.transform.parent != null)) ? null : rope.RopeStart.transform.parent.gameObject);
			}
			int num = 0;
			for (int j = 0; j < rope.RopeNodes.Count; j++)
			{
				if (rope.RopeNodes[j].goNode != null)
				{
					ropeData.m_transformInfoSegments[j] = ComputeTransformInfo(rope, rope.RopeNodes[j].goNode, (!(rope.RopeNodes[j].goNode.transform.parent != null)) ? null : rope.RopeNodes[j].goNode.transform.parent.gameObject);
				}
				GameObject[] segmentLinks = rope.RopeNodes[j].segmentLinks;
				foreach (GameObject node in segmentLinks)
				{
					ropeData.m_aLinkTransformInfo[num] = ComputeTransformInfo(rope, node, (rope.RopeType != UltimateRope.ERopeType.ImportBones) ? rope.RopeNodes[j].goNode.transform.gameObject : rope.ImportedBones[num].tfNonBoneParent.gameObject);
					num++;
				}
				ropeData.m_aaJointsBroken[j] = new bool[rope.RopeNodes[j].linkJoints.Length];
				ropeData.m_aaJointsProcessed[j] = new bool[rope.RopeNodes[j].linkJointBreaksProcessed.Length];
				for (int l = 0; l < rope.RopeNodes[j].linkJoints.Length; l++)
				{
					ropeData.m_aaJointsBroken[j][l] = rope.RopeNodes[j].linkJoints[l] == null;
				}
				for (int m = 0; m < rope.RopeNodes[j].linkJoints.Length; m++)
				{
					ropeData.m_aaJointsProcessed[j][m] = rope.RopeNodes[j].linkJointBreaksProcessed[m];
				}
			}
			ropeData.m_bDeleted = false;
		}
		s_hashInstanceID2RopeData.Add(rope.GetInstanceID(), ropeData);
	}

	public static void RetrievePersistentData(UltimateRope rope)
	{
		RopeData ropeData = s_hashInstanceID2RopeData[rope.GetInstanceID()];
		FieldInfo[] fields = rope.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(rope, ropeData.m_hashFieldName2Value[fieldInfo.Name]);
		}
		if (ropeData.m_bDeleted)
		{
			rope.DeleteRope();
			return;
		}
		SetTransformInfo(ropeData.m_transformInfoRope, rope.gameObject);
		if (rope.RopeStart != null)
		{
			if (ropeData.m_transformInfoStart.goObject == null)
			{
				rope.RopeStart = new GameObject(ropeData.m_transformInfoStart.strObjectName);
			}
			SetTransformInfo(ropeData.m_transformInfoStart, rope.RopeStart);
		}
		if (rope.RopeType != UltimateRope.ERopeType.ImportBones)
		{
			rope.DeleteRopeLinks();
		}
		int num = 0;
		for (int j = 0; j < rope.RopeNodes.Count; j++)
		{
			if (rope.RopeType != UltimateRope.ERopeType.ImportBones)
			{
				for (int k = 0; k < rope.RopeNodes[j].linkJoints.Length; k++)
				{
					rope.RopeNodes[j].linkJointBreaksProcessed[k] = ropeData.m_aaJointsProcessed[j][k];
				}
				if (rope.RopeNodes[j].goNode != null)
				{
					if (ropeData.m_transformInfoSegments[j].goObject == null)
					{
						rope.RopeNodes[j].goNode = new GameObject(ropeData.m_transformInfoSegments[j].strObjectName);
					}
					SetTransformInfo(ropeData.m_transformInfoSegments[j], rope.RopeNodes[j].goNode);
				}
			}
			if (rope.RopeType != UltimateRope.ERopeType.ImportBones)
			{
				rope.RopeNodes[j].segmentLinks = new GameObject[rope.RopeNodes[j].nTotalLinks];
			}
			for (int l = 0; l < rope.RopeNodes[j].segmentLinks.Length; l++)
			{
				if (rope.RopeType != UltimateRope.ERopeType.ImportBones)
				{
					if (rope.RopeType == UltimateRope.ERopeType.Procedural)
					{
						rope.RopeNodes[j].segmentLinks[l] = new GameObject(ropeData.m_aLinkTransformInfo[num].strObjectName);
					}
					else if (rope.RopeType == UltimateRope.ERopeType.LinkedObjects)
					{
						rope.RopeNodes[j].segmentLinks[l] = UnityEngine.Object.Instantiate(rope.LinkObject);
						rope.RopeNodes[j].segmentLinks[l].name = ropeData.m_aLinkTransformInfo[num].strObjectName;
					}
					rope.RopeNodes[j].segmentLinks[l].AddComponent<UltimateRopeLink>();
					rope.RopeNodes[j].segmentLinks[l].transform.parent = ((!rope.FirstNodeIsCoil() || j != 0) ? rope.gameObject.transform : rope.CoilObject.transform);
					if (!rope.RopeNodes[j].bIsCoil)
					{
						rope.RopeNodes[j].segmentLinks[l].AddComponent<Rigidbody>();
						rope.RopeNodes[j].segmentLinks[l].GetComponent<Rigidbody>().isKinematic = ropeData.m_aLinkTransformInfo[num].bExtensibleKinematic || ropeData.m_aLinkTransformInfo[num].bLinkMarkedKinematic;
					}
				}
				SetTransformInfo(ropeData.m_aLinkTransformInfo[num], rope.RopeNodes[j].segmentLinks[l]);
				if (rope.RopeType == UltimateRope.ERopeType.ImportBones)
				{
					rope.RopeNodes[j].segmentLinks[l].transform.parent = ((!rope.ImportedBones[num].bIsStatic) ? rope.transform : rope.ImportedBones[num].tfNonBoneParent);
				}
				if (ropeData.m_aLinkTransformInfo[num].bExtensibleKinematic)
				{
					UltimateRopeLink component = rope.RopeNodes[j].segmentLinks[l].GetComponent<UltimateRopeLink>();
					if (component != null)
					{
						component.ExtensibleKinematic = true;
					}
					rope.RopeNodes[j].segmentLinks[l].transform.parent = ((j <= rope.m_nFirstNonCoilNode) ? rope.RopeStart.transform : rope.RopeNodes[j - 1].goNode.transform);
					rope.RopeNodes[j].segmentLinks[l].transform.position = rope.RopeNodes[j].segmentLinks[l].transform.parent.position;
					Vector3 forward = rope.RopeNodes[j].segmentLinks[l].transform.parent.TransformDirection(rope.RopeNodes[j].m_v3LocalDirectionForward);
					Vector3 upwards = rope.RopeNodes[j].segmentLinks[l].transform.parent.TransformDirection(rope.RopeNodes[j].m_v3LocalDirectionUp);
					rope.RopeNodes[j].segmentLinks[l].transform.rotation = Quaternion.LookRotation(forward, upwards);
				}
				num++;
			}
		}
		rope.SetupRopeLinks();
		SkinnedMeshRenderer skinnedMeshRenderer = rope.GetComponent<SkinnedMeshRenderer>();
		if (!ropeData.m_bSkin)
		{
			if (skinnedMeshRenderer != null)
			{
				UnityEngine.Object.DestroyImmediate(skinnedMeshRenderer);
			}
			return;
		}
		if (skinnedMeshRenderer == null)
		{
			skinnedMeshRenderer = rope.gameObject.AddComponent<SkinnedMeshRenderer>();
		}
		int num2 = ropeData.m_av3SkinVertices.Length;
		int num3 = ropeData.m_anSkinTrianglesRope.Length;
		int num4 = ropeData.m_anSkinTrianglesSections.Length;
		Vector3[] array = new Vector3[num2];
		Vector2[] array2 = new Vector2[num2];
		Vector4[] array3 = ((ropeData.m_av4SkinTangents == null) ? null : new Vector4[ropeData.m_av4SkinTangents.Length]);
		BoneWeight[] array4 = new BoneWeight[num2];
		int[] array5 = new int[num3];
		int[] array6 = new int[num4];
		Matrix4x4[] array7 = new Matrix4x4[ropeData.m_amtxSkinBindPoses.Length];
		Mesh mesh = new Mesh();
		RopeData.MakeSkinDeepCopy(ropeData.m_av3SkinVertices, ropeData.m_av2SkinMapping, ropeData.m_av4SkinTangents, ropeData.m_aSkinBoneWeights, ropeData.m_anSkinTrianglesRope, ropeData.m_anSkinTrianglesSections, ropeData.m_amtxSkinBindPoses, array, array2, array3, array4, array5, array6, array7);
		Transform[] array8 = new Transform[rope.TotalLinks];
		num = 0;
		for (int m = 0; m < rope.RopeNodes.Count; m++)
		{
			for (int n = 0; n < rope.RopeNodes[m].segmentLinks.Length; n++)
			{
				array8[num++] = rope.RopeNodes[m].segmentLinks[n].transform;
			}
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.boneWeights = array4;
		mesh.bindposes = array7;
		mesh.subMeshCount = 2;
		mesh.SetTriangles(array5, 0);
		mesh.SetTriangles(array6, 1);
		mesh.RecalculateNormals();
		if (array3 != null && array3.Length == num2)
		{
			mesh.tangents = array3;
		}
		skinnedMeshRenderer.bones = array8;
		skinnedMeshRenderer.sharedMesh = mesh;
		skinnedMeshRenderer.materials = new Material[2] { rope.RopeMaterial, rope.RopeSectionMaterial };
	}

	public static bool PersistentDataExists(UltimateRope rope)
	{
		return s_hashInstanceID2RopeData.ContainsKey(rope.GetInstanceID());
	}

	public static void RemovePersistentData(UltimateRope rope)
	{
		s_hashInstanceID2RopeData.Remove(rope.GetInstanceID());
	}

	private static RopeData.TransformInfo ComputeTransformInfo(UltimateRope rope, GameObject node, GameObject parent)
	{
		RopeData.TransformInfo transformInfo = new RopeData.TransformInfo();
		transformInfo.goObject = node;
		transformInfo.strObjectName = node.name;
		transformInfo.tfParent = ((!(parent == null)) ? parent.transform : null);
		if (transformInfo.tfParent != null)
		{
			transformInfo.v3LocalPosition = transformInfo.tfParent.InverseTransformPoint(node.transform.position);
			transformInfo.qLocalOrientation = Quaternion.Inverse(transformInfo.tfParent.rotation) * node.transform.rotation;
		}
		else
		{
			transformInfo.v3LocalPosition = node.transform.position;
			transformInfo.qLocalOrientation = node.transform.rotation;
		}
		transformInfo.v3LocalScale = node.transform.localScale;
		UltimateRopeLink component = node.GetComponent<UltimateRopeLink>();
		if (component != null)
		{
			transformInfo.bExtensibleKinematic = component.ExtensibleKinematic;
			transformInfo.bLinkMarkedKinematic = node.GetComponent<Rigidbody>() != null && node.GetComponent<Rigidbody>().isKinematic;
		}
		else
		{
			transformInfo.bExtensibleKinematic = false;
			transformInfo.bLinkMarkedKinematic = false;
		}
		return transformInfo;
	}

	private static void SetTransformInfo(RopeData.TransformInfo transformInfo, GameObject node)
	{
		if (transformInfo.tfParent != null)
		{
			node.transform.position = transformInfo.tfParent.TransformPoint(transformInfo.v3LocalPosition);
			node.transform.rotation = transformInfo.tfParent.rotation * transformInfo.qLocalOrientation;
		}
		else
		{
			node.transform.position = transformInfo.v3LocalPosition;
			node.transform.rotation = transformInfo.qLocalOrientation;
		}
		node.transform.localScale = transformInfo.v3LocalScale;
	}
}
