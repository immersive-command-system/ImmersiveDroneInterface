using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;

/**
 * Define a PointField message.
 * @author Miquel Massot Campos
 */

// Datatype:
//uint8 INT8=1
//uint8 UINT8=2
//uint8 INT16=3
//uint8 UINT16=4
//uint8 INT32=5
//uint8 UINT32=6
//uint8 FLOAT32=7
//uint8 FLOAT64=8

namespace ROSBridgeLib {
	namespace sensor_msgs {
		public class PointFieldMsg : ROSBridgeMsg {
			private string _name;
			private uint _offset;
			private byte _datatype;
			private uint _count;

			public const byte INT8 = 1;
			public const byte UINT8 = 2;
			public const byte INT16 = 3;
			public const byte UINT16 = 4;
			public const byte INT32 = 5;
			public const byte UINT32 = 6;
			public const byte FLOAT32 = 7;
			public const byte FLOAT64 = 8;

			public PointFieldMsg(JSONNode msg) {
				_name = msg ["name"];
				_offset = uint.Parse(msg ["offset"]);
				_datatype = byte.Parse(msg ["datatype"]);
				_count = uint.Parse(msg ["count"]);
			}

			public PointFieldMsg(string name, uint offset, byte datatype, uint count) {
				_name = name;
				_offset = offset;
				_datatype = datatype;
				_count = count;
			}

			public string GetName() {
				return _name;
			}

			public uint GetOffset() {
				return _offset;
			}

			public byte GetDatatype() {
				return _datatype;
			}

			public uint GetCount() {
				return _count;
			}

			public static string GetMessageType() {
				return "sensor_msgs/PointField";
			}

			public override string ToString() {
				return "PointField [name=" + _name + ",  offset=" + _offset + ", datatype=" + _datatype + ", count=" + _count + "]";
			}

			public override string ToYAMLString() {
				return "{\"name\" : " + "\"" + _name + "\", \"offset\" : \"" + _offset + "\", \"datatype\" : " + _datatype + "\", \"count\" : " + _count + "}";
			}
		}
	}
}
