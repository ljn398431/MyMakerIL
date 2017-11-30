using System;
using System.Reflection;

namespace BloxEditor
{
	public class BloxEventDef : BloxEdDef
	{
		public class Param
		{
			public string name;

			public Type type;

			public string typeName;
		}

		public string iconName;

		public MethodInfo methodNfo;

		public bool yieldAllowed;

		public Param[] pars = new Param[0];
	}
}
