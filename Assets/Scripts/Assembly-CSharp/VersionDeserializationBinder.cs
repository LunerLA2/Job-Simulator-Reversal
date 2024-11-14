using System;
using System.Reflection;
using System.Runtime.Serialization;

public sealed class VersionDeserializationBinder : SerializationBinder
{
	public override Type BindToType(string assemblyName, string typeName)
	{
		if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
		{
			Type type = null;
			assemblyName = Assembly.GetExecutingAssembly().FullName;
			return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
		}
		return null;
	}
}
