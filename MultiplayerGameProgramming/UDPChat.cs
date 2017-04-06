using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

namespace HD
{
  public class UDPChat : MonoBehaviour
  {
    public static UDPChat instance;
    string message;

    public IPAddress ip;
    UdpClient udp;
    public Text text;

    IPEndPoint e;

    public void Awake()
    {
      instance = this;
      int port;
      if(ip == null)
      {
        port = 56789;
        e = new IPEndPoint(IPAddress.Any, port);
      }
      else
      {
        port = 56799;
        e = new IPEndPoint(ip, 56789);
      }
      udp = new UdpClient(port);

      udp.BeginReceive(OnReceive, null);
    }

    void OnReceive(IAsyncResult ar)
    {
      byte[] data = udp.EndReceive(ar, ref e);

      if(ip == null)
      {
        ip = e.Address;
        e = new IPEndPoint(ip, e.Port);
      }

      message = System.Text.Encoding.UTF8.GetString(data);
      udp.BeginReceive(OnReceive, null);
    }

    private void Update()
    {
      text.text = message;
    }

    public void Send(
      string message)
    {
      if(ip == null)
      {
        return;
      }

      byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
      udp.Send(data, data.Length, e);
    }
  }
}
