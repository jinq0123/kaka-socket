# kaka-socket
Unity3D Socket 网络模块

最初来自: 小开KAKA [【Unity3D_常用模块】 Socket网络模块](https://blog.csdn.net/Claine/article/details/52374546)

## 更改

### 网络数据格式

按需要更改了网络数据格式，不再与原版本通用。长度不再包括自身的4个字节。
```
    //消息：消息长度(4byte) + 消息[数据类型(2byte) + 数据(N byte)]
    //消息长度为 2+N, 即不包括自身的4个字节
```

## 测试脚本

### `Client.cs` 测试内容
1.连接服务器。
2.断开服务器。
3.发送游戏事件。
4.以二进制方式发送网络消息。
5.以ProtuBuf方式发送网络消息。

### `Server.cs` 功能
1.收到的数据不做修改即可发送

### 目录说明
* Scripts/Socket/Protobuf：Protobuf源码
* Scripts/Socket：其他，Socket相关脚本。
* Scripts/ProtocalData: 存放*.proto 转换后的 *.cs 协议数据结构文件

备注：使用的是Protobuf的源码，如果后期有效率上的需求，可自行替换为DLL方式。（注意：.Net 2.0 的库和 .Net 2.0 Subset的选择使用。代码中只需要维护 SocketManager.cs 中的两个静态序列化相关函数即可）
