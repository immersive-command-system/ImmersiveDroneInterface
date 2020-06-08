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
                String tempColor = msg["color_r"].Value;
                color_r = Convert.FromBase64String(tempColor);
                tempColor = msg["color_b"].Value;
                color_b = Convert.FromBase64String(tempColor);
                tempColor = msg["color_g"].Value;
                color_g = Convert.FromBase64String(tempColor);
                tempColor = msg["color_a"].Value;
                color_a = Convert.FromBase64String(tempColor);

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