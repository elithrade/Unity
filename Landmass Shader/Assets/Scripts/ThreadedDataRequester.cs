using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadedDataRequester : MonoBehaviour
{
    private static ThreadedDataRequester _instance;

    private void Awake()
    {
        _instance = FindObjectOfType<ThreadedDataRequester>();
        _pendingDataQueue = new Queue<MapThreadInfo>();
    }

    private Queue<MapThreadInfo> _pendingDataQueue;

    private void Update()
    {
        while (_pendingDataQueue.Count > 0)
        {
            MapThreadInfo info = _pendingDataQueue.Dequeue();
            info.InvokeCallback();
        }
    }

    public static void RequestData(Func<object> requestData, Action<object> onReceiveData)
    {
        ThreadStart requestDataThread = new ThreadStart(() => _instance.DataThread(requestData, onReceiveData));
        new Thread(requestDataThread).Start();
    }

    private void DataThread(Func<object> getData, Action<object> onMapData)
    {
        object data = getData();
        lock (_pendingDataQueue)
        {
            _pendingDataQueue.Enqueue(new MapThreadInfo(data, onMapData));
        }
    }
}
