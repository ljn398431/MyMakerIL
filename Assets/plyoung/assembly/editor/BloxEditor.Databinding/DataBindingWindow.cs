using BloxEngine.Databinding;
using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	public class DataBindingWindow : EditorWindow
	{
		private DataBinding bind;

		private UnityEngine.Object bindOwner;

		private bool isForSetter;

		private Type[] limitTypes;

		private bool accepted;

		private GUIContent[] providerLabels;

		private DataProviderEd providerEd;

		private int providerIdx;

		private bool isNewlyCreated;

		private static readonly GUIContent GC_Space = new GUIContent(" ");

		private static readonly GUIContent GC_Dataprovider = new GUIContent("Provider");

		private static readonly GUIContent GC_CreateNew = new GUIContent("Create New");

		private static readonly GUIContent GC_Rename = new GUIContent(Ico._rename, "Rename");

		private static readonly GUIContent GC_Delete = new GUIContent(Ico._delete, "Delete");

		private static readonly GUIContent GC_Text = new GUIContent();

		public static void Show_DataBindingWindow(DataBinding bind, UnityEngine.Object bindOwner, bool isForSetter, Type[] limitTypes)
		{
			DataBindingWindow dataBindingWindow = ScriptableObject.CreateInstance<DataBindingWindow>();
			dataBindingWindow.titleContent = new GUIContent("Data Binding");
			dataBindingWindow.bind = bind;
			dataBindingWindow.bindOwner = bindOwner;
			dataBindingWindow.isForSetter = isForSetter;
			dataBindingWindow.limitTypes = limitTypes;
			dataBindingWindow.Init();
			dataBindingWindow.minSize = new Vector2(300f, 200f);
			dataBindingWindow.ShowUtility();
		}

		protected void OnDestroy()
		{
		}

		private void Init()
		{
			List<plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute>.CustomEditorTypeInfo> list = new List<plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute>.CustomEditorTypeInfo>();
			foreach (plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute>.CustomEditorTypeInfo value in DataProviderEd.factory.editorInfos.Values)
			{
				if (!string.IsNullOrEmpty(value.editorNfo.Name) && (!this.isForSetter || value.editorNfo.opt == 1))
				{
					list.Add(value);
				}
			}
			list.Sort((plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute>.CustomEditorTypeInfo a, plyCustomEdFactory<DataProviderEd, plyCustomEdAttribute>.CustomEditorTypeInfo b) => a.editorNfo.Name.CompareTo(b.editorNfo.Name));
			this.providerLabels = new GUIContent[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				this.providerLabels[i] = new GUIContent(list[i].editorNfo.Name);
			}
			if ((UnityEngine.Object)this.bind.dataprovider != (UnityEngine.Object)null)
			{
				this.CreateProviderEd();
			}
		}

		protected void Update()
		{
			if (this.bind == null || this.bindOwner == (UnityEngine.Object)null)
			{
				base.Close();
			}
			if (this.accepted)
			{
				base.Close();
			}
		}

		protected void OnGUI()
		{
			EditorGUIUtility.labelWidth = 85f;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(DataBindingWindow.GC_Dataprovider, GUILayout.Width((float)(EditorGUIUtility.labelWidth - 4.0)));
			GUI.enabled = false;
			EditorGUILayout.TextField(((UnityEngine.Object)this.bind.dataprovider == (UnityEngine.Object)null) ? "" : this.bind.dataprovider.ident, plyEdGUI.Styles.TextField_NoRightMargin);
			GUI.enabled = ((UnityEngine.Object)this.bind.dataprovider != (UnityEngine.Object)null);
			if (GUILayout.Button(DataBindingWindow.GC_Rename, plyEdGUI.Styles.MiniButtonMid, GUILayout.Width(25f)))
			{
				plyTextInputWiz.ShowWiz("Rename Data Provider", "Enter a unique name", this.bind.dataprovider.ident, this.RenameDataProvider, null, 250f);
			}
			if (GUILayout.Button(DataBindingWindow.GC_Delete, plyEdGUI.Styles.MiniButtonMid, GUILayout.Width(25f)) && EditorUtility.DisplayDialog("Delete Data Provider", "Deleting the data provider can't be undone. Continue?", "Yes", "Cancel"))
			{
				DataProviderEd.DestroyDataprovider(this.bind.dataprovider);
				this.bind.dataprovider = null;
				this.providerEd = null;
				this.providerIdx = -1;
			}
			GUI.enabled = true;
			if (GUILayout.Button(DataBindingWindow.GC_CreateNew, plyEdGUI.Styles.MiniButtonRight, GUILayout.Width(80f)))
			{
				plyTextInputWiz.ShowWiz("New Data Provider", "Enter unique name or leave empty", "", this.AddDataProvider, null, 250f);
			}
			EditorGUILayout.EndHorizontal();
			if ((UnityEngine.Object)this.bind.dataprovider != (UnityEngine.Object)null)
			{
				this.DoDataProvider();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginHorizontal(plyEdGUI.Styles.BottomBar);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Accept", GUILayout.Width(80f)))
			{
				this.accepted = true;
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void DoDataProvider()
		{
			if (this.providerLabels == null)
			{
				base.Close();
				GUIUtility.ExitGUI();
			}
			else
			{
				if (this.isNewlyCreated)
				{
					EditorGUI.BeginChangeCheck();
					this.providerIdx = EditorGUILayout.Popup(DataBindingWindow.GC_Space, this.providerIdx, this.providerLabels);
					if (EditorGUI.EndChangeCheck())
					{
						string ident = this.bind.dataprovider.ident;
						DataProviderEd.DestroyDataprovider(this.bind.dataprovider);
						this.providerEd = DataProviderEd.factory.CreateEditor(this.providerLabels[this.providerIdx].text);
						this.bind.dataprovider = DataProviderEd.CreateDataprovider(this.providerEd.nfo.TargetType, ident, this.isForSetter);
						plyEdUtil.SetDirty(this.bindOwner);
					}
				}
				else
				{
					GUI.enabled = false;
					EditorGUILayout.Popup(DataBindingWindow.GC_Space, this.providerIdx, this.providerLabels);
					GUI.enabled = true;
				}
				DataBindingWindow.GC_Text.text = BloxEd.PrettyTypeName(this.bind.dataprovider.DataType(), !this.isForSetter);
				EditorGUILayout.LabelField(DataBindingWindow.GC_Space, DataBindingWindow.GC_Text);
				if (this.providerEd != null)
				{
					Rect rect = GUILayoutUtility.GetRect(0f, (float)(this.providerEd.EditorHeight(this.bind.dataprovider, this.isForSetter) + 10.0), GUILayout.ExpandWidth(true));
					rect.x += 5f;
					rect.width -= 10f;
					rect.y += 10f;
					rect.height -= 10f;
					this.providerEd.DrawEditor(rect, this.bind.dataprovider, this.isForSetter);
					base.Repaint();
				}
			}
		}

		private void AddDataProvider(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (string.IsNullOrEmpty(text))
			{
				this.isNewlyCreated = true;
				this.bind.dataprovider = DataProviderEd.CreateDataprovider<plyVarDataProvider>("", this.isForSetter);
				plyEdUtil.SetDirty(this.bindOwner);
				this.CreateProviderEd();
			}
			else if (this.NameIsUniqueInDefined(text))
			{
				this.isNewlyCreated = true;
				this.bind.dataprovider = DataProviderEd.CreateDataprovider<plyVarDataProvider>(text, this.isForSetter);
				plyEdUtil.SetDirty(this.bindOwner);
				this.CreateProviderEd();
			}
			else
			{
				EditorUtility.DisplayDialog("Create", "The name must be unique.", "OK");
			}
			base.Repaint();
		}

		private void RenameDataProvider(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text) && !text.Equals(this.bind.dataprovider.ident))
			{
				if (this.NameIsUniqueInDefined(text))
				{
					this.bind.dataprovider.ident = text;
					plyEdUtil.SetDirty(this.bind.dataprovider);
					DataProviderEd.UpdateProviderLabels();
				}
				else
				{
					EditorUtility.DisplayDialog("Rename", "The name must be unique.", "OK");
				}
				base.Repaint();
			}
		}

		private bool NameIsUniqueInDefined(string s)
		{
			DataProviderEd.LoadDefinedProviders();
			for (int i = 0; i < DataProviderEd.getterProviders.Count; i++)
			{
				if (!((UnityEngine.Object)DataProviderEd.getterProviders[i] == (UnityEngine.Object)null) && s.Equals(DataProviderEd.getterProviders[i].ident))
				{
					return false;
				}
			}
			for (int j = 0; j < DataProviderEd.setterProviders.Count; j++)
			{
				if (!((UnityEngine.Object)DataProviderEd.setterProviders[j] == (UnityEngine.Object)null) && s.Equals(DataProviderEd.setterProviders[j].ident))
				{
					return false;
				}
			}
			return true;
		}

		private void CreateProviderEd()
		{
			this.providerIdx = 0;
			this.providerEd = DataProviderEd.factory.CreateEditor(this.bind.dataprovider.GetType());
			if (this.providerEd != null)
			{
				int num = 0;
				while (true)
				{
					if (num < this.providerLabels.Length)
					{
						if (!(this.providerLabels[num].text == this.providerEd.nfo.Name))
						{
							num++;
							continue;
						}
						break;
					}
					return;
				}
				this.providerIdx = num;
			}
		}
	}
}
