using System.IO;
using System;
using UnityEngine;

//常量数据
public class Constants
{
    //消息：消息长度(4byte) + 消息[数据类型(2byte) + 数据(N byte)]
    //消息长度为 2+N, 即不包括自身的4个字节
    public static int HEAD_MSG_LEN = 4;
    public static int HEAD_TYPE_LEN = 2;
    public static int HEAD_LEN//6byte
    {
        get { return HEAD_MSG_LEN + HEAD_TYPE_LEN; }
    }

    // 最大消息长度
    public static int MAX_MSG_LEN = 4 * 1024 * 1024;  // 4M
}

/// <summary>
/// 网络数据结构
/// </summary>
[System.Serializable]
public struct sSocketData
{
    public byte[] _data;
    public UInt16 _protocallType;
}

/// <summary>
/// 网络数据缓存器，
/// </summary>
[System.Serializable]
public class DataBuffer
{//自动大小数据缓存器
    private int _minBuffLen;
    private byte[] _buff;
    private int _curBuffPosition;
    private UInt32 _msgLength = 0;  // dataLength + 2
    private UInt16 _protocalType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_minBuffLen">最小缓冲区大小</param>
    public DataBuffer(int _minBuffLen = 1024)
    {
        if (_minBuffLen <= 0)
        {
            this._minBuffLen = 1024;
        }
        else
        {
            this._minBuffLen = _minBuffLen;
        }
        _buff = new byte[this._minBuffLen];
    }

    /// <summary>
    /// 添加缓存数据
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_dataLen"></param>
    public void AddBuffer(byte[] _data, int _dataLen)
    {
        if (_dataLen > _buff.Length - _curBuffPosition)//超过当前缓存
        {
            byte[] _tmpBuff = new byte[_curBuffPosition + _dataLen];
            Array.Copy(_buff, 0, _tmpBuff, 0, _curBuffPosition);
            Array.Copy(_data, 0, _tmpBuff, _curBuffPosition, _dataLen);
            _buff = _tmpBuff;
            _tmpBuff = null;
        }
        else
        {
            Array.Copy(_data, 0, _buff, _curBuffPosition, _dataLen);
        }
        _curBuffPosition += _dataLen;//修改当前数据标记
    }

    /// <summary>
    /// 更新消息长度和类型
    /// </summary>
    public void UpdateMsgLengthAndType()
    {
        if (_curBuffPosition < Constants.HEAD_LEN)
        {
            return;
        }

        byte[] tmpMsgLen = new byte[Constants.HEAD_MSG_LEN];
        Array.Copy(_buff, 0, tmpMsgLen, 0, Constants.HEAD_MSG_LEN);
        _msgLength = BitConverter.ToUInt32(tmpMsgLen, 0);
        if (_msgLength > Constants.MAX_MSG_LEN)
        {
            throw new Exception("Message length is too large.");
        }
        if (_msgLength < Constants.HEAD_TYPE_LEN)
        {
            throw new Exception("Message length is too small.");
        }

        byte[] tmpProtocalType = new byte[Constants.HEAD_TYPE_LEN];
        Array.Copy(_buff, Constants.HEAD_MSG_LEN, tmpProtocalType, 0, Constants.HEAD_TYPE_LEN);
        _protocalType = BitConverter.ToUInt16(tmpProtocalType, 0);
    }

    /// <summary>
    /// 获取一条可用数据，返回值标记是否有数据
    /// </summary>
    /// <param name="_tmpSocketData"></param>
    /// <returns></returns>
    public bool GetData(out sSocketData _tmpSocketData)
    {
        _tmpSocketData = new sSocketData();

        if (_msgLength == 0)
        {
            UpdateMsgLengthAndType();
            if (_msgLength == 0)
            {
                return false;
            }
        }

        if (_curBuffPosition < _msgLength + Constants.HEAD_MSG_LEN)
        {
            return false;
        }

        int dataLength = (int)_msgLength - Constants.HEAD_TYPE_LEN;
        _tmpSocketData._protocallType = _protocalType;
        _tmpSocketData._data = new byte[dataLength];
        Array.Copy(_buff, Constants.HEAD_LEN, _tmpSocketData._data, 0, dataLength);
        int doneLength = (int)_msgLength + Constants.HEAD_MSG_LEN;
        _curBuffPosition -= doneLength;
        byte[] _tmpBuff = new byte[_curBuffPosition < _minBuffLen ? _minBuffLen : _curBuffPosition];
        Array.Copy(_buff, doneLength, _tmpBuff, 0, _curBuffPosition);
        _buff = _tmpBuff;

        _msgLength = 0;
        _protocalType = 0;
        return true;
    }  // GetData()
}
