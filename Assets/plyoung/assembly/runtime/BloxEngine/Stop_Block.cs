namespace BloxEngine
{
	[BloxBlock("Flow/Stop", BloxBlockType.Action, Order = 320, Style = "red")]
	public class Stop_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			base.flowSig = BloxFlowSignal.Stop;
			return null;
		}
	}
}
