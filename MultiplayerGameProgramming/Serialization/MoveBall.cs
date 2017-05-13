using System;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HD
{
  public class MoveBall : MonoBehaviour
  {
    public bool isForServerOnly;
    public MoveBall theOtherBall;
    public float percision = 1;
    const float min = -20;


    private void Update()
    {
      if(Globals.isServer && isForServerOnly || Globals.isServer == false && isForServerOnly == false)
      {
        if(Input.GetKey(KeyCode.RightArrow))
        {
          transform.position += new Vector3(.1f, 0, 0);
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
          transform.position -= new Vector3(.1f, 0, 0);
        }

        //{ // Approach 1 Serialize Vector
        //  SurrogateSelector surrogateSelector = new SurrogateSelector();
        //  StreamingContext context = new StreamingContext(StreamingContextStates.Persistence);
        //  BinaryFormatter formatter = new BinaryFormatter(surrogateSelector, context);
        //  surrogateSelector.AddSurrogate(typeof(Vector3), context, new Vector3Surrogate());

        //  byte[] data;
        //  using(MemoryStream stream = new MemoryStream())
        //  {
        //    formatter.Serialize(stream, transform.position);
        //    data = stream.GetBuffer();
        //  }

        //  theOtherBall.SendPositionUpdateViaSerialization(data);
        //}

        { // Approach 2 Byte/Bit stream with entropy bit
          byte[] data, buffer;
          using(MemoryStream stream = new MemoryStream())
          {
            // Compress the x position to ushort (16 bits)
            buffer = BitConverter.GetBytes(ConvertToFixed(transform.position.x));
            stream.Write(buffer, 0, buffer.Length);

            // Simply send the y position
            buffer = BitConverter.GetBytes(transform.position.y);
            stream.Write(buffer, 0, buffer.Length);

            // Considering entropy, replace z with a bool when possible
            if(Math.Abs(transform.position.z) < .001)
            {
              buffer = BitConverter.GetBytes(true); // Currently this is a byte, could be improved to consume only a bit
              stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
              buffer = BitConverter.GetBytes(false);
              stream.Write(buffer, 0, buffer.Length);
              buffer = BitConverter.GetBytes(transform.position.z);
              stream.Write(buffer, 0, buffer.Length);
            }

            data = stream.GetBuffer();
          }

          theOtherBall.SendPositionUpdateViaMemoryStream(data);
        }
      }
    }

    short ConvertToFixed(float value)
    {
      return (short)((value - min) / percision);
    }

    float ConvertFromFixed(short value)
    {
      return value * percision + min;
    }

    private void SendPositionUpdateViaSerialization(byte[] data)
    {
      SurrogateSelector surrogateSelector = new SurrogateSelector();
      StreamingContext context = new StreamingContext(StreamingContextStates.Persistence);
      BinaryFormatter formatter = new BinaryFormatter(surrogateSelector, context);
      surrogateSelector.AddSurrogate(typeof(Vector3), context, new Vector3Surrogate());

      object objectGraph;
      using(MemoryStream stream = new MemoryStream(data))
      {
        objectGraph = formatter.Deserialize(stream);
      }

      Vector3 newPosition = (Vector3)objectGraph;
      transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
    }

    private void SendPositionUpdateViaMemoryStream(byte[] data)
    {
      int currentOffset = 0; 
      short compressedX = (short)BitConverter.ToInt16(data, currentOffset);
      currentOffset += sizeof(short);
      float x = ConvertFromFixed(compressedX);

      float y = BitConverter.ToSingle(data, currentOffset);
      currentOffset += sizeof(float);

      bool isZ0 = BitConverter.ToBoolean(data, currentOffset);
      currentOffset += sizeof(bool);

      float z;
      if(isZ0)
      {
        z = 0;
      } else
      {
        z = BitConverter.ToSingle(data, currentOffset);
        currentOffset += sizeof(float);
      }

      transform.position = new Vector3(x, transform.position.y, z);
    }
  }
}
