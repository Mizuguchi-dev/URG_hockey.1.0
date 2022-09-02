using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using SCIP_library;

public class URGSensor
{
    private TcpClient urg;
    private long time_stamp;
    NetworkStream stream;
    public List<long> distances;

    // Start is called before the first frame update
    public URGSensor(string _ip_address = "192.168.1.199", int _port_number = 10940)
    {
        const int start_step = 0;
        const int end_step = 1080;

        try
        {
            string ip_address = _ip_address;
            int port_number = _port_number;
 
            urg = new TcpClient();
            urg.Connect(ip_address, port_number);
            stream = urg.GetStream();

            write(stream, SCIP_Writer.SCIP2());
            read_line(stream); // ignore echo back
            write(stream, SCIP_Writer.MD(start_step, end_step));
            read_line(stream);  // ignore echo back

            distances = new List<long>();
            time_stamp = 0;

            read_data();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log(ex.StackTrace);
        }
        finally
        {
        }
    }

    // Update is called once per frame
    public void read_data()
    {
        string receive_data = read_line(stream);
 
        if (!SCIP_Reader.MD(receive_data, ref time_stamp, ref distances))
        {
            Debug.Log(receive_data);
        }
        if (distances.Count == 0)
        {
            Debug.Log(receive_data);
        }
        // show distance data
//        Debug.Log("time stamp: " + time_stamp.ToString() + " distance : " + distances.ToString());
    }

    public void closeStream()
    {
        write(stream, SCIP_Writer.QT());    // stop measurement mode
        read_line(stream); // ignore echo back
        stream.Close();
        urg.Close();
    }

    /// <summary>
    /// Read to "\n\n" from NetworkStream
    /// </summary>
    /// <returns>receive data</returns>
    static string read_line(NetworkStream stream)
    {
        if (stream.CanRead)
        {
            StringBuilder sb = new StringBuilder();
            bool is_NL2 = false;
            bool is_NL = false;
            do
            {
                char buf = (char)stream.ReadByte();
                if (buf == '\n')
                {
                    if (is_NL)
                    {
                        is_NL2 = true;
                    }
                    else
                    {
                        is_NL = true;
                    }
                }
                else
                {
                    is_NL = false;
                }
                sb.Append(buf);
            } while (!is_NL2);

            return sb.ToString();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// write data
    /// </summary>
    static bool write(NetworkStream stream, string data)
    {
        if (stream.CanWrite)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            return true;
        }
        else
        {
            return false;
        }
    }

}
