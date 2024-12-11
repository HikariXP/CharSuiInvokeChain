/*
 * Author: CharSui
 * Created On: 2024.12.11
 * Description: Demo示范上下文
 */

public class IAPInvokeChainContext : InvokeChainContext
{
	// "CharSui"
	public string productName;
	
	// 20.00$
	public float price;
	
	public override void Clear()
	{
		productName = string.Empty;
		price = -0.01f;
	}
}