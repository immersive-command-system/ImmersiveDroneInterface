﻿using UnityEngine;
using SimpleJSON;
using System;
using System.Runtime.CompilerServices;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace ROSBridgeLib
{
    namespace voxblox_msgs
    {
        public class MeshBlockMsg: ROSBridgeMsg
        {
            /// <summary>
            /// Index of meshed points in block map
            /// </summary>
            Int64[] _index;

            /// <summary>
            /// Triangle Information (always in groups of 3)
            /// </summary>
            UInt16[] _x, _y, _z;

            /// <summary>
            /// Color information
            /// Note that some information may be missing.
            /// </summary>
            byte[] _r, _g, _b;

            public Int64[] GetIndex()
            {
                //return (Int64[])_index.Clone();
                return _index;
            }

            public UInt16[] GetX()
            {
                //return (UInt16[])_x.Clone();
                return _x;
            }

            public UInt16[] GetY()
            {
                //return (UInt16[])_y.Clone();
                return _y;
            }

            public UInt16[] GetZ()
            {
                //return (UInt16[])_z.Clone();
                return _z;
            }

            public byte[] GetR()
            {
                //return (byte[])_r.Clone();
                return _r;
            }

            public byte[] GetG()
            {
                //return (byte[])_g.Clone();
                return _g;
            }

            public byte[] GetB()
            {
                //return (byte[])_b.Clone();
                return _b;
            }

            /// <summary>
            /// Returns MeshBlock from JSON
            /// </summary>
            /// <param name="msg"></param>
            public MeshBlockMsg(JSONNode msg)
            {
                JSONArray temp = msg["index"].AsArray;
                _index = new Int64[3];
                for (int i = 0; i < _index.Length; i++)
                {
                    // TODO get an int64 instead of an int32.
                    _index[i] = temp[i].AsInt;
                }
                temp = msg["x"].AsArray;
                _x = new UInt16[temp.Count];
                for (int i = 0; i < _x.Length; i++)
                {
                    _x[i] = (UInt16) temp[i].AsInt;
                }
                
                temp = msg["y"].AsArray;
                _y = new UInt16[temp.Count];
                for (int i = 0; i < _y.Length; i++)
                {
                    _y[i] = (UInt16) temp[i].AsInt;
                }
                
                temp = msg["z"].AsArray;
                _z = new UInt16[temp.Count];
                for (int i = 0; i < _z.Length; i++)
                {
                    _z[i] = (UInt16) temp[i].AsInt;
                }
                Debug.Log("Block");
                
                String tempColor = msg["r"].Value;
                _r = Encoding.UTF7.GetBytes(tempColor);
                tempColor = msg["b"].Value;
                _b = Encoding.UTF8.GetBytes(tempColor);
                tempColor = msg["g"].Value;
                _g = Encoding.UTF8.GetBytes(tempColor);

                if (_r.Length != _b.Length || _r.Length != _g.Length)
                {
                    Debug.Log("Color Length Missmatch");
                }

                if (_r.Length != _x.Length)
                {
                    Debug.Log("Vert and Color Missmatch");
                    Debug.Log("Color length: " + msg["r"].Value.Length + " Vert Length: " + _x.Length);
                }

                if (_x.Length > 0)
                {
                    File.WriteAllText("JSONMsgBlock.txt", msg.ToString());
                }
                /*_r = new byte[tempColor.Length];
                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = (byte)tempColor[i];
                }

                tempColor = msg["g"].Value;
                _g = new byte[tempColor.Count];
                for (int i = 0; i < _g.Length; i++)
                {
                    _g[i] = (byte)tempColor[i].AsInt;
                }

                tempColor = msg["b"].Value;
                _b = new byte[tempColor.Count];
                for (int i = 0; i < _b.Length; i++)
                {
                    _b[i] = (byte)tempColor[i].AsInt;
                }
                else
                {
                    //Debug.Log("Null Color: " + msg["r"]);
                    _r = new byte[0];
                    _b = new byte[0];
                    _g = new byte[0];
                }*/
                
            }

            public static string getMessageType()
            {
                return "voxblox_msgs/MeshBlock";
            }

        }
    }
}