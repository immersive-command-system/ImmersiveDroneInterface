using SimpleJSON;
using ROSBridgeLib.std_msgs;

/**
 * Message for a single range from an active ranger that emits energy and reports
 * one range reading that is valid along an arc at the distance measured. 
 * This message is  not appropriate for laser scanners. 
 * http://docs.ros.org/kinetic/api/sensor_msgs/html/msg/Range.html
 * 
 * @author Michael Herren
 * @version 1
 */

namespace ROSBridgeLib
{
	namespace sensor_msgs
	{
		public class RangeMsg : ROSBridgeMsg
		{
			private HeaderMsg _header;

			private float _range;
			private float _min_range;
			private float _max_range;

			/**
			 * The size of the arc that the distance reading is valid for [rad] the object causing the range reading may have been anywhere 
			 * within -field_of_view/2 and field_of_view/2 at the measured range. 0 angle corresponds to the x-axis of the sensor.
			 */
			private float _field_of_view;

			/**
			 * The type of radiation used by the sensor (sound, IR, etc)
			 */
			private uint _radiation_type;

			public const uint ULTRASOUND = 0;
			public const uint INFRARED = 1;

			public RangeMsg (JSONNode msg)
			{
				_header = new HeaderMsg (msg ["header"]);
				_range = float.Parse (msg ["range"]);
				_min_range = float.Parse (msg ["min_range"]);
				_max_range = float.Parse (msg ["max_range"]);
				_radiation_type = uint.Parse (msg ["radiation_type"]);
				_field_of_view = float.Parse (msg ["field_of_view"]);
			}

			public RangeMsg (HeaderMsg header, float range, float min_range, float max_range, float field_of_view, uint radiation_type)
			{
				_header = header;
				_range = range;
				_min_range = min_range;
				_max_range = max_range;
				_field_of_view = field_of_view;
				_radiation_type = radiation_type;
			}

			public static string GetMessageType ()
			{
				return "sensor_msgs/Range";
			}

			public float GetRange ()
			{
				return _range;
			}

			public float GetMinRange ()
			{
				return _min_range;
			}

			public float GetMaxRange ()
			{
				return _max_range;
			}

			public float GetFieldOfView ()
			{
				return _field_of_view;
			}

			public uint GetRadiationType ()
			{
				return _radiation_type;
			}

			public override string ToString ()
			{
				return "Range [radiation_type=" + _radiation_type + ",  field_of_view=" + _field_of_view + ", min_range=" + _min_range + ", max_range=" + _max_range + ", range=" + _range + ", Header " + _header.ToString () + "]";
			}

			public override string ToYAMLString ()
			{
				return "{\"radiation_type\" : " + _radiation_type + ", \"field_of_view\" : " + _field_of_view + ", \"min_range\" : " + _min_range + ", \"max_range\" : " + _max_range + ", \"range\" : " + _range + ", \"header\" : " + _header.ToYAMLString () + "}";
			}
		}
	}
}