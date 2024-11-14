using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UltimateRope : MonoBehaviour
{
	public enum ERopeType
	{
		Procedural = 0,
		LinkedObjects = 1,
		ImportBones = 2
	}

	public enum EAxis
	{
		MinusX = 0,
		MinusY = 1,
		MinusZ = 2,
		X = 3,
		Y = 4,
		Z = 5
	}

	public enum EColliderType
	{
		None = 0,
		Capsule = 1,
		Box = 2
	}

	public enum ERopeExtensionMode
	{
		CoilRotationIncrement = 0,
		LinearExtensionIncrement = 1
	}

	[Serializable]
	public class RopeNode
	{
		public GameObject goNode;

		public float fLength;

		public float fTotalLength;

		public int nNumLinks;

		public int nTotalLinks;

		public EColliderType eColliderType;

		public int nColliderSkip;

		public bool bFold;

		public bool bIsCoil;

		public bool bInitialOrientationInitialized;

		public Vector3 v3InitialLocalPos;

		public Quaternion qInitialLocalRot;

		public Vector3 v3InitialLocalScale;

		public bool m_bExtensionInitialized;

		public int m_nExtensionLinkIn;

		public int m_nExtensionLinkOut;

		public float m_fExtensionRemainingLength;

		public float m_fExtensionRemainderIn;

		public float m_fExtensionRemainderOut;

		public Vector3 m_v3LocalDirectionForward;

		public Vector3 m_v3LocalDirectionUp;

		public GameObject[] segmentLinks;

		public ConfigurableJoint[] linkJoints;

		public bool[] linkJointBreaksProcessed;

		public bool bSegmentBroken;

		public RopeNode()
		{
			goNode = null;
			fLength = 5f;
			fTotalLength = fLength;
			nNumLinks = 20;
			nTotalLinks = nNumLinks;
			eColliderType = EColliderType.Capsule;
			nColliderSkip = 1;
			bFold = true;
			bIsCoil = false;
			bInitialOrientationInitialized = false;
			linkJoints = new ConfigurableJoint[0];
			linkJointBreaksProcessed = new bool[0];
			bSegmentBroken = false;
		}
	}

	[Serializable]
	public class RopeBone
	{
		public GameObject goBone;

		public Transform tfParent;

		public Transform tfNonBoneParent;

		public bool bCreatedCollider;

		public bool bIsStatic;

		public float fLength;

		public bool bCreatedRigidbody;

		public int nOriginalLayer;

		public Vector3 v3OriginalLocalScale;

		public Vector3 v3OriginalLocalPos;

		public Quaternion qOriginalLocalRot;

		public RopeBone()
		{
			goBone = null;
			tfParent = null;
			tfNonBoneParent = null;
			bCreatedCollider = false;
			bIsStatic = false;
			fLength = 0f;
			bCreatedRigidbody = false;
			nOriginalLayer = 0;
		}
	}

	public class RopeBreakEventInfo
	{
		public UltimateRope rope;

		public GameObject link1;

		public GameObject link2;

		public Vector3 worldPos;

		public Vector3 localLink1Pos;

		public Vector3 localLink2Pos;
	}

	[RopePersist]
	public ERopeType RopeType;

	[RopePersist]
	public GameObject RopeStart;

	[RopePersist]
	public List<RopeNode> RopeNodes;

	[RopePersist]
	public int RopeLayer;

	[RopePersist]
	public PhysicMaterial RopePhysicsMaterial;

	[RopePersist]
	public float RopeDiameter = 0.1f;

	[RopePersist]
	public float RopeDiameterScaleX = 1f;

	[RopePersist]
	public float RopeDiameterScaleY = 1f;

	[RopePersist]
	public int RopeSegmentSides = 8;

	[RopePersist]
	public Material RopeMaterial;

	[RopePersist]
	public float RopeTextureTileMeters = 1f;

	[RopePersist]
	public Material RopeSectionMaterial;

	[RopePersist]
	public float RopeTextureSectionTileMeters = 1f;

	[RopePersist]
	public bool IsExtensible;

	[RopePersist]
	public float ExtensibleLength = 10f;

	[RopePersist]
	public bool HasACoil;

	[RopePersist]
	public GameObject CoilObject;

	[RopePersist]
	public EAxis CoilAxisRight = EAxis.X;

	[RopePersist]
	public EAxis CoilAxisUp = EAxis.Y;

	[RopePersist]
	public float CoilWidth = 0.5f;

	[RopePersist]
	public float CoilDiameter = 0.5f;

	[RopePersist]
	public int CoilNumBones = 50;

	[RopePersist]
	public GameObject LinkObject;

	[RopePersist]
	public EAxis LinkAxis = EAxis.Z;

	[RopePersist]
	public float LinkOffsetObject;

	[RopePersist]
	public float LinkTwistAngleStart;

	[RopePersist]
	public float LinkTwistAngleIncrement;

	[RopePersist]
	public GameObject BoneFirst;

	[RopePersist]
	public GameObject BoneLast;

	[RopePersist]
	public string BoneListNamesStatic;

	[RopePersist]
	public string BoneListNamesNoColliders;

	[RopePersist]
	public EAxis BoneAxis = EAxis.Z;

	[RopePersist]
	public EColliderType BoneColliderType = EColliderType.Capsule;

	[RopePersist]
	public float BoneColliderDiameter = 0.1f;

	[RopePersist]
	public int BoneColliderSkip;

	[RopePersist]
	public float BoneColliderLength = 1f;

	[RopePersist]
	public float BoneColliderOffset;

	[RopePersist]
	public float LinkMass = 1f;

	[RopePersist]
	public int LinkSolverIterationCount = 100;

	[RopePersist]
	public float LinkJointAngularXLimit = 30f;

	[RopePersist]
	public float LinkJointAngularYLimit = 30f;

	[RopePersist]
	public float LinkJointAngularZLimit = 30f;

	[RopePersist]
	public float LinkJointSpringValue = 1f;

	[RopePersist]
	public float LinkJointDamperValue;

	[RopePersist]
	public float LinkJointMaxForceValue = 1f;

	[RopePersist]
	public float LinkJointBreakForce = float.PositiveInfinity;

	[RopePersist]
	public float LinkJointBreakTorque = float.PositiveInfinity;

	[RopePersist]
	public bool LockStartEndInZAxis;

	[RopePersist]
	public bool SendEvents;

	[RopePersist]
	public GameObject EventsObjectReceiver;

	[RopePersist]
	public string OnBreakMethodName;

	[RopePersist]
	public bool PersistAfterPlayMode;

	[RopePersist]
	public bool EnablePrefabUsage;

	[RopePersist]
	public bool AutoRegenerate = true;

	[HideInInspector]
	[RopePersist]
	public bool Deleted = true;

	[RopePersist]
	[HideInInspector]
	public float[] LinkLengths;

	[HideInInspector]
	[RopePersist]
	public int TotalLinks;

	[RopePersist]
	[HideInInspector]
	public float TotalRopeLength;

	[HideInInspector]
	[RopePersist]
	public bool m_bRopeStartInitialOrientationInitialized;

	[RopePersist]
	[HideInInspector]
	public Vector3 m_v3InitialRopeStartLocalPos;

	[HideInInspector]
	[RopePersist]
	public Quaternion m_qInitialRopeStartLocalRot;

	[HideInInspector]
	[RopePersist]
	public Vector3 m_v3InitialRopeStartLocalScale;

	[HideInInspector]
	[RopePersist]
	public int m_nFirstNonCoilNode;

	[HideInInspector]
	[RopePersist]
	public float[] m_afCoilBoneRadiuses;

	[HideInInspector]
	[RopePersist]
	public float[] m_afCoilBoneAngles;

	[HideInInspector]
	[RopePersist]
	public float[] m_afCoilBoneX;

	[RopePersist]
	[HideInInspector]
	public float m_fCurrentCoilRopeRadius;

	[RopePersist]
	[HideInInspector]
	public float m_fCurrentCoilTurnsLeft;

	[RopePersist]
	[HideInInspector]
	public float m_fCurrentCoilLength;

	[RopePersist]
	[HideInInspector]
	public float m_fCurrentExtension;

	[RopePersist]
	[HideInInspector]
	public float m_fCurrentExtensionInput;

	[RopePersist]
	[HideInInspector]
	public RopeBone[] ImportedBones;

	[HideInInspector]
	[RopePersist]
	public bool m_bBonesAreImported;

	[RopePersist]
	[HideInInspector]
	public string m_strStatus;

	[RopePersist]
	[HideInInspector]
	public bool m_bLastStatusIsError = true;

	[RopePersist]
	[HideInInspector]
	public string m_strAssetFile = string.Empty;

	[HideInInspector]
	public string Status
	{
		get
		{
			return m_strStatus;
		}
		set
		{
			m_strStatus = value;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			CreateRopeJoints(true);
			SetupRopeLinks();
			if (FirstNodeIsCoil())
			{
				RecomputeCoil();
			}
		}
		else
		{
			CheckLoadPersistentData();
		}
	}

	private void OnApplicationQuit()
	{
		CheckSavePersistentData();
	}

	private void Start()
	{
		m_fCurrentExtensionInput = m_fCurrentExtension;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (RopeNodes == null || RopeNodes.Count == 0)
		{
			return;
		}
		int num = -1;
		if (RopeType == ERopeType.Procedural && (LinkJointBreakForce != float.PositiveInfinity || LinkJointBreakTorque != float.PositiveInfinity))
		{
			SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			if (component == null)
			{
				return;
			}
			Mesh sharedMesh = component.sharedMesh;
			int[] indices = null;
			int[] indices2 = null;
			int num2 = 0;
			for (int i = 0; i < RopeNodes.Count; i++)
			{
				RopeNode ropeNode = RopeNodes[i];
				if (ropeNode.bIsCoil)
				{
					num2 += ropeNode.segmentLinks.Length;
					continue;
				}
				for (int j = 0; j < ropeNode.linkJoints.Length; j++)
				{
					if (ropeNode.linkJoints[j] == null && !ropeNode.linkJointBreaksProcessed[j])
					{
						ropeNode.linkJointBreaksProcessed[j] = true;
						bool flag = i == 0 && j == 0 && !FirstNodeIsCoil();
						bool flag2 = i == RopeNodes.Count - 1 && j == ropeNode.linkJoints.Length - 1;
						if (!flag && !flag2)
						{
							indices = sharedMesh.GetTriangles(0);
							indices2 = sharedMesh.GetTriangles(1);
							FillLinkMeshIndicesRope(num2 - 1, TotalLinks, ref indices, true, true);
							FillLinkMeshIndicesSections(num2 - 1, TotalLinks, ref indices2, true, true);
							num = i;
						}
						if (SendEvents && EventsObjectReceiver != null && OnBreakMethodName.Length > 0)
						{
							RopeBreakEventInfo ropeBreakEventInfo = new RopeBreakEventInfo();
							ropeBreakEventInfo.rope = this;
							ropeBreakEventInfo.worldPos = ((j != ropeNode.linkJoints.Length - 1) ? ropeNode.segmentLinks[j].transform.position : ropeNode.goNode.transform.position);
							ropeBreakEventInfo.link2 = ((j != ropeNode.linkJoints.Length - 1) ? ropeNode.segmentLinks[j] : ropeNode.goNode);
							ropeBreakEventInfo.localLink2Pos = Vector3.zero;
							if (flag)
							{
								ropeBreakEventInfo.link1 = RopeStart.gameObject;
								ropeBreakEventInfo.localLink1Pos = Vector3.zero;
							}
							else
							{
								if (j > 0)
								{
									ropeBreakEventInfo.link1 = ropeNode.segmentLinks[j - 1];
								}
								else
								{
									ropeBreakEventInfo.link1 = RopeNodes[i - 1].goNode;
								}
								ropeBreakEventInfo.localLink1Pos = GetLinkAxisOffset(LinkLengths[num2 - 1]);
							}
							EventsObjectReceiver.SendMessage(OnBreakMethodName, ropeBreakEventInfo);
						}
					}
					if (j < ropeNode.segmentLinks.Length)
					{
						num2++;
					}
				}
			}
			if (num != -1 && indices != null && indices2 != null)
			{
				sharedMesh.SetTriangles(indices, 0);
				sharedMesh.SetTriangles(indices2, 1);
				Vector4[] array = null;
				if (sharedMesh.tangents != null && sharedMesh.tangents.Length == sharedMesh.vertexCount)
				{
					array = sharedMesh.tangents;
				}
				sharedMesh.RecalculateNormals();
				if (array != null)
				{
					sharedMesh.tangents = array;
				}
			}
		}
		else if (RopeType == ERopeType.LinkedObjects && (LinkJointBreakForce != float.PositiveInfinity || LinkJointBreakTorque != float.PositiveInfinity) && SendEvents)
		{
			int num3 = 0;
			for (int k = 0; k < RopeNodes.Count; k++)
			{
				RopeNode ropeNode2 = RopeNodes[k];
				if (ropeNode2.bIsCoil)
				{
					num3 += ropeNode2.segmentLinks.Length;
					continue;
				}
				for (int l = 0; l < ropeNode2.linkJoints.Length; l++)
				{
					if (ropeNode2.linkJoints[l] == null && !ropeNode2.linkJointBreaksProcessed[l])
					{
						ropeNode2.linkJointBreaksProcessed[l] = true;
						bool flag3 = k == 0 && l == 0 && !FirstNodeIsCoil();
						num = k;
						if (SendEvents && EventsObjectReceiver != null && OnBreakMethodName.Length > 0)
						{
							RopeBreakEventInfo ropeBreakEventInfo2 = new RopeBreakEventInfo();
							ropeBreakEventInfo2.rope = this;
							ropeBreakEventInfo2.worldPos = ((l != ropeNode2.linkJoints.Length - 1) ? ropeNode2.segmentLinks[l].transform.position : ropeNode2.goNode.transform.position);
							ropeBreakEventInfo2.link2 = ((l != ropeNode2.linkJoints.Length - 1) ? ropeNode2.segmentLinks[l] : ropeNode2.goNode);
							ropeBreakEventInfo2.localLink2Pos = Vector3.zero;
							if (flag3)
							{
								ropeBreakEventInfo2.link1 = RopeStart.gameObject;
								ropeBreakEventInfo2.localLink1Pos = Vector3.zero;
							}
							else
							{
								if (l > 0)
								{
									ropeBreakEventInfo2.link1 = ropeNode2.segmentLinks[l - 1];
								}
								else
								{
									ropeBreakEventInfo2.link1 = RopeNodes[k - 1].goNode;
								}
								ropeBreakEventInfo2.localLink1Pos = GetLinkAxisOffset(LinkLengths[num3 - 1]);
							}
							EventsObjectReceiver.SendMessage(OnBreakMethodName, ropeBreakEventInfo2);
						}
					}
					if (l < ropeNode2.segmentLinks.Length)
					{
						num3++;
					}
				}
			}
		}
		if (num == -1)
		{
			return;
		}
		RopeNode ropeNode3 = RopeNodes[num];
		ropeNode3.bSegmentBroken = true;
		for (int m = 0; m < ropeNode3.linkJoints.Length; m++)
		{
			if (ropeNode3.linkJoints[m] != null)
			{
				ropeNode3.linkJoints[m].breakForce = float.PositiveInfinity;
				ropeNode3.linkJoints[m].breakTorque = float.PositiveInfinity;
			}
		}
	}

	public void DeleteRope(bool bResetNodePositions = false, bool bDestroySkin = true)
	{
		DeleteRopeLinks();
		foreach (RopeNode ropeNode in RopeNodes)
		{
			ropeNode.bSegmentBroken = false;
			if (ropeNode.bInitialOrientationInitialized && bResetNodePositions)
			{
				ropeNode.goNode.transform.localPosition = ropeNode.v3InitialLocalPos;
				ropeNode.goNode.transform.localRotation = ropeNode.qInitialLocalRot;
				ropeNode.goNode.transform.localScale = ropeNode.v3InitialLocalScale;
			}
			ropeNode.bInitialOrientationInitialized = false;
			for (int i = 0; i < ropeNode.linkJoints.Length; i++)
			{
				if (ropeNode.linkJoints[i] != null)
				{
					UnityEngine.Object.DestroyImmediate(ropeNode.linkJoints[i]);
				}
			}
		}
		if (RopeStart != null && m_bRopeStartInitialOrientationInitialized && bResetNodePositions)
		{
			RopeStart.transform.localPosition = m_v3InitialRopeStartLocalPos;
			RopeStart.transform.localRotation = m_qInitialRopeStartLocalRot;
			RopeStart.transform.localScale = m_v3InitialRopeStartLocalScale;
		}
		m_bRopeStartInitialOrientationInitialized = false;
		if (ImportedBones != null)
		{
			RopeBone[] importedBones = ImportedBones;
			foreach (RopeBone ropeBone in importedBones)
			{
				if (ropeBone.goBone != null)
				{
					ropeBone.goBone.layer = ropeBone.nOriginalLayer;
					if (ropeBone.bCreatedCollider && ropeBone.goBone.GetComponent<Collider>() != null)
					{
						UnityEngine.Object.DestroyImmediate(ropeBone.goBone.GetComponent<Collider>());
					}
					if (ropeBone.bCreatedRigidbody && ropeBone.goBone.GetComponent<Rigidbody>() != null)
					{
						UnityEngine.Object.DestroyImmediate(ropeBone.goBone.GetComponent<Rigidbody>());
					}
				}
			}
			RopeBone[] importedBones2 = ImportedBones;
			foreach (RopeBone ropeBone2 in importedBones2)
			{
				if (ropeBone2.goBone != null)
				{
					if (ropeBone2.tfNonBoneParent != null)
					{
						ropeBone2.goBone.transform.parent = ropeBone2.tfNonBoneParent;
						ropeBone2.goBone.transform.localPosition = ropeBone2.v3OriginalLocalPos;
						ropeBone2.goBone.transform.localRotation = ropeBone2.qOriginalLocalRot;
					}
					ropeBone2.goBone.transform.parent = ropeBone2.tfParent;
					ropeBone2.goBone.transform.localScale = ropeBone2.v3OriginalLocalScale;
				}
			}
		}
		if (!Application.isEditor || !Application.isPlaying)
		{
			ImportedBones = null;
		}
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		if ((bool)component)
		{
			UnityEngine.Object.DestroyImmediate(component.sharedMesh);
			if (bDestroySkin)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
		}
		CheckDelCoilNode();
		Deleted = true;
	}

	public void DeleteRopeLinks()
	{
		if (m_bBonesAreImported)
		{
			return;
		}
		if (CoilObject != null)
		{
			for (int i = 0; i < CoilObject.transform.childCount; i++)
			{
				Transform child = CoilObject.transform.GetChild(i);
				if (child.gameObject.GetComponent<UltimateRopeLink>() != null)
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
					i--;
				}
			}
		}
		if (RopeStart != null)
		{
			for (int j = 0; j < RopeStart.transform.childCount; j++)
			{
				Transform child2 = RopeStart.transform.GetChild(j);
				if (child2.gameObject.GetComponent<UltimateRopeLink>() != null)
				{
					UnityEngine.Object.DestroyImmediate(child2.gameObject);
					j--;
				}
			}
		}
		for (int k = 0; k < base.transform.childCount; k++)
		{
			Transform child3 = base.transform.GetChild(k);
			if (child3.gameObject.GetComponent<UltimateRopeLink>() != null)
			{
				UnityEngine.Object.DestroyImmediate(child3.gameObject);
				k--;
			}
		}
		foreach (RopeNode ropeNode in RopeNodes)
		{
			if ((bool)ropeNode.goNode)
			{
				for (int l = 0; l < ropeNode.goNode.transform.childCount; l++)
				{
					Transform child4 = ropeNode.goNode.transform.GetChild(l);
					if (child4.gameObject.GetComponent<UltimateRopeLink>() != null)
					{
						UnityEngine.Object.DestroyImmediate(child4.gameObject);
						l--;
					}
				}
			}
			ropeNode.segmentLinks = null;
		}
	}

	public bool Regenerate(bool bResetNodePositions = false)
	{
		m_bLastStatusIsError = true;
		DeleteRope(bResetNodePositions, false);
		if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
		{
			if (RopeStart == null)
			{
				Status = "A rope start GameObject needs to be specified";
				return false;
			}
			if (RopeNodes == null)
			{
				Status = "At least a rope node needs to be added";
				return false;
			}
			if (RopeNodes.Count == 0)
			{
				Status = "At least a rope node needs to be added";
				return false;
			}
			if (RopeType == ERopeType.Procedural && IsExtensible && HasACoil && CoilObject == null)
			{
				Status = "A coil object needs to be specified";
				return false;
			}
			if (RopeType == ERopeType.LinkedObjects && LinkObject == null)
			{
				Status = "A link object needs to be specified";
				return false;
			}
			for (int i = 0; i < RopeNodes.Count; i++)
			{
				if (RopeNodes[i].goNode == null)
				{
					Status = string.Format("Rope segment {0} has unassigned Segment End property", i);
					return false;
				}
			}
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<RopeBone> outListImportedBones = null;
		if (RopeType == ERopeType.ImportBones)
		{
			Status = string.Empty;
			if (BoneFirst == null)
			{
				Status = "The first bone needs to be specified";
				return false;
			}
			if (BoneLast == null)
			{
				Status = "The last bone needs to be specified";
				return false;
			}
			List<int> outListBoneIndices;
			string strErrorMessage;
			if (!ParseBoneIndices(BoneListNamesStatic, out outListBoneIndices, out strErrorMessage))
			{
				Status = "Error parsing static bone list:\n" + strErrorMessage;
				return false;
			}
			List<int> outListBoneIndices2;
			if (!ParseBoneIndices(BoneListNamesNoColliders, out outListBoneIndices2, out strErrorMessage))
			{
				Status = "Error parsing collider bone list:\n" + strErrorMessage;
				return false;
			}
			if (!BuildImportedBoneList(BoneFirst, BoneLast, outListBoneIndices, outListBoneIndices2, out outListImportedBones, out strErrorMessage))
			{
				Status = "Error building bone list:\n" + strErrorMessage;
				return false;
			}
		}
		base.gameObject.layer = RopeLayer;
		CheckAddCoilNode();
		if (!m_bRopeStartInitialOrientationInitialized && RopeStart != null)
		{
			m_v3InitialRopeStartLocalPos = RopeStart.transform.localPosition;
			m_qInitialRopeStartLocalRot = RopeStart.transform.localRotation;
			m_v3InitialRopeStartLocalScale = RopeStart.transform.localScale;
			m_bRopeStartInitialOrientationInitialized = true;
		}
		if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
		{
			TotalLinks = 0;
			TotalRopeLength = 0f;
			for (int j = 0; j < RopeNodes.Count; j++)
			{
				RopeNode ropeNode = RopeNodes[j];
				if (!ropeNode.bInitialOrientationInitialized)
				{
					ropeNode.v3InitialLocalPos = ropeNode.goNode.transform.localPosition;
					ropeNode.qInitialLocalRot = ropeNode.goNode.transform.localRotation;
					ropeNode.v3InitialLocalScale = ropeNode.goNode.transform.localScale;
					ropeNode.bInitialOrientationInitialized = true;
				}
				if (ropeNode.nNumLinks < 1)
				{
					ropeNode.nNumLinks = 1;
				}
				if (ropeNode.fLength < 0f)
				{
					ropeNode.fLength = 0.001f;
				}
				ropeNode.nTotalLinks = ropeNode.nNumLinks;
				ropeNode.fTotalLength = ropeNode.fLength;
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				if (FirstNodeIsCoil() && j == 0)
				{
					gameObject = CoilObject;
					gameObject2 = RopeStart;
				}
				else
				{
					gameObject = ((j != m_nFirstNonCoilNode) ? RopeNodes[j - 1].goNode : RopeStart);
					gameObject2 = RopeNodes[j].goNode;
				}
				ropeNode.m_v3LocalDirectionForward = gameObject.transform.InverseTransformDirection((gameObject2.transform.position - gameObject.transform.position).normalized);
				if (j == RopeNodes.Count - 1 && IsExtensible && ExtensibleLength > 0f)
				{
					ropeNode.nTotalLinks += (int)(ExtensibleLength / (ropeNode.fLength / (float)ropeNode.nNumLinks)) + 1;
					ropeNode.fTotalLength += ExtensibleLength;
					ropeNode.m_bExtensionInitialized = false;
					ropeNode.m_nExtensionLinkIn = ropeNode.nTotalLinks - ropeNode.nNumLinks;
					ropeNode.m_nExtensionLinkOut = ropeNode.m_nExtensionLinkIn - 1;
					ropeNode.m_fExtensionRemainingLength = ExtensibleLength;
					ropeNode.m_fExtensionRemainderIn = 0f;
					ropeNode.m_fExtensionRemainderOut = 0f;
					m_fCurrentExtension = 0f;
				}
				ropeNode.linkJoints = new ConfigurableJoint[ropeNode.nTotalLinks + 1];
				ropeNode.linkJointBreaksProcessed = new bool[ropeNode.nTotalLinks + 1];
				ropeNode.segmentLinks = new GameObject[ropeNode.nTotalLinks];
				if (FirstNodeIsCoil() && j == 0)
				{
					for (int k = 0; k < ropeNode.segmentLinks.Length; k++)
					{
						string text = "Coil Link " + k;
						ropeNode.segmentLinks[k] = new GameObject(text);
						ropeNode.segmentLinks[k].AddComponent<UltimateRopeLink>();
						ropeNode.segmentLinks[k].transform.parent = CoilObject.transform;
						ropeNode.segmentLinks[k].layer = RopeLayer;
					}
					if (CoilDiameter < 0f)
					{
						CoilDiameter = 0f;
					}
					if (CoilWidth < 0f)
					{
						CoilWidth = 0f;
					}
					SetupCoilBones(ExtensibleLength);
				}
				else
				{
					float num = ropeNode.fLength / (float)ropeNode.nNumLinks;
					float num2 = ((gameObject2.transform.position - gameObject.transform.position).magnitude - num) / (gameObject2.transform.position - gameObject.transform.position).magnitude;
					float num3 = ((RopeType != ERopeType.LinkedObjects) ? 1f : GetLinkedObjectScale(ropeNode.fLength, ropeNode.nNumLinks));
					for (int l = 0; l < ropeNode.segmentLinks.Length; l++)
					{
						float num4 = (float)l / ((ropeNode.segmentLinks.Length != 1) ? ((float)ropeNode.segmentLinks.Length - 1f) : 1f);
						string text2 = "Node " + j + " Link " + l;
						if (ropeNode.nTotalLinks > ropeNode.nNumLinks && l < ropeNode.nTotalLinks - ropeNode.nNumLinks)
						{
							text2 += " (extension)";
						}
						if (RopeType == ERopeType.Procedural)
						{
							ropeNode.segmentLinks[l] = new GameObject(text2);
						}
						else if (RopeType == ERopeType.LinkedObjects)
						{
							ropeNode.segmentLinks[l] = UnityEngine.Object.Instantiate(LinkObject);
							ropeNode.segmentLinks[l].name = text2;
						}
						ropeNode.segmentLinks[l].AddComponent<UltimateRopeLink>();
						if (Vector3.Distance(gameObject.transform.position, gameObject2.transform.position) < 0.001f)
						{
							ropeNode.segmentLinks[l].transform.position = gameObject.transform.position;
							ropeNode.segmentLinks[l].transform.rotation = gameObject.transform.rotation;
						}
						else
						{
							ropeNode.segmentLinks[l].transform.position = Vector3.Lerp(gameObject.transform.position, gameObject2.transform.position, num4 * num2);
							ropeNode.segmentLinks[l].transform.rotation = Quaternion.LookRotation((gameObject2.transform.position - gameObject.transform.position).normalized);
						}
						if (RopeType == ERopeType.LinkedObjects)
						{
							ropeNode.segmentLinks[l].transform.rotation *= GetLinkedObjectLocalRotation(LinkTwistAngleStart + LinkTwistAngleIncrement * (float)l);
							ropeNode.segmentLinks[l].transform.localScale = new Vector3(num3, num3, num3);
						}
						if (ropeNode.segmentLinks[l].GetComponent<Rigidbody>() == null)
						{
							ropeNode.segmentLinks[l].AddComponent<Rigidbody>();
						}
						ropeNode.segmentLinks[l].transform.parent = base.transform;
						ropeNode.segmentLinks[l].layer = RopeLayer;
					}
				}
				TotalLinks += ropeNode.segmentLinks.Length;
				TotalRopeLength += ropeNode.fTotalLength;
			}
			m_bBonesAreImported = false;
		}
		else if (RopeType == ERopeType.ImportBones)
		{
			TotalLinks = 0;
			TotalRopeLength = 0f;
			ImportedBones = outListImportedBones.ToArray();
			bool flag = false;
			if (RopeNodes != null && RopeNodes.Count != 0)
			{
				flag = true;
			}
			if (!flag)
			{
				RopeNodes = new List<RopeNode>();
				RopeNodes.Add(new RopeNode());
			}
			RopeNode ropeNode2 = RopeNodes[0];
			ropeNode2.nNumLinks = ImportedBones.Length;
			ropeNode2.nTotalLinks = ropeNode2.nNumLinks;
			ropeNode2.linkJoints = new ConfigurableJoint[ImportedBones.Length];
			ropeNode2.linkJointBreaksProcessed = new bool[ImportedBones.Length];
			ropeNode2.segmentLinks = new GameObject[ropeNode2.nTotalLinks];
			int num5 = 0;
			for (int m = 0; m < ImportedBones.Length; m++)
			{
				ropeNode2.segmentLinks[num5] = ImportedBones[m].goBone;
				if (ImportedBones[m].goBone.GetComponent<Rigidbody>() == null)
				{
					ImportedBones[m].goBone.AddComponent<Rigidbody>();
					ImportedBones[m].bCreatedRigidbody = true;
				}
				else
				{
					ImportedBones[m].bCreatedRigidbody = false;
				}
				ImportedBones[m].goBone.layer = RopeLayer;
				float num6 = 0f;
				num6 = ((num5 >= ImportedBones.Length - 1) ? 0f : Vector3.Distance(ImportedBones[m].goBone.transform.position, ImportedBones[m + 1].goBone.transform.position));
				TotalLinks += ropeNode2.segmentLinks.Length;
				TotalRopeLength += num6;
				ImportedBones[m].fLength = num6;
				num5++;
			}
			ropeNode2.fLength = TotalRopeLength;
			ropeNode2.fTotalLength = ropeNode2.fLength;
			ropeNode2.eColliderType = BoneColliderType;
			ropeNode2.nColliderSkip = BoneColliderSkip;
			m_bBonesAreImported = true;
		}
		if (RopeType == ERopeType.Procedural)
		{
			Transform[] array = new Transform[TotalLinks];
			Matrix4x4[] array2 = new Matrix4x4[TotalLinks];
			LinkLengths = new float[TotalLinks];
			int num7 = 0;
			for (int n = 0; n < RopeNodes.Count; n++)
			{
				RopeNode ropeNode3 = RopeNodes[n];
				for (int num8 = 0; num8 < ropeNode3.segmentLinks.Length; num8++)
				{
					array[num7] = ropeNode3.segmentLinks[num8].transform;
					array2[num7] = ropeNode3.segmentLinks[num8].transform.worldToLocalMatrix;
					if (ropeNode3.segmentLinks[num8].transform.parent != null)
					{
						array2[num7] *= base.transform.localToWorldMatrix;
					}
					LinkLengths[num7] = ropeNode3.fLength / (float)ropeNode3.nNumLinks;
					num7++;
				}
			}
			if (RopeDiameter < 0.01f)
			{
				RopeDiameter = 0.01f;
			}
			if (RopeDiameterScaleX < 0.01f)
			{
				RopeDiameterScaleX = 0.01f;
			}
			if (RopeDiameterScaleY < 0.01f)
			{
				RopeDiameterScaleY = 0.01f;
			}
			bool flag2 = LinkJointBreakForce != float.PositiveInfinity || LinkJointBreakTorque != float.PositiveInfinity;
			Mesh mesh = new Mesh();
			int num9 = ((!flag2) ? ((TotalLinks + 1) * (RopeSegmentSides + 1) + (RopeSegmentSides + 1) * 2) : (TotalLinks * (RopeSegmentSides + 1) * 4));
			int num10 = TotalLinks * RopeSegmentSides * 2;
			int num11 = ((!flag2) ? (2 * (RopeSegmentSides - 2)) : (TotalLinks * 2 * (RopeSegmentSides - 2)));
			Vector3[] array3 = new Vector3[num9];
			Vector2[] array4 = new Vector2[num9];
			Vector4[] array5 = new Vector4[num9];
			BoneWeight[] array6 = new BoneWeight[num9];
			int[] indices = new int[num10 * 3];
			int[] indices2 = new int[num11 * 3];
			if (flag2)
			{
				int num12 = 0;
				for (int num13 = 0; num13 < TotalLinks; num13++)
				{
					int num14 = num13;
					int num15 = num14;
					float num16 = 1f;
					float num17 = 1f - num16;
					FillLinkMeshIndicesRope(num13, TotalLinks, ref indices, flag2);
					FillLinkMeshIndicesSections(num13, TotalLinks, ref indices2, flag2);
					for (int num18 = 0; num18 < 4; num18++)
					{
						for (int num19 = 0; num19 < RopeSegmentSides + 1; num19++)
						{
							int num20 = ((num18 >= 2) ? 1 : 0);
							float num21 = (float)(num13 + num20) / (float)TotalLinks;
							float num22 = Mathf.Cos((float)num19 / (float)RopeSegmentSides * (float)Math.PI * 2f);
							float num23 = Mathf.Sin((float)num19 / (float)RopeSegmentSides * (float)Math.PI * 2f);
							array3[num12] = new Vector3(num22 * RopeDiameter * RopeDiameterScaleX * 0.5f, num23 * RopeDiameter * RopeDiameterScaleY * 0.5f, LinkLengths[num13] * (float)num20);
							array3[num12] = array[num14].TransformPoint(array3[num12]) * num16 + array[num15].TransformPoint(array3[num12]) * num17;
							array3[num12] = base.transform.InverseTransformPoint(array3[num12]);
							if (num18 == 0 || num18 == 3)
							{
								array4[num12] = new Vector2(Mathf.Clamp01((num22 + 1f) * 0.5f), Mathf.Clamp01((num23 + 1f) * 0.5f));
								array5[num12] = new Vector4(1f, 0f, 0f, 1f);
							}
							else
							{
								array4[num12] = new Vector2(num21 * TotalRopeLength * RopeTextureTileMeters, (float)num19 / (float)RopeSegmentSides);
								array5[num12] = new Vector4(0f, 0f, 1f, 1f);
							}
							array6[num12].boneIndex0 = num14;
							array6[num12].boneIndex1 = num15;
							array6[num12].weight0 = num16;
							array6[num12].weight1 = num17;
							num12++;
						}
					}
				}
			}
			else
			{
				int num24 = 0;
				FillLinkMeshIndicesSections(0, TotalLinks, ref indices2, flag2);
				for (int num25 = 0; num25 < TotalLinks + 1; num25++)
				{
					int num26 = ((num25 >= TotalLinks) ? (TotalLinks - 1) : num25);
					int num27 = num26;
					float num28 = 1f;
					float num29 = 1f - num28;
					if (num25 < TotalLinks)
					{
						FillLinkMeshIndicesRope(num25, TotalLinks, ref indices, flag2);
					}
					bool flag3 = false;
					bool flag4 = false;
					int num30 = 1;
					if (num25 == 0)
					{
						num30++;
						flag3 = true;
					}
					if (num25 == TotalLinks)
					{
						num30++;
						flag4 = true;
					}
					for (int num31 = 0; num31 < num30; num31++)
					{
						for (int num32 = 0; num32 < RopeSegmentSides + 1; num32++)
						{
							float num33 = (float)num25 / (float)TotalLinks;
							float num34 = Mathf.Cos((float)num32 / (float)RopeSegmentSides * (float)Math.PI * 2f);
							float num35 = Mathf.Sin((float)num32 / (float)RopeSegmentSides * (float)Math.PI * 2f);
							array3[num24] = new Vector3(num34 * RopeDiameter * RopeDiameterScaleX * 0.5f, num35 * RopeDiameter * RopeDiameterScaleY * 0.5f, flag4 ? LinkLengths[TotalLinks - 1] : 0f);
							array3[num24] = array[num26].TransformPoint(array3[num24]) * num28 + array[num27].TransformPoint(array3[num24]) * num29;
							array3[num24] = base.transform.InverseTransformPoint(array3[num24]);
							if ((flag3 && num31 == 0) || (flag4 && num31 == num30 - 1))
							{
								array4[num24] = new Vector2(Mathf.Clamp01((num34 + 1f) * 0.5f), Mathf.Clamp01((num35 + 1f) * 0.5f));
								array5[num24] = new Vector4(1f, 0f, 0f, 1f);
							}
							else
							{
								array4[num24] = new Vector2(num33 * TotalRopeLength * RopeTextureTileMeters, (float)num32 / (float)RopeSegmentSides);
								array5[num24] = new Vector4(0f, 0f, 1f, 1f);
							}
							array6[num24].boneIndex0 = num26;
							array6[num24].boneIndex1 = num27;
							array6[num24].weight0 = num28;
							array6[num24].weight1 = num29;
							num24++;
						}
					}
				}
			}
			mesh.vertices = array3;
			mesh.uv = array4;
			mesh.boneWeights = array6;
			mesh.bindposes = array2;
			mesh.subMeshCount = 2;
			mesh.SetTriangles(indices, 0);
			mesh.SetTriangles(indices2, 1);
			mesh.RecalculateNormals();
			mesh.tangents = array5;
			SkinnedMeshRenderer skinnedMeshRenderer = ((!(base.gameObject.GetComponent<SkinnedMeshRenderer>() != null)) ? base.gameObject.AddComponent<SkinnedMeshRenderer>() : base.gameObject.GetComponent<SkinnedMeshRenderer>());
			skinnedMeshRenderer.materials = new Material[2] { RopeMaterial, RopeSectionMaterial };
			skinnedMeshRenderer.bones = array;
			skinnedMeshRenderer.sharedMesh = mesh;
			skinnedMeshRenderer.updateWhenOffscreen = true;
		}
		Deleted = false;
		if (Application.isPlaying)
		{
			CreateRopeJoints();
		}
		SetupRopeLinks();
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Status = string.Format("Rope generated in {0} seconds", realtimeSinceStartup2 - realtimeSinceStartup);
		m_bLastStatusIsError = false;
		return true;
	}

	public bool IsLastStatusError()
	{
		return m_bLastStatusIsError;
	}

	public bool ChangeRopeDiameter(float fNewDiameter, float fNewScaleX, float fNewScaleY)
	{
		if (RopeType != 0)
		{
			return false;
		}
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (component == null)
		{
			return false;
		}
		RopeDiameter = fNewDiameter;
		RopeDiameterScaleX = fNewScaleX;
		RopeDiameterScaleY = fNewScaleY;
		if (RopeDiameter < 0.01f)
		{
			RopeDiameter = 0.01f;
		}
		if (RopeDiameterScaleX < 0.01f)
		{
			RopeDiameterScaleX = 0.01f;
		}
		if (RopeDiameterScaleY < 0.01f)
		{
			RopeDiameterScaleY = 0.01f;
		}
		bool flag = LinkJointBreakForce != float.PositiveInfinity || LinkJointBreakTorque != float.PositiveInfinity;
		Vector3[] vertices = component.sharedMesh.vertices;
		Matrix4x4[] bindposes = component.sharedMesh.bindposes;
		Vector2[] array = new Vector2[RopeSegmentSides + 1];
		for (int i = 0; i < RopeSegmentSides + 1; i++)
		{
			float num = Mathf.Cos((float)i / (float)RopeSegmentSides * (float)Math.PI * 2f);
			float num2 = Mathf.Sin((float)i / (float)RopeSegmentSides * (float)Math.PI * 2f);
			array[i] = new Vector2(num * RopeDiameter * RopeDiameterScaleX * 0.5f, num2 * RopeDiameter * RopeDiameterScaleY * 0.5f);
		}
		if (flag)
		{
			int num3 = 0;
			for (int j = 0; j < TotalLinks; j++)
			{
				int num4 = j;
				int num5 = num4;
				float num6 = 1f;
				float num7 = 1f - num6;
				bindposes[j] = component.bones[j].transform.worldToLocalMatrix;
				if (component.bones[j].transform.parent != null)
				{
					bindposes[j] *= base.transform.localToWorldMatrix;
				}
				for (int k = 0; k < 4; k++)
				{
					for (int l = 0; l < RopeSegmentSides + 1; l++)
					{
						int num8 = ((k >= 2) ? 1 : 0);
						vertices[num3] = new Vector3(array[l].x, array[l].y, LinkLengths[j] * (float)num8);
						vertices[num3] = component.bones[num4].TransformPoint(vertices[num3]) * num6 + component.bones[num5].TransformPoint(vertices[num3]) * num7;
						vertices[num3] = base.transform.InverseTransformPoint(vertices[num3]);
						num3++;
					}
				}
			}
		}
		else
		{
			int num9 = 0;
			for (int m = 0; m < TotalLinks + 1; m++)
			{
				int num10 = ((m >= TotalLinks) ? (TotalLinks - 1) : m);
				int num11 = num10;
				float num12 = 1f;
				float num13 = 1f - num12;
				bool flag2 = false;
				int num14 = 1;
				if (m == 0)
				{
					num14++;
				}
				if (m == TotalLinks)
				{
					num14++;
					flag2 = true;
				}
				if (m < TotalLinks)
				{
					bindposes[m] = component.bones[m].transform.worldToLocalMatrix;
					if (component.bones[m].transform.parent != null)
					{
						bindposes[m] *= base.transform.localToWorldMatrix;
					}
				}
				for (int n = 0; n < num14; n++)
				{
					for (int num15 = 0; num15 < RopeSegmentSides + 1; num15++)
					{
						vertices[num9] = new Vector3(array[num15].x, array[num15].y, flag2 ? LinkLengths[TotalLinks - 1] : 0f);
						vertices[num9] = component.bones[num10].TransformPoint(vertices[num9]) * num12 + component.bones[num11].TransformPoint(vertices[num9]) * num13;
						vertices[num9] = base.transform.InverseTransformPoint(vertices[num9]);
						num9++;
					}
				}
			}
		}
		component.sharedMesh.vertices = vertices;
		component.sharedMesh.bindposes = bindposes;
		SetupRopeLinks();
		return true;
	}

	public bool ChangeRopeSegmentSides(int nNewSegmentSides)
	{
		if (RopeType != 0)
		{
			return false;
		}
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (component == null)
		{
			return false;
		}
		RopeSegmentSides = nNewSegmentSides;
		if (RopeSegmentSides < 3)
		{
			RopeSegmentSides = 3;
		}
		bool flag = LinkJointBreakForce != float.PositiveInfinity || LinkJointBreakTorque != float.PositiveInfinity;
		Mesh mesh = new Mesh();
		int num = ((!flag) ? ((TotalLinks + 1) * (RopeSegmentSides + 1) + (RopeSegmentSides + 1) * 2) : (TotalLinks * (RopeSegmentSides + 1) * 4));
		int num2 = TotalLinks * RopeSegmentSides * 2;
		int num3 = ((!flag) ? (2 * (RopeSegmentSides - 2)) : (TotalLinks * 2 * (RopeSegmentSides - 2)));
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Vector4[] array3 = new Vector4[num];
		BoneWeight[] array4 = new BoneWeight[num];
		int[] indices = new int[num2 * 3];
		int[] indices2 = new int[num3 * 3];
		Matrix4x4[] bindposes = component.sharedMesh.bindposes;
		if (flag)
		{
			int num4 = 0;
			for (int i = 0; i < TotalLinks; i++)
			{
				int num5 = i;
				int num6 = num5;
				float num7 = 1f;
				float num8 = 1f - num7;
				bindposes[i] = component.bones[i].transform.worldToLocalMatrix;
				if (component.bones[i].transform.parent != null)
				{
					bindposes[i] *= base.transform.localToWorldMatrix;
				}
				FillLinkMeshIndicesRope(i, TotalLinks, ref indices, flag);
				FillLinkMeshIndicesSections(i, TotalLinks, ref indices2, flag);
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < RopeSegmentSides + 1; k++)
					{
						int num9 = ((j >= 2) ? 1 : 0);
						float num10 = (float)(i + num9) / (float)TotalLinks;
						float num11 = Mathf.Cos((float)k / (float)RopeSegmentSides * (float)Math.PI * 2f);
						float num12 = Mathf.Sin((float)k / (float)RopeSegmentSides * (float)Math.PI * 2f);
						array[num4] = new Vector3(num11 * RopeDiameter * RopeDiameterScaleX * 0.5f, num12 * RopeDiameter * RopeDiameterScaleY * 0.5f, LinkLengths[i] * (float)num9);
						array[num4] = component.bones[num5].TransformPoint(array[num4]) * num7 + component.bones[num6].TransformPoint(array[num4]) * num8;
						array[num4] = base.transform.InverseTransformPoint(array[num4]);
						if (j == 0 || j == 3)
						{
							array2[num4] = new Vector2(Mathf.Clamp01((num11 + 1f) * 0.5f), Mathf.Clamp01((num12 + 1f) * 0.5f));
							array3[num4] = new Vector4(1f, 0f, 0f, 1f);
						}
						else
						{
							array2[num4] = new Vector2(num10 * TotalRopeLength * RopeTextureTileMeters, (float)k / (float)RopeSegmentSides);
							array3[num4] = new Vector4(0f, 0f, 1f, 1f);
						}
						array4[num4].boneIndex0 = num5;
						array4[num4].boneIndex1 = num6;
						array4[num4].weight0 = num7;
						array4[num4].weight1 = num8;
						num4++;
					}
				}
			}
		}
		else
		{
			int num13 = 0;
			FillLinkMeshIndicesSections(0, TotalLinks, ref indices2, flag);
			for (int l = 0; l < TotalLinks + 1; l++)
			{
				int num14 = ((l >= TotalLinks) ? (TotalLinks - 1) : l);
				int num15 = num14;
				float num16 = 1f;
				float num17 = 1f - num16;
				if (l < TotalLinks)
				{
					FillLinkMeshIndicesRope(l, TotalLinks, ref indices, flag);
				}
				bool flag2 = false;
				bool flag3 = false;
				int num18 = 1;
				if (l == 0)
				{
					num18++;
					flag2 = true;
				}
				if (l == TotalLinks)
				{
					num18++;
					flag3 = true;
				}
				if (l < TotalLinks)
				{
					bindposes[l] = component.bones[l].transform.worldToLocalMatrix;
					if (component.bones[l].transform.parent != null)
					{
						bindposes[l] *= base.transform.localToWorldMatrix;
					}
				}
				for (int m = 0; m < num18; m++)
				{
					for (int n = 0; n < RopeSegmentSides + 1; n++)
					{
						float num19 = (float)l / (float)TotalLinks;
						float num20 = Mathf.Cos((float)n / (float)RopeSegmentSides * (float)Math.PI * 2f);
						float num21 = Mathf.Sin((float)n / (float)RopeSegmentSides * (float)Math.PI * 2f);
						array[num13] = new Vector3(num20 * RopeDiameter * RopeDiameterScaleX * 0.5f, num21 * RopeDiameter * RopeDiameterScaleY * 0.5f, flag3 ? LinkLengths[TotalLinks - 1] : 0f);
						array[num13] = component.bones[num14].TransformPoint(array[num13]) * num16 + component.bones[num15].TransformPoint(array[num13]) * num17;
						array[num13] = base.transform.InverseTransformPoint(array[num13]);
						if ((flag2 && m == 0) || (flag3 && m == num18 - 1))
						{
							array2[num13] = new Vector2(Mathf.Clamp01((num20 + 1f) * 0.5f), Mathf.Clamp01((num21 + 1f) * 0.5f));
							array3[num13] = new Vector4(1f, 0f, 0f, 1f);
						}
						else
						{
							array2[num13] = new Vector2(num19 * TotalRopeLength * RopeTextureTileMeters, (float)n / (float)RopeSegmentSides);
							array3[num13] = new Vector4(0f, 0f, 1f, 1f);
						}
						array4[num13].boneIndex0 = num14;
						array4[num13].boneIndex1 = num15;
						array4[num13].weight0 = num16;
						array4[num13].weight1 = num17;
						num13++;
					}
				}
			}
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.boneWeights = array4;
		mesh.bindposes = bindposes;
		mesh.subMeshCount = 2;
		mesh.SetTriangles(indices, 0);
		mesh.SetTriangles(indices2, 1);
		mesh.RecalculateNormals();
		mesh.tangents = array3;
		if (Application.isEditor && !Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(component.sharedMesh);
		}
		else
		{
			UnityEngine.Object.Destroy(component.sharedMesh);
		}
		component.sharedMesh = mesh;
		SetupRopeLinks();
		return true;
	}

	public void SetupRopeMaterials()
	{
		if (RopeType == ERopeType.Procedural)
		{
			SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.materials = new Material[2] { RopeMaterial, RopeSectionMaterial };
			}
		}
	}

	public void SetupRopeLinks()
	{
		if (RopeNodes == null || RopeNodes.Count == 0 || Deleted || (RopeType == ERopeType.ImportBones && ImportedBones == null))
		{
			return;
		}
		base.gameObject.layer = RopeLayer;
		if (RopeDiameter < 0.01f)
		{
			RopeDiameter = 0.01f;
		}
		for (int i = 0; i < RopeNodes.Count; i++)
		{
			RopeNode ropeNode = RopeNodes[i];
			if (ropeNode.bIsCoil)
			{
				continue;
			}
			if (RopeType == ERopeType.ImportBones)
			{
				ropeNode.eColliderType = BoneColliderType;
				ropeNode.nColliderSkip = BoneColliderSkip;
			}
			float num = ropeNode.fLength / (float)ropeNode.nNumLinks;
			float linkDiameter = GetLinkDiameter();
			int nColliderSkip = ropeNode.nColliderSkip;
			float fValue = ((RopeType != 0) ? 0f : (num * 0.5f));
			int num2 = 0;
			GameObject[] segmentLinks = ropeNode.segmentLinks;
			foreach (GameObject gameObject in segmentLinks)
			{
				if (!gameObject)
				{
					continue;
				}
				if ((bool)gameObject.GetComponent<Collider>())
				{
					UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Collider>());
				}
				bool flag = num2 % (nColliderSkip + 1) == 0;
				bool flag2 = gameObject.GetComponent<Rigidbody>() != null && gameObject.GetComponent<Rigidbody>().isKinematic;
				if (RopeType == ERopeType.ImportBones)
				{
					if (Mathf.Approximately(ImportedBones[num2].fLength, 0f))
					{
						flag = false;
					}
					else if (flag)
					{
						flag = ImportedBones[num2].bCreatedCollider;
					}
					num = ImportedBones[num2].fLength * BoneColliderLength;
					fValue = num * BoneColliderOffset;
					flag2 = ImportedBones[num2].bIsStatic;
				}
				if (flag)
				{
					switch (ropeNode.eColliderType)
					{
					case EColliderType.Capsule:
					{
						CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
						capsuleCollider.material = RopePhysicsMaterial;
						capsuleCollider.center = GetLinkAxisOffset(fValue);
						capsuleCollider.radius = linkDiameter * 0.5f;
						capsuleCollider.height = num;
						capsuleCollider.direction = GetLinkAxisIndex();
						capsuleCollider.material = RopePhysicsMaterial;
						capsuleCollider.enabled = flag;
						break;
					}
					case EColliderType.Box:
					{
						BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
						Vector3 v3CenterInOut = GetLinkAxisOffset(fValue);
						Vector3 v3SizeInOut = Vector3.zero;
						boxCollider.material = RopePhysicsMaterial;
						if (GetLinkBoxColliderCenterAndSize(num, linkDiameter, ref v3CenterInOut, ref v3SizeInOut))
						{
							boxCollider.center = v3CenterInOut;
							boxCollider.size = v3SizeInOut;
							boxCollider.enabled = flag;
						}
						else
						{
							boxCollider.enabled = false;
						}
						break;
					}
					}
				}
				if (gameObject.GetComponent<Collider>() != null)
				{
					gameObject.GetComponent<Collider>().enabled = !flag2;
				}
				Rigidbody rigidbody = ((!(gameObject.GetComponent<Rigidbody>() != null)) ? gameObject.AddComponent<Rigidbody>() : gameObject.GetComponent<Rigidbody>());
				rigidbody.mass = LinkMass;
				rigidbody.solverIterations = LinkSolverIterationCount;
				rigidbody.isKinematic = flag2;
				gameObject.layer = RopeLayer;
				num2++;
			}
		}
	}

	public void SetupRopeJoints()
	{
		if (RopeNodes == null || RopeNodes.Count == 0 || Deleted || (RopeType == ERopeType.ImportBones && ImportedBones == null))
		{
			return;
		}
		foreach (RopeNode ropeNode6 in RopeNodes)
		{
			if (ropeNode6.segmentLinks == null)
			{
				return;
			}
		}
		int num = 0;
		Vector3[] array = new Vector3[TotalLinks];
		Quaternion[] array2 = new Quaternion[TotalLinks];
		Vector3 localPosition = ((!(RopeStart != null)) ? Vector3.zero : RopeStart.transform.localPosition);
		Quaternion localRotation = ((!(RopeStart != null)) ? Quaternion.identity : RopeStart.transform.localRotation);
		Vector3[] array3 = new Vector3[RopeNodes.Count];
		Quaternion[] array4 = new Quaternion[RopeNodes.Count];
		if (m_bRopeStartInitialOrientationInitialized && RopeStart != null)
		{
			RopeStart.transform.localPosition = m_v3InitialRopeStartLocalPos;
			RopeStart.transform.localRotation = m_qInitialRopeStartLocalRot;
		}
		for (int i = 0; i < RopeNodes.Count; i++)
		{
			RopeNode ropeNode = RopeNodes[i];
			if (ropeNode.bInitialOrientationInitialized && ropeNode.goNode != null)
			{
				array3[i] = ropeNode.goNode.transform.localPosition;
				array4[i] = ropeNode.goNode.transform.localRotation;
				ropeNode.goNode.transform.localPosition = ropeNode.v3InitialLocalPos;
				ropeNode.goNode.transform.localRotation = ropeNode.qInitialLocalRot;
			}
		}
		if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
		{
			for (int j = 0; j < RopeNodes.Count; j++)
			{
				RopeNode ropeNode2 = RopeNodes[j];
				float num2 = ropeNode2.fLength / (float)ropeNode2.nNumLinks;
				float z = num2 * (float)(ropeNode2.segmentLinks.Length - 1);
				for (int k = 0; k < ropeNode2.segmentLinks.Length; k++)
				{
					float t = (float)k / ((ropeNode2.segmentLinks.Length != 1) ? ((float)ropeNode2.segmentLinks.Length - 1f) : 1f);
					array[num] = ropeNode2.segmentLinks[k].transform.position;
					array2[num] = ropeNode2.segmentLinks[k].transform.rotation;
					if (!ropeNode2.bIsCoil)
					{
						ropeNode2.segmentLinks[k].transform.position = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, z), t);
						ropeNode2.segmentLinks[k].transform.rotation = Quaternion.identity;
						if (RopeType == ERopeType.LinkedObjects)
						{
							ropeNode2.segmentLinks[k].transform.rotation *= GetLinkedObjectLocalRotation(LinkTwistAngleStart + LinkTwistAngleIncrement * (float)k);
						}
					}
					num++;
				}
			}
		}
		else if (RopeType == ERopeType.ImportBones)
		{
			for (int l = 0; l < ImportedBones.Length; l++)
			{
				array[l] = ImportedBones[l].goBone.transform.position;
				array2[l] = ImportedBones[l].goBone.transform.rotation;
				if (ImportedBones[l].tfNonBoneParent != null)
				{
					Transform parent = ImportedBones[l].goBone.transform.parent;
					ImportedBones[l].goBone.transform.parent = ImportedBones[l].tfNonBoneParent;
					ImportedBones[l].goBone.transform.localPosition = ImportedBones[l].v3OriginalLocalPos;
					ImportedBones[l].goBone.transform.localRotation = ImportedBones[l].qOriginalLocalRot;
					ImportedBones[l].goBone.transform.parent = parent;
					ImportedBones[l].goBone.transform.localScale = ImportedBones[l].v3OriginalLocalScale;
				}
			}
		}
		for (int m = 0; m < RopeNodes.Count; m++)
		{
			RopeNode ropeNode3 = RopeNodes[m];
			if (ropeNode3.bIsCoil)
			{
				continue;
			}
			ConfigurableJoint[] linkJoints = ropeNode3.linkJoints;
			foreach (ConfigurableJoint configurableJoint in linkJoints)
			{
				if ((bool)configurableJoint)
				{
					SetupJoint(configurableJoint);
				}
			}
			if ((RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects) && ropeNode3.bInitialOrientationInitialized)
			{
				GameObject gameObject = ((m != m_nFirstNonCoilNode) ? RopeNodes[m - 1].goNode : RopeStart);
				GameObject goNode = RopeNodes[m].goNode;
				Vector3 vector = gameObject.transform.TransformDirection(ropeNode3.m_v3LocalDirectionForward);
				Vector3 upwards = gameObject.transform.TransformDirection(ropeNode3.m_v3LocalDirectionUp);
				ropeNode3.segmentLinks[0].transform.position = gameObject.transform.position;
				ropeNode3.segmentLinks[0].transform.rotation = Quaternion.LookRotation(vector, upwards);
				ropeNode3.segmentLinks[ropeNode3.segmentLinks.Length - 1].transform.position = goNode.transform.position - vector * (ropeNode3.fLength / (float)ropeNode3.nNumLinks);
				ropeNode3.segmentLinks[ropeNode3.segmentLinks.Length - 1].transform.rotation = Quaternion.LookRotation(vector, upwards);
				if (RopeType == ERopeType.LinkedObjects)
				{
					ropeNode3.segmentLinks[0].transform.rotation *= GetLinkedObjectLocalRotation(LinkTwistAngleStart);
					ropeNode3.segmentLinks[ropeNode3.segmentLinks.Length - 1].transform.rotation *= GetLinkedObjectLocalRotation(LinkTwistAngleStart + LinkTwistAngleIncrement * (float)(ropeNode3.segmentLinks.Length - 1));
				}
				if (ropeNode3.linkJoints[0] != null)
				{
					SetupJoint(ropeNode3.linkJoints[0]);
				}
				if (ropeNode3.linkJoints[ropeNode3.linkJoints.Length - 1] != null)
				{
					SetupJoint(ropeNode3.linkJoints[ropeNode3.linkJoints.Length - 1]);
				}
			}
		}
		num = 0;
		if (m_bRopeStartInitialOrientationInitialized && RopeStart != null)
		{
			RopeStart.transform.localPosition = localPosition;
			RopeStart.transform.localRotation = localRotation;
		}
		for (int num3 = 0; num3 < RopeNodes.Count; num3++)
		{
			RopeNode ropeNode4 = RopeNodes[num3];
			if (ropeNode4.bInitialOrientationInitialized && ropeNode4.goNode != null)
			{
				ropeNode4.goNode.transform.localPosition = array3[num3];
				ropeNode4.goNode.transform.localRotation = array4[num3];
			}
		}
		if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
		{
			for (int num4 = 0; num4 < RopeNodes.Count; num4++)
			{
				RopeNode ropeNode5 = RopeNodes[num4];
				for (int num5 = 0; num5 < ropeNode5.segmentLinks.Length; num5++)
				{
					ropeNode5.segmentLinks[num5].transform.position = array[num];
					ropeNode5.segmentLinks[num5].transform.rotation = array2[num];
					num++;
				}
			}
		}
		else if (RopeType == ERopeType.ImportBones)
		{
			for (int num6 = 0; num6 < ImportedBones.Length; num6++)
			{
				ImportedBones[num6].goBone.transform.position = array[num6];
				ImportedBones[num6].goBone.transform.rotation = array2[num6];
			}
		}
	}

	public void CheckNeedsStartExitLockZ()
	{
		if (RopeType != 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < RopeNodes.Count; i++)
		{
			RopeNode ropeNode = RopeNodes[i];
			for (int j = 0; j < ropeNode.segmentLinks.Length; j++)
			{
				Transform transform = null;
				Transform transform2 = null;
				if (!FirstNodeIsCoil())
				{
					transform = ((i != m_nFirstNonCoilNode) ? RopeNodes[i - 1].goNode.transform : RopeStart.transform);
					transform2 = RopeNodes[i].goNode.transform;
				}
				if (transform != null && transform2 != null)
				{
					if (j == 0)
					{
						ropeNode.segmentLinks[j].transform.rotation = ((!LockStartEndInZAxis) ? Quaternion.LookRotation((transform2.position - transform.position).normalized) : transform.rotation);
						ropeNode.segmentLinks[j].transform.parent = ((!LockStartEndInZAxis) ? ropeNode.segmentLinks[j].transform.parent : transform);
						ropeNode.segmentLinks[j].GetComponent<Rigidbody>().isKinematic = LockStartEndInZAxis || ropeNode.segmentLinks[j].GetComponent<Rigidbody>().isKinematic;
					}
					else if (j == ropeNode.segmentLinks.Length - 1)
					{
						ropeNode.segmentLinks[j].transform.position = ((!LockStartEndInZAxis) ? (transform2.position - (transform2.position - transform.position).normalized * LinkLengths[num]) : (transform2.position - transform2.forward * LinkLengths[num]));
						ropeNode.segmentLinks[j].transform.rotation = ((!LockStartEndInZAxis) ? Quaternion.LookRotation((transform2.position - transform.position).normalized) : transform2.rotation);
						ropeNode.segmentLinks[j].transform.parent = ((!LockStartEndInZAxis) ? ropeNode.segmentLinks[j].transform.parent : transform2);
						ropeNode.segmentLinks[j].GetComponent<Rigidbody>().isKinematic = LockStartEndInZAxis || ropeNode.segmentLinks[j].GetComponent<Rigidbody>().isKinematic;
					}
				}
				num++;
			}
		}
	}

	public void FillLinkMeshIndicesRope(int nLinearLinkIndex, int nTotalLinks, ref int[] indices, bool bBreakable, bool bBrokenLink = false)
	{
		if (bBreakable)
		{
			int num = nLinearLinkIndex * RopeSegmentSides * 2;
			int num2 = nLinearLinkIndex * (RopeSegmentSides + 1) * 4 + (RopeSegmentSides + 1);
			int num3 = (RopeSegmentSides + 1) * 3;
			int num4 = ((!bBrokenLink && nLinearLinkIndex < nTotalLinks - 1) ? num3 : 0);
			for (int i = 0; i < RopeSegmentSides + 1; i++)
			{
				if (i < RopeSegmentSides)
				{
					int num5 = num2 + i;
					indices[num * 3 + 2] = num5;
					indices[num * 3 + 1] = num5 + num4 + (RopeSegmentSides + 1);
					indices[num * 3] = num5 + 1;
					indices[num * 3 + 5] = num5 + 1;
					indices[num * 3 + 4] = num5 + num4 + (RopeSegmentSides + 1);
					indices[num * 3 + 3] = num5 + num4 + (RopeSegmentSides + 1) + 1;
					num += 2;
				}
			}
			return;
		}
		int num6 = nLinearLinkIndex * RopeSegmentSides * 2;
		int num7 = nLinearLinkIndex * (RopeSegmentSides + 1) + (RopeSegmentSides + 1);
		for (int j = 0; j < RopeSegmentSides + 1; j++)
		{
			if (j < RopeSegmentSides)
			{
				int num8 = num7 + j;
				indices[num6 * 3 + 2] = num8;
				indices[num6 * 3 + 1] = num8 + RopeSegmentSides + 1;
				indices[num6 * 3] = num8 + 1;
				indices[num6 * 3 + 5] = num8 + 1;
				indices[num6 * 3 + 4] = num8 + RopeSegmentSides + 1;
				indices[num6 * 3 + 3] = num8 + 1 + RopeSegmentSides + 1;
				num6 += 2;
			}
		}
	}

	public void FillLinkMeshIndicesSections(int nLinearLinkIndex, int nTotalLinks, ref int[] indices, bool bBreakable, bool bBrokenLink = false)
	{
		if (bBreakable)
		{
			int num = nLinearLinkIndex * 2 * (RopeSegmentSides - 2);
			int num2 = nLinearLinkIndex * (RopeSegmentSides + 1) * 4;
			int num3 = (RopeSegmentSides + 1) * 2;
			for (int i = 0; i < RopeSegmentSides - 2; i++)
			{
				indices[num * 3] = num2;
				indices[num * 3 + 1] = num2 + (i + 2);
				indices[num * 3 + 2] = num2 + (i + 1);
				num++;
			}
			int num4 = ((!bBrokenLink && nLinearLinkIndex < nTotalLinks - 1) ? num3 : 0);
			for (int j = 0; j < RopeSegmentSides - 2; j++)
			{
				indices[num * 3 + 2] = num2 + (RopeSegmentSides + 1) * 3 + num4;
				indices[num * 3 + 1] = num2 + (RopeSegmentSides + 1) * 3 + num4 + (j + 2);
				indices[num * 3] = num2 + (RopeSegmentSides + 1) * 3 + num4 + (j + 1);
				num++;
			}
		}
		else
		{
			int num5 = 0;
			int num6 = 0;
			for (int k = 0; k < RopeSegmentSides - 2; k++)
			{
				indices[num5 * 3] = num6;
				indices[num5 * 3 + 1] = num6 + (k + 2);
				indices[num5 * 3 + 2] = num6 + (k + 1);
				num5++;
			}
			num6 = (TotalLinks + 1) * (RopeSegmentSides + 1) + (RopeSegmentSides + 1);
			for (int l = 0; l < RopeSegmentSides - 2; l++)
			{
				indices[num5 * 3 + 2] = num6;
				indices[num5 * 3 + 1] = num6 + (l + 2);
				indices[num5 * 3] = num6 + (l + 1);
				num5++;
			}
		}
	}

	public bool HasDynamicSegmentNodes()
	{
		if (RopeNodes == null)
		{
			return false;
		}
		if (RopeNodes.Count == 0)
		{
			return false;
		}
		foreach (RopeNode ropeNode in RopeNodes)
		{
			if ((bool)ropeNode.goNode && (bool)ropeNode.goNode.GetComponent<Rigidbody>() && !ropeNode.goNode.GetComponent<Rigidbody>().isKinematic)
			{
				return true;
			}
		}
		return false;
	}

	public void BeforeImportedBonesObjectRespawn()
	{
		if (ImportedBones == null)
		{
			return;
		}
		RopeBone[] importedBones = ImportedBones;
		foreach (RopeBone ropeBone in importedBones)
		{
			if (ropeBone.goBone != null)
			{
				ropeBone.goBone.transform.parent = ropeBone.tfParent;
			}
		}
	}

	public void AfterImportedBonesObjectRespawn()
	{
		if (ImportedBones == null)
		{
			return;
		}
		RopeBone[] importedBones = ImportedBones;
		foreach (RopeBone ropeBone in importedBones)
		{
			if (ropeBone.goBone != null)
			{
				ropeBone.goBone.transform.parent = ((!ropeBone.bIsStatic) ? base.transform : ropeBone.tfNonBoneParent);
			}
		}
	}

	public void ExtendRope(ERopeExtensionMode eRopeExtensionMode, float fIncrement)
	{
		if (!IsExtensible)
		{
			Debug.LogError("Rope can not be extended since the IsExtensible property has been marked as false");
			return;
		}
		if (eRopeExtensionMode == ERopeExtensionMode.CoilRotationIncrement && !FirstNodeIsCoil())
		{
			Debug.LogError("Rope can not be extended through coil rotation since no coil is present");
			return;
		}
		float fLinearIncrement = ((eRopeExtensionMode != ERopeExtensionMode.LinearExtensionIncrement) ? 0f : fIncrement);
		float fCurrentCoilRopeRadius = m_fCurrentCoilRopeRadius;
		if (eRopeExtensionMode == ERopeExtensionMode.CoilRotationIncrement)
		{
			fLinearIncrement = m_fCurrentCoilRopeRadius * (fIncrement / 360f) * 2f * (float)Math.PI;
		}
		float num = ExtendRopeLinear(fLinearIncrement);
		float num2 = num * 360f / ((float)Math.PI * 2f * fCurrentCoilRopeRadius);
		if (!Mathf.Approximately(num, 0f) && FirstNodeIsCoil())
		{
			CoilObject.transform.Rotate(GetAxisVector(CoilAxisRight, 1f) * num2);
			SetupCoilBones(m_fCurrentCoilLength - num);
		}
	}

	public void RecomputeCoil()
	{
		SetupCoilBones(m_fCurrentCoilLength);
	}

	public GameObject BuildStaticMeshObject(out string strStatusMessage)
	{
		if (Application.isEditor && Application.isPlaying)
		{
			strStatusMessage = "Error: Rope can't be made static from the editor in play mode";
			return null;
		}
		if (RopeType == ERopeType.Procedural)
		{
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			if (component == null)
			{
				strStatusMessage = "Error: Procedural rope has no skinned mesh renderer";
				return null;
			}
			Mesh sharedMesh = component.sharedMesh;
			Mesh mesh = new Mesh();
			int vertexCount = component.sharedMesh.vertexCount;
			int num = component.sharedMesh.GetTriangles(0).Length;
			int num2 = component.sharedMesh.GetTriangles(1).Length;
			Vector3[] vertices = sharedMesh.vertices;
			Vector2[] uv = sharedMesh.uv;
			Vector4[] tangents = sharedMesh.tangents;
			int[] triangles = sharedMesh.GetTriangles(0);
			int[] triangles2 = sharedMesh.GetTriangles(1);
			Vector3[] array = new Vector3[vertexCount];
			Vector2[] array2 = new Vector2[vertexCount];
			Vector4[] array3 = ((sharedMesh.tangents == null) ? null : new Vector4[sharedMesh.tangents.Length]);
			int[] array4 = new int[num];
			int[] array5 = new int[num2];
			BoneWeight[] boneWeights = sharedMesh.boneWeights;
			Matrix4x4[] bindposes = sharedMesh.bindposes;
			Transform[] bones = component.bones;
			Vector3 position = new Vector3(0f, 0f, 0f);
			for (int i = 0; i < vertexCount; i++)
			{
				BoneWeight boneWeight = boneWeights[i];
				array[i] = new Vector3(0f, 0f, 0f);
				if (Math.Abs(boneWeight.weight0) > 1E-05f)
				{
					Vector3 v = bindposes[boneWeight.boneIndex0].MultiplyPoint3x4(vertices[i]);
					array[i] += bones[boneWeight.boneIndex0].transform.localToWorldMatrix.MultiplyPoint3x4(v) * boneWeight.weight0;
				}
				if (Math.Abs(boneWeight.weight1) > 1E-05f)
				{
					Vector3 v = bindposes[boneWeight.boneIndex1].MultiplyPoint3x4(vertices[i]);
					array[i] += bones[boneWeight.boneIndex1].transform.localToWorldMatrix.MultiplyPoint3x4(v) * boneWeight.weight1;
				}
				if (Math.Abs(boneWeight.weight2) > 1E-05f)
				{
					Vector3 v = bindposes[boneWeight.boneIndex2].MultiplyPoint3x4(vertices[i]);
					array[i] += bones[boneWeight.boneIndex2].transform.localToWorldMatrix.MultiplyPoint3x4(v) * boneWeight.weight2;
				}
				if (Math.Abs(boneWeight.weight3) > 1E-05f)
				{
					Vector3 v = bindposes[boneWeight.boneIndex3].MultiplyPoint3x4(vertices[i]);
					array[i] += bones[boneWeight.boneIndex3].transform.localToWorldMatrix.MultiplyPoint3x4(v) * boneWeight.weight3;
				}
				position += array[i];
				array2[i] = uv[i];
				if (array3 != null && array3.Length == vertexCount)
				{
					array3[i] = tangents[i];
				}
			}
			if (vertexCount > 0)
			{
				position /= (float)vertexCount;
			}
			Vector3 position2 = base.transform.position;
			base.transform.position = position;
			for (int j = 0; j < vertexCount; j++)
			{
				array[j] = base.transform.InverseTransformPoint(array[j]);
			}
			base.transform.position = position2;
			for (int k = 0; k < num; k++)
			{
				array4[k] = triangles[k];
			}
			for (int l = 0; l < num2; l++)
			{
				array5[l] = triangles2[l];
			}
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.subMeshCount = 2;
			mesh.SetTriangles(array4, 0);
			mesh.SetTriangles(array5, 1);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			if (array3 != null && array3.Length == vertexCount)
			{
				mesh.tangents = array3;
			}
			GameObject gameObject = new GameObject(base.gameObject.name + " (static)");
			gameObject.transform.position = position;
			gameObject.transform.rotation = base.transform.rotation;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshFilter.sharedMesh = mesh;
			meshRenderer.sharedMaterials = new Material[2] { RopeMaterial, RopeSectionMaterial };
			gameObject.isStatic = true;
			MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = mesh;
			meshCollider.convex = false;
			meshCollider.material = RopePhysicsMaterial;
			base.gameObject.SetActive(false);
			strStatusMessage = "Rope converted succesfully";
			return gameObject;
		}
		if (RopeType == ERopeType.LinkedObjects)
		{
			if (LinkObject == null)
			{
				strStatusMessage = "Error: LinkObject not specified. Can't continue.";
				return null;
			}
			Renderer component2 = LinkObject.GetComponent<Renderer>();
			MeshFilter component3 = LinkObject.GetComponent<MeshFilter>();
			if (component2 == null)
			{
				strStatusMessage = "Error: LinkObject has no Renderer. Can't continue.";
				return null;
			}
			if (component3 == null)
			{
				strStatusMessage = "Error: LinkObject has no Mesh Filter. Can't continue.";
				return null;
			}
			if (component3.sharedMesh == null)
			{
				strStatusMessage = "Error: LinkObject has no mesh. Can't continue.";
				return null;
			}
			Material[] array6 = new Material[component2.sharedMaterials.Length];
			for (int m = 0; m < component2.sharedMaterials.Length; m++)
			{
				array6[m] = component2.sharedMaterials[m];
			}
			List<CombineInstance> list = new List<CombineInstance>();
			for (int n = 0; n < RopeNodes.Count; n++)
			{
				RopeNode ropeNode = RopeNodes[n];
				for (int num3 = 0; num3 < ropeNode.segmentLinks.Length; num3++)
				{
					CombineInstance item = default(CombineInstance);
					item.mesh = component3.sharedMesh;
					item.transform = ropeNode.segmentLinks[num3].transform.localToWorldMatrix;
					list.Add(item);
				}
			}
			GameObject gameObject2 = new GameObject(base.gameObject.name + " (static)");
			MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
			meshFilter2.sharedMesh = new Mesh();
			meshFilter2.sharedMesh.CombineMeshes(list.ToArray());
			meshRenderer2.sharedMaterials = array6;
			gameObject2.isStatic = true;
			Vector3[] vertices2 = meshFilter2.sharedMesh.vertices;
			Vector3 zero = Vector3.zero;
			for (int num4 = 0; num4 < meshFilter2.sharedMesh.vertexCount; num4++)
			{
				vertices2[num4] = base.transform.TransformPoint(vertices2[num4]);
				zero += vertices2[num4];
			}
			if (meshFilter2.sharedMesh.vertexCount > 1)
			{
				zero /= (float)meshFilter2.sharedMesh.vertexCount;
			}
			gameObject2.transform.position = zero;
			gameObject2.transform.rotation = base.transform.rotation;
			for (int num5 = 0; num5 < meshFilter2.sharedMesh.vertexCount; num5++)
			{
				vertices2[num5] = gameObject2.transform.InverseTransformPoint(vertices2[num5]);
			}
			meshFilter2.sharedMesh.vertices = vertices2;
			meshFilter2.sharedMesh.RecalculateBounds();
			MeshCollider meshCollider2 = gameObject2.AddComponent<MeshCollider>();
			meshCollider2.sharedMesh = meshFilter2.sharedMesh;
			meshCollider2.convex = false;
			meshCollider2.material = RopePhysicsMaterial;
			base.gameObject.SetActive(false);
			strStatusMessage = "Rope converted succesfully";
			return gameObject2;
		}
		if (RopeType == ERopeType.ImportBones)
		{
			strStatusMessage = "Error: ImportBones rope type not supported";
			return null;
		}
		strStatusMessage = "Error: Unknown rope type not supported";
		return null;
	}

	public void MoveNodeUp(int nNode)
	{
		if (RopeNodes != null && nNode > 0 && nNode < RopeNodes.Count)
		{
			RopeNode value = RopeNodes[nNode];
			RopeNodes[nNode] = RopeNodes[nNode - 1];
			RopeNodes[nNode - 1] = value;
		}
	}

	public void MoveNodeDown(int nNode)
	{
		if (RopeNodes != null && nNode >= 0 && nNode < RopeNodes.Count - 1)
		{
			RopeNode value = RopeNodes[nNode];
			RopeNodes[nNode] = RopeNodes[nNode + 1];
			RopeNodes[nNode + 1] = value;
		}
	}

	public void CreateNewNode(int nNode)
	{
		if (RopeNodes == null)
		{
			RopeNodes = new List<RopeNode>();
		}
		RopeNodes.Insert(nNode + 1, new RopeNode());
	}

	public void RemoveNode(int nNode)
	{
		if (RopeNodes != null)
		{
			RopeNodes.RemoveAt(nNode);
		}
	}

	public bool FirstNodeIsCoil()
	{
		if (RopeNodes != null && RopeNodes.Count > 0 && RopeNodes[0].bIsCoil)
		{
			return true;
		}
		return false;
	}

	private void CheckAddCoilNode()
	{
		if (RopeType != 0 || !IsExtensible || !HasACoil || !(CoilObject != null) || !RopeStart)
		{
			return;
		}
		if (!RopeNodes[0].bIsCoil)
		{
			RopeNodes.Insert(0, new RopeNode());
			if (CoilNumBones < 1)
			{
				CoilNumBones = 1;
			}
			RopeNodes[0].goNode = CoilObject;
			RopeNodes[0].fLength = ExtensibleLength;
			RopeNodes[0].fTotalLength = RopeNodes[0].fLength;
			RopeNodes[0].nNumLinks = CoilNumBones;
			RopeNodes[0].nTotalLinks = RopeNodes[0].nNumLinks;
			RopeNodes[0].eColliderType = EColliderType.None;
			RopeNodes[0].nColliderSkip = 0;
			RopeNodes[0].bFold = true;
			RopeNodes[0].bIsCoil = true;
			m_afCoilBoneRadiuses = new float[RopeNodes[0].nTotalLinks];
			m_afCoilBoneAngles = new float[RopeNodes[0].nTotalLinks];
			m_afCoilBoneX = new float[RopeNodes[0].nTotalLinks];
		}
		m_nFirstNonCoilNode = 1;
	}

	private void CheckDelCoilNode()
	{
		if (RopeNodes[0].bIsCoil)
		{
			RopeNodes.RemoveAt(0);
			m_afCoilBoneRadiuses = null;
			m_afCoilBoneAngles = null;
			m_afCoilBoneX = null;
		}
		m_nFirstNonCoilNode = 0;
	}

	private void CreateRopeJoints(bool bCheckIfBroken = false)
	{
		if (RopeNodes == null || RopeNodes.Count == 0 || Deleted || (RopeType == ERopeType.ImportBones && (ImportedBones == null || ImportedBones.Length == 0)))
		{
			return;
		}
		foreach (RopeNode ropeNode6 in RopeNodes)
		{
			if (ropeNode6.segmentLinks == null)
			{
				return;
			}
		}
		if (RopeStart != null && RopeStart.GetComponent<Rigidbody>() == null)
		{
			RopeStart.AddComponent<Rigidbody>();
			RopeStart.GetComponent<Rigidbody>().isKinematic = true;
		}
		for (int i = 0; i < RopeNodes.Count; i++)
		{
			RopeNode ropeNode = RopeNodes[i];
			if (ropeNode.goNode != null && ropeNode.goNode.GetComponent<Rigidbody>() == null)
			{
				ropeNode.goNode.AddComponent<Rigidbody>();
				ropeNode.goNode.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		int num = 0;
		int num2 = 0;
		Vector3[] array = new Vector3[TotalLinks];
		Quaternion[] array2 = new Quaternion[TotalLinks];
		Vector3 localPosition = ((!(RopeStart != null)) ? Vector3.zero : RopeStart.transform.localPosition);
		Quaternion localRotation = ((!(RopeStart != null)) ? Quaternion.identity : RopeStart.transform.localRotation);
		Vector3[] array3 = new Vector3[RopeNodes.Count];
		Quaternion[] array4 = new Quaternion[RopeNodes.Count];
		if (m_bRopeStartInitialOrientationInitialized && RopeStart != null)
		{
			RopeStart.transform.localPosition = m_v3InitialRopeStartLocalPos;
			RopeStart.transform.localRotation = m_qInitialRopeStartLocalRot;
		}
		for (int j = 0; j < RopeNodes.Count; j++)
		{
			RopeNode ropeNode2 = RopeNodes[j];
			if (ropeNode2.bInitialOrientationInitialized && ropeNode2.goNode != null)
			{
				array3[j] = ropeNode2.goNode.transform.localPosition;
				array4[j] = ropeNode2.goNode.transform.localRotation;
				ropeNode2.goNode.transform.localPosition = ropeNode2.v3InitialLocalPos;
				ropeNode2.goNode.transform.localRotation = ropeNode2.qInitialLocalRot;
			}
		}
		for (int k = 0; k < RopeNodes.Count; k++)
		{
			RopeNode ropeNode3 = RopeNodes[k];
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			if (FirstNodeIsCoil() && k == 0)
			{
				gameObject = CoilObject;
				gameObject2 = RopeStart;
			}
			else
			{
				gameObject = ((k != m_nFirstNonCoilNode) ? RopeNodes[k - 1].goNode : RopeStart);
				gameObject2 = RopeNodes[k].goNode;
			}
			float num3 = ropeNode3.fLength / (float)ropeNode3.nNumLinks;
			float z = num3 * (float)(ropeNode3.segmentLinks.Length - 1);
			for (int l = 0; l < ropeNode3.segmentLinks.Length; l++)
			{
				if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
				{
					float t = (float)l / ((ropeNode3.segmentLinks.Length != 1) ? ((float)ropeNode3.segmentLinks.Length - 1f) : 1f);
					if (l == 0)
					{
						ropeNode3.m_v3LocalDirectionUp = gameObject.transform.InverseTransformDirection(ropeNode3.segmentLinks[l].transform.up);
					}
					Vector3 normalized = (gameObject2.transform.position - gameObject.transform.position).normalized;
					if (ropeNode3.nTotalLinks > ropeNode3.nNumLinks && !ropeNode3.m_bExtensionInitialized)
					{
						ropeNode3.segmentLinks[l].transform.rotation = Quaternion.LookRotation((gameObject2.transform.position - gameObject.transform.position).normalized);
						if (l < ropeNode3.m_nExtensionLinkIn)
						{
							ropeNode3.segmentLinks[l].transform.position = gameObject.transform.position;
							ropeNode3.segmentLinks[l].transform.parent = ((k <= m_nFirstNonCoilNode) ? RopeStart.transform : RopeNodes[k - 1].goNode.transform);
							ropeNode3.segmentLinks[l].GetComponent<Rigidbody>().isKinematic = true;
							UltimateRopeLink component = ropeNode3.segmentLinks[l].GetComponent<UltimateRopeLink>();
							if (component != null)
							{
								component.ExtensibleKinematic = true;
							}
						}
						else
						{
							float t2 = (float)(l - ropeNode3.m_nExtensionLinkIn) / ((ropeNode3.nNumLinks <= 1) ? 1f : ((float)(ropeNode3.nNumLinks - 1)));
							ropeNode3.segmentLinks[l].transform.position = Vector3.Lerp(gameObject.transform.position + normalized * num3, gameObject2.transform.position - normalized * num3, t2);
							ropeNode3.segmentLinks[l].GetComponent<Rigidbody>().isKinematic = false;
							UltimateRopeLink component2 = ropeNode3.segmentLinks[l].GetComponent<UltimateRopeLink>();
							if (component2 != null)
							{
								component2.ExtensibleKinematic = false;
							}
						}
					}
					array[num2] = ropeNode3.segmentLinks[l].transform.position;
					array2[num2] = ropeNode3.segmentLinks[l].transform.rotation;
					ropeNode3.segmentLinks[l].transform.position = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, z), t);
					ropeNode3.segmentLinks[l].transform.rotation = Quaternion.identity;
					if (RopeType == ERopeType.LinkedObjects)
					{
						ropeNode3.segmentLinks[l].transform.rotation *= GetLinkedObjectLocalRotation(LinkTwistAngleStart + LinkTwistAngleIncrement * (float)l);
					}
					num2++;
				}
				else if (RopeType == ERopeType.ImportBones)
				{
					array[l] = ImportedBones[l].goBone.transform.position;
					array2[l] = ImportedBones[l].goBone.transform.rotation;
					if (ImportedBones[l].tfNonBoneParent != null)
					{
						Transform parent = ImportedBones[l].goBone.transform.parent;
						ImportedBones[l].goBone.transform.parent = ImportedBones[l].tfNonBoneParent;
						ImportedBones[l].goBone.transform.localPosition = ImportedBones[l].v3OriginalLocalPos;
						ImportedBones[l].goBone.transform.localRotation = ImportedBones[l].qOriginalLocalRot;
						ImportedBones[l].goBone.transform.parent = parent;
						ImportedBones[l].goBone.transform.localScale = ImportedBones[l].v3OriginalLocalScale;
					}
				}
				bool flag = !bCheckIfBroken || !ropeNode3.linkJointBreaksProcessed[l];
				if (RopeType == ERopeType.ImportBones)
				{
					bool flag2 = true;
					if (l > 0)
					{
						flag2 = !ImportedBones[l - 1].goBone.GetComponent<Rigidbody>().isKinematic;
					}
					if (!flag2 && ImportedBones[l].goBone.GetComponent<Rigidbody>().isKinematic)
					{
						flag = false;
					}
				}
				if (flag && l > 0 && !ropeNode3.bIsCoil)
				{
					ropeNode3.linkJoints[l] = CreateJoint(ropeNode3.segmentLinks[l], ropeNode3.segmentLinks[l - 1], ropeNode3.segmentLinks[l].transform.position);
					ropeNode3.linkJointBreaksProcessed[l] = false;
				}
				else
				{
					ropeNode3.linkJoints[l] = null;
				}
			}
			float num4 = ((RopeType != ERopeType.ImportBones) ? (((gameObject2.transform.position - gameObject.transform.position).magnitude - num3) / (gameObject2.transform.position - gameObject.transform.position).magnitude) : 0f);
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			for (int m = 0; m < ropeNode3.segmentLinks.Length; m++)
			{
				if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
				{
					float num5 = (float)m / ((ropeNode3.segmentLinks.Length != 1) ? ((float)ropeNode3.segmentLinks.Length - 1f) : 1f);
					if (Vector3.Distance(gameObject.transform.position, gameObject2.transform.position) < 0.001f)
					{
						ropeNode3.segmentLinks[m].transform.position = gameObject.transform.position;
						ropeNode3.segmentLinks[m].transform.rotation = gameObject.transform.rotation;
					}
					else
					{
						ropeNode3.segmentLinks[m].transform.position = Vector3.Lerp(gameObject.transform.position, gameObject2.transform.position, num5 * num4);
						ropeNode3.segmentLinks[m].transform.rotation = Quaternion.LookRotation((gameObject2.transform.position - gameObject.transform.position).normalized);
					}
					if (RopeType == ERopeType.LinkedObjects)
					{
						ropeNode3.segmentLinks[m].transform.rotation *= GetLinkedObjectLocalRotation(LinkTwistAngleStart + LinkTwistAngleIncrement * (float)m);
					}
				}
				num++;
			}
			if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
			{
				if (!ropeNode3.bIsCoil)
				{
					if (!bCheckIfBroken || !ropeNode3.linkJointBreaksProcessed[0])
					{
						if (ropeNode3.nTotalLinks == ropeNode3.nNumLinks)
						{
							ropeNode3.linkJoints[0] = CreateJoint(ropeNode3.segmentLinks[0], gameObject, gameObject.transform.position);
							ropeNode3.linkJointBreaksProcessed[0] = false;
						}
						else
						{
							ropeNode3.linkJoints[0] = null;
							ropeNode3.linkJointBreaksProcessed[0] = true;
						}
					}
					else
					{
						ropeNode3.linkJoints[0] = null;
					}
					if (!bCheckIfBroken || !ropeNode3.linkJointBreaksProcessed[ropeNode3.segmentLinks.Length])
					{
						ropeNode3.linkJoints[ropeNode3.segmentLinks.Length] = CreateJoint(ropeNode3.segmentLinks[ropeNode3.segmentLinks.Length - 1], gameObject2, gameObject2.transform.position);
						ropeNode3.linkJointBreaksProcessed[ropeNode3.segmentLinks.Length] = false;
					}
					else
					{
						ropeNode3.linkJoints[ropeNode3.segmentLinks.Length] = null;
					}
				}
			}
			else if (RopeType == ERopeType.ImportBones)
			{
				ropeNode3.linkJointBreaksProcessed[0] = true;
			}
			if (ropeNode3.nTotalLinks > ropeNode3.nNumLinks && !ropeNode3.m_bExtensionInitialized)
			{
				ropeNode3.m_bExtensionInitialized = true;
			}
		}
		if (m_bRopeStartInitialOrientationInitialized && RopeStart != null)
		{
			RopeStart.transform.localPosition = localPosition;
			RopeStart.transform.localRotation = localRotation;
		}
		for (int n = 0; n < RopeNodes.Count; n++)
		{
			RopeNode ropeNode4 = RopeNodes[n];
			if (ropeNode4.bInitialOrientationInitialized && ropeNode4.goNode != null)
			{
				ropeNode4.goNode.transform.localPosition = array3[n];
				ropeNode4.goNode.transform.localRotation = array4[n];
			}
		}
		num2 = 0;
		if (RopeType == ERopeType.Procedural || RopeType == ERopeType.LinkedObjects)
		{
			for (int num6 = 0; num6 < RopeNodes.Count; num6++)
			{
				RopeNode ropeNode5 = RopeNodes[num6];
				for (int num7 = 0; num7 < ropeNode5.segmentLinks.Length; num7++)
				{
					ropeNode5.segmentLinks[num7].transform.position = array[num2];
					ropeNode5.segmentLinks[num7].transform.rotation = array2[num2];
					num2++;
				}
			}
		}
		else if (RopeType == ERopeType.ImportBones)
		{
			for (int num8 = 0; num8 < ImportedBones.Length; num8++)
			{
				ImportedBones[num8].goBone.transform.position = array[num8];
				ImportedBones[num8].goBone.transform.rotation = array2[num8];
			}
		}
		CheckNeedsStartExitLockZ();
	}

	private ConfigurableJoint CreateJoint(GameObject goObject, GameObject goConnectedTo, Vector3 v3Pivot)
	{
		ConfigurableJoint configurableJoint = goObject.AddComponent<ConfigurableJoint>();
		SetupJoint(configurableJoint);
		configurableJoint.connectedBody = goConnectedTo.GetComponent<Rigidbody>();
		configurableJoint.anchor = goObject.transform.InverseTransformPoint(v3Pivot);
		return configurableJoint;
	}

	private void SetupJoint(ConfigurableJoint joint)
	{
		SoftJointLimit softJointLimit = default(SoftJointLimit);
		softJointLimit.contactDistance = 0f;
		softJointLimit.bounciness = 0f;
		JointDrive jointDrive = default(JointDrive);
		jointDrive.positionSpring = LinkJointSpringValue;
		jointDrive.positionDamper = LinkJointDamperValue;
		jointDrive.maximumForce = LinkJointMaxForceValue;
		joint.axis = Vector3.right;
		joint.secondaryAxis = Vector3.up;
		joint.breakForce = LinkJointBreakForce;
		joint.breakTorque = LinkJointBreakTorque;
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ((!Mathf.Approximately(LinkJointAngularXLimit, 0f)) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
		joint.angularYMotion = ((!Mathf.Approximately(LinkJointAngularYLimit, 0f)) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
		joint.angularZMotion = ((!Mathf.Approximately(LinkJointAngularZLimit, 0f)) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
		softJointLimit.limit = 0f - LinkJointAngularXLimit;
		joint.lowAngularXLimit = softJointLimit;
		softJointLimit.limit = LinkJointAngularXLimit;
		joint.highAngularXLimit = softJointLimit;
		softJointLimit.limit = LinkJointAngularYLimit;
		joint.angularYLimit = softJointLimit;
		softJointLimit.limit = LinkJointAngularZLimit;
		joint.angularZLimit = softJointLimit;
		joint.angularXDrive = jointDrive;
		joint.angularYZDrive = jointDrive;
		joint.projectionMode = JointProjectionMode.PositionAndRotation;
		joint.projectionDistance = 0.01f;
		joint.projectionAngle = 0.01f;
	}

	private Vector3 GetAxisVector(EAxis eAxis, float fLength)
	{
		switch (eAxis)
		{
		case EAxis.X:
			return new Vector3(fLength, 0f, 0f);
		case EAxis.Y:
			return new Vector3(0f, fLength, 0f);
		case EAxis.Z:
			return new Vector3(0f, 0f, fLength);
		case EAxis.MinusX:
			return new Vector3(0f - fLength, 0f, 0f);
		case EAxis.MinusY:
			return new Vector3(0f, 0f - fLength, 0f);
		case EAxis.MinusZ:
			return new Vector3(0f, 0f, 0f - fLength);
		default:
			return Vector3.zero;
		}
	}

	private float ExtendRopeLinear(float fLinearIncrement)
	{
		if (fLinearIncrement > 0f && Mathf.Approximately(m_fCurrentExtension, ExtensibleLength))
		{
			return 0f;
		}
		if (fLinearIncrement < 0f && Mathf.Approximately(m_fCurrentExtension, 0f))
		{
			return 0f;
		}
		RopeNode ropeNode = RopeNodes[RopeNodes.Count - 1];
		bool flag = false;
		float fCurrentExtension = m_fCurrentExtension;
		float num = ropeNode.fLength / (float)ropeNode.nNumLinks;
		Transform transform = ((RopeNodes.Count - 1 <= m_nFirstNonCoilNode) ? RopeStart.transform : RopeNodes[RopeNodes.Count - 2].goNode.transform);
		Vector3 vector = transform.TransformDirection(ropeNode.m_v3LocalDirectionForward);
		if (fLinearIncrement < 0f)
		{
			while (fLinearIncrement < 0f && ropeNode.m_nExtensionLinkIn > 0 && ropeNode.m_nExtensionLinkIn < ropeNode.segmentLinks.Length - 1 && !flag)
			{
				float num2 = Mathf.Max((0f - num) * 0.5f, fLinearIncrement);
				if (Mathf.Abs(num2) > m_fCurrentExtension)
				{
					num2 = 0f - m_fCurrentExtension;
					flag = true;
				}
				ropeNode.m_fExtensionRemainderIn += num2;
				if (ropeNode.m_fExtensionRemainderIn < 0f - num)
				{
					num2 += Mathf.Abs(ropeNode.m_fExtensionRemainderIn - (0f - num));
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.position = ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn - 1].transform.position;
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.rotation = ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn - 1].transform.rotation;
					SetExtensibleLinkToKinematic(ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn], true);
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.parent = transform;
					ropeNode.m_nExtensionLinkIn++;
					ropeNode.m_nExtensionLinkOut = ropeNode.m_nExtensionLinkIn - 1;
					ropeNode.m_fExtensionRemainderIn = 0f;
					ropeNode.m_fExtensionRemainderOut = 0f;
				}
				else
				{
					float t = (0f - ropeNode.m_fExtensionRemainderIn) / num;
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.position = transform.position + vector * (num + ropeNode.m_fExtensionRemainderIn);
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.rotation = Quaternion.Slerp(ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.rotation, ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn - 1].transform.rotation, t);
					SetExtensibleLinkToKinematic(ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn], true);
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkIn].transform.parent = transform;
					ropeNode.m_nExtensionLinkOut = ropeNode.m_nExtensionLinkIn;
					ropeNode.m_fExtensionRemainderOut = num + ropeNode.m_fExtensionRemainderIn;
				}
				fLinearIncrement -= num2;
				m_fCurrentExtension += num2;
			}
		}
		else if (fLinearIncrement > 0f)
		{
			while (fLinearIncrement > 0f && ropeNode.m_nExtensionLinkOut > 0 && ropeNode.m_nExtensionLinkOut < ropeNode.segmentLinks.Length - 1 && !flag)
			{
				float num3 = Mathf.Min(num * 0.5f, fLinearIncrement);
				if (m_fCurrentExtension + num3 > ExtensibleLength)
				{
					num3 = ExtensibleLength - m_fCurrentExtension;
					flag = true;
				}
				ropeNode.m_fExtensionRemainderOut += num3;
				if (ropeNode.m_fExtensionRemainderOut > num)
				{
					num3 -= ropeNode.m_fExtensionRemainderOut - num;
					SetExtensibleLinkToKinematic(ropeNode.segmentLinks[ropeNode.m_nExtensionLinkOut], false);
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkOut].transform.parent = base.transform;
					ropeNode.m_nExtensionLinkOut--;
					ropeNode.m_nExtensionLinkIn = ropeNode.m_nExtensionLinkOut + 1;
					ropeNode.m_fExtensionRemainderIn = 0f;
					ropeNode.m_fExtensionRemainderOut = 0f;
				}
				else
				{
					ropeNode.segmentLinks[ropeNode.m_nExtensionLinkOut].transform.position = transform.position + vector * ropeNode.m_fExtensionRemainderOut;
					ropeNode.m_nExtensionLinkIn = ropeNode.m_nExtensionLinkOut;
					ropeNode.m_fExtensionRemainderIn = 0f - num + ropeNode.m_fExtensionRemainderOut;
				}
				fLinearIncrement -= num3;
				m_fCurrentExtension += num3;
			}
		}
		return m_fCurrentExtension - fCurrentExtension;
	}

	private void SetExtensibleLinkToKinematic(GameObject link, bool bKinematic)
	{
		if (link.GetComponent<Rigidbody>().isKinematic == bKinematic)
		{
			return;
		}
		link.GetComponent<Rigidbody>().isKinematic = bKinematic;
		if (link.GetComponent<Collider>() != null)
		{
			link.GetComponent<Collider>().enabled = !bKinematic;
		}
		UltimateRopeLink component = link.GetComponent<UltimateRopeLink>();
		if (component != null)
		{
			component.ExtensibleKinematic = bKinematic;
		}
		ConfigurableJoint component2 = link.GetComponent<ConfigurableJoint>();
		if ((bool)component2)
		{
			if (bKinematic)
			{
				component2.breakForce = float.PositiveInfinity;
				component2.breakTorque = float.PositiveInfinity;
			}
			else
			{
				component2.breakForce = LinkJointBreakForce;
				component2.breakTorque = LinkJointBreakTorque;
			}
		}
	}

	private void SetupCoilBones(float fCoilLength)
	{
		float num = 0f;
		float num2 = CoilWidth * -0.5f + RopeDiameter * 0.5f;
		float num3 = CoilDiameter * 0.5f + RopeDiameter * 0.5f;
		float num4 = 0f;
		float num5 = 1f;
		float num6 = -1f;
		float num7 = Vector3.Distance(CoilObject.transform.position, RopeStart.transform.position) + CoilDiameter;
		float num8 = fCoilLength + num7;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		int num12 = 0;
		RopeNode ropeNode = RopeNodes[0];
		Vector3 localPosition = ropeNode.goNode.transform.localPosition;
		Quaternion localRotation = ropeNode.goNode.transform.localRotation;
		Vector3 localScale = ropeNode.goNode.transform.localScale;
		if (ropeNode.bInitialOrientationInitialized)
		{
			ropeNode.goNode.transform.localPosition = ropeNode.v3InitialLocalPos;
			ropeNode.goNode.transform.localRotation = ropeNode.qInitialLocalRot;
			ropeNode.goNode.transform.localScale = ropeNode.v3InitialLocalScale;
		}
		Vector3 vector = -CoilObject.transform.TransformDirection(GetAxisVector(CoilAxisRight, 1f));
		Vector3 vector2 = CoilObject.transform.TransformDirection(GetAxisVector(CoilAxisUp, 1f));
		Vector3 forward = Vector3.Cross(vector2, vector);
		Quaternion rotation = Quaternion.LookRotation(forward, vector2);
		ropeNode.goNode.transform.localPosition = localPosition;
		ropeNode.goNode.transform.localRotation = localRotation;
		ropeNode.goNode.transform.localScale = localScale;
		float num13 = (RopeNodes[0].fLength + num7) / (float)RopeNodes[0].nNumLinks;
		for (int i = 0; i < RopeNodes[0].segmentLinks.Length; i++)
		{
			m_afCoilBoneRadiuses[i] = num3;
			m_afCoilBoneAngles[i] = num4;
			m_afCoilBoneX[i] = num2;
			Vector3 vector3 = CoilObject.transform.position + vector2 * num3 + vector * num2;
			num9 = (vector3 - RopeStart.transform.position).magnitude;
			num += num13;
			num12++;
			float num14 = num8 - num9;
			if (num > num14)
			{
				num11 = num - num14;
				num10 = num4 - num11 / (num3 * (float)Math.PI * 2f) * 360f;
				m_fCurrentCoilRopeRadius = num3;
				m_fCurrentCoilTurnsLeft = num4 / 360f;
				break;
			}
			float num15 = num13 / (num3 * (float)Math.PI * 2f) * 360f;
			float num16 = num3 * (float)Math.PI * 2f / num13;
			num4 += num15;
			if (num6 > 0f)
			{
				num3 += RopeDiameter / num16;
				num6 -= num15;
			}
			else
			{
				num2 += RopeDiameter * num5 / num16;
			}
			if (num5 > 0f && num2 > CoilWidth * 0.5f - RopeDiameter * 0.5f)
			{
				num2 = CoilWidth * 0.5f - RopeDiameter * 0.5f;
				num6 = 360f;
				num5 = -1f;
			}
			if (num5 < 0f && num2 < CoilWidth * -0.5f + RopeDiameter * 0.5f)
			{
				num2 = CoilWidth * -0.5f + RopeDiameter * 0.5f;
				num6 = 360f;
				num5 = 1f;
			}
		}
		for (int j = 0; j < num12; j++)
		{
			m_afCoilBoneAngles[j] -= num10;
			RopeNodes[0].segmentLinks[j].transform.position = CoilObject.transform.position + vector2 * m_afCoilBoneRadiuses[j];
			RopeNodes[0].segmentLinks[j].transform.rotation = rotation;
			RopeNodes[0].segmentLinks[j].transform.RotateAround(CoilObject.transform.position, -vector, m_afCoilBoneAngles[j]);
			RopeNodes[0].segmentLinks[j].transform.position += vector * m_afCoilBoneX[j];
		}
		Vector3 vector4 = CoilObject.transform.position + vector2 * num3 + vector * num2;
		Vector3 normalized = (RopeStart.transform.position - vector4).normalized;
		num = (RopeNodes[0].segmentLinks[num12 - 1].transform.position - vector4).magnitude;
		float magnitude = (RopeStart.transform.position - vector4).magnitude;
		Quaternion rotation2 = Quaternion.LookRotation((RopeStart.transform.position - CoilObject.transform.position).normalized, vector2);
		for (int k = num12; k < RopeNodes[0].segmentLinks.Length; k++)
		{
			num += num13;
			if (num < magnitude)
			{
				RopeNodes[0].segmentLinks[k].transform.position = vector4 + normalized * num;
				RopeNodes[0].segmentLinks[k].transform.rotation = rotation2;
			}
			else
			{
				RopeNodes[0].segmentLinks[k].transform.position = RopeStart.transform.position;
				RopeNodes[0].segmentLinks[k].transform.rotation = rotation2;
			}
		}
		m_fCurrentCoilLength = fCoilLength;
	}

	private Quaternion GetLinkedObjectLocalRotation(float fTwistAngle = 0f)
	{
		if (LinkAxis == EAxis.X)
		{
			return Quaternion.LookRotation(Vector3.right) * Quaternion.AngleAxis(fTwistAngle, Vector3.right);
		}
		if (LinkAxis == EAxis.Y)
		{
			return Quaternion.LookRotation(Vector3.up) * Quaternion.AngleAxis(fTwistAngle, Vector3.up);
		}
		if (LinkAxis == EAxis.Z)
		{
			return Quaternion.LookRotation(Vector3.forward) * Quaternion.AngleAxis(fTwistAngle, Vector3.forward);
		}
		if (LinkAxis == EAxis.MinusX)
		{
			return Quaternion.LookRotation(-Vector3.right) * Quaternion.AngleAxis(fTwistAngle, -Vector3.right);
		}
		if (LinkAxis == EAxis.MinusY)
		{
			return Quaternion.LookRotation(-Vector3.up) * Quaternion.AngleAxis(fTwistAngle, -Vector3.up);
		}
		if (LinkAxis == EAxis.MinusZ)
		{
			return Quaternion.LookRotation(-Vector3.forward) * Quaternion.AngleAxis(fTwistAngle, -Vector3.forward);
		}
		return Quaternion.identity;
	}

	private float GetLinkedObjectScale(float fSegmentLength, int nNumLinks)
	{
		if (LinkObject == null)
		{
			return 0f;
		}
		MeshFilter component = LinkObject.GetComponent<MeshFilter>();
		if (component == null)
		{
			return 0f;
		}
		float num = 0f;
		if (RopeType == ERopeType.LinkedObjects)
		{
			if (LinkAxis == EAxis.X || LinkAxis == EAxis.MinusX)
			{
				num = component.sharedMesh.bounds.size.x;
			}
			if (LinkAxis == EAxis.Y || LinkAxis == EAxis.MinusY)
			{
				num = component.sharedMesh.bounds.size.y;
			}
			if (LinkAxis == EAxis.Z || LinkAxis == EAxis.MinusZ)
			{
				num = component.sharedMesh.bounds.size.z;
			}
		}
		float num2 = fSegmentLength / (float)nNumLinks - LinkOffsetObject * (fSegmentLength / (float)(nNumLinks - 1));
		return num2 / num;
	}

	private float GetLinkDiameter()
	{
		if (RopeType == ERopeType.Procedural)
		{
			return RopeDiameter;
		}
		if (RopeType == ERopeType.LinkedObjects)
		{
			if (LinkObject == null)
			{
				return 0f;
			}
			MeshFilter component = LinkObject.GetComponent<MeshFilter>();
			if (component == null)
			{
				return 0f;
			}
			float result = 0f;
			if (RopeType == ERopeType.LinkedObjects)
			{
				if (LinkAxis == EAxis.X || LinkAxis == EAxis.MinusX)
				{
					result = Mathf.Max(component.sharedMesh.bounds.size.y, component.sharedMesh.bounds.size.z);
				}
				if (LinkAxis == EAxis.Y || LinkAxis == EAxis.MinusY)
				{
					result = Mathf.Max(component.sharedMesh.bounds.size.x, component.sharedMesh.bounds.size.z);
				}
				if (LinkAxis == EAxis.Z || LinkAxis == EAxis.MinusZ)
				{
					result = Mathf.Max(component.sharedMesh.bounds.size.x, component.sharedMesh.bounds.size.y);
				}
			}
			return result;
		}
		if (RopeType == ERopeType.ImportBones)
		{
			return BoneColliderDiameter;
		}
		return 0f;
	}

	private Vector3 GetLinkAxisOffset(float fValue)
	{
		EAxis eAxis = EAxis.Z;
		if (RopeType == ERopeType.LinkedObjects)
		{
			eAxis = LinkAxis;
		}
		if (RopeType == ERopeType.ImportBones)
		{
			eAxis = BoneAxis;
		}
		switch (eAxis)
		{
		case EAxis.X:
			return new Vector3(fValue, 0f, 0f);
		case EAxis.Y:
			return new Vector3(0f, fValue, 0f);
		case EAxis.Z:
			return new Vector3(0f, 0f, fValue);
		case EAxis.MinusX:
			return new Vector3(0f - fValue, 0f, 0f);
		case EAxis.MinusY:
			return new Vector3(0f, 0f - fValue, 0f);
		case EAxis.MinusZ:
			return new Vector3(0f, 0f, 0f - fValue);
		default:
			return new Vector3(0f, 0f, fValue);
		}
	}

	private int GetLinkAxisIndex()
	{
		EAxis eAxis = EAxis.Z;
		if (RopeType == ERopeType.LinkedObjects)
		{
			eAxis = LinkAxis;
		}
		if (RopeType == ERopeType.ImportBones)
		{
			eAxis = BoneAxis;
		}
		switch (eAxis)
		{
		case EAxis.X:
			return 0;
		case EAxis.Y:
			return 1;
		case EAxis.Z:
			return 2;
		case EAxis.MinusX:
			return 0;
		case EAxis.MinusY:
			return 1;
		case EAxis.MinusZ:
			return 2;
		default:
			return 2;
		}
	}

	private bool GetLinkBoxColliderCenterAndSize(float fLinkLength, float fRopeDiameter, ref Vector3 v3CenterInOut, ref Vector3 v3SizeInOut)
	{
		if (RopeType == ERopeType.Procedural)
		{
			v3CenterInOut = Vector3.zero;
			v3SizeInOut = new Vector3(fRopeDiameter, fRopeDiameter, fLinkLength);
			return true;
		}
		if (RopeType == ERopeType.LinkedObjects)
		{
			MeshFilter component = LinkObject.GetComponent<MeshFilter>();
			if (component == null)
			{
				return false;
			}
			v3CenterInOut = component.sharedMesh.bounds.center;
			v3SizeInOut = component.sharedMesh.bounds.size;
			return true;
		}
		if (RopeType == ERopeType.ImportBones)
		{
			if (BoneAxis == EAxis.X)
			{
				v3SizeInOut = new Vector3(fLinkLength, fRopeDiameter, fRopeDiameter);
			}
			if (BoneAxis == EAxis.Y)
			{
				v3SizeInOut = new Vector3(fRopeDiameter, fLinkLength, fRopeDiameter);
			}
			if (BoneAxis == EAxis.Z)
			{
				v3SizeInOut = new Vector3(fRopeDiameter, fRopeDiameter, fLinkLength);
			}
			if (BoneAxis == EAxis.MinusX)
			{
				v3SizeInOut = new Vector3(fLinkLength, fRopeDiameter, fRopeDiameter);
			}
			if (BoneAxis == EAxis.MinusY)
			{
				v3SizeInOut = new Vector3(fRopeDiameter, fLinkLength, fRopeDiameter);
			}
			if (BoneAxis == EAxis.MinusZ)
			{
				v3SizeInOut = new Vector3(fRopeDiameter, fRopeDiameter, fLinkLength);
			}
			return true;
		}
		v3CenterInOut = Vector3.zero;
		v3SizeInOut = new Vector3(fRopeDiameter, fRopeDiameter, fLinkLength);
		return true;
	}

	private bool BuildImportedBoneList(GameObject goBoneFirst, GameObject goBoneLast, List<int> ListImportBonesStatic, List<int> ListImportBonesNoCollider, out List<RopeBone> outListImportedBones, out string strErrorMessage)
	{
		strErrorMessage = string.Empty;
		outListImportedBones = new List<RopeBone>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = goBoneFirst.name.Length - 1;
		while (num5 >= 0 && char.IsDigit(goBoneFirst.name[num5]))
		{
			num++;
			num5--;
		}
		if (num == 0)
		{
			strErrorMessage = "First bone name needs to end with digits in order to infer bone sequence";
			return false;
		}
		num3 = int.Parse(goBoneFirst.name.Substring(goBoneFirst.name.Length - num));
		int num6 = goBoneLast.name.Length - 1;
		while (num6 >= 0 && char.IsDigit(goBoneLast.name[num6]))
		{
			num2++;
			num6--;
		}
		if (num2 == 0)
		{
			strErrorMessage = "Last bone name needs to end with digits in order to infer bone sequence";
			return false;
		}
		num4 = int.Parse(goBoneLast.name.Substring(goBoneLast.name.Length - num2));
		string text = goBoneFirst.name.Substring(0, goBoneFirst.name.Length - num);
		string text2 = goBoneLast.name.Substring(0, goBoneLast.name.Length - num2);
		if (text != text2)
		{
			strErrorMessage = string.Format("First bone name prefix ({0}) and last bone name prefix ({1}) don't match", text, text2);
			return false;
		}
		if (BoneFirst.transform.parent == null || BoneLast.transform.parent == null)
		{
			strErrorMessage = string.Format("First and last bones need to share a common parent object");
			return false;
		}
		GameObject gameObject = ((!BoneLast.transform.IsChildOf(BoneFirst.transform)) ? BoneLast.transform.parent.gameObject : BoneFirst.transform.parent.gameObject);
		if (BuildImportedBoneListTry(gameObject, text, num3, num4, num, num2, ListImportBonesStatic, ListImportBonesNoCollider, out outListImportedBones, ref strErrorMessage))
		{
			return true;
		}
		gameObject = gameObject.transform.root.gameObject;
		string text3 = string.Format("Try1: {0}\nTry2: ", strErrorMessage);
		if (BuildImportedBoneListTry(gameObject, text, num3, num4, num, num2, ListImportBonesStatic, ListImportBonesNoCollider, out outListImportedBones, ref strErrorMessage))
		{
			return true;
		}
		strErrorMessage = text3 + strErrorMessage;
		return false;
	}

	private bool BuildImportedBoneListTry(GameObject goRoot, string strPrefix, int nIndexFirst, int nIndexLast, int nDigitsFirst, int nDigitsLast, List<int> ListImportBonesStatic, List<int> ListImportBonesNoCollider, out List<RopeBone> outListImportedBones, ref string strErrorMessage)
	{
		outListImportedBones = new List<RopeBone>();
		Dictionary<string, GameObject> outHashString2GameObjects = new Dictionary<string, GameObject>();
		if (!BuildBoneHashString2GameObject(goRoot, goRoot, ref outHashString2GameObjects, ref strErrorMessage))
		{
			return false;
		}
		Dictionary<GameObject, Transform> dictionary = new Dictionary<GameObject, Transform>();
		int num = ((nIndexFirst <= nIndexLast) ? 1 : (-1));
		for (int i = nIndexFirst; (num != 1) ? (i >= nIndexLast) : (i <= nIndexLast); i += num)
		{
			bool flag = false;
			for (int j = nDigitsFirst; j <= nDigitsLast; j++)
			{
				string key = strPrefix + i.ToString("D" + j);
				if (outHashString2GameObjects.ContainsKey(key))
				{
					RopeBone ropeBone = new RopeBone();
					ropeBone.goBone = outHashString2GameObjects[key];
					ropeBone.tfParent = ropeBone.goBone.transform.parent;
					ropeBone.bCreatedCollider = !ListImportBonesNoCollider.Contains(i);
					ropeBone.bIsStatic = ListImportBonesStatic.Contains(i);
					ropeBone.nOriginalLayer = ropeBone.goBone.layer;
					outListImportedBones.Add(ropeBone);
					dictionary.Add(ropeBone.goBone, ropeBone.goBone.transform);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				strErrorMessage = string.Format("Bone not found (bone number suffix {0}, trying to find below node {1}'s hierarchy)", i, goRoot.name);
				return false;
			}
		}
		foreach (RopeBone outListImportedBone in outListImportedBones)
		{
			Transform parent = outListImportedBone.goBone.transform.parent;
			while (parent != null && dictionary.ContainsKey(parent.gameObject))
			{
				parent = parent.parent;
			}
			if (parent == null)
			{
				parent = goRoot.transform;
			}
			dictionary[outListImportedBone.goBone] = parent;
		}
		foreach (RopeBone outListImportedBone2 in outListImportedBones)
		{
			Transform transform = dictionary[outListImportedBone2.goBone];
			GameObject gameObject = new GameObject();
			outListImportedBone2.v3OriginalLocalScale = outListImportedBone2.goBone.transform.localScale;
			gameObject.transform.position = outListImportedBone2.goBone.transform.position;
			gameObject.transform.rotation = outListImportedBone2.goBone.transform.rotation;
			gameObject.transform.parent = transform.transform;
			outListImportedBone2.v3OriginalLocalPos = gameObject.transform.localPosition;
			outListImportedBone2.qOriginalLocalRot = gameObject.transform.localRotation;
			outListImportedBone2.tfNonBoneParent = transform;
			UnityEngine.Object.DestroyImmediate(gameObject);
			if (outListImportedBone2.bIsStatic)
			{
				outListImportedBone2.goBone.transform.parent = transform;
			}
			else
			{
				outListImportedBone2.goBone.transform.parent = base.transform;
			}
		}
		return true;
	}

	private bool BuildBoneHashString2GameObject(GameObject goRoot, GameObject goCurrent, ref Dictionary<string, GameObject> outHashString2GameObjects, ref string strErrorMessage)
	{
		for (int i = 0; i < goCurrent.transform.childCount; i++)
		{
			GameObject goCurrent2 = goCurrent.transform.GetChild(i).gameObject;
			if (!BuildBoneHashString2GameObject(goRoot, goCurrent2, ref outHashString2GameObjects, ref strErrorMessage))
			{
				return false;
			}
		}
		if (outHashString2GameObjects.ContainsKey(goCurrent.name))
		{
			strErrorMessage = string.Format("Bone name {0} is found more than once in GameObject {1}'s hierarchy. The name must be unique.", goCurrent.name, goRoot.name);
			return false;
		}
		outHashString2GameObjects.Add(goCurrent.name, goCurrent);
		return true;
	}

	private bool ParseBoneIndices(string strBoneList, out List<int> outListBoneIndices, out string strErrorMessage)
	{
		outListBoneIndices = new List<int>();
		strErrorMessage = string.Empty;
		if (strBoneList.Length == 0)
		{
			return true;
		}
		string[] array = strBoneList.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('-');
			if (array2.Length == 1)
			{
				int num = 0;
				try
				{
					num = int.Parse(array2[0]);
				}
				catch
				{
					strErrorMessage = "Field " + (i + 1) + " is invalid (error parsing number: " + array2[0] + ")";
					return false;
				}
				outListBoneIndices.Add(num);
				continue;
			}
			if (array2.Length == 2)
			{
				int num2 = 0;
				int num3 = 0;
				try
				{
					num2 = int.Parse(array2[0]);
				}
				catch
				{
					strErrorMessage = "Field " + (i + 1) + " is invalid (error parsing range start: " + array2[0] + ")";
					return false;
				}
				try
				{
					num3 = int.Parse(array2[1]);
				}
				catch
				{
					strErrorMessage = "Field " + (i + 1) + " is invalid (error parsing range end: " + array2[1] + ")";
					return false;
				}
				if (num3 < num2)
				{
					strErrorMessage = "Field " + (i + 1) + " has invalid range (" + num2 + " is greater than " + num3 + ")";
					return false;
				}
				for (int j = num2; j <= num3; j++)
				{
					outListBoneIndices.Add(j);
				}
				continue;
			}
			strErrorMessage = "Field " + (i + 1) + " has invalid range (field content: " + array[i] + ")";
			return false;
		}
		outListBoneIndices.Sort();
		List<int> list = new List<int>();
		int num4 = -1;
		foreach (int outListBoneIndex in outListBoneIndices)
		{
			if (outListBoneIndex != num4)
			{
				num4 = outListBoneIndex;
				list.Add(outListBoneIndex);
			}
		}
		outListBoneIndices = list;
		return true;
	}

	private void CheckLoadPersistentData()
	{
		if (Application.isEditor && RopePersistManager.PersistentDataExists(this))
		{
			RopePersistManager.RetrievePersistentData(this);
			RopePersistManager.RemovePersistentData(this);
		}
	}

	private void CheckSavePersistentData()
	{
		if (Application.isEditor && PersistAfterPlayMode && !m_bLastStatusIsError)
		{
			RopePersistManager.StorePersistentData(this);
		}
	}
}
