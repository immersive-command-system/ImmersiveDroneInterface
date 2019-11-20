using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.geographic_msgs;
using UnityEngine;

/**
 * Define a auv_msgs NavSts  message. This has been hand-crafted from the corresponding
 * auv_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace auv_msgs {
		public class NavStsMsg : ROSBridgeMsg {
			public HeaderMsg _header;
			public DecimalLatLonMsg _global_position;
			public DecimalLatLonMsg _origin;
			public NEDMsg _position;
			public Float32Msg _altitude;
			public PointMsg _body_velocity;
			public RPYMsg _orientation;
			public RPYMsg _orientation_rate;
			public NEDMsg _position_variance;
			public RPYMsg _orientation_variance;
			public UInt8Msg _status;

			public NavStsMsg(JSONNode msg) {
				//Debug.Log("NavStsMsg with " + msg.ToString());

                _header = new HeaderMsg(msg["header"]);
				_global_position = new DecimalLatLonMsg(msg["global_position"]);
				_origin = new DecimalLatLonMsg(msg["origin"]);
				_position = new NEDMsg(msg["position"]);
				_altitude = new Float32Msg(msg["altitude"]);
				_body_velocity = new PointMsg(msg["body_velocity"]);
				_orientation = new RPYMsg(msg["orientation"]);
				_orientation_rate = new RPYMsg(msg["orientation_rate"]);
				_position_variance = new NEDMsg(msg["position_variance"]);
				_orientation_variance = new RPYMsg(msg["orientation_variance"]);
				_status = new UInt8Msg(msg["status"]);

				//Debug.Log("NavStsMsg done and it looks like " + this.ToString());
            }

			public NavStsMsg(HeaderMsg header, 
							 DecimalLatLonMsg global_position, 
							 DecimalLatLonMsg origin, 
							 NEDMsg position,
							 Float32Msg altitude,
							 PointMsg body_velocity,
							 RPYMsg orientation,
							 RPYMsg orientation_rate,
							 NEDMsg position_variance,
							 RPYMsg orientation_variance,
							 UInt8Msg status) {
                _header = header;
                _global_position = global_position;
				_origin = origin;
				_position = position;
				_altitude = altitude;
				_body_velocity = body_velocity;
				_orientation = orientation;
				_orientation_rate = orientation_rate;
				_position_variance = position_variance;
				_orientation_variance = orientation_variance;
				_status = status;
            }
			
			public static string getMessageType() {
				return "auv_msgs/NavSts ";
			}

            public HeaderMsg GetHeader() {
                return _header;
            }

			public DecimalLatLonMsg GetGlobalPosition()
            {
                return _global_position;
            }

			public DecimalLatLonMsg GetOrigin()
            {
                return _origin;
            }

            public NEDMsg GetPosition() {
				return _position;
			}

			public Float32Msg GetAltitude()
            {
                return _altitude;
            }

            public PointMsg GetBodyVelocity()
            {
                return _body_velocity;
            }

			public RPYMsg GetOrientation() {
				return _orientation;
			}

			public RPYMsg GetOrientationRate()
            {
                return _orientation_rate;
            }

            public NEDMsg GetPositionVariance()
            {
                return _position_variance;
            }

			public RPYMsg GetOrientationVariance()
            {
                return _orientation_variance;
            }

            public UInt8Msg GetStatus()
            {
                return _status;
            }

            public override string ToString() {
				return "auv_msgs/NavSts  [header=" + _header.ToString() +
						", global_position=" + _global_position.ToString() +
						", origin=" + _origin.ToString() +
						", position=" + _position.ToString() +
						", altitude=" + _altitude.ToString() +
						", body_velocity=" + _body_velocity.ToString() +
						", orientation=" + _orientation.ToString() +
						", orientation_rate=" + _orientation_rate.ToString() +
						", position_variance=" + _position_variance.ToString() +
						", orientation_variance=" + _orientation_variance.ToString() +
						", status=" + _status.ToString() + "]";
			}
			
			public override string ToYAMLString() {
				return "{\"header\":" + _header.ToYAMLString() +
						", \"global_position\":" + _global_position.ToYAMLString() +
						", \"origin\":" + _origin.ToYAMLString() +
						", \"position\":" + _position.ToYAMLString() +
						", \"altitude\":" + _altitude.ToYAMLString() +
						", \"body_velocity\":" + _body_velocity.ToYAMLString() +
						", \"orientation\":" + _orientation.ToYAMLString() +
						", \"orientation_rate\":" + _orientation_rate.ToYAMLString() +
						", \"position_variance\":" + _position_variance.ToYAMLString() +
						", \"orientation_variance\":" + _orientation_variance.ToYAMLString() +
						", \"status\":" + _status.ToYAMLString() + "}";
			}
		}
	}
}