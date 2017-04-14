using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using UnityEngine;
using System.Net;

namespace HD
{
  public class TcpConnectedClient
  {
    #region Data
    /// <summary>
    /// For Clients, the connection to the server.
    /// For Servers, the connection to a client.
    /// </summary>
    readonly TcpClient connection;

    readonly byte[] readBuffer = new byte[5000];

    NetworkStream stream
    {
      get
      {
        return connection.GetStream();
      }
    }
    #endregion

    #region Init
    public TcpConnectedClient(TcpClient tcpClient)
    {
      this.connection = tcpClient;
      this.connection.NoDelay = true; // Disable Nagle's cache algorithm
      if(TCPChat.instance.isServer)
      { // Client is awaiting EndConnect
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
      }
    }

    internal void Close()
    {
      connection.Close();
    }
    #endregion

    #region Async Events
    void OnRead(IAsyncResult ar)
    {
      int length = stream.EndRead(ar);
      if(length <= 0)
      { // Connection closed
        TCPChat.instance.OnDisconnect(this);
        return;
      }

      string newMessage = System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);
      TCPChat.messageToDisplay += newMessage + Environment.NewLine;

      if(TCPChat.instance.isServer)
      {
        TCPChat.BroadcastChatMessage(newMessage);
      }
      
      stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    internal void EndConnect(IAsyncResult ar)
    {
      connection.EndConnect(ar);

      stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }
    #endregion

    #region API
    internal void Send(string message)
    {
      byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);

      stream.Write(buffer, 0, buffer.Length);
    }
    #endregion
  }
}
