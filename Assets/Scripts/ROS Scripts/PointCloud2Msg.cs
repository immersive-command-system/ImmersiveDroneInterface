using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;
using PointCloud;
using System.Text;
using ROSBridgeLib.sensor_msgs;

/**
 * Define a PointCloud2 message.
 *  
 * @author Miquel Massot Campos
 */

public class PointCloud2Msg : ROSBridgeMsg {
	private HeaderMsg _header;
	private uint _height;
	private uint _width;
	private PointFieldMsg[] _fields;
	private bool _is_bigendian;
	private bool _is_dense;
	private uint _point_step;
	private uint _row_step;
	private byte[] _data;
// TODO Change cloud data type depending on field type
    private PointCloud<PointXYZRGBAIntensity> _cloud;

    public PointCloud2Msg(JSONNode msg) {
		_header = new HeaderMsg (msg ["header"]);
		_height = uint.Parse(msg ["height"]);
		_width = uint.Parse(msg ["width"]);
		_is_bigendian = msg["is_bigendian"].AsBool;
		_is_dense = msg["is_dense"].AsBool;
		_point_step = uint.Parse(msg ["point_step"]);
		_row_step = uint.Parse(msg ["row_step"]);
        _fields = new PointFieldMsg[msg["fields"].Count];
        for (int i = 0; i < _fields.Length; i++)
        {
            _fields[i] = new PointFieldMsg(msg["fields"][i]);
        }
        if (msg["data"] == null)
        {
            _cloud = new PointCloud<PointXYZRGBAIntensity>();
        } else
        {
            _data = System.Convert.FromBase64String(msg["data"]);
            _cloud = ReadData(_data);
        }
    }

	public PointCloud2Msg(HeaderMsg header, uint height, uint width, PointFieldMsg[] fields, bool is_bigendian, uint point_step, uint row_step, byte[] data, bool is_dense) {
		_header = header;
		_height = height;
		_width = width;
        _fields = fields;
        _is_dense = is_dense;
		_is_bigendian = is_bigendian;
		_point_step = point_step;
		_row_step = row_step;
		_cloud = ReadData(data);
	}

    private PointCloud<PointXYZRGBAIntensity> ReadData(byte[] byteArray)
    {
        return ReadDataRGBAIntensity(byteArray);
    }

    private PointCloud<PointXYZRGB> ReadDataRGB(byte[] byteArray) {
        PointCloud<PointXYZRGB> cloud = new PointCloud<PointXYZRGB> ();
        for (int i = 0; i < _width * _height; i++) {
            float x = System.BitConverter.ToSingle(_data, i * (int)_point_step + 0);
            float y = System.BitConverter.ToSingle(_data, i * (int)_point_step + 4);
            float z = System.BitConverter.ToSingle(_data, i * (int)_point_step + 8);
            float rgb = System.BitConverter.ToSingle(_data, i * (int)_point_step + 16);
            if (!float.IsNaN(x + y + z))
            {
                PointXYZRGB p = new PointXYZRGB(x, y, z, rgb);
                cloud.Add(p);
            }   
        }
        return cloud;
    }

