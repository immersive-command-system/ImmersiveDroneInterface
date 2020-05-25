using UnityEngine;
using SimpleJSON;
using System;
using System.Runtime.CompilerServices;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

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
                return (Int64[])_index.Clone();
            }

            public UInt16[] GetX()
            {
                return (UInt16[])_x.Clone();
            }

            public UInt16[] GetY()
            {
                return (UInt16[])_y.Clone();
            }

            public UInt16[] GetZ()
            {
                return (UInt16[])_z.Clone();
            }

            public byte[] GetR()
            {
                return (byte[])_r.Clone();
            }

            public byte[] GetG()
            {
                return (byte[])_g.Clone();
            }

            public byte[] GetB()
            {
                return (byte[])_b.Clone();
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
                
                temp = msg["r"].AsArray;
                _r = new byte[temp.Count];
                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = (byte) temp[i].AsInt;
                }
                
                temp = msg["g"].AsArray;
                _g = new byte[temp.Count];
                for (int i = 0; i < _g.Length; i++)
                {
                    _g[i] = (byte) temp[i].AsInt;
                }
                
                temp = msg["b"].AsArray;
                _b = new byte[temp.Count];
                for (int i = 0; i < _b.Length; i++)
                {
                    _b[i] = (byte) temp[i].AsInt;
                }
                
            }

            public static string getMessageType()
            {
                return "voxblox_msgs/MeshBlock";
            }

        }
    }
}