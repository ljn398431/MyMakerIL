namespace BloxEngine
{
	[ExcludeFromBlox]
	public class BloxEventArg
	{
		public string name;

		public object val;

		public BloxEventArg(string name, object val)
		{
			this.name = name;
			this.val = val;
		}
	}
}
