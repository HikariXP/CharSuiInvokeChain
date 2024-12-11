/*
 * Author: CharSui
 * Created On: 2024.12.10
 * Description: 调用链的上下文，
 */

public abstract class InvokeChainContext
{
	/// <summary>
	/// 通常是给研发看的错误信息
	/// </summary>
     public string errorMsg;
     
     /// <summary>
     /// 用于返还给调用链的中断处告知外部代码如何执行错误的情况，0代表没有问题发生
     /// </summary>
     public uint errorCode;
     
     /// <summary>
     /// 目前设计下，调用链本身只会持有一个数据引用类，所以在使用前需要对其进行清除。
     /// </summary>
     public abstract void Clear();
}
