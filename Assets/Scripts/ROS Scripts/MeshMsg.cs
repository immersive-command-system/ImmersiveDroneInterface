using UnityEngine;
using SimpleJSON;
using System;
using System.Runtime.CompilerServices;

namespace ROSBridgeLib
{
    namespace voxblox_msgs
    {
        public class MeshMsg: ROSBridgeMsg
        {
            std_msgs.HeaderMsg _header;
            float _block_edge_length;
            voxblox_msgs.MeshBlockMsg[] _mesh_blocks;

            public MeshMsg(JSONNode msg)
            {
                _header = new std_msgs.HeaderMsg(msg["header"]);
                _block_edge_length = float.Parse(msg["block_edge_length"]);
                JSONArray temp = msg["mesh_blocks"].AsArray;
                _mesh_blocks = new voxblox_msgs.MeshBlockMsg[temp.Count];
                for (int i = 0; i < _mesh_blocks.Length; i++)
                {
                    _mesh_blocks[i] = new voxblox_msgs.MeshBlockMsg(temp[i]);
                }
            }

            public static string getMessageType()
            {
                return "voxblox_msgs/Mesh";
            }

            public std_msgs.HeaderMsg GetHeader()
            {
                return _header;
            }

            public float GetBlockEdgeLength()
            {
                return _block_edge_length;
            }

            public voxblox_msgs.MeshBlockMsg[] GetMeshBlocks()
            {
                //return (voxblox_msgs.MeshBlockMsg[])_mesh_blocks.Clone();
                return _mesh_blocks;
            }
        }
    }
}