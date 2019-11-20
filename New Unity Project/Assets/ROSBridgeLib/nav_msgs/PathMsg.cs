using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.geometry_msgs;

/**
 * Define a nav_msgs Path message. This has been hand-crafted from the corresponding
 * nav_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace nav_msgs {
		public class PathMsg : ROSBridgeMsg {
			public HeaderMsg _header;
			public List<PoseStampedMsg> _poses;
			
			public PathMsg(JSONNode msg) {
				_header = new HeaderMsg(msg["header"]);
                // Treat poses
                for (int i = 0; i < msg["poses"].Count; i++ ) {
					_poses.Add(new PoseStampedMsg(msg["poses"][i]));
				}
			}

			public static string GetMessageType() {
				return "nav_msgs/Path";
			}

			public HeaderMsg GetHeader() {
				return _header;
			}

			public PoseStampedMsg GetPoseStamped(int idx = 0) {
				if (idx < _poses.Count) {
					return _poses [idx];
				} else {
					return null;
				}
			}

			public override string ToString() {
				string array = "[";
				for (int i = 0; i < _poses.Count; i++) {
					array = array + _poses[i].ToString();
					if (_poses.Count - i <= 1)
						array += ",";
				}
				array += "]";

				return "Path [header=" + _header.ToString() 
					+ ",  poses=" + array + "]";
			}

			public override string ToYAMLString() {
				string array = "{";
				for (int i = 0; i < _poses.Count; i++) {
					array = array + _poses[i].ToYAMLString();
					if (_poses.Count - i <= 1)
						array += ",";
				}
				array += "}";
				return "{\"header\" : " + _header.ToYAMLString() 
					+ ", \"poses\" : " + array + "}";
			}
		}
	}
}