/// <summary>
/// 网络消息处理中心
/// 
/// create at 2014.8.26 by sun
/// </summary>


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct sEvent_NetMessageData
{
    public UInt16 _eventType;
    public byte[] _eventData;
}

public delegate void Callback_NetMessage_Handle(byte[] _data);

public class MessageCenter : SingletonMonoBehaviour<MessageCenter>
{
    private Dictionary<UInt16, Callback_NetMessage_Handle> _netMessage_EventList = new Dictionary<UInt16, Callback_NetMessage_Handle>();
    public Queue<sEvent_NetMessageData> _netMessageDataQueue = new Queue<sEvent_NetMessageData>();

    //添加网络事件观察者
    public void addObsever(UInt16 _protocalType, Callback_NetMessage_Handle _callback)
    {
        if (_netMessage_EventList.ContainsKey(_protocalType))
        {
            _netMessage_EventList[_protocalType] += _callback;
        }
        else
        {
            _netMessage_EventList.Add(_protocalType, _callback);
        }
    }
    //删除网络事件观察者
    public void removeObserver(UInt16 _protocalType, Callback_NetMessage_Handle _callback)
    {
        if (_netMessage_EventList.ContainsKey(_protocalType))
        {
            _netMessage_EventList[_protocalType] -= _callback;
            if (_netMessage_EventList[_protocalType] == null)
            {
                _netMessage_EventList.Remove(_protocalType);
            }
        }
    }

    void Update()
    {
        while (_netMessageDataQueue.Count > 0)
        {
            lock (_netMessageDataQueue)
            {
                sEvent_NetMessageData tmpNetMessageData = _netMessageDataQueue.Dequeue();
                if (_netMessage_EventList.ContainsKey(tmpNetMessageData._eventType))
                {
                    _netMessage_EventList[tmpNetMessageData._eventType](tmpNetMessageData._eventData);
                }
            }
        }
    }
}
