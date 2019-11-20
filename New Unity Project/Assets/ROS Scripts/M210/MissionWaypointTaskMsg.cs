using SimpleJSON;
using System.Text;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class MissionWaypointTaskMsg : ROSBridgeMsg
        {
            float _velocity_range, _idle_velocity;

            public enum ActionOnFinish
            {
                NO_ACTION = 0,  // no action
                RETURN_TO_HOME = 1,  // return to home
                AUTO_LANDING = 2,  // auto landing
                RETURN_TO_POINT = 3,  // return to point 0
                NO_EXIT = 4  // infinite mode， no exit
            }
            ActionOnFinish _action_on_finish;

            uint _mission_exec_times;

            public enum YawMode
            {
                AUTO = 0,       // auto mode (point to next waypoint)
                LOCK = 1,       // lock as an initial value
                RC = 2,       // controlled by RC
                WAYPOINT = 3       // use waypoint's yaw(tgt_yaw)
            }
            YawMode _yaw_mode;

            public enum TraceMode
            {
                POINT = 0,       // point to point, after reaching the target waypoint hover, complete waypt action (if any), then fly to the next waypt
                COORDINATED = 1       // 1: Coordinated turn mode, smooth transition between waypts, no waypts task
            }
            TraceMode _trace_mode;

            public enum ActionOnRCLost
            {
                FREE = 0,       // exit waypoint and failsafe
                AUTO = 1       // continue the waypoint
            }
            ActionOnRCLost _action_on_rc_lost;

            public enum GimbalPitchMode
            {
                FREE = 0,       // free mode, no control on gimbal
                AUTO = 1       // auto mode, Smooth transition between waypoints on gimbal
            }
            GimbalPitchMode _gimbal_pitch_mode;

            MissionWaypointMsg[] _mission_waypoints;

            public MissionWaypointTaskMsg(JSONNode msg)
            {
                _velocity_range = msg["velocity_range"].AsFloat;
                _idle_velocity = msg["idle_velocity"].AsFloat;
                _action_on_finish = (ActionOnFinish)msg["action_on_finish"].AsInt;
                _mission_exec_times = (uint)msg["mission_exec_times"].AsInt;
                _yaw_mode = (YawMode)msg["yaw_mode"].AsInt;
                _trace_mode = (TraceMode)msg["trace_mode"].AsInt;
                _action_on_rc_lost = (ActionOnRCLost)msg["action_on_rc_lost"].AsInt;
                _gimbal_pitch_mode = (GimbalPitchMode)msg["gimbal_pitch_mode"].AsInt;

                JSONArray waypoints = msg["mission_waypoint"].AsArray;
                _mission_waypoints = new MissionWaypointMsg[waypoints.Count];
                for (int i = 0; i < _mission_waypoints.Length; i++)
                {
                    _mission_waypoints[i] = new MissionWaypointMsg(waypoints[i]);
                }
            }

            public MissionWaypointTaskMsg(float velocity_range, float idle_velocity, ActionOnFinish finish_action, uint mission_exec_times, YawMode yaw_mode, TraceMode trace_mode, ActionOnRCLost rc_lost_action, GimbalPitchMode gimbal_pitch_mode, MissionWaypointMsg[] mission_waypoints)
            {
                _velocity_range = velocity_range;
                _idle_velocity = idle_velocity;
                _action_on_finish = finish_action;
                _mission_exec_times = mission_exec_times;
                _yaw_mode = yaw_mode;
                _trace_mode = trace_mode;
                _action_on_rc_lost = rc_lost_action;
                _gimbal_pitch_mode = gimbal_pitch_mode;
                _mission_waypoints = mission_waypoints;
            }

            public static string GetMessageType()
            {
                return "dji_sdk/MissionWaypointTask";
            }

            public float GetVelocityRange()
            {
                return _velocity_range;
            }

            public float GetIdleVelocity()
            {
                return _idle_velocity;
            }

            public ActionOnFinish GetFinishAction()
            {
                return _action_on_finish;
            }

            public uint GetMissionExecTimes()
            {
                return _mission_exec_times;
            }

            public YawMode GetYawMode()
            {
                return _yaw_mode;
            }

            public TraceMode GetTraceMode()
            {
                return _trace_mode;
            }

            public ActionOnRCLost GetRCLostAction()
            {
                return _action_on_rc_lost;
            }

            public GimbalPitchMode GetGimbalPitchMode()
            {
                return _gimbal_pitch_mode;
            }

            public MissionWaypointMsg[] GetMissionWaypoints()
            {
                return _mission_waypoints;
            }

            public override string ToYAMLString()
            {
                StringBuilder sb = new StringBuilder("{");
                sb.AppendFormat("\"velocity_range\": {0}, ", _velocity_range);
                sb.AppendFormat("\"idle_velocity\": {0}, ", _idle_velocity);
                sb.AppendFormat("\"action_on_finish\": {0}, ", (int)_action_on_finish);
                sb.AppendFormat("\"mission_exec_times\": {0}, ", _mission_exec_times);
                sb.AppendFormat("\"yaw_mode\": {0}, ", (int)_yaw_mode);
                sb.AppendFormat("\"trace_mode\": {0}, ", (int)_trace_mode);
                sb.AppendFormat("\"action_on_rc_lost\": {0}, ", (int)_action_on_rc_lost);
                sb.AppendFormat("\"gimbal_pitch_mode\": {0}, ", (int)_gimbal_pitch_mode);

                string[] waypoints = new string[_mission_waypoints.Length];
                for (int i = 0; i < waypoints.Length; i++)
                {
                    waypoints[i] = _mission_waypoints[i].ToYAMLString();
                }
                sb.AppendFormat("\"mission_waypoint\": [{0}]", string.Join(", ", waypoints));

                sb.Append("}");
                return sb.ToString();
            }
        }
    }
}