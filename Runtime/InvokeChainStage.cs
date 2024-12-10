/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: CharSui
 * Created On: 2024.12.10
 * Description: 调用链抽象。对于有调用链需求的行为可以继承这个去建立一条调用链
 * 对于同时调用的问题，需要自己根据自己的需求去评估，当前这套实现不考虑兼容"多次同时调用"，只做单例式的单任务，只有上一个任务结束了才可以执行下一次，所以也需要对所有错误情况做正确的处理，不然会卡住无法开启下一次调用。
 * 
 */

using System;
using UnityEngine;

public abstract class InvokeChainStage<T> : IComparable<InvokeChainStage<T>> where T : InvokeChainContext
{
	private int priority;

	/// <summary>
	/// 结束整条调用链的地方。如果报错，则在订单原型里面写错误信心，如果成功则直接调用即可
	/// 由OrderFactory管理，继承者只需要告知本身的职责已经Finish就行
	/// </summary>
	private Action m_ExcuteCallBack;

	/// <summary>
	/// 异步的需要用到，如果有方案可以优化
	/// </summary>
	protected T context;

	/// <summary>
	/// 下一个建造者，组成单链表
	/// 由OrderFactory管理，继承者不需要知道下一链是谁
	/// </summary>
	private InvokeChainStage<T> m_NextStage;

	/// <summary>
	/// 设置回调,目前设计上，InvokeChainStage只要执行完告诉InvokeChain就行，实际内容还是以InvokeChain内部存的为主，InvokeChain主要是对其进行修改。
	/// </summary>
	/// <param name="callBack"></param>
	public void SetInvokeCallBack(Action callBack)
	{
		m_ExcuteCallBack = callBack;
	}

	public void SetNextStage(InvokeChainStage<T> nextStage)
	{
		m_NextStage = nextStage;
	}

	public void Invoke(T contextFromInvokeChain)
	{
		context = contextFromInvokeChain;
		InvokeImplementation();
	}

	/// <summary>
	/// 具体的调用链实现
	/// </summary>
	protected abstract void InvokeImplementation();

	/// <summary>
	/// 有错误，无下家 = ExcuteCallBack
	/// 无错误，无下架 = ExcuteCallBack
	/// 有错误，有下家 = ExcuteCallBack
	/// 无所谓，有下家 = nextStage
	/// 构造结束
	///	这里也可以算作有重复代码，每个Builder都要激活Finish，但是有的builder是异步的，所以需要保持Builder自己去Invoke结束代码是有必要的
	/// </summary>
	/// <param name="errorCode">供UI使用的错误码</param>
	/// <param name="errorMsg">供开发者看的Log</param>
	protected void Finish(uint errorCode = 0, string errorMsg = null)
	{
		// 检查上下文信息，如果错误码不等于0，或者错误信息不为空，那么就是有错误
		context.errorMsg = errorMsg;
		context.errorCode = errorCode;

		var haveError = errorCode != 0 || !string.IsNullOrEmpty(errorMsg);

		// 如果没有错误，就会继续触发链接的下一个阶段
		if (!haveError && m_NextStage != null)
		{
			Debug.Log($"[{GetType().Name}]Stage excuted success, nextStage is :{m_NextStage.GetType().Name}");
			m_NextStage.Invoke(context);
			return;
		}

		if (haveError)
		{
			Debug.LogError($"[{GetType().Name}]Excute with error:{errorMsg}");
		}

		// 结束整个构造链，外部需要检查Context的有效性valid
		m_ExcuteCallBack?.Invoke();
	}

	/// <summary>
	/// 可根据Stage的优先度(priority)排序
	/// </summary>
	/// <param name="otherStage"></param>
	/// <returns></returns>
	public int CompareTo(InvokeChainStage<T> otherStage)
	{
		if (otherStage.priority < priority)
		{
			return -1;
		}

		return otherStage.priority > priority ? 1 : 0;
	}
}