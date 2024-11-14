using UnityEngine;

namespace SiliconTrail
{
	public class SurfingMinigame : MonoBehaviour
	{
		[SerializeField]
		private float duration = 30f;

		[SerializeField]
		private Surfer surfer;

		[SerializeField]
		private DataPacket dataPacketPrefab;

		[SerializeField]
		private RectTransform dataPacketsRoot;

		[SerializeField]
		private float minDataPacketY = -110f;

		[SerializeField]
		private float maxDataPacketY = 110f;

		[SerializeField]
		private float minDataPacketTimeout = 0.25f;

		[SerializeField]
		private float maxDataPacketTimeout = 1f;

		private int totalDataCollected;

		private float time;

		private float timeUntilNextDataPacket;

		public bool IsOver
		{
			get
			{
				return time >= duration;
			}
		}

		public int TotalDataCollected
		{
			get
			{
				return totalDataCollected;
			}
		}

		private void OnEnable()
		{
			surfer.Hit += OnSurferHit;
			time = 0f;
			Vector3 localPosition = surfer.transform.localPosition;
			localPosition.y = 0f;
			surfer.transform.localPosition = localPosition;
			timeUntilNextDataPacket = Random.Range(minDataPacketTimeout, maxDataPacketTimeout);
			ClearDataPackets();
		}

		private void OnDisable()
		{
			surfer.Hit -= OnSurferHit;
			ClearDataPackets();
		}

		private void Update()
		{
			if (timeUntilNextDataPacket <= 0f)
			{
				SpawnDataPacket();
				timeUntilNextDataPacket = Random.Range(minDataPacketTimeout, maxDataPacketTimeout);
			}
			timeUntilNextDataPacket -= Time.deltaTime;
			time += Time.deltaTime;
		}

		private void SpawnDataPacket()
		{
			DataPacket dataPacket = Object.Instantiate(dataPacketPrefab);
			dataPacket.name = dataPacketPrefab.name;
			RectTransform component = dataPacket.GetComponent<RectTransform>();
			component.SetParent(dataPacketsRoot, false);
			component.anchoredPosition = new Vector2(0f, Random.Range(minDataPacketY, maxDataPacketY));
			component.localScale = Vector3.one;
		}

		private void ClearDataPackets()
		{
			foreach (Transform item in dataPacketsRoot)
			{
				Object.Destroy(item.gameObject);
			}
		}

		private void OnSurferHit(DataPacket dataPacket)
		{
			totalDataCollected = Mathf.Max(0, totalDataCollected + dataPacket.Data);
			Object.Destroy(dataPacket.gameObject);
		}

		public void OnKeyPress(string code)
		{
			if (code == "1")
			{
				surfer.MoveUp();
			}
			else if (code == "0")
			{
				surfer.MoveDown();
			}
		}
	}
}
