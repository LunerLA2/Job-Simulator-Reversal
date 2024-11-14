using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public class FieldOrPropertyInfo
{
	public enum InfoTypes
	{
		Unset = 0,
		FieldInfo = 1,
		PropertyInfo = 2
	}

	private Component component;

	private FieldInfo fieldInfo;

	private PropertyInfo propInfo;

	public InfoTypes infoType;

	public string Name
	{
		get
		{
			if (infoType == InfoTypes.FieldInfo)
			{
				return fieldInfo.Name;
			}
			if (infoType == InfoTypes.PropertyInfo)
			{
				return propInfo.Name;
			}
			Debug.LogWarning("Unknown prop type");
			return string.Empty;
		}
	}

	public FieldOrPropertyInfo(Component component, FieldInfo info)
	{
		this.component = component;
		fieldInfo = info;
		propInfo = null;
		infoType = InfoTypes.FieldInfo;
	}

	public FieldOrPropertyInfo(Component component, PropertyInfo info)
	{
		this.component = component;
		propInfo = info;
		fieldInfo = null;
		infoType = InfoTypes.PropertyInfo;
	}

	public object GetValue()
	{
		if (infoType == InfoTypes.FieldInfo)
		{
			return fieldInfo.GetValue(component);
		}
		if (infoType == InfoTypes.PropertyInfo)
		{
			return propInfo.GetValue(component, null);
		}
		Debug.LogWarning("Unknown info type:" + infoType);
		return null;
	}

	public string GetValueAsString()
	{
		object value = GetValue();
		if (value != null)
		{
			return value.ToString();
		}
		return string.Empty;
	}

	public void SetValue(object value)
	{
		if (infoType == InfoTypes.FieldInfo)
		{
			fieldInfo.SetValue(component, value);
		}
		else if (infoType == InfoTypes.PropertyInfo)
		{
			propInfo.SetValue(component, value, null);
		}
		else
		{
			Debug.LogWarning("Unknown info type:" + infoType);
		}
	}

	public void SetValueWithString(string value)
	{
		Type fieldPropType = GetFieldPropType();
		if (fieldPropType == typeof(float))
		{
			float result;
			if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				SetValue(result);
			}
		}
		else if (fieldPropType == typeof(int))
		{
			int result2;
			if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
			{
				SetValue(result2);
			}
		}
		else if (fieldPropType == typeof(string))
		{
			SetValue(value);
		}
		else
		{
			Debug.LogWarning("Unable to set value with string, unsupported type:" + fieldPropType);
		}
	}

	public Type GetFieldPropType()
	{
		if (infoType == InfoTypes.FieldInfo)
		{
			return fieldInfo.FieldType;
		}
		if (infoType == InfoTypes.PropertyInfo)
		{
			return propInfo.PropertyType;
		}
		Debug.LogWarning("Unknown info type:" + infoType);
		return null;
	}
}