   	private PointCloud<PointXYZRGBAIntensity> ReadDataRGBAIntensity(byte[] byteArray) {
		PointCloud<PointXYZRGBAIntensity> cloud = new PointCloud<PointXYZRGBAIntensity> ();
        int temp = byteArray.Length / (int)_point_step;
        for (int i = 0; i < _width * _height; i++) {
            float x = System.BitConverter.ToSingle(_data, i * (int)_point_step + 0);
            float y = System.BitConverter.ToSingle(_data, i * (int)_point_step + 4);
            float z = System.BitConverter.ToSingle(_data, i * (int)_point_step + 8);
            uint rgba = System.BitConverter.ToUInt32(_data, i * (int)_point_step + 12);
            float intensity = System.BitConverter.ToSingle(_data, i * (int)_point_step + 12);
            if (!(float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z)))
            {
             
                PointXYZRGBAIntensity p = new PointXYZRGBAIntensity(x, y, z, rgba, intensity);
                cloud.Add(p);
            }
		}
        return cloud;
	}

   	private PointCloud<PointXYZRGBA> ReadDataRGBA(byte[] byteArray) {
		PointCloud<PointXYZRGBA> cloud = new PointCloud<PointXYZRGBA> ();
        int temp = byteArray.Length / (int)_point_step;
        for (int i = 0; i < _width * _height; i++) {
            float x = System.BitConverter.ToSingle(_data, i * (int)_point_step + 0);
            float y = System.BitConverter.ToSingle(_data, i * (int)_point_step + 4);
            float z = System.BitConverter.ToSingle(_data, i * (int)_point_step + 8);
            uint rgba = System.BitConverter.ToUInt32(_data, i * (int)_point_step + 12);
            if (!(float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z)))
            {
                PointXYZRGBA p = new PointXYZRGBA(x, y, z, rgba);
                //Debug.Log(x + ", " + y + ", " + z);
                cloud.Add(p);
            }
            //Debug.Log(i + "/" + _width * _height + ", " + temp);
		}
        //Debug.Log("Done1");
        return cloud;
	}

	private PointCloud<PointXYZIntensity> ReadDataIntensity(byte[] byteArray) {
		PointCloud<PointXYZIntensity> cloud = new PointCloud<PointXYZIntensity> ();
        int temp = byteArray.Length / (int)_point_step;
        for (int i = 0; i < _width * _height; i++) {
            float x = System.BitConverter.ToSingle(_data, i * (int)_point_step + 0);
            float y = System.BitConverter.ToSingle(_data, i * (int)_point_step + 4);
            float z = System.BitConverter.ToSingle(_data, i * (int)_point_step + 8);
            float intensity = System.BitConverter.ToSingle(_data, i * (int)_point_step + 12);
            if (!(float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) || float.IsNaN(intensity)))
            {
                PointXYZIntensity p = new PointXYZIntensity(x, y, z, intensity);
                //Debug.Log(x + ", " + y + ", " + z);
                cloud.Add(p);
            }
            //Debug.Log(i + "/" + _width * _height + ", " + temp);
		}
        //Debug.Log("Done1");
        return cloud;
	}

    public HeaderMsg GetHeader()
    {
        return _header;
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

	public PointCloud<PointXYZRGBAIntensity> GetCloud() {
		return _cloud;
	}

	public static string GetMessageType() {
		return "sensor_msgs/PointCloud2";
	}

	public override string ToString() {
		return "PointCloud2 [header=" + _header.ToString() +
				"height=" + _height +
				"width=" + _width +
				//"fields=" + _fields.ToString() +
				"is_bigendian=" + _is_bigendian +
				"is_dense=" + _is_dense +
				"point_step=" + _point_step +
				"row_step=" + _row_step + "]";
	}

	public override string ToYAMLString() {
		return "{\"header\" :" + _header.ToYAMLString() +
				"\"height\" :" + _height +
				"\"width\" :" + _width +
				//"\"fields\" :" + _fields.ToYAMLString() +
				"\"is_bigendian\" :" + _is_bigendian +
				"\"is_dense\" :" + _is_dense +
				"\"point_step\" :" + _point_step +
				"\"row_step\" :" + _row_step + "}";
	}

    public string GetFieldString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (PointFieldMsg field in _fields)
        {
            sb.AppendFormat("Name: {0}\n\tOffset: {1}\n\tData Type: {2}\n\tCount: {3}\n", field.GetName(), field.GetOffset(), GetFieldTypeString(field.GetDatatype()), field.GetCount());
        }
        return sb.ToString();
    }

    private static string GetFieldTypeString(byte typeNumber)
    {
        switch (typeNumber)
        {
            case 1:
                return "INT8";
            case 2:
                return "UINT8";
            case 3:
                return "INT16";
            case 4:
                return "UINT16";
            case 5:
                return "INT32";
            case 6:
                return "UINT32";
            case 7:
                return "FLOAT32";
            case 8:
                return "FLOAT64";
            default:
                return "UKNOWN";
        }
    }
}