using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

namespace HD
{
  public class ButtonClient : MonoBehaviour
  {
    public GameObject tcpClient, udp;

    public void Awake()
    {
      GuiHelpers.OpenStep1();
    }

    public void OnClick()
    {
      IPAddress ip = IPAddress.Parse(GameObject.Find("InputFieldIP").GetComponent<InputField>().text);

      udp.GetComponent<UDPChat>().ip = ip;
      tcpClient.GetComponent<TCPChat>().ip = ip;

      udp.SetActive(true);
      tcpClient.SetActive(true);

      GuiHelpers.OpenStep2();
    }
  }
}
