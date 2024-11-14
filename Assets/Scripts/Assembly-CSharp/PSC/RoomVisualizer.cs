using UnityEngine;

namespace PSC
{
	[RequireComponent(typeof(Room))]
	[ExecuteInEditMode]
	public class RoomVisualizer : MonoBehaviour
	{
		private Room m_Room;

		[SerializeField]
		private Color m_VolumeColor = new Color(0.019f, 0.96f, 0.96f, 0.5f);

		[SerializeField]
		[Range(0f, 5f)]
		private float m_VolumeHeight = 1.57f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_VolumeFootSize = 0.2f;

		private bool m_ShowVolume = true;

		private Material m_VolumeMaterial;

		private int[] m_Triangles;

		private int m_TriangleIndex;

		private Mesh m_Mesh;

		public Color volumeColor
		{
			get
			{
				return m_VolumeColor;
			}
			set
			{
				UpdateColor();
				m_VolumeColor = value;
			}
		}

		public bool showVolume
		{
			get
			{
				return m_ShowVolume;
			}
			set
			{
				m_ShowVolume = value;
			}
		}

		public float volumeHeight
		{
			get
			{
				return m_VolumeHeight;
			}
			set
			{
				CalculateVertextPositions();
				m_VolumeHeight = value;
			}
		}

		public Room room
		{
			get
			{
				if (m_Room == null)
				{
					m_Room = GetComponent<Room>();
				}
				return m_Room;
			}
		}

		public float volumeFootSize
		{
			get
			{
				return m_VolumeFootSize;
			}
			set
			{
				m_VolumeFootSize = value;
			}
		}

		public Mesh mesh
		{
			get
			{
				if (m_Mesh == null)
				{
					m_Mesh = new Mesh();
					m_Mesh.vertices = new Vector3[12];
					m_Mesh.hideFlags = HideFlags.DontSave;
				}
				return m_Mesh;
			}
		}

		public Material material
		{
			get
			{
				if (m_VolumeMaterial == null)
				{
					Shader shader = Shader.Find("Hidden/Internal-Colored");
					m_VolumeMaterial = new Material(shader);
					m_VolumeMaterial.hideFlags = HideFlags.HideAndDontSave;
					m_VolumeMaterial.SetInt("_srcBlend", 5);
					m_VolumeMaterial.SetInt("_DstBlend", 5);
					m_VolumeMaterial.SetInt("_Cull", 0);
					m_VolumeMaterial.SetInt("_ZWrite", 0);
					m_VolumeMaterial.SetColor("_Color", m_VolumeColor);
					m_VolumeMaterial.hideFlags = HideFlags.DontSave;
				}
				return m_VolumeMaterial;
			}
		}

		public void OnEnable()
		{
			Object.Destroy(this);
		}

		public void OnDisable()
		{
			if (m_Mesh != null)
			{
				Object.DestroyImmediate(m_Mesh, true);
			}
			if (m_VolumeMaterial != null)
			{
				Object.DestroyImmediate(m_VolumeMaterial, true);
			}
		}

		private void InitializeMesh()
		{
			CreateMaterial();
			CalculateVertextPositions();
			CalculateTriangles();
			UpdateColor();
		}

		private void UpdateColor()
		{
			material.SetColor("_Color", m_VolumeColor);
		}

		private void StartTriangle()
		{
			m_TriangleIndex = 0;
			m_Triangles = new int[72];
		}

		private void EndTriangle()
		{
			mesh.triangles = m_Triangles;
		}

		public void Update()
		{
		}

		private void SetNextTriangle(int value)
		{
			m_Triangles[m_TriangleIndex] = value;
			m_TriangleIndex++;
		}

		[ContextMenu("Create Material")]
		private void CreateMaterial()
		{
		}

		public void OnLayoutChanged(LayoutConfiguration config)
		{
			if (config != null)
			{
				CalculateVertextPositions();
			}
		}

		[ContextMenu("Calculate Vertex Positions")]
		private void CalculateVertextPositions()
		{
			Vector3 zero = Vector3.zero;
			float num = volumeFootSize * Mathf.Sin(45f);
			float num2 = 0.02f * Mathf.Sin(45f);
			if (room.configuration != null)
			{
				zero.x = room.configuration.sizeInMeters.x / 2f;
				zero.y = volumeHeight;
				zero.z = room.configuration.sizeInMeters.y / 2f;
			}
			Vector3[] vertices = new Vector3[16]
			{
				new Vector3(zero.x + num, 0f, 0f - zero.z - num),
				new Vector3(0f - zero.x - num, 0f, 0f - zero.z - num),
				new Vector3(0f - zero.x - num, 0f, zero.z + num),
				new Vector3(zero.x + num, 0f, zero.z + num),
				new Vector3(zero.x, 0f, 0f - zero.z),
				new Vector3(0f - zero.x, 0f, 0f - zero.z),
				new Vector3(0f - zero.x, 0f, zero.z),
				new Vector3(zero.x, 0f, zero.z),
				new Vector3(zero.x, zero.y, 0f - zero.z),
				new Vector3(0f - zero.x, zero.y, 0f - zero.z),
				new Vector3(0f - zero.x, zero.y, zero.z),
				new Vector3(zero.x, zero.y, zero.z),
				new Vector3(zero.x + num2, 0f, 0f - zero.z - num2),
				new Vector3(0f - zero.x - num2, 0f, 0f - zero.z - num2),
				new Vector3(0f - zero.x - num2, 0f, zero.z + num2),
				new Vector3(zero.x + num2, 0f, zero.z + num2)
			};
			mesh.name = "Volume Mesh";
			mesh.vertices = vertices;
			mesh.RecalculateBounds();
		}

		private void CalculateTriangles()
		{
			CreateMaterial();
			StartTriangle();
			int num = 4;
			int num2 = 8;
			int num3 = 0;
			int num4 = 12;
			for (int i = 0; i < 4; i++)
			{
				SetNextTriangle(num2 + (1 + i) % 4);
				SetNextTriangle(num2 + (0 + i) % 4);
				SetNextTriangle(num + (0 + i) % 4);
				SetNextTriangle(num + (1 + i) % 4);
				SetNextTriangle(num2 + (1 + i) % 4);
				SetNextTriangle(num + (0 + i) % 4);
			}
			for (int j = 0; j < 4; j++)
			{
				SetNextTriangle(num + (0 + j) % 4);
				SetNextTriangle(num3 + (0 + j) % 4);
				SetNextTriangle(num3 + (1 + j) % 4);
				SetNextTriangle(num + (0 + j) % 4);
				SetNextTriangle(num3 + (1 + j) % 4);
				SetNextTriangle(num + (1 + j) % 4);
			}
			for (int k = 0; k < 4; k++)
			{
				SetNextTriangle(num + (0 + k) % 4);
				SetNextTriangle(num4 + (0 + k) % 4);
				SetNextTriangle(num4 + (1 + k) % 4);
				SetNextTriangle(num + (0 + k) % 4);
				SetNextTriangle(num4 + (1 + k) % 4);
				SetNextTriangle(num + (1 + k) % 4);
			}
			EndTriangle();
		}

		private void OnValidate()
		{
			volumeColor = volumeColor;
			volumeHeight = volumeHeight;
		}
	}
}
