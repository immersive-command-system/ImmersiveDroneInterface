using System.Collections;
using System.Text;
using SimpleJSON;

/**
 * Define a geometry_msgs pose message. This has been hand-crafted from the corresponding
 * geometry_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PoseMsg : ROSBridgeMsg {
			public PointMsg _position;
			public QuaternionMsg _orientation;

			public PoseMsg(JSONNode msg) {
                _position = new PointMsg(msg["position"]);
                _orientation = new QuaternionMsg(msg["orientation"]);
            }

			public PoseMsg(PointMsg p, QuaternionMsg q) {
                _position = p;
                _orientation = q;
            }
			
			public static string getMessageType() {
				return "geometry_msgs/Pose";
			}

			public PointMsg GetPosition() {
				return _position;
			}

			public QuaternionMsg GetOrientation() {
				return _orientation;
			}
			
			public override string ToString() {
				return "geometry_msgs/Pose [position=" + _position.ToString() + ",  orientation=" + _orientation.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"position\": " + _position.ToYAMLString() + ", \"orientation\": " + _orientation.ToYAMLString() + "}";
			}
		}
	}
}
