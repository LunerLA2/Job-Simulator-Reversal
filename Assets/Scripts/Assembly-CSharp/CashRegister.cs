using System;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
	[SerializeField]
	private LegacyCashierScanner scanner;

	[SerializeField]
	private TextMeshPro totalDisplayTextMesh;

	[SerializeField]
	private TextMeshPro primaryDisplayItemNameTextMesh;

	[SerializeField]
	private TextMeshPro primaryDisplayItemCostTextMesh;

	[SerializeField]
	private TextMeshPro primaryDisplayMessage;

	private string totalDisplayStartText = "Total:";

	private int runningTotal;

	private void Awake()
	{
		RefreshTotalDisplay();
		primaryDisplayItemNameTextMesh.text = string.Empty;
		primaryDisplayItemCostTextMesh.text = string.Empty;
	}

	private void OnEnable()
	{
		LegacyCashierScanner legacyCashierScanner = scanner;
		legacyCashierScanner.OnScanSuccess = (Action<WorldItem, bool>)Delegate.Combine(legacyCashierScanner.OnScanSuccess, new Action<WorldItem, bool>(ScanSuccess));
		LegacyCashierScanner legacyCashierScanner2 = scanner;
		legacyCashierScanner2.OnScanFailure = (Action<WorldItem>)Delegate.Combine(legacyCashierScanner2.OnScanFailure, new Action<WorldItem>(ScanFailure));
	}

	private void OnDisable()
	{
		LegacyCashierScanner legacyCashierScanner = scanner;
		legacyCashierScanner.OnScanSuccess = (Action<WorldItem, bool>)Delegate.Remove(legacyCashierScanner.OnScanSuccess, new Action<WorldItem, bool>(ScanSuccess));
		LegacyCashierScanner legacyCashierScanner2 = scanner;
		legacyCashierScanner2.OnScanFailure = (Action<WorldItem>)Delegate.Remove(legacyCashierScanner2.OnScanFailure, new Action<WorldItem>(ScanFailure));
	}

	private void ScanSuccess(WorldItem item, bool onSale)
	{
		float cost = item.Data.Cost;
		primaryDisplayMessage.gameObject.SetActive(false);
		if (onSale)
		{
			primaryDisplayItemNameTextMesh.text = "*" + item.Data.ItemFullName + "*";
			primaryDisplayItemCostTextMesh.text = string.Format("{0:C}", cost / 2f);
			AddTotal(cost / 2f);
		}
		else
		{
			primaryDisplayItemNameTextMesh.text = item.Data.ItemFullName;
			primaryDisplayItemCostTextMesh.text = string.Format("{0:C}", cost);
			AddTotal(cost);
		}
	}

	private void ScanFailure(WorldItem item)
	{
		primaryDisplayMessage.gameObject.SetActive(true);
		primaryDisplayMessage.text = "Unable to Scan Item";
		primaryDisplayItemNameTextMesh.text = string.Empty;
		primaryDisplayItemCostTextMesh.text = string.Empty;
	}

	private void AddTotal(float amount)
	{
		runningTotal += Mathf.RoundToInt(amount * 100f);
		RefreshTotalDisplay();
	}

	private void RefreshTotalDisplay()
	{
		totalDisplayTextMesh.text = string.Format(totalDisplayStartText + "{0:C}", (float)runningTotal / 100f);
	}
}
