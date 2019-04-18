# Control your Anki Vector with .NET

This API matches the Alpha 0.5.1 release for Python found [here](https://github.com/anki/vector-python-sdk)\
![Vector](https://raw.githubusercontent.com/anki/vector-python-sdk/master/docs/source/images/vector-sdk-alpha.jpg)

## Getting Started
### Install from nuget
https://www.nuget.org/packages/Vector/

Make sure you have connect your Vector robot to you wifi using the [Vector companion app](https://play.google.com/store/apps/details?id=com.anki.vector)

Vector requires all requests be authorized by an authenticated Anki user\
This script will enable this device to authenticate with your Vector robot for use with a Vector Python SDK program.

## Basic code examples
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

//say something
await robot.Audio.SayTextAsync("all done");

//disconnect
await robot.DisconnectAsync();
```

## First time connection
If its your first time connection to your vector with this API you will need to grant your device API access
```cs
//grant this device api access - one time only
await robot.GrantApiAccessAsync("[your robot Name]", "[ip address]", "[serial num]", "[user account]", "[password]");
```

by default the connection info is stored automaticly for you and you can reconnect using
```cs
await robot.ConnectAsync("[your robot name]");
```

For up to a year before your authentication token in the connection info expires, and you will need to grant new access.

## Suppress its personality
Sometimes you want Vector to act like a zombie drone and do exactly what you say.  to do this you can suppress its personality
```cs
robot.OnSuppressPersonality += (sender, e) =>
{
    //do stuff each time its personality is suppressed
};
robot.StartSuppressingPersonality();
```

## Get the camera feed
The camera feed comes back as a stream of System.Drawing.Image
##### Return on same thread
```cs
robot.Camera.ImageReceived += (sender, e) =>
{
	//show my image.  NOTE: on the *same* thread
};
robot.Camera.CameraFeedAsync().ThrowFeedException();
```
##### Return on its own thread
```cs
robot.Camera.ImageReceived += (sender, e) =>
{
	//show my image.  NOTE: on a *diffrent* thread
};
robot.Camera.StartCameraFeed();
```

## About events
All events comming back from the robot come through an async feed.  you must activate these feeds to start receving events.  
These include events for getting the robots position changes, internal map changes, camera, voice and object recognition and others.
All event feeds have an async way to start the feed, and a syncronous way to start the feed.  the sycronous ways starts it own thread to manage the events.  **This means that the event handler will be called on a diffrent thread.**
All syncronous methods to start event feeds begin with `Start` and `Stop`

example: `robot.StartEventListening()` and `robot.StopEventListening()`

While async methods are missing the start and stop prefix

example: `robot.EventListeningAsync()`

Running the async version of the event feed **means that event handlers will be call on the same thread.**
When running the async event feed its inportant to handle exceptions, otherwize you will not be notified when the feed shuts down.  You can chain togeather event handlers using the `.ContinueWith` method on the task.
I recomend placing `.ThrowFeedException()` at the end of your exception handling chain.  This will throw an exception for any remaining unhandled exceptions.  

*The `.ThrowFeedException()` is a task extention method you can enabled with `using Vector;` namespace*

### How to run everything on a single thread
To start all event handlers on the same thread call
```cs
//start all event listeners on the *same* thread
robot.SuppressPersonalityAsync().ThrowFeedException();
robot.Camera.CameraFeedAsync().ContinueWith(RestartCamera).ThrowFeedException();
robot.World.MapFeedAsync().ThrowFeedException();
robot.EventListeningAsync().ThrowFeedException();
robot.Audio.AudioFeedAsync().ThrowFeedException();
```

To start all event handlers on their own thread call
```cs
//start all event listeners on *their own* thread
robot.StartSuppressingPersonality();
robot.Camera.StartCameraFeed();
robot.World.StartMapFeed();
robot.StartEventListening();
robot.Audio.StartAudioFeed();
```

## How to call things without async
Almost every signature in the API is a async method.  Its highly recomended to use the async signature, but if you want you can call an async method syncronously
```cs
//works well if your method has no return value
robot.Audio.SayTextAsync("synchronous method call").Wait();

//works well if you method returns something
var animations = robot.Animation.ListAsync().Result;
```

## Show an image on the screen
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