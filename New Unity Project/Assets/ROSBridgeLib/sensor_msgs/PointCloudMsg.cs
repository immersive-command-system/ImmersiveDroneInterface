using System.Collections;
using System;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;

/**
 * Define a point cloud message.
 * 
 * @author Mathias Ciarlo Thorstensen
 */

namespace ROSBridgeLib {
		namespace sensor_msgs {
				public class PointCloudMsg : ROSBridgeMsg {
						private static bool print = true;
						private HeaderMsg _header;
						private uint _width, _height;
						private uint _point_step, _row_step;
						private uint _num_points;

						private byte[] _data;

						public PointCloudMsg (JSONNode msg) {
								_header = new HeaderMsg (msg ["header"]);

								_width = uint.Parse(msg["width"]);
								_height = uint.Parse(msg["height"]);
								_point_step = uint.Parse(msg["point_step"]);
								_row_step = uint.Parse(msg["row_step"]);
								_num_points = _width * _height;

								_data = System.Convert.FromBase64String(msg ["data"]); // Converts the JSONNode to a byte array

								if (print) {
										string time = FromUnixTime(_header.GetTimeMsg().GetSecs()).ToString("hh:mm:ss");

										int middleIndex = (int)(_point_step * _width * (int)_height / 2 + (int)_width / 2 * _point_step);
										float middleZ = System.BitConverter.ToSingle (_data,  middleIndex + 8);
										Debug.Log ("Time: " + time + " Middle z: " + middleZ);
										/*
										// Printing points for the first row
										for (int i = 0; i < (int)_width; i++) {
												float x = System.BitConverter.ToSingle (_data, i * 16 + 0);
												float y = System.BitConverter.ToSingle (_data, i * 16 + 4);
												float z = System.BitConverter.ToSingle (_data, i * 16 + 8);

												double xd = System.Math.Round (System.Convert.ToDouble (x), 2);
												double yd = System.Math.Round (System.Convert.ToDouble (y), 2);
												double zd = System.Math.Round (System.Convert.ToDouble (z), 4);
											

												// Print point if not NaN
												if (x == x | y == y | z == z) {
														Debug.Log ("Point " + i + ", xyz= " + xd + ", " + yd + ", " + zd);
												}
										}
										*/
										/*
									Debug.Log ("data[0]: " + _data [0]);
									Debug.Log ("data[1]: " + _data [1]);
									Debug.Log ("data[2]: " + _data [2]);
									Debug.Log ("data[3]: " + _data [3]);
									Debug.Log ("data[4]: " + _data [4]);
									Debug.Log ("data[5]: " + _data [5]);
									Debug.Log ("data[6]: " + _data [6]);
									Debug.Log ("data[7]: " + _data [7]);
									*/
										//print = false;
								}
						}

						public static string GetMessageType() {
								return "sensor:msgs/PointCloud2";
						}

						public byte[] GetData()	{
								return _data;
						}

						public uint GetWidth() {
								return _width;
						}

						public uint GetHeight() {
								return _height;
						}

						public uint GetPointStep() {
								return _point_step;
						}

						public uint GetRowStep() {
								return _row_step;
						}

						public uint GetNumPoints() {
								return _num_points;
						}

						public override string ToString() {
								return "Point cloud. dim=" + _width + "x" + _height;
						}

						public DateTime FromUnixTime(long unixTime)
						{
								DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
								return epoch.AddSeconds(unixTime);
						}
				}
		}
}