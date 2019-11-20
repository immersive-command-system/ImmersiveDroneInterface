using System.Collections;
using System.Text;
using SimpleJSON;

/* 
 * @brief ROSBridgeLib
 * @author Michael Jenkin, Robert Codd-Downey, Andrew Speers and Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace geometry_msgs {
		public class TwistWithCovarianceMsg : ROSBridgeMsg {
			private TwistMsg _twist;
			private double[] _covariance = new double[36] {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
														   0.0, 0.0, 0.0, 0.0, 0.0, 0.0};
			
			public TwistWithCovarianceMsg(JSONNode msg) {
				_twist = new TwistMsg(msg["twist"]);
				// Treat covariance
				for (int i = 0; i < msg["covariance"].Count; i++ ) {
					_covariance[i] = double.Parse(msg["covariance"][i]);
				}
			}
			
			public TwistWithCovarianceMsg(TwistMsg twist, double[] covariance) {
				_twist = twist;
				_covariance = covariance;
			}
			
			public static string GetMessageType() {
				return "geometry_msgs/TwistWithCovariance";
			}
			
			public TwistMsg GetTwist() {
				return _twist;
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
				return "TwistWithCovariance [twist=" + _twist.ToString() + ",  covariance=" + array + "]";
			}
			
			public override string ToYAMLString() {
				string array = "[";
                for (int i = 0; i < _covariance.Length; i++) {
                    array = array + _covariance[i].ToString();
                    if (_covariance.Length - i <= 1) array += ",";
                }
                array += "]";
				return "{\"twist\" : " + _twist.ToYAMLString() + ", \"covariance\" : " + array + "}";
			}
		}
	}
}