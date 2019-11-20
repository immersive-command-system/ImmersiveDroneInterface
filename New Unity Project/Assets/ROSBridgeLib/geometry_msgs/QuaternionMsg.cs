using System.Collections;
using System.Text;
using SimpleJSON;

/**
 * Define a geometry_msgs quaternion message. This has been hand-crafted from the corresponding
 * geometry_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class QuaternionMsg : ROSBridgeMsg {
			private float _x, _y, _z, _w;

			public QuaternionMsg(JSONNode msg) {
				_x = float.Parse(msg["x"]);
				_y = float.Parse(msg["y"]);
				_z = float.Parse(msg["z"]);
				_w = float.Parse(msg["w"]);
			}

			public QuaternionMsg(float x, float y, float z, float w) {
				_x = x;
				_y = y;
				_z = z;
				_w = w;
			}
			
			public static string getMessageType() {
				return "geometry_msgs/Quaternion";
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

			public float GetW() {
				return _w;
			}
			
			public override string ToString() {
				return "geometry_msgs/Quaternion [x=" + _x + ",  y=" + _y + ", z=" + _z + ", w=" + _w + "]";
			}
					
			public override string ToYAMLString() {
				return "{\"x\": " + _x + ", \"y\": " + _y + ", \"z\": " + _z + ", \"w\": " + _w + "}";
			}
		}
	}
}
