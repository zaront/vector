# Control your Anki Vector with .NET

this API matches the Alpha 0.5.0 release for Python found [here](https://github.com/anki/vector-python-sdk)\
![Vector](https://raw.githubusercontent.com/anki/vector-python-sdk/master/docs/source/images/vector-sdk-alpha.jpg)

## Getting Started
**Install from nuget**\
https://www.nuget.org/packages/Vector/

make sure your Vector in 

**basic example**
```cs
public class test
{
}
```

## How to regenerate the gRPC
Vector uses a gRPC service definition to communicate.  You can regenerate this definition as their firmware updates for their robot.

To do this, first install this extention for Visual Studio:\
[Protobuf Generator](https://marketplace.visualstudio.com/items?itemName=jonasjakobsson.ProtobufGeneratorvisualstudio)\
This will install the gRPC generator tools into your nuget package location.

Next, in the same parent directory that you have cloned this repository, also clone the following:\
`git clone https://github.com/googleapis/googleapis.git`\
`git clone https://github.com/anki/vector-python-sdk.git`

Now you can run the following script to regenerate the **Vector.Communication** files\
`GenerateRPC.bat`