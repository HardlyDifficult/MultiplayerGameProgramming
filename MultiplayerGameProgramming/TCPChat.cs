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
  /// <summary>
  /// TODO
  ///  - DNS
  ///  - Graceful shutdown
  ///  - Multiple clients (helps explain UDP vs TCP)
  ///  - Clean up code
  ///  - Select
  ///  - Tcp no Nagle
  /// 
  /// 
  /// Discussion points
  ///  - Multithreading, non-blocking, select (rec select or non-blocking on single thread)
  /// </summary>



  public class TCPChat : MonoBehaviour
  {
    public static TCPChat instance;
    public IPAddress ip;
    TcpListener listener;
    TcpClient client;
    byte[] readBuffer = new byte[6000];
    string message;
    public Text text;

    public void Awake()
    {
      instance = this;

        int port = 56789;
      if(ip == null)
      {
        listener = new TcpListener(IPAddress.Any, 56789);
        listener.Start();
        listener.BeginAcceptTcpClient(OnConnect, null);
      }
      else
      {
        client = new TcpClient();
        client.BeginConnect(ip, port, OnClientConnect, null);
      }
    }

    private void OnClientConnect(IAsyncResult ar)
    {
      client.EndConnect(ar);
      client.GetStream().BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    private void OnConnect(IAsyncResult ar)
    {
      client = listener.EndAcceptTcpClient(ar);
      client.GetStream().BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    private void OnRead(IAsyncResult ar)
    {
      int length = client.GetStream().EndRead(ar);
      message = System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);
      client.GetStream().BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }
    private void Update()
    {
      text.text = message;
    }

    internal void Send(
      string message)
    {
      if(client == null)
      {
        return;
      }

      byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
      client.GetStream().Write(buffer, 0, buffer.Length);
    }
  }
}
