using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using System.Text;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class MissionWaypointActionMsg : ROSBridgeMsg
        {
            private uint _action_repeat;
            private uint[] _command_list;
            private uint[] _command_parameter;

            public MissionWaypointActionMsg(JSONNode msg)
            {
                _action_repeat = (uint)msg["action_repeat"].AsInt;

                JSONArray temp = msg["command_list"].AsArray;
                _command_list = new uint[temp.Count];
                for (int i = 0; i < _command_list.Length; i++)
                {
                    _command_list[i] = (uint)temp[i].AsInt;
                }

                temp = msg["command_parameter"].AsArray;
                _command_parameter = new uint[temp.Count];
                for (int i = 0; i < _command_parameter.Length; i++)
                {
                    _command_parameter[i] = (uint)temp[i].AsInt;
                }
            }

            public MissionWaypointActionMsg(uint action_repeat, uint[] command_list, uint[] command_parameter)
            {
                _action_repeat = action_repeat;
                _command_list = command_list;
                _command_parameter = command_parameter;
            }

            public MissionWaypointActionMsg(uint num_actions, uint num_repeats, uint[] command_list, uint[] command_parameter)
            {
                _action_repeat = (num_actions & 15) | ((num_repeats & 15) << 4);
                _command_list = command_list;
                _command_parameter = command_parameter;
            }

            public static string GetMessageType()
            {
                return "dji_sdk/MissionWaypointAction";
            }

            public uint GetActionRepeat()
            {
                return _action_repeat;
            }

            public uint GetNumActions()
            {
                return _action_repeat & 15;
            }

            public uint GetNumRepeats()
            {
                return (_action_repeat >> 4) & 15;
            }

            public uint[] GetCommandList()
            {
                return (uint[])_command_list.Clone();
            }

            public uint[] GetCommandParameters()
            {
                return (uint[])_command_parameter.Clone();
            }

            public override string ToYAMLString()
            {
                return JsonUtility.ToJson(this);
            }
        }
    }
}