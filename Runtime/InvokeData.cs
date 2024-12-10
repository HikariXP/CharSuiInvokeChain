/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: CharSui
 * Created On: 2024.12.10
 * Description: 为了让调用链可以接收额外的信息进行调用前初始化，定义此接口以让调用链依赖此接口。
 * 具体作用可以看继承类。目前贪图方便直接定义了空接口。如果想要更规范的使用，可以选择一种中间语言进行编码(Encode)和解码(Decode)，比如选Json
 */

public interface InvokeData
{
}