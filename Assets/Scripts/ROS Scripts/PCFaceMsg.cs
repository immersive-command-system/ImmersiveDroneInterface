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
                Vert_x = new float[temp.Count];
                for (int i = 0; i < Vert_x.Length; i++)
                {
                    Vert_x[i] = temp[i].AsFloat;
                }
                
                temp = msg["vert_y"].AsArray;
                Vert_y = new float[temp.Count];
                for (int i = 0; i < Vert_y.Length; i++)
                {
                    Vert_y[i] = temp[i].AsFloat;
                }
                temp = msg["vert_z"].AsArray;
                Vert_z = new float[temp.Count];
                for (int i = 0; i < Vert_z.Length; i++)
                {
                    Vert_z[i] = temp[i].AsFloat;
                }
                String tempColor = msg["color_r"].Value;
                Color_r = Convert.FromBase64String(tempColor);
                tempColor = msg["color_b"].Value;
                Color_b = Convert.FromBase64String(tempColor);
                tempColor = msg["color_g"].Value;
                Color_g = Convert.FromBase64String(tempColor);
                tempColor = msg["color_a"].Value;
                Color_a = Convert.FromBase64String(tempColor);

                temp = msg["face_0"].AsArray;
                Face_0 = new UInt16[temp.Count];
                for (int i = 0; i < Face_0.Length; i++)
                {
                    Face_0[i] = (UInt16)temp[i].AsInt;
                }
                temp = msg["face_1"].AsArray;
                Face_1 = new UInt16[temp.Count];
                for (int i = 0; i < Face_1.Length; i++)
                {
                    Face_1[i] = (UInt16)temp[i].AsInt;
                }
                temp = msg["face_2"].AsArray;
                Face_2 = new UInt16[temp.Count];
                for (int i = 0; i < Face_2.Length; i++)
                {
                    Face_2[i] = (UInt16)temp[i].AsInt;
                }
            }

            public float[] Vert_x
            {
                get
                {
                    return vert_x;
                }

                set
                {
                    vert_x = value;
                }
            }

            public float[] Vert_y
            {
                get
                {
                    return vert_y;
                }

                set
                {
                    vert_y = value;
                }
            }

            public float[] Vert_z
            {
                get
                {
                    return vert_z;
                }

                set
                {
                    vert_z = value;
                }
            }

            public byte[] Color_r
            {
                get
                {
                    return color_r;
                }

                set
                {
                    color_r = value;
                }
            }

            public byte[] Color_g
            {
                get
                {
                    return color_g;
                }

                set
                {
                    color_g = value;
                }
            }

            public byte[] Color_b
            {
                get
                {
                    return color_b;
                }

                set
                {
                    color_b = value;
                }
            }

            public byte[] Color_a
            {
                get
                {
                    return color_a;
                }

                set
                {
                    color_a = value;
                }
            }

            public ushort[] Face_0
            {
                get
                {
                    return face_0;
                }

                set
                {
                    face_0 = value;
                }
            }

            public ushort[] Face_1
            {
                get
                {
                    return face_1;
                }

                set
                {
                    face_1 = value;
                }
            }

            public ushort[] Face_2
            {
                get
                {
                    return face_2;
                }

                set
                {
                    face_2 = value;
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