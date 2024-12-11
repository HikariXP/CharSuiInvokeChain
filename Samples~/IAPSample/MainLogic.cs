/*
 * Author: CharSui
 * Created On: 2024.12.11
 * Description: Demo模拟逻辑外部调用
 */

using UnityEngine;

public class MainLogic : MonoBehaviour
{
	public void Start()
	{
		var iapInvokeChain = new IAPInvokeChain();
		var iapInvokeData = new IAPInvokeData(){productId = "CharSui"};
		iapInvokeChain.invokeFinishCallback = Callback;
		iapInvokeChain.Invoke(iapInvokeData);
	}

	private void Callback(IAPInvokeChainContext iapInvokeChainContext)
	{
		Debug.Log($"IAPData : productIds = {iapInvokeChainContext.productName}, price = {iapInvokeChainContext.price}");
	}
}
