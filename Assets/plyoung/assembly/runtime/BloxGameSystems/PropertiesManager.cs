using BloxEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox(ExceptForSpecifiedMembers = true)]
	[BloxBlockIcon("bgs")]
	[HelpURL("https://plyoung.github.io/blox-property-manager.html")]
	public class PropertiesManager : ScriptableObject
	{
		public List<ManagedProperty> properties = new List<ManagedProperty>();

		[NonSerialized]
		private static PropertiesManager manager;

		[NonSerialized]
		private static Dictionary<string, ManagedProperty> cachedProperties;

		public static void OnBootstrapAwake()
		{
			PropertiesManager.manager = Resources.Load<PropertiesManager>("Blox/PropertiesManager");
			if (!((UnityEngine.Object)PropertiesManager.manager == (UnityEngine.Object)null))
			{
				PropertiesManager.cachedProperties = new Dictionary<string, ManagedProperty>(PropertiesManager.manager.properties.Count);
				for (int i = 0; i < PropertiesManager.manager.properties.Count; i++)
				{
					PropertiesManager.cachedProperties.Add(PropertiesManager.manager.properties[i].propertyName, PropertiesManager.manager.properties[i]);
				}
				for (int j = 0; j < PropertiesManager.manager.properties.Count; j++)
				{
					if (PropertiesManager.manager.properties[j].runDuringBoot)
					{
						if (PropertiesManager.manager.properties[j].setter == null)
						{
							Debug.LogError("[PropertiesManager.OnBootstrap] The property does not have a setter bind: " + PropertiesManager.manager.properties[j].propertyName);
						}
						else
						{
							string @string = PlayerPrefs.GetString("PropertiesManager." + PropertiesManager.manager.properties[j].propertyName, null);
							if (@string != null)
							{
								object blackboardValue = BloxStringSerializer.Deserialize(@string, PropertiesManager.manager.properties[j].setter.DataType(false));
								PropertiesManager.manager.properties[j].Initialize();
								PropertiesManager.manager.properties[j].setter.Execute(blackboardValue);
							}
						}
					}
				}
			}
		}

		private void OnDestroy()
		{
			PropertiesManager.manager = null;
			PropertiesManager.cachedProperties = null;
		}

		public static void AddValueChangeListener(string propertyName, Action callback)
		{
			if (PropertiesManager.cachedProperties != null)
			{
				ManagedProperty managedProperty = null;
				if (PropertiesManager.cachedProperties.TryGetValue(propertyName, out managedProperty))
				{
					managedProperty.Initialize();
					managedProperty.AddValueChangeListener(callback);
				}
				else
				{
					Debug.LogError("[PropertiesManager] Property not defined: " + propertyName);
				}
			}
		}

		public static void RemoveValueChangeListener(string propertyName, Action callback)
		{
			if (PropertiesManager.cachedProperties != null && !((UnityEngine.Object)PropertiesManager.manager == (UnityEngine.Object)null))
			{
				ManagedProperty managedProperty = null;
				if (PropertiesManager.cachedProperties.TryGetValue(propertyName, out managedProperty))
				{
					managedProperty.RemoveValueChangeListener(callback);
				}
				else
				{
					Debug.LogError("[PropertiesManager] Property not defined: " + propertyName);
				}
			}
		}

		/// <summary> Set the value of a managed property. The property must have a setter bind else this will fail. </summary>
		[IncludeInBlox]
		public static void SetValue(string name, object value)
		{
			if (PropertiesManager.cachedProperties == null)
			{
				Debug.LogError("PropertiesManager not ready for use.");
			}
			else
			{
				ManagedProperty managedProperty = null;
				if (PropertiesManager.cachedProperties.TryGetValue(name, out managedProperty))
				{
					if (managedProperty.setter == null)
					{
						Debug.LogError("[PropertiesManager.SetValue] Property does not have a setter bind: " + name);
					}
					else
					{
						managedProperty.Initialize();
						managedProperty.setter.Execute(value);
						if (managedProperty.runDuringBoot)
						{
							string value2 = BloxStringSerializer.Serialize(value);
							if (string.IsNullOrEmpty(value2))
							{
								PlayerPrefs.DeleteKey("PropertiesManager." + name);
							}
							else
							{
								PlayerPrefs.SetString("PropertiesManager." + name, value2);
							}
						}
					}
				}
				else
				{
					Debug.LogError("[PropertiesManager.SetValue] Property not defined: " + name);
				}
			}
		}

		/// <summary> Get value from a managed property. The property must have a getter bind else this will fail and return null. </summary>
		[IncludeInBlox]
		public static object GetValue(string name)
		{
			if (PropertiesManager.cachedProperties == null)
			{
				Debug.LogError("PropertiesManager not ready for use.");
				return null;
			}
			ManagedProperty managedProperty = null;
			if (PropertiesManager.cachedProperties.TryGetValue(name, out managedProperty))
			{
				if (managedProperty.getter == null)
				{
					Debug.LogError("[PropertiesManager.GetValue] Property does not have a getter bind: " + name);
					return null;
				}
				managedProperty.Initialize();
				return managedProperty.getter.GetValue();
			}
			Debug.LogError("[PropertiesManager.GetValue] Property not defined: " + name);
			return null;
		}

		/// <summary> Returns reference to a ManagedProperty. This reference should not be used to write values to the property if the value must be persisted. Rather use PropertiesManager.SetValue() </summary>
		public static ManagedProperty GetProperty(string name)
		{
			if (PropertiesManager.cachedProperties == null)
			{
				Debug.LogError("PropertiesManager not ready for use.");
				return null;
			}
			ManagedProperty managedProperty = null;
			if (PropertiesManager.cachedProperties.TryGetValue(name, out managedProperty))
			{
				managedProperty.Initialize();
				return managedProperty;
			}
			Debug.LogError("[PropertiesManager.GetProperty] Property not defined: " + name);
			return null;
		}
	}
}
