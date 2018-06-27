using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib {
	namespace interface_msgs {
		public class ObstacleMsg : ROSBridgeMsg {
			public float _x, _y, _z, scale_x;
            public int id;

			public ObstacleMsg(JSONNode msg) {
				_x = float.Parse(msg["pose"]["position"]["x"]);
				_y = float.Parse(msg["pose"]["position"]["y"]);
				_z = float.Parse(msg["pose"]["position"]["z"]);
                scale_x = float.Parse(msg["scale"]["x"]);
                id = int.Parse(msg["id"]); ;
            }

			public ObstacleMsg(float x, float y, float z) {
			    _x = x;
				_y = y;
				_z = z;
                scale_x = 1;
			}
		}
	}
}
