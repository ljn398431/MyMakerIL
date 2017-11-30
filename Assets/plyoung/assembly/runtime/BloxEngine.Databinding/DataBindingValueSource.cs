using BloxEngine.Variables;
using System;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[Serializable]
	[ExcludeFromBlox]
	public class DataBindingValueSource
	{
		public enum Source
		{
			Blackboard = 0,
			DataProvider = 1
		}

		public Source source;

		public string blackboardValueName;

		public DataBinding databind;

		public DataBindingValueSource Copy()
		{
			return new DataBindingValueSource
			{
				source = this.source,
				blackboardValueName = this.blackboardValueName,
				databind = this.databind.Copy()
			};
		}

		public object GetValue(Component owner, object blackboardValue)
		{
			if (this.source == Source.Blackboard)
			{
				if ((UnityEngine.Object)owner == (UnityEngine.Object)null && this.blackboardValueName == "value")
				{
					return blackboardValue;
				}
				IplyVariablesOwner iplyVariablesOwner = owner as IplyVariablesOwner;
				if (iplyVariablesOwner != null)
				{
					return iplyVariablesOwner.Variables().GetVarValue(this.blackboardValueName);
				}
				Debug.LogError("DataBinding owner does not seem to have a Blackboard.");
			}
			else if (this.source == Source.DataProvider)
			{
				if (this.databind != null)
				{
					return this.databind.GetValue();
				}
				Debug.LogError("No DataProvider supplied for binding value source.");
			}
			return null;
		}
	}
}
