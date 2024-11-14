using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(PickupableItem))]
public class DecalGunController : MonoBehaviour
{
	[SerializeField]
	private bool useCusor;

	[SerializeField]
	private GameObject cursor;

	[SerializeField]
	private Transform raycastPosition;

	[SerializeField]
	private Sprite currentDecal;

	[SerializeField]
	private WorldItemData currentDecalWItem;

	[SerializeField]
	private WorldItem decalGunWItem;

	[SerializeField]
	private SpriteRenderer[] currentDecalPreview;

	[SerializeField]
	private PickupableItem pickupableItem;

	private Transform container;

	private float orthographicCameraSize;

	private Vector3 brushObjScale;

	public Sprite CurrentDecal
	{
		get
		{
			return currentDecal;
		}
	}

	private void Start()
	{
		container = TexturePainterController.Instance.Container;
		orthographicCameraSize = TexturePainterController.Instance.TargetCamera.orthographicSize;
		brushObjScale = ((!cursor) ? (Vector3.one * 0.015f) : cursor.transform.localScale);
		if (useCusor)
		{
			TexturePainterController.Instance.TargetCamera.enabled = true;
			return;
		}
		cursor.SetActive(false);
		TexturePainterController.Instance.Refresh();
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj.OnReleased, new Action<GrabbableItem>(OnRelease));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj.OnReleased, new Action<GrabbableItem>(OnRelease));
	}

	public void Undo()
	{
		TexturePainterController.Instance.ClearDecals();
		TexturePainterController.Instance.Refresh();
	}

	public void SetDecal(Sprite newDecal, WorldItem newDecalWItem)
	{
		if ((bool)cursor && useCusor)
		{
			cursor.GetComponent<SpriteRenderer>().sprite = newDecal;
		}
		currentDecal = newDecal;
		currentDecalWItem = newDecalWItem.Data;
		for (int i = 0; i < currentDecalPreview.Length; i++)
		{
			currentDecalPreview[i].sprite = newDecal;
		}
		TexturePainterController.Instance.Refresh();
	}

	public void DoAction()
	{
		Vector3 uvWorldPosition = Vector3.zero;
		if (HitUVPosition(ref uvWorldPosition))
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("TexturePainter-Instances/DecalEntity"));
			gameObject.GetComponent<SpriteRenderer>().sprite = currentDecal;
			gameObject.transform.parent = container;
			gameObject.transform.localPosition = uvWorldPosition;
			gameObject.transform.localScale = brushObjScale;
			gameObject.transform.eulerAngles = new Vector3(0f, 0f, base.transform.eulerAngles.z);
			GameEventsManager.Instance.ItemActionOccurred(decalGunWItem.Data, "USED");
			GameEventsManager.Instance.ItemActionOccurred(currentDecalWItem, "USED");
			if (!useCusor)
			{
				TexturePainterController.Instance.Refresh();
			}
		}
	}

	private void OnRelease(GrabbableItem item)
	{
		cursor.SetActive(false);
	}

	private void Update()
	{
		if (!pickupableItem.IsCurrInHand)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			DoAction();
		}
		if (useCusor && (bool)cursor)
		{
			Vector3 uvWorldPosition = Vector3.zero;
			if (HitUVPosition(ref uvWorldPosition))
			{
				cursor.SetActive(true);
				cursor.transform.position = uvWorldPosition + container.position;
				cursor.transform.eulerAngles = new Vector3(0f, 0f, base.transform.eulerAngles.z);
			}
			else
			{
				cursor.SetActive(false);
			}
		}
	}

	private bool HitUVPosition(ref Vector3 uvWorldPosition)
	{
		Ray ray = new Ray(raycastPosition.position, raycastPosition.forward * 5f);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 200f, LayerMaskHelper.OnlyIncluding(25)))
		{
			MeshCollider meshCollider = hitInfo.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null)
			{
				return false;
			}
			Vector2 vector = new Vector2(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
			uvWorldPosition.x = vector.x - orthographicCameraSize;
			uvWorldPosition.y = vector.y - orthographicCameraSize;
			uvWorldPosition.z = 0f;
			return true;
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(raycastPosition.position, raycastPosition.forward * 3f);
	}
}
