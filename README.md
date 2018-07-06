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
