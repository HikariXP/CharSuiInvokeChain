/*
 * Author: CharSui
 * Created On: 2024.12.11
 * Description: Demo示例内购调用链
 */

public class IAPInvokeChain : InvokeChain<IAPInvokeChainContext>
{
	protected override void DefineStage()
	{
		stages = new InvokeChainStage<IAPInvokeChainContext>[]
		{
			new ServerPriceStage(),
			new ClientDiscountStage()
		};
	}

	protected override void ContextInitializeByInvokeData(InvokeData invokedata)
	{
		if (invokedata is IAPInvokeData iapInvokeData)
		{
			context.productName = iapInvokeData.productId;
		}
	}
}