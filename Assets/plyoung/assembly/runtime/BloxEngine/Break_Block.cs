namespace BloxEngine
{
	[BloxBlock("Flow/Break", BloxBlockType.Action, Order = 320, Style = "red")]
	public class Break_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			base.flowSig = BloxFlowSignal.Break;
			return null;
		}
	}
}
