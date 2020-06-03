using UnityEngine;
using SimpleJSON;
using System;

namespace ROSBridgeLib
{
    namespace rntools
    {
        public class PCFaceMsg: ROSBridgeMsg
        {
            std_msgs.HeaderMsg _header;
            float[] vert_x;
            float[] vert_y;
            float[] vert_z;
            byte[] color_r;
            byte[] color_g;
            byte[] color_b;
            byte[] color_a;
            UInt16[] face_0;
            UInt16[] face_1;
            UInt16[] face_2;

            public PCFaceMsg(JSONNode msg)
            {
                _header = new std_msgs.HeaderMsg(msg["header"]);
                JSONArray temp = msg["vert_x"].AsArray;
                vert_x = new float[temp.Count];
                for (int i = 0; i < vert_x.Length; i++)
                {
                    vert_x[i] = temp[i].AsFloat;
                }
                
                temp = msg["vert_y"].AsArray;
                vert_y = new float[temp.Count];
                for (int i = 0; i < vert_y.Length; i++)
                {
                    vert_y[i] = temp[i].AsFloat;
                }
                temp = msg["vert_z"].AsArray;
                vert_z = new float[temp.Count];
                for (int i = 0; i < vert_z.Length; i++)
                {
                    vert_z[i] = temp[i].AsFloat;
                }
                temp = msg["color_r"].AsArray;
                color_r = new byte[temp.Count];
                for (int i = 0; i < color_r.Length; i++)
                {
                    color_r[i] = (byte)temp[i].AsInt;
                }
                temp = msg["color_g"].AsArray;
                color_g = new byte[temp.Count];
                for (int i = 0; i < color_g.Length; i++)
                {
                    color_g[i] = (byte)temp[i].AsInt;
                }
                temp = msg["color_b"].AsArray;
                color_b = new byte[temp.Count];
                for (int i = 0; i < color_b.Length; i++)
                {
                    color_b[i] = (byte)temp[i].AsInt;
                }
                temp = msg["color_a"].AsArray;
                color_a = new byte[temp.Count];
                for (int i = 0; i < color_a.Length; i++)
                {
                    color_a[i] = (byte)temp[i].AsInt;
                }
                temp = msg["face_0"].AsArray;
                face_0 = new UInt16[temp.Count];
                for (int i = 0; i < face_0.Length; i++)
                {
                    face_0[i] = (UInt16)temp[i].AsInt;
                }
                temp = msg["face_1"].AsArray;
                face_1 = new UInt16[temp.Count];
                for (int i = 0; i < face_1.Length; i++)
                {
                    face_1[i] = (UInt16)temp[i].AsInt;
                }
                temp = msg["face_2"].AsArray;
                face_2 = new UInt16[temp.Count];
                for (int i = 0; i < face_2.Length; i++)
                {
                    face_2[i] = (UInt16)temp[i].AsInt;
                }
            }

            public static string getMessageType()
            {
                return "rntools/PCFace";
            }

            public std_msgs.HeaderMsg GetHeader()
            {
                return _header;
            }
        }
    }
}