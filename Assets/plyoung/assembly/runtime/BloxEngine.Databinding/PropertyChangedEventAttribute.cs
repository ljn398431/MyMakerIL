using System;

namespace BloxEngine.Databinding
{
	[AttributeUsage(AttributeTargets.Event, AllowMultiple = true, Inherited = true)]
	public class PropertyChangedEventAttribute : Attribute
	{
		public string[] Properties;

		public PropertyChangedEventAttribute(params string[] properties)
		{
			this.Properties = properties;
		}
	}
}
