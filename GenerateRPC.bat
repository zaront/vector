echo Generates C# interfaces for Vector
echo off
set grpcToolRootDir=%USERPROFILE%\.nuget\packages\grpc.tools\1.17.1
set toolsDir=%grpcToolRootDir%\tools\windows_x64
set includeDir=%grpcToolRootDir%\build\native\include
set googleApiDir=..\googleapis
set vectorRootDir=..\vector-python-sdk
set vectorDir=%vectorRootDir%\anki_vector\messaging
set outputDir=.\Vector.Communication

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\messages.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\cube.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\response_status.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\extensions.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\alexa.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\behavior.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\nav_map.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\settings.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\shared.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

%toolsDir%\protoc.exe -I"%vectorRootDir%" -I"%toolsDir%" -I"%includeDir%" -I"%googleApiDir%" --csharp_out %outputDir% --grpc_out %outputDir% %vectorDir%\external_interface.proto --plugin=protoc-gen-grpc=%toolsDir%\grpc_csharp_plugin.exe

pause
