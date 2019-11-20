# ROSBridgeLib
A Unity library for communicattion with ROS through [RosBridge](http://wiki.ros.org/rosbridge_suite)

The first version of this I believe origins from [Michael Jenkin](https://github.com/michaeljenkin), in the repo [unityros](https://github.com/michaeljenkin/unityros). He has made a sample unity project showing turtlesim, with good instructions on how to use this project. All honor goes to him. I created this project because there was no repository containing the barebone library.

This repository is intended to be imported as a git [submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules).

## Included messages
This repository does not contain every ROS message. If you need to add one, please fork this repository, add the file and make a pull request.

## Documentation
Documentation is in the code. I have added some more in addition to what Michael Jenkin (original
author). The main file is ROSBridgeWebSocketConnection.cs, which sets up everything.

## Example usage
This is an example application where a ball is controlled. Basically, there are three important script types to notice. Firstly, create a main script responsible for initializing RosBridge:

``` cs
public class RollABallRosController : MonoBehaviour {
  private ROSBridgeWebSocketConnection ros = null;
    
  void Start() {
    // Where the rosbridge instance is running, could be localhost, or some external IP
    ros = new ROSBridgeWebSocketConnection ("ws://localhost", 9090);

    // Add subscribers and publishers (if any)
    ros.AddSubscriber (typeof(BallPoseSubscriber));
    ros.AddPublisher (typeof(BallControlPublisher));

    // Fire up the subscriber(s) and publisher(s)
    ros.Connect ();
  }
  
  // Extremely important to disconnect from ROS. Otherwise packets continue to flow
  void OnApplicationQuit() {
    if(ros!=null) {
      ros.Disconnect ();
    }
  }
  // Update is called once per frame in Unity
  void Update () {
    ros.Render ();
  }
}
```

Then, create a subscriber script which will receive updates from a chosen ROS topic
``` cs
// Ball subscriber:
public class RealsenseImageSubscriber : ROSBridgeSubscriber {
  static GameObject ball;

  // These two are important
  public new static string GetMessageTopic() {
    return "/path/to/pose/topic";
  }

  public new static string GetMessageType() {
    return "std_msgs/PoseMsg";
  }

  // Important function (I think, converting json to PoseMsg)
  public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
    return new PoseMsg (msg);
  }

  // This function should fire on each ros message
  public new static void CallBack(ROSBridgeMsg msg) {

    // Update ball position, or whatever
    ball.x = msg.x; // Check msg definition in rosbridgelib
    ball.y = msg.y;
    ball.z = msg.z;
  }
}
```
If you need to publish data to ROS, create a publisher:
``` cs
// Ball publisher: // Using twist msgs for example?
public class BallControlPublisher: ROSBridgePublisher {

  // The following three functions are important
  public static string GetMessageTopic() {
    return "/topic/to/publish/to";
  }

  public static string GetMessageType() {
      return "std_msgs/TwistMsg";
  }

  public static string ToYAMLString(TwistMsg msg) {
    return msg.ToYAMLString();
  }

  public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
    return new TwistMsg(msg);
  }    
}

// And in some other class where the ball is controlled:
TwistMsg msg = new TwistMsg(x, y, z); // Circa

// Publish it (ros is the object defined in the first class)
ros.Publish(BallControlPublisher.GetMessageTopic(), msg);
```

## License
Note: SimpleJSON is included here as a convenience. It has its own licensing requirements. See source code and unity store for details.
