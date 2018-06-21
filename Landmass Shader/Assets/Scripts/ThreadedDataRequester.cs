using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadedDataRequester : MonoBehaviour
{
    private static ThreadedDataRequester _instance;

    private void  Awake()
    {
        _instance = FindObjectOfType<ThreadedDataRequester>();
        _pendingDataQueue = new Queue<MapThreadInfo>();
    }

    private Queue<MapThreadInfo> _pendingDataQueue;

    private void Update()
    {
        Process(_pendingDataQueue);
    }

    private void Process(Queue<MapThreadInfo> queue)
    {
        // Locking inside Update method impacts performance a lot
        while (queue.Count > 0)
        {
            lock (queue)
            {
                MapThreadInfo info = queue.Dequeue();
                info.InvokeCallback();
            }
        }
    }

    public static void RequestData(Func<object> requestData, Action<object> onReceiveData)
    {
        ThreadStart requestDataThread = new ThreadStart(() => _instance.DataThread(requestData, onReceiveData));
        new Thread(requestDataThread).Start();
    }

    private void DataThread(Func<object> getData, Action<object> onMapData)
    {
        lock (_pendingDataQueue)
        {
            object data = getData();
            _pendingDataQueue.Enqueue(new MapThreadInfo(data, onMapData));
        }
    }
}
