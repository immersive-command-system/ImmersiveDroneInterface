using System.Collections;
using System.Text;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class PoseWithCovarianceMsg : ROSBridgeMsg {
			public PoseMsg _pose;
			private double[] _covariance = new double[36] {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0};

			public PoseWithCovarianceMsg(JSONNode msg) {
				_pose = new PoseMsg(msg["pose"]);
				// Treat covariance
				for (int i = 0; i < msg["covariance"].Count; i++ ) {
					_covariance[i] = double.Parse(msg["covariance"][i]);
				}
			}
			
			public PoseWithCovarianceMsg(PoseMsg pose, double[] covariance) {
				_pose = pose;
				_covariance = covariance;
			}
 			
			public static string GetMessageType() {
				return "geometry_msgs/PoseWithCovariance";
			}
			
			public PoseMsg GetPose() {
				return _pose;
			}

			public double[] GetCovariance() {
				return _covariance;
			}
			
			public override string ToString() {
				string array = "[";
                for (int i = 0; i < _covariance.Length; i++) {
                    array = array + _covariance[i].ToString();
                    if (_covariance.Length - i <= 1) array += ",";
                }
                array += "]";
				return "PoseWithCovariance [pose=" + _pose.ToString() + ",  covariance=" + array + "]";
			}
			
			public override string ToYAMLString() {
				string array = "[";
                for (int i = 0; i < _covariance.Length; i++) {
                    array = array + _covariance[i].ToString();
                    if (_covariance.Length - i <= 1) array += ",";
                }
                array += "]";
				return "{\"pose\" : " + _pose.ToYAMLString() + ", \"covariance\" : " + array + "}";
			}
		}
	}
}