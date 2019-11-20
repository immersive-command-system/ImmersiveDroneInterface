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
		public class NEDMsg : ROSBridgeMsg {
			private float _north, _east, _depth;

			public NEDMsg(JSONNode msg) {
				//Debug.Log ("NEDMsg with " + msg.ToString());
				_north = float.Parse(msg["north"]);
				_east  = float.Parse(msg["east"]);
				_depth = float.Parse(msg["depth"]);
			}

			public NEDMsg(float north, float east, float depth) {
				_north = north;
				_east = east;
				_depth = depth;
			}
			
			public static string getMessageType() {
				return "auv_msgs/NED";
			}
			
			public float GetNorth() {
				return _north;
			}
			
			public float GetEast() {
				return _east;
			}
			
			public float GetDepth() {
				return _depth;
			}
			
			public override string ToString() {
				return "auv_msgs/NED [north=" + _north + ",  east=" + _east + ", depth=" + _depth + "]";
			}
					
			public override string ToYAMLString() {
				return "{\"north\": " + _north + ", \"east\": " + _east + ", \"depth\": " + _depth + "}";
			}
		}
	}
}
