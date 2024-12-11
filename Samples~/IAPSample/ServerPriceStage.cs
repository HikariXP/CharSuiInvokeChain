/*
 * Author: CharSui
 * Created On: 2024.12.11
 * Description: Demo示例,从服务器获取价格
 */

public class ServerPriceStage : InvokeChainStage<IAPInvokeChainContext>
{
	protected override void InvokeImplementation()
	{
		// 假设从服务器获取了价格
		context.price = 20.00f;
		
		Finish();
	}
}