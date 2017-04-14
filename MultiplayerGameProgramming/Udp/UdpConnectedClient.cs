using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace HD
{
  public class UdpConnectedClient
  {
    #region Data
    /// <summary>
    /// For Clients, the connection to the server.
    /// For Servers, the connection to a client.
    /// </summary>
    readonly UdpClient connection;
    #endregion

    #region Init
    public UdpConnectedClient(IPAddress ip = null)
    {
      if(UDPChat.instance.isServer)
      {
        connection = new UdpClient(Globals.port);
      }
      else
      {
        connection = new UdpClient(); // Auto-bind port
      }
      connection.BeginReceive(OnReceive, null);
    }

    public void Close()
    {
      connection.Close();
    }
    #endregion

    #region API
    void OnReceive(IAsyncResult ar)
    {
      try
      {
        IPEndPoint ipEndpoint = null;
        byte[] data = connection.EndReceive(ar, ref ipEndpoint);

        UDPChat.AddClient(ipEndpoint);

        string message = System.Text.Encoding.UTF8.GetString(data);
        UDPChat.messageToDisplay += message + Environment.NewLine;

        if(UDPChat.instance.isServer)
        {
          UDPChat.BroadcastChatMessage(message);
        }
      }
      catch(SocketException e)
      {
        // This happens when a client disconnects, as we fail to send to that port.
      }
      connection.BeginReceive(OnReceive, null);
    }

    internal void Send(string message, IPEndPoint ipEndpoint)
    {
      byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
      connection.Send(data, data.Length, ipEndpoint);
    }
    #endregion
  }
}
