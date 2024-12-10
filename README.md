# CharSui's InvokeChain

叉烧的调用链。

定义了一套通用的调用链逻辑，当你的逻辑上有链式调用的时候，它可以帮到你去规范你的调用链。

# 适用范围

## 应用内购买

比如购买前，你需要知道你要买什么，这个产品的名字去问问服务器后台多少钱，本地判断有没有一些逻辑去修改或者做一些UI处理，你明确你这个购买流程，是逐步递进的，中间错一步就失败：比如获取不了服务器价格。

## 资源加载流程

资源加载前，确认要加载的资源，比如"charsui_texture"。

于是先确认是否符合命名规范，查询清单有没有这个资源，这个资源现在是本地有还是远端有，下载或者直接读取，返回资源本身给逻辑。

# 怎么使用

只有四个类，核心逻辑是两个类 : { InvokeChain 、 InvokeChainStage }，数据类是: { InvokeData、InvokeChainContext }

## InvokeChain 

最上层的调用管理，比如负责调用链的初始化，调用，设置一些调用前的处理比如传入字符串。InvokeChain也实际持有Stage数组，还有一个当前Chain使用的临时上下文（继承InvokeChainContext）

以下是你对其要做的：

- 新建一个自己的类，继承InvokeChain
- 重写DefineStage，在里面写入逻辑加入你写的具体的Stage到stages里
- 重写SetInvokeData，让一个类继承InvokeData，里面有你要的数据就行。
- 在要使用调用链的地方，新建此调用链，其会自动初始化
- 调用SetInvokeData，写入你要处理的原始数据
- 大部分情况下，你需要添加执行后的处理到invokeFinishCallback里面。可视你实际需求。
- 调用Invoke()

## InvokeChainStage 

具体调用链的链。InvokeChain会根据它所持有的Stage，按顺序逐个调用。



## 举例 - 游戏内购需求

### 需求背景

内购使用的是平台提供的内购，比如GooglePlay。需要从平台获取价格信息后，客户端固定给一个"五折"的优惠，然后再发起购买。在购买的开始和结尾分别有一个对于UI的调用，打开一个转圈圈的遮罩让玩家等待流程。

### 实现