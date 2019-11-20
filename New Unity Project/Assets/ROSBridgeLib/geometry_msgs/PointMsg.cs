using System.Collections;
using System.Text;
using SimpleJSON;
using UnityEngine;

/**
 * Define a geometry_msgs point message. This has been hand-crafted from the corresponding
 * geometry_msgs message file.
 * 
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PointMsg : ROSBridgeMsg {
			private float _x, _y, _z;

			public PointMsg(JSONNode msg) {
				//Debug.Log ("PointMsg with " + msg.ToString());
				_x = float.Parse(msg["x"]);
				_y = float.Parse(msg["y"]);
				_z = float.Parse(msg["z"]);
			}

			public PointMsg(float x, float y, float z) {
				_x = x;
				_y = y;
				_z = z;
			}
			
			public static string getMessageType() {
				return "geometry_msgs/Point";
			}
			
			public float GetX() {
				return _x;
			}
			
			public float GetY() {
				return _y;
			}
			
			public float GetZ() {
				return _z;
			}
			
			public override string ToString() {
				return "geometry_msgs/Point [x=" + _x + ",  y=" + _y + ", z=" + _z + "]";
			}
					
			public override string ToYAMLString() {
				return "{\"x\": " + _x + ", \"y\": " + _y + ", \"z\": " + _z + "}";
			}
		}
	}
}
