using System;

public struct MapThreadInfo<T>
    where T : class
{
    public readonly Action<T> Callback;
    public readonly T Parameters;

    public MapThreadInfo(T parameters, Action<T> callback)
    {
        Parameters = parameters;
        Callback = callback;
    }

    public void InvokeCallback()
    {
        Callback(Parameters);
    }
}