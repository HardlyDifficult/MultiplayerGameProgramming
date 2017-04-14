using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace HD
{
  /// Multithreading (may lead to a lot of mostly-idle threads), 
  /// non-blocking, 
  /// select (rec select or non-blocking on single thread)

  /// DNS
  //IPHostEntry ipHost = Dns.Resolve("google.com");
  //ipHost.AddressList[0];

  /// Select
  //List<Socket> listOfSocketsToCheckForRead = new List<Socket>();
  //listOfSocketsToCheckForRead.Add(tcpClient.Client);
  //Socket.Select(listOfSocketsToCheckForRead, null, null, 0);
  //for(int i = 0; i < listOfSocketsToCheckForRead.Count; i++)
  //{
  //  listOfSocketsToCheckForRead[i].Receive(...);
  //}

  public class TCPChat : MonoBehaviour
  {
    #region Data
    public static TCPChat instance;

    public bool isServer;

    /// <summary>
    /// IP for clients to connect to. Null if you are the server.
    /// </summary>
    public IPAddress serverIp;

    /// <summary>
    /// For Clients, there is only one and it's the connection to the server.
    /// For Servers, there are many - one per connected client.
    /// </summary>
    List<TcpConnectedClient> clientList = new List<TcpConnectedClient>();

    /// <summary>
    /// The string to render in Unity.
    /// </summary>
    public static string messageToDisplay;
    public Text text;

    /// <summary>
    /// Accepts new connections.  Null for clients.
    /// </summary>
    TcpListener listener;
    #endregion

    #region Unity Events
    public void Awake()
    {
      instance = this;
      
      if(serverIp == null)
      { // Server: start listening for connections
        this.isServer = true;
        listener = new TcpListener(localaddr: IPAddress.Any, port: Globals.port);
        listener.Start();
        listener.BeginAcceptTcpClient(OnServerConnect, null);
      }
      else
      { // Client: try connecting to the server
        TcpClient client = new TcpClient();
        TcpConnectedClient connectedClient = new TcpConnectedClient(client);
        clientList.Add(connectedClient);
        client.BeginConnect(serverIp, Globals.port, (ar) => connectedClient.EndConnect(ar), null);
      }
    }

    protected void OnApplicationQuit()
    {
      listener?.Stop();
      for(int i = 0; i < clientList.Count; i++)
      {
        clientList[i].Close();
      }
    }

    protected void Update()
    {
      text.text = messageToDisplay;
    }
    #endregion

    #region Async Events
    void OnServerConnect(IAsyncResult ar)
    {
      TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
      clientList.Add(new TcpConnectedClient(tcpClient));
      listener.BeginAcceptTcpClient(OnServerConnect, null);
    }
    #endregion

    #region API
    public void OnDisconnect(TcpConnectedClient client)
    {
      clientList.Remove(client);
    }

    internal void Send(
      string message)
    {
      BroadcastChatMessage(message);

      if(isServer)
      {
        messageToDisplay += message + Environment.NewLine;
      }
    }

    internal static void BroadcastChatMessage(string message)
    {
      for(int i = 0; i < instance.clientList.Count; i++)
      {
        TcpConnectedClient client = instance.clientList[i];
        client.Send(message);
      }
    }
    #endregion
  }
}
