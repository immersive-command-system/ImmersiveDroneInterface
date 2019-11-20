/**
 * http://docs.ros.org/api/sensor_msgs/html/msg/JointState.html

# This is a message that holds data to describe the state of a set of torque controlled joints. 
#
# The state of each joint (revolute or prismatic) is defined by:
#  * the position of the joint (rad or m),
#  * the velocity of the joint (rad/s or m/s) and 
#  * the effort that is applied in the joint (Nm or N).
#
# Each joint is uniquely identified by its name
# The header specifies the time at which the joint states were recorded. All the joint states
# in one message have to be recorded at the same time.
#
# This message consists of a multiple arrays, one for each part of the joint state. 
# The goal is to make each of the fields optional. When e.g. your joints have no
# effort associated with them, you can leave the effort array empty. 
#
# All arrays in this message should have the same size, or be empty.
# This is the only way to uniquely associate the joint name with the correct
# states.

Header header

string[] name
float64[] position
float64[] velocity
float64[] effort

 */

using System.Collections;
using System;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ROSBridgeLib {
	namespace sensor_msgs {

		public class JointStateMsg: ROSBridgeMsg {

			private HeaderMsg _header;
			private string [] _name;
			private float [] _position;
			private float [] _velocity;
			private float [] _effort;

			private static Regex floats = new Regex ("[^-0-9.,]");
			private static Regex names = new Regex ("[^-_a-zA-Z0-9.,]");

			/**
			 * C-tor with JSONNode msg
			 */
			public JointStateMsg (JSONNode msg) {

				_header = new HeaderMsg (msg ["header"]);
				_name = names.Replace(msg["name"].ToString(), "").Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);

				string [] p_strings = floats.Replace (msg ["position"].ToString (), "").Split (new string[]{ "," }, StringSplitOptions.RemoveEmptyEntries);
				if (p_strings.Length > 0) {
					_position = Array.ConvertAll( p_strings, float.Parse);
				}

				string [] v_strings = floats.Replace (msg ["velocity"].ToString (), "").Split (new string[]{ "," }, StringSplitOptions.RemoveEmptyEntries);
				if (v_strings.Length > 0) {
					_velocity = Array.ConvertAll( v_strings, float.Parse);
				}

				string [] e_strings = floats.Replace (msg ["effort"].ToString (), "").Split (new string[]{ "," }, StringSplitOptions.RemoveEmptyEntries);
				if (e_strings.Length > 0) {
					_effort = Array.ConvertAll( e_strings, float.Parse);
				}

			}

			/**
			 * C-tor with a member list
			 */
			public JointStateMsg (HeaderMsg h, string [] n, float [] p, float [] v, float [] e) {
				_header = h;
				_name = n;
				_position = p;
				_velocity = v;
				_effort = e;
			}

			/**
			 * Getters
			 */
			public HeaderMsg getHeader(){ return _header; }
			public string [] getName(){ return _name; }
			public float [] getPosition() { return _position; }
			public float [] getVelocity() { return _velocity; }
			public float [] getEffort() { return _effort; }

			/**
			 * message type
			 */
			public static string GetMessageType() {
				return "sensor_msgs/JointState";
			}

			/**
			 * toString functions
			 */
			public override string ToString() {
				return null;
//				return "JointStates [header=" + _header.ToString() +
////					"name=[\'" + string.Join("\', \'", _name) + "\']" + 
////					"position=[" + string.Join(", ", _position) + "]" + 
////					"velocity=[" + _velocity + "]" + 
//					"effort=[" + _effort + "]" +  "]";
			}
			public override string ToYAMLString() {
				return null;
//				return "{\"header\" :" + _header.ToYAMLString() +
////					"\"name\" :" + _name +
////					"\"position\" :" + _position +
////					"\"velocity\" :" + _velocity +
//					"\"effort\" :" + _effort + "}";
			}

		}
	}
}
