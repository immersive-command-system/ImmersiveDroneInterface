using System.Collections;
using System.Text;
using SimpleJSON;
using UnityEngine;

/**
 * Define a geographic_msgs GeoPoint message. This has been hand-crafted from the corresponding
 * geographic_msgs message file.
 * 
 * @author Miquel Massot Campos
 */

namespace ROSBridgeLib {
	namespace auv_msgs {
		public class DecimalLatLonMsg : ROSBridgeMsg {
			private double _latitude, _longitude;

			public DecimalLatLonMsg(JSONNode msg) {
				//Debug.Log ("DecimalLatLonMsg with " + msg.ToString());
				_latitude = double.Parse(msg["latitude"]);
				_longitude  = double.Parse(msg["longitude"]);
			}

			public DecimalLatLonMsg(double latitude, double longitude) {
				_latitude = latitude;
				_longitude = longitude;
			}

			public static string getMessageType() {
				return "auv_msgs/DecimalLatLon";
			}

			public double GetLatitude() {
				return _latitude;
			}

			public double GetLongitude() {
				return _longitude;
			}

			public override string ToString() {
				return "auv_msgs/DecimalLatLon [latitude=" + _latitude + ",  longitude=" + _longitude + "]";
			}

			public override string ToYAMLString() {
				return "{\"latitude\": " + _latitude + ", \"longitude\": " + _longitude + "}";
			}
		}
	}
}
