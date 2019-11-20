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
	namespace geographic_msgs {
		public class GeoPointMsg : ROSBridgeMsg {
			private float _latitude, _longitude, _altitude;

			public GeoPointMsg(JSONNode msg) {
				//Debug.Log ("GeoPointMsg with " + msg.ToString());
				_latitude = float.Parse(msg["latitude"]);
				_longitude  = float.Parse(msg["longitude"]);
				_altitude = float.Parse(msg["altitude"]);
			}

			public GeoPointMsg(float latitude, float longitude, float altitude) {
				_latitude = latitude;
				_longitude = longitude;
				_altitude = altitude;
			}
			
			public static string getMessageType() {
				return "geographic_msgs/GeoPoint";
			}
			
			public float GetLatitude() {
				return _latitude;
			}
			
			public float GetLongitude() {
				return _longitude;
			}
			
			public float GetAltitude() {
				return _altitude;
			}
			
			public override string ToString() {
				return "geographic_msgs/GeoPoint [latitude=" + _latitude + ",  longitude=" + _longitude + ", altitude=" + _altitude + "]";
			}
					
			public override string ToYAMLString() {
				return "{\"latitude\": " + _latitude + ", \"longitude\": " + _longitude + ", \"altitude\": " + _altitude + "}";
			}
		}
	}
}