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
            //changed type float to type double for lat and long
            public double latitude, longitude;
            public float altitude, damping_distance;
            public int target_yaw, target_gimbal_pitch;

            public enum TurnMode
            {
                CLOCKWISE = 0,
                COUNTERCLOCKWISE = 1
            };
            public TurnMode turn_mode;

            public int has_action;
            public uint action_time_limit;
            public MissionWaypointActionMsg waypoint_action;

            public MissionWaypointMsg(JSONNode msg)
            {
                //changes "asDouble" to AsFloat
                latitude = msg["latitude"].AsDouble;
                longitude = msg["longitude"].AsDouble;
                altitude = msg["altitude"].AsFloat;
                damping_distance = msg["damping_distance"].AsFloat;
                target_yaw = msg["target_yaw"].AsInt;
                target_gimbal_pitch = msg["target_gimbal_pitch"].AsInt;
                turn_mode = (TurnMode)msg["turn_mode"].AsInt;
                //_has_action = (msg["has_action"].AsInt != 0);
                has_action = (msg["has_action"].AsInt);
                action_time_limit = (uint)msg["action_time_limit"].AsInt;
                waypoint_action = new MissionWaypointActionMsg(msg["waypoint_action"]);
            }
            //changed lat and long to double from float, changes has-action from bool to int
            public MissionWaypointMsg(double _latitude, double _longitude, float _altitude, float _damping_distance, int _target_yaw, int _target_gimbal_pitch, TurnMode _turn_mode, int _has_action, uint _action_time_limit, MissionWaypointActionMsg _waypoint_action)
            {
                latitude = _latitude;
                longitude = _longitude;
                altitude = _altitude;
                damping_distance = _damping_distance;
                target_yaw = _target_yaw;
                target_gimbal_pitch = _target_gimbal_pitch;
                turn_mode = _turn_mode;
                has_action = _has_action;
                action_time_limit = _action_time_limit;
                waypoint_action = _waypoint_action;
                Debug.Log("constructor accuracy: " + _latitude);
            }

            public static string GetMessageType()
            {
                return "dji_sdk/MissionWaypoint";
            }

            public double GetLatitude()
            {
                return latitude;
            }

            public double GetLongitude()
            {
                return longitude;
            }

            public float GetAltitude()
            {
                return altitude;
            }

            public float GetDampingDistance()
            {
                return damping_distance;
            }

            public int GetTargetYaw()
            {
                return target_yaw;
            }

            public int GetTargetGimbalPitch()
            {
                return target_gimbal_pitch;
            }

            public TurnMode GetTurnMode()
            {
                return turn_mode;
            }
            //changed from bool to int 
            public int HasAction()
            {
                return has_action;
            }

            public uint GetActionTimeLimit()
            {
                return action_time_limit;
            }

            public MissionWaypointActionMsg GetWaypointAction()
            {
                return waypoint_action;
            }

            public override string ToString()
            {
                return string.Format("MissionWaypoint: latitude/longitude: {0}, {1}, altitude: {2}", latitude, longitude, altitude);
            }

            public override string ToYAMLString()
            {
                return JsonUtility.ToJson(this);
            }
        }
    }
}