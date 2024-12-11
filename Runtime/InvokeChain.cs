/*
 * Author: CharSui
 * Created On: 2024.12.10
 * Description: 调用链的管理器，负责调用链的初始化和调用。
 * 调用链的使用法：
 * 1、定义你的InvokeChain的子类，这是调用链的执行器，由它负责Stage的初始化和彼此链接。声明你要用到的Stage也在这。如果你有使用Stage的priority属性也可以让其排序一次。
 * 2、定义你要用的InvokeChainStage，这是具体执行的部分。
 * 3、定义你要用的数据InvokeChainContext，这是调用链中途会修改的上下文以及提供你要你数据（不知道有没有办法拆开）
 * 4、对于需要外部传入的内容，比如如果是内购管线，那就是发起支付的产品，定义你的内容继承InvokeData，传入到InvokeChain里面去，进行Invoke
 * 5、等回调。
 */

using System;
using UnityEngine;

public abstract class InvokeChain<T> where T : InvokeChainContext, new()
{
	/// <summary>
	/// 是否有调用链正在执行
	/// 如果有并行运行需求，需要自己再去写一套，本逻辑通过类去作为上下文，在处理调用链的时候需要保持"唯一性"。
	/// </summary>
	private bool m_IsInvoking = false;
	
	private bool m_IsInited = false;

	protected readonly T context;
	
	protected InvokeChainStage<T>[] stages;
	
	/// <summary>
	/// 无论调用链是否成功，都会触发，需要自己在后处理里面根据里面的errorCode进行处理。
	/// </summary>
	public Action<T> invokeFinishCallback; 

	/// <summary>
	/// 构造函数中定义是否允许并发执行
	/// </summary>
	/// <param name="allowConcurrentExecution"></param>
	protected InvokeChain()
	{
		context = new T();
		
		StageInitialize();
	}

	private void StageInitialize()
	{
		m_IsInited = false;
		DefineStage();
		
		if (stages == null || stages.Length <= 0)
		{
			Debug.LogError($"[{GetType().Name}]There no stage in InvokeChain, initialize failed");
			return;
		}
		
		BuildStagesChain();
		m_IsInited = true;
	}

	/// <summary>
	/// 添加你要用到的Stage
	/// </summary>
	protected abstract void DefineStage();

	/// <summary>
	/// 调用链初始化进行回调注册。
	/// </summary>
	private void BuildStagesChain()
	{
		var count = stages.Length;

		// 将Stage串联起来 -> 这里有歧义，是否应该让stage知晓彼此的存在？不过其实也只是链成了单链表，是否考虑由
		for (int i = 1; i < count; i++)
		{
			var previousStage = stages[i-1];
			var currentStage = stages[i];
			
			if(previousStage == null || currentStage == null) continue;
			
			previousStage.SetNextStage(currentStage);
			
			// 确保每一个Stage都有CallBack且放在一个循环里面做。 -> 也可以单独给第一个Stage添加回调来减少后续对回调的修改。
			previousStage.SetInvokeCallBack(InvokeFinish);
			currentStage.SetInvokeCallBack(InvokeFinish);
		}
	}

	/// <summary>
	/// 开始触发调用链
	/// </summary>
	public void Invoke(InvokeData invokedata)
	{
		if (!m_IsInited)
		{
			Debug.LogError($"[{GetType().Name}]This InvokeChain have not inited");
			return;
		}

		if (m_IsInvoking)
		{
			Debug.LogError($"[{GetType().Name}]This InvokeChain is Invoking, check your logic");
			return;
		}
		
		context.Clear();
		
		// 将要原料数据填入
		ContextInitializeByInvokeData(invokedata);
		
		// 调用链开始运行
		StartInvoke();
	}

	/// <summary>
	/// 定义如何使用InvokeData对要使用的context进行初始化
	/// </summary>
	/// <param name="invokedata"></param>
	protected abstract void ContextInitializeByInvokeData(InvokeData invokedata);

	/// <summary>
	/// 开启调用链
	/// </summary>
	private void StartInvoke()
	{
		m_IsInvoking = true;
		stages[0].Invoke(context);
	}

	private void InvokeFinish()
	{
		Debug.Log($"[{GetType().Name}]Invoke Finish");
		m_IsInvoking = false;
		InvokeFinishImplementation(context);
	}

	protected virtual void InvokeFinishImplementation(T contextArg)
	{
		invokeFinishCallback?.Invoke(contextArg);
	}
}