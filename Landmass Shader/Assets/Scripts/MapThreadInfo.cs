using System;

public struct MapThreadInfo
{
    public readonly Action<object> Callback;
    public readonly object Parameters;

    public MapThreadInfo(object parameters, Action<object> callback)
    {
        Parameters = parameters;
        Callback = callback;
    }

    public void InvokeCallback()
    {
        Callback(Parameters);
    }
}