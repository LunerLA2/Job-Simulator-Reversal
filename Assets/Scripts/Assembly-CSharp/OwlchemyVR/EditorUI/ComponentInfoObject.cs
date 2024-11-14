using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public class ComponentInfoObject
	{
		private Component componentObj;

		private List<PropertyInfo> properties;

		private List<FieldInfo> fields;

		private List<FieldOrPropertyInfo> fieldsAndProperties;

		public Component ComponentObj
		{
			get
			{
				return componentObj;
			}
		}

		public List<PropertyInfo> Properties
		{
			get
			{
				return properties;
			}
		}

		public List<FieldInfo> Fields
		{
			get
			{
				return fields;
			}
		}

		public List<FieldOrPropertyInfo> FieldsAndProperties
		{
			get
			{
				return fieldsAndProperties;
			}
		}

		public string Name
		{
			get
			{
				string text = componentObj.GetType().ToString();
				if (text.StartsWith("UnityEngine."))
				{
					text = text.Remove(0, "UnityEngine.".Length);
				}
				return text;
			}
		}

		public ComponentInfoObject(Component component, List<PropertyInfo> properties, List<FieldInfo> fields)
		{
			componentObj = component;
			this.properties = properties;
			this.fields = fields;
			fieldsAndProperties = new List<FieldOrPropertyInfo>();
			for (int i = 0; i < properties.Count; i++)
			{
				fieldsAndProperties.Add(new FieldOrPropertyInfo(component, properties[i]));
			}
			for (int j = 0; j < fields.Count; j++)
			{
				fieldsAndProperties.Add(new FieldOrPropertyInfo(component, fields[j]));
			}
		}

		public override string ToString()
		{
			string text = "Component:" + componentObj.GetType();
			foreach (PropertyInfo property in properties)
			{
				text = text + "Prop:" + property.ToString();
			}
			foreach (FieldInfo field in fields)
			{
				text = text + "Field:" + field.ToString();
			}
			return text;
		}
	}
}
