using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.geometry_msgs;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace nav_msgs {
		public class OdometryMsg : ROSBridgeMsg {
			public HeaderMsg _header;
			public string _child_frame_id;
			public PoseWithCovarianceMsg _pose;
			public TwistWithCovarianceMsg _twist;
			
			public OdometryMsg(JSONNode msg) {
				_header = new HeaderMsg(msg["header"]);
				_child_frame_id = msg["child_frame_id"].ToString();
				_pose = new PoseWithCovarianceMsg(msg["pose"]);
				_twist = new TwistWithCovarianceMsg(msg["twist"]);
			}

			public HeaderMsg GetHeader() {
				return _header;
			}

			public string GetChildFrameId() {
				return _child_frame_id;
			}

			public PoseWithCovarianceMsg GetPoseWithCovariance() {
				return _pose;
			}

			public TwistWithCovarianceMsg GetTwistWithCovariance() {
				return _twist;
			}
			
			public override string ToString() {
				return "Odometry [header=" + _header.ToString() 
						  + ",  child_frame_id=" + _child_frame_id
						  + ",  pose=" + _pose.ToString() 
						  + ",  twist=" + _twist.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"header\" : " + _header.ToYAMLString() 
				  + ", \"child_frame_id\" : " + _child_frame_id
				  + ", \"pose\" : " + _pose.ToYAMLString() 
				  + ", \"twist\" : " + _twist.ToYAMLString() + "}";
			}
		}
	}
}