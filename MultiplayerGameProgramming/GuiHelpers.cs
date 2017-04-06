using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace HD
{
  public static class GuiHelpers
  {
    static GameObject step1, step2;

    public static void OpenStep1()
    {
      step1 = GameObject.Find("Step1");
      step2 = GameObject.Find("Step2");

      step1.SetActive(true);
      step2.SetActive(false);
    }

    public static void OpenStep2()
    {
      step1.SetActive(false);
      step2.SetActive(true);
    }
  }
}
