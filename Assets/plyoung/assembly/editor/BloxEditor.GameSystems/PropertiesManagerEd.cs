using BloxEditor.Databinding;
using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class PropertiesManagerEd
	{
		private static PropertiesManager _propertiesManager;

		private static GUIContent[] _managedPropertyLabels;

		public static PropertiesManager PropertiesManager
		{
			get
			{
				if ((Object)PropertiesManagerEd._propertiesManager == (Object)null)
				{
					PropertiesManagerEd.Load();
				}
				return PropertiesManagerEd._propertiesManager;
			}
		}

		public static GUIContent[] PropertyLabels
		{
			get
			{
				if (PropertiesManagerEd._managedPropertyLabels == null)
				{
					PropertiesManagerEd.Load();
				}
				return PropertiesManagerEd._managedPropertyLabels;
			}
		}

		private static void Load()
		{
			plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot);
			PropertiesManagerEd._propertiesManager = plyEdUtil.LoadOrCreateAsset<PropertiesManager>(BloxEdGlobal.ResourcesRoot + "PropertiesManager.asset", true);
			PropertiesManagerEd._managedPropertyLabels = new GUIContent[PropertiesManagerEd._propertiesManager.properties.Count];
			for (int i = 0; i < PropertiesManagerEd._propertiesManager.properties.Count; i++)
			{
				PropertiesManagerEd._managedPropertyLabels[i] = new GUIContent(PropertiesManagerEd._propertiesManager.properties[i].propertyName);
			}
		}

		public static void AddProperty(string name)
		{
			if (string.IsNullOrEmpty(name) || !plyEdUtil.StringIsUnique(PropertiesManagerEd.PropertiesManager.properties, name))
			{
				EditorUtility.DisplayDialog("Properties Manager", "The new property name is not valid. It must be a unique name among all defined properties.", "OK");
			}
			else
			{
				ManagedProperty managedProperty = new ManagedProperty();
				managedProperty.propertyName = name;
				PropertiesManagerEd.PropertiesManager.properties.Add(managedProperty);
				plyEdUtil.SetDirty(PropertiesManagerEd.PropertiesManager);
				PropertiesManagerEd._managedPropertyLabels = new GUIContent[PropertiesManagerEd._propertiesManager.properties.Count];
				for (int i = 0; i < PropertiesManagerEd._propertiesManager.properties.Count; i++)
				{
					PropertiesManagerEd._managedPropertyLabels[i] = new GUIContent(PropertiesManagerEd._propertiesManager.properties[i].propertyName);
				}
			}
		}

		public static void RemoveProperty(int idx)
		{
			DataProviderEd.DestroyDataprovider(PropertiesManagerEd.PropertiesManager.properties[idx].getter.dataprovider);
			DataProviderEd.DestroyDataprovider(PropertiesManagerEd.PropertiesManager.properties[idx].setter.dataprovider);
			PropertiesManagerEd.PropertiesManager.properties.RemoveAt(idx);
			plyEdUtil.SetDirty(PropertiesManagerEd.PropertiesManager);
			PropertiesManagerEd._managedPropertyLabels = new GUIContent[PropertiesManagerEd._propertiesManager.properties.Count];
			for (int i = 0; i < PropertiesManagerEd._propertiesManager.properties.Count; i++)
			{
				PropertiesManagerEd._managedPropertyLabels[i] = new GUIContent(PropertiesManagerEd._propertiesManager.properties[i].propertyName);
			}
		}

		public static void RenameProperty(int idx, string newName)
		{
			if (!PropertiesManagerEd.PropertiesManager.properties[idx].propertyName.Equals(newName))
			{
				if (string.IsNullOrEmpty(newName) || !plyEdUtil.StringIsUnique(PropertiesManagerEd.PropertiesManager.properties, newName))
				{
					EditorUtility.DisplayDialog("Properties Manager", "The property name is not valid. It must be a unique name among all defined properties.", "OK");
				}
				else
				{
					PropertiesManagerEd.PropertiesManager.properties[idx].propertyName = newName;
					plyEdUtil.SetDirty(PropertiesManagerEd.PropertiesManager);
					PropertiesManagerEd.PropertyLabels[idx].text = newName;
				}
			}
		}

		public static ManagedProperty GetProperty(string name)
		{
			for (int i = 0; i < PropertiesManagerEd.PropertiesManager.properties.Count; i++)
			{
				if (PropertiesManagerEd.PropertiesManager.properties[i].propertyName == name)
				{
					return PropertiesManagerEd.PropertiesManager.properties[i];
				}
			}
			return null;
		}
	}
}
