/*
 * Author: CharSui
 * Created On: 2024.12.11
 * Description: Demo示例,客户端给他算个五折
 */

public class ClientDiscountStage : InvokeChainStage<IAPInvokeChainContext>
{
	protected override void InvokeImplementation()
	{
		context.price /= 2f ;
		
		Finish();
		
		// 如果有报错,比如context.price <= 0, 那就填入报错信息
		// Finish(1, "Product Price is less than Zero.");
	}
}