using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class MissionWaypointMsg : ROSBridgeMsg
        {
            private double _latitude, _longitude;
            private float _altitude, _damping_distance;
            private int _target_yaw, _target_gimbal_pitch;

            public enum TurnMode
            {
                CLOCKWISE = 0,
                COUNTERCLOCKWISE = 1
            };
            private TurnMode _turn_mode;

            private bool _has_action;
            private uint _action_time_limit;
            private MissionWaypointActionMsg _waypoint_action;

            public MissionWaypointMsg(JSONNode msg)
            {
                _latitude = msg["latitude"].AsDouble;
                _longitude = msg["longitude"].AsDouble;
                _altitude = msg["altitude"].AsFloat;
                _damping_distance = msg["damping_distance"].AsFloat;
                _target_yaw = msg["target_yaw"].AsInt;
                _target_gimbal_pitch = msg["target_gimbal_pitch"].AsInt;
                _turn_mode = (TurnMode)msg["turn_mode"].AsInt;
                _has_action = (msg["has_action"].AsInt != 0);
                _action_time_limit = (uint)msg["action_time_limit"].AsInt;
                _waypoint_action = new MissionWaypointActionMsg(msg["waypoint_action"]);
            }

            public MissionWaypointMsg(double latitude, double longitude, float altitude, float damping_distance, int target_yaw, int target_gimbal_pitch, TurnMode turn_mode, bool has_action, uint action_time_limit, MissionWaypointActionMsg waypoint_action)
            {
                _latitude = latitude;
                _longitude = longitude;
                _altitude = altitude;
                _damping_distance = damping_distance;
                _target_yaw = target_yaw;
                _target_gimbal_pitch = target_gimbal_pitch;
                _turn_mode = turn_mode;
                _has_action = has_action;
                _action_time_limit = action_time_limit;
                _waypoint_action = waypoint_action;
            }

            public static string GetMessageType()
            {
                return "dji_sdk/MissionWaypoint";
            }

            public double GetLatitude()
            {
                return _latitude;
            }

            public double GetLongitude()
            {
                return _longitude;
            }

            public float GetAltitude()
            {
                return _altitude;
            }

            public float GetDampingDistance()
            {
                return _damping_distance;
            }

            public int GetTargetYaw()
            {
                return _target_yaw;
            }

            public int GetTargetGimbalPitch()
            {
                return _target_gimbal_pitch;
            }

            public TurnMode GetTurnMode()
            {
                return _turn_mode;
            }

            public bool HasAction()
            {
                return _has_action;
            }

            public uint GetActionTimeLimit()
            {
                return _action_time_limit;
            }

            public MissionWaypointActionMsg GetWaypointAction()
            {
                return _waypoint_action;
            }

            public override string ToString()
            {
                return string.Format("MissionWaypoint: latitude/longitude: {0}, {1}, altitude: {2}", _latitude, _longitude, _altitude);
            }

            public override string ToYAMLString()
            {
                return JsonUtility.ToJson(this);
            }
        }
    }
}