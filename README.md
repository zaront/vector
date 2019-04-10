# Control your Anki Vector with .NET

This API matches the Alpha 0.5.1 release for Python found [here](https://github.com/anki/vector-python-sdk)\
![Vector](https://raw.githubusercontent.com/anki/vector-python-sdk/master/docs/source/images/vector-sdk-alpha.jpg)

## Getting Started
**Install from nuget**\
https://www.nuget.org/packages/Vector/

Make sure you have connect your Vector robot to you wifi using the [Vector companion app](https://play.google.com/store/apps/details?id=com.anki.vector)

Vector requires all requests be authorized by an authenticated Anki user\
This script will enable this device to authenticate with your Vector robot for use with a Vector Python SDK program.

**basic code example**
```cs
//connect
var robot = new Robot();
await robot.ConnectAsync("[your robot name]"); //example: M9W4

//drive off Charger
await robot.Motors.DriveOffChargerAsync();
await robot.Motors.DriveStraightAsync(50, 50);

//drive in a square
await robot.Motors.DriveStraightAsync(50, 50);
await robot.Motors.TurnInPlaceAsync(1.5708f, 5);
await robot.Motors.DriveStraightAsync(50, 50);
await robot.Motors.TurnInPlaceAsync(1.5708f, 5);
await robot.Motors.DriveStraightAsync(50, 50);
await robot.Motors.TurnInPlaceAsync(1.5708f, 5);
await robot.Motors.DriveStraightAsync(50, 50);
await robot.Motors.TurnInPlaceAsync(1.5708f, 5);

//play an animation
await robot.Animation.PlayAsync("anim_vc_laser_lookdown_01");

//disconnect
await robot.DisconnectAsync();
```

**first time connection**\
If its your first time connection to your vector with this API you will need to grant your device API access
```cs
//grant this device api access - one time only
await robot.GrantApiAccessAsync("[your robot Name]", "[ip address]", "[serial num]", "[user account]", "[password]");
```

by default the connection info is stored automaticly for you and you can reconnect using
```cs
await robot.ConnectAsync("[your robot name]");
```

for up to a year before your authentication token in the connection info expires, and you will need to grant new access.

**suppress its personality**\
Sometimes you want Vector to act like a zombie drone and do exactly what you say.  to do this you can suppress its personality
```cs
robot.OnSuppressPersonality += (sender, e) =>
{
    //do stuff each time its personality is suppressed
};
robot.StartSuppressingPersonality();
```

**get the camera feed**\
the camera feed comes back as a stream of System.Drawing.Image
```cs
robot.Camera.ImageReceived += (sender, e) =>
{
	//show my image.  NOTE: on a diffrent thread
};
robot.Camera.StartCameraFeed();
```

**show an image on the screen**\
images are automaticly resized
```cs
robot.Screen.SetScreenImage(@"c:\mycoolimage.jpg");
```


## How to regenerate the gRPC
Vector uses a gRPC service definition to communicate.  You can regenerate a c# library (Vector.Communication) that matches their service definition using .proto files.  This may need to be regenerated as their firmware evolves for their robot.

To do this, first install this extention for Visual Studio:\
[Protobuf Generator](https://marketplace.visualstudio.com/items?itemName=jonasjakobsson.ProtobufGeneratorvisualstudio)\
This will install the gRPC generator tools into your nuget package location.

Next, in the same parent directory that you have cloned this repository, also clone the following:\
`git clone https://github.com/googleapis/googleapis.git`\
`git clone https://github.com/anki/vector-python-sdk.git`

Now you can run the following script to regenerate the files in the **Vector.Communication** project\
`GenerateRPC.bat`