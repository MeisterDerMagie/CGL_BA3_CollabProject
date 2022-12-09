//(c) copyright by Martin M. Klöckener
using System;

public static class NetworkEvents
{
    //NetworkManager.Singleton.OnClientConnectedCallback doesn't work smh...
    public static Action OnClientShutdown = delegate {  };
}