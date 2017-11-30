using BloxEngine.Databinding;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	public class DataProviderEd : plyCustomEd
	{
		public class SimpleParamInfo
		{
			public string name;

			public string typeName;

			public Type type;
		}

		public static plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute> factory = new plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute>();

		private static Dictionary<Type, DataProviderEd> _editors = null;

		public static List<DataProvider> getterProviders = null;

		public static List<DataProvider> setterProviders = null;

		private static GUIContent[] getterProviderLabels = new GUIContent[0];

		private static GUIContent[] setterProviderLabels = new GUIContent[0];

		private static GUIContent GC_EdButton = new GUIContent(Ico._rename, "Edit or create new");

		private static Dictionary<Type, DataProviderEd> editors
		{
			get
			{
				if (DataProviderEd._editors == null)
				{
					DataProviderEd._editors = plyCustomEd.CreateCustomEditorsDict<DataProviderEd>(typeof(plyCustomEdAttribute));
				}
				return DataProviderEd._editors;
			}
		}

		public static T CreateDataprovider<T>(string ident, bool isSetter) where T : DataProvider
		{
			return (T)DataProviderEd.CreateDataprovider(typeof(T), ident, isSetter);
		}

		public static DataProvider CreateDataprovider(Type t, string ident, bool isSetter)
		{
			plyEdUtil.CheckPath(BloxEdGlobal.ProvidersPath);
			string str = plyEdUtil.GenerateUniqueFileGUID("", BloxEdGlobal.ProvidersPath, ".asset");
			DataProvider dataProvider = (DataProvider)plyEdUtil.LoadOrCreateAsset(t, BloxEdGlobal.ProvidersPath + str + ".asset", false);
			dataProvider.ident = ident;
			dataProvider.isSetter = isSetter;
			dataProvider.OnCreated();
			EditorUtility.SetDirty(dataProvider);
			AssetDatabase.SaveAssets();
			if (!string.IsNullOrEmpty(ident))
			{
				DataProviderEd.LoadDefinedProviders();
				if (isSetter)
				{
					DataProviderEd.setterProviders.Add(dataProvider);
				}
				else
				{
					DataProviderEd.getterProviders.Add(dataProvider);
				}
				DataProviderEd.UpdateProviderLabels();
			}
			return dataProvider;
		}

		public static void DestroyDataprovider(DataProvider p)
		{
			if (!((UnityEngine.Object)p == (UnityEngine.Object)null))
			{
				DataProviderEd.LoadDefinedProviders();
				List<DataProvider> dataProvidersForDestruction = p.GetDataProvidersForDestruction();
				dataProvidersForDestruction.Add(p);
				for (int i = 0; i < dataProvidersForDestruction.Count; i++)
				{
					if (dataProvidersForDestruction[i].isSetter)
					{
						DataProviderEd.setterProviders.Remove(dataProvidersForDestruction[i]);
					}
					else
					{
						DataProviderEd.getterProviders.Remove(dataProvidersForDestruction[i]);
					}
					plyEdUtil.DeleteAsset(dataProvidersForDestruction[i]);
				}
				DataProviderEd.UpdateProviderLabels();
			}
		}

		public static string GetDataBindingLabel(DataProvider p)
		{
			if ((UnityEngine.Object)p == (UnityEngine.Object)null)
			{
				return "None";
			}
			if (!string.IsNullOrEmpty(p.ident))
			{
				return p.ident;
			}
			DataProviderEd dataProviderEd = DataProviderEd.editors[p.GetType()];
			if (dataProviderEd != null)
			{
				return dataProviderEd.Label(p);
			}
			return "-unknown-dataprovider-";
		}

		public void DrawEditor(Rect rect, DataProvider target, bool isSetter)
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.Draw(rect, target, isSetter);
			if (GUI.changed)
			{
				target._SetDirty();
				plyEdUtil.SetDirty(target);
			}
			GUI.changed = changed;
		}

		public static SimpleParamInfo[] GetBindParams(DataBinding bind)
		{
			if ((UnityEngine.Object)bind.dataprovider != (UnityEngine.Object)null)
			{
				DataProviderEd dataProviderEd = DataProviderEd.editors[bind.dataprovider.GetType()];
				if (dataProviderEd != null)
				{
					return dataProviderEd.GetSetterParams(bind.dataprovider);
				}
			}
			return new SimpleParamInfo[0];
		}

		public static DataBindingValueSource DataBindingValueSourceField(Rect rect, GUIContent label, DataBindingValueSource val, GUIContent bindWinLabel, UnityEngine.Object ownerObj)
		{
			if (val == null)
			{
				val = new DataBindingValueSource();
				GUI.changed = true;
			}
			Rect rect2 = rect;
			if (label != null)
			{
				rect2.width = EditorGUIUtility.labelWidth;
				GUI.Label(rect2, label);
				rect2.x += EditorGUIUtility.labelWidth;
				rect2.width = rect.width - EditorGUIUtility.labelWidth;
			}
			float num = (float)((rect2.width - 2.0) / 2.0);
			rect2.width = num;
			val.source = (DataBindingValueSource.Source)EditorGUI.EnumPopup(rect2, (Enum)(object)val.source);
			rect2.x = (float)(rect2.x + num + 2.0);
			if (val.source == DataBindingValueSource.Source.Blackboard)
			{
				if (val.databind != null)
				{
					DataProviderEd.DestroyDataprovider(val.databind.dataprovider);
					val.databind = null;
				}
				val.blackboardValueName = EditorGUI.TextArea(rect2, val.blackboardValueName);
			}
			else if (val.source == DataBindingValueSource.Source.DataProvider)
			{
				if (val.databind == null)
				{
					val.databind = new DataBinding();
				}
				DataProviderEd.DataBindingField(rect2, null, val.databind, ownerObj, false, null);
			}
			return val;
		}

		public static void DataBindingField(Rect r, GUIContent label, DataBinding bind, UnityEngine.Object bindOwner, bool isForSetter, Type[] limitTypes = null)
		{
			DataProviderEd.LoadDefinedProviders();
			int selectedIndex = (!((UnityEngine.Object)bind.dataprovider == (UnityEngine.Object)null)) ? (isForSetter ? DataProviderEd.setterProviders.IndexOf(bind.dataprovider) : DataProviderEd.getterProviders.IndexOf(bind.dataprovider)) : 0;
			r.width -= 25f;
			if ((UnityEngine.Object)bind.dataprovider != (UnityEngine.Object)null && string.IsNullOrEmpty(bind.dataprovider.ident))
			{
				if (label == null)
				{
					if (GUI.Button(r, DataProviderEd.GetDataBindingLabel(bind.dataprovider), EditorStyles.miniButtonLeft))
					{
						DataBindingWindow.Show_DataBindingWindow(bind, bindOwner, isForSetter, limitTypes);
					}
				}
				else if (plyEdGUI.LabelButton(r, label, DataProviderEd.GetDataBindingLabel(bind.dataprovider), EditorStyles.miniButtonLeft))
				{
					DataBindingWindow.Show_DataBindingWindow(bind, bindOwner, isForSetter, limitTypes);
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				selectedIndex = ((label != null) ? EditorGUI.Popup(r, label, selectedIndex, isForSetter ? DataProviderEd.setterProviderLabels : DataProviderEd.getterProviderLabels, EditorStyles.miniButtonLeft) : EditorGUI.Popup(r, selectedIndex, isForSetter ? DataProviderEd.setterProviderLabels : DataProviderEd.getterProviderLabels, EditorStyles.miniButtonLeft));
				if (EditorGUI.EndChangeCheck())
				{
					bind.dataprovider = (isForSetter ? DataProviderEd.setterProviders[selectedIndex] : DataProviderEd.getterProviders[selectedIndex]);
				}
			}
			r.x += r.width;
			r.width = 25f;
			if (GUI.Button(r, DataProviderEd.GC_EdButton, plyEdGUI.Styles.MiniButtonRight))
			{
				DataBindingWindow.Show_DataBindingWindow(bind, bindOwner, isForSetter, limitTypes);
			}
		}

		public static void DataBindingField(GUIContent label, DataBinding bind, UnityEngine.Object bindOwner, bool isForSetter, Type[] limitTypes = null)
		{
			DataProviderEd.LoadDefinedProviders();
			int selectedIndex = (!((UnityEngine.Object)bind.dataprovider == (UnityEngine.Object)null)) ? (isForSetter ? DataProviderEd.setterProviders.IndexOf(bind.dataprovider) : DataProviderEd.getterProviders.IndexOf(bind.dataprovider)) : 0;
			EditorGUILayout.BeginHorizontal();
			if ((UnityEngine.Object)bind.dataprovider != (UnityEngine.Object)null && string.IsNullOrEmpty(bind.dataprovider.ident))
			{
				if (label == null)
				{
					if (GUILayout.Button(DataProviderEd.GetDataBindingLabel(bind.dataprovider), plyEdGUI.Styles.MiniButtonLeft))
					{
						DataBindingWindow.Show_DataBindingWindow(bind, bindOwner, isForSetter, limitTypes);
					}
				}
				else if (plyEdGUI.LabelButton(label, DataProviderEd.GetDataBindingLabel(bind.dataprovider), plyEdGUI.Styles.MiniButtonLeft))
				{
					DataBindingWindow.Show_DataBindingWindow(bind, bindOwner, isForSetter, limitTypes);
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				selectedIndex = ((label != null) ? EditorGUILayout.Popup(label, selectedIndex, isForSetter ? DataProviderEd.setterProviderLabels : DataProviderEd.getterProviderLabels, plyEdGUI.Styles.MiniButtonLeft) : EditorGUILayout.Popup(selectedIndex, isForSetter ? DataProviderEd.setterProviderLabels : DataProviderEd.getterProviderLabels, plyEdGUI.Styles.MiniButtonLeft));
				if (EditorGUI.EndChangeCheck())
				{
					bind.dataprovider = (isForSetter ? DataProviderEd.setterProviders[selectedIndex] : DataProviderEd.getterProviders[selectedIndex]);
				}
			}
			if (GUILayout.Button(DataProviderEd.GC_EdButton, plyEdGUI.Styles.MiniButtonRight, GUILayout.Width(25f)))
			{
				DataBindingWindow.Show_DataBindingWindow(bind, bindOwner, isForSetter, limitTypes);
			}
			EditorGUILayout.EndHorizontal();
		}

		public static void LoadDefinedProviders()
		{
			if (DataProviderEd.getterProviders != null && DataProviderEd.setterProviders != null)
				return;
			DataProviderEd.getterProviders = new List<DataProvider>();
			DataProviderEd.setterProviders = new List<DataProvider>();
			plyEdUtil.CheckPath(BloxEdGlobal.ProvidersPath);
			List<DataProvider> list = plyEdUtil.LoadAssets<DataProvider>(BloxEdGlobal.ProvidersPath);
			for (int i = 0; i < list.Count; i++)
			{
				if (!string.IsNullOrEmpty(list[i].ident))
				{
					if (list[i].isSetter)
					{
						DataProviderEd.setterProviders.Add(list[i]);
					}
					else
					{
						DataProviderEd.getterProviders.Add(list[i]);
					}
				}
			}
			DataProviderEd.getterProviders.Insert(0, null);
			DataProviderEd.setterProviders.Insert(0, null);
			DataProviderEd.UpdateProviderLabels();
		}

		public static void UpdateProviderLabels()
		{
			DataProviderEd.LoadDefinedProviders();
			DataProviderEd.getterProviders.Sort(delegate(DataProvider a, DataProvider b)
			{
				if (!((UnityEngine.Object)a == (UnityEngine.Object)null))
				{
					if (!((UnityEngine.Object)b == (UnityEngine.Object)null))
					{
						return a.ident.CompareTo(b.ident);
					}
					return 1;
				}
				return -1;
			});
			DataProviderEd.setterProviders.Sort(delegate(DataProvider a, DataProvider b)
			{
				if (!((UnityEngine.Object)a == (UnityEngine.Object)null))
				{
					if (!((UnityEngine.Object)b == (UnityEngine.Object)null))
					{
						return a.ident.CompareTo(b.ident);
					}
					return 1;
				}
				return -1;
			});
			DataProviderEd.getterProviderLabels = new GUIContent[DataProviderEd.getterProviders.Count];
			DataProviderEd.setterProviderLabels = new GUIContent[DataProviderEd.setterProviders.Count];
			for (int i = 0; i < DataProviderEd.getterProviders.Count; i++)
			{
				if ((UnityEngine.Object)DataProviderEd.getterProviders[i] == (UnityEngine.Object)null)
				{
					DataProviderEd.getterProviderLabels[i] = new GUIContent("None");
				}
				else
				{
					DataProviderEd.getterProviderLabels[i] = new GUIContent(DataProviderEd.getterProviders[i].ident);
				}
			}
			for (int j = 0; j < DataProviderEd.setterProviders.Count; j++)
			{
				if ((UnityEngine.Object)DataProviderEd.setterProviders[j] == (UnityEngine.Object)null)
				{
					DataProviderEd.setterProviderLabels[j] = new GUIContent("None");
				}
				else
				{
					DataProviderEd.setterProviderLabels[j] = new GUIContent(DataProviderEd.setterProviders[j].ident);
				}
			}
		}

		public virtual string Label(DataProvider target)
		{
			return base.nfo.Name;
		}

		public virtual float EditorHeight(DataProvider target, bool isSetter)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		protected virtual void Draw(Rect rect, DataProvider target, bool isSetter)
		{
		}

		public virtual SimpleParamInfo[] GetSetterParams(DataProvider target)
		{
			return new SimpleParamInfo[0];
		}
	}
}
