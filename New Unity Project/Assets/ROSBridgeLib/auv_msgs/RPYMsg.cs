using System.Collections;
using System.Text;
using SimpleJSON;
using UnityEngine;

/**
 * Define a auv_msgs NED message. This has been hand-crafted from the corresponding
 * auv_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace auv_msgs {
		public class RPYMsg : ROSBridgeMsg {
			private float _roll, _pitch, _yaw;

			public RPYMsg(JSONNode msg) {
				//Debug.Log ("RPYMsg with " + msg.ToString());
				_roll = float.Parse(msg["roll"]);
				_pitch  = float.Parse(msg["pitch"]);
				_yaw = float.Parse(msg["yaw"]);
			}

			public RPYMsg(float roll, float pitch, float yaw) {
				_roll = roll;
				_pitch = pitch;
				_yaw = yaw;
			}

			public static string getMessageType() {
				return "auv_msgs/NED";
			}

			public float GetRoll() {
				return _roll;
			}

			public float GetPitch() {
				return _pitch;
			}

			public float GetYaw() {
				return _yaw;
			}

			public float GetRollDegrees()  {
				return (float)(_roll / Mathf.PI * 180.0);
			}

			public float GetPitchDegrees()  {
				return (float)(_pitch / Mathf.PI * 180.0);
			}

			public float GetYawDegrees()  {
						return (float)(_yaw / Mathf.PI * 180.0);
			}

			public override string ToString() {
				return "auv_msgs/RPY [roll=" + _roll + ",  pitch=" + _pitch + ", yaw=" + _yaw + "]";
			}

			public override string ToYAMLString() {
				return "{\"roll\": " + _roll + ", \"pitch\": " + _pitch + ", \"yaw\": " + _yaw + "}";
			}
		}
	}
}

