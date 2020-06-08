# Server Setup

Look at Radiation Visualization's Hololens Testing.md

## Setting up PCFace messages

1. Create a Catkin workspace if you have not already
1. `cd src/`
1. Create a package called `rntools`
    1. `catkin_create_pkg rntools std_msgs rospy roscpp`
1. `cd rntools`
1. `mkdir msg`
1. Place `PCFace.msg` inside the `msg` directory
1. Follow the instructions in [this tutorial](http://wiki.ros.org/ROS/Tutorials/CreatingMsgAndSrv#Creating_a_msg) to modify `package.xml` and `CMakeLists.txt`
1. Return to your catkin workspace root directory, (ex. `cd ~/catkin_ws`)
1. `catkin build`
1. `source devel/setup.bash`
1. Verify that everything works with `rosmsg show PCFace`. You should see a listing under the `rntools` package.
