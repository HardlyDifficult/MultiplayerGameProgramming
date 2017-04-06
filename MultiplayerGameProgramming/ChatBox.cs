using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace HD
{
  public class ChatBox : MonoBehaviour
  {
    public Text text;

    public void Send()
    {
      UDPChat.instance.Send(text.text);
      TCPChat.instance.Send(text.text);
      text.text = "";
    }
  }
}
