using BloxEngine;
using BloxEngine.Databinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Dropdown")]
	public class UIDropdownUpdater : UIElementUpdater
	{
		public DataBinding labelsSource = new DataBinding();

		[SerializeField]
		private Dropdown target;

		private bool ignore;

		protected void Reset()
		{
			this.target = base.GetComponent<Dropdown>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<Dropdown>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UIDropdownUpdater] Could not find any Dropdown component on the GameObject.", base.gameObject);
					return;
				}
			}
			this.target.ClearOptions();
			this.target.onValueChanged.AddListener(this.OnTargetUIValueChange);
			base.Awake();
		}

		protected override void OnEnable()
		{
			if ((UnityEngine.Object)this.target != (UnityEngine.Object)null)
			{
				this.ignore = true;
				this.UpdateOptions();
				this.ignore = false;
			}
			base.OnEnable();
		}

		protected override void OnValueChanged()
		{
			if (!((UnityEngine.Object)this.target == (UnityEngine.Object)null))
			{
				this.ignore = true;
				if (base.getter != null)
				{
					try
					{
						int num = (int)base.getter.Value;
						if (num < 0)
						{
							num = 0;
						}
						if (num >= this.target.options.Count)
						{
							num = this.target.options.Count - 1;
						}
						this.target.value = num;
					}
					catch (Exception exception)
					{
						Debug.LogError("[UIDropdownUpdater] The managed property's getter must return an Integer value.", base.gameObject);
						Debug.LogException(exception);
					}
				}
				this.ignore = false;
			}
		}

		private void UpdateOptions()
		{
			this.target.ClearOptions();
			object value = this.labelsSource.GetValue();
			if (value == null)
			{
				Debug.LogError("[UIDropdownUpdater] The Dropdown Options could not be set since the Options Source returned null.");
			}
			else
			{
				Array array = value as Array;
				if (array != null)
				{
					List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
					for (int i = 0; i < array.Length; i++)
					{
						object value2 = array.GetValue(i);
						string text = (value2 != null) ? value2.ToString() : null;
						if (text == null)
						{
							text = "-invalid-";
						}
						list.Add(new Dropdown.OptionData(text));
					}
					this.target.AddOptions(list);
				}
				else
				{
					IList list2 = value as IList;
					if (list2 != null)
					{
						List<Dropdown.OptionData> list3 = new List<Dropdown.OptionData>();
						for (int j = 0; j < list2.Count; j++)
						{
							object obj = list2[j];
							string text2 = (obj != null) ? obj.ToString() : null;
							if (text2 == null)
							{
								text2 = "-invalid-";
							}
							list3.Add(new Dropdown.OptionData(text2));
						}
						this.target.AddOptions(list3);
					}
					else
					{
						Debug.LogError("[UIDropdownUpdater] The Dropdown Options could not be set since the Options Source did not return a List or Array of String values.");
					}
				}
			}
		}

		private void OnTargetUIValueChange(int value)
		{
			if (!this.ignore)
			{
				PropertiesManager.SetValue(base.sourceTargetName, value);
			}
		}
	}
}
