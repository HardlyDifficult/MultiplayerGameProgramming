using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace HD
{
  public class Vector3Surrogate : ISerializationSurrogate
  {
    void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
      Vector3 vector = (Vector3)obj;
      info.AddValue("x", vector.x);
      info.AddValue("y", vector.y);
      info.AddValue("z", vector.z);
    }

    object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
      float x = info.GetSingle("x");
      float y = info.GetSingle("y");
      float z = info.GetSingle("z");
      return new Vector3(x, y, z);
    }
  }
}
