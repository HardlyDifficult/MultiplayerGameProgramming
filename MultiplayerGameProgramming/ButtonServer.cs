using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace HD
{
  public class ButtonServer : MonoBehaviour
  {
    public GameObject tcpServer, udp;

    public void OnClick()
    {
      tcpServer.SetActive(true);
      udp.SetActive(true);
      GuiHelpers.OpenStep2();
    }
  }
}
