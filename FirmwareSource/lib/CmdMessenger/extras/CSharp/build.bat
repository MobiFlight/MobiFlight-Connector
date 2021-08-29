CLS

'Just clean and build all projects here, even the examples just to simplify things
msbuild CmdMessenger.sln  /t:clean /p:Configuration=Release
msbuild CmdMessenger.sln  /t:Build /p:Configuration=Release

'Create nuget packages
nuget pack ./CommandMessenger/CommandMessenger.csproj -IncludeReferencedProjects -Prop Configuration=Release -Prop Platform=AnyCPU -Symbols
nuget pack ./CommandMessenger.Transport.Bluetooth/CommandMessenger.Transport.Bluetooth.csproj -IncludeReferencedProjects -Prop Configuration=Release -Prop Platform=AnyCPU -Symbols
nuget pack ./CommandMessenger.Transport.Network/CommandMessenger.Transport.Network.csproj -IncludeReferencedProjects -Prop Configuration=Release -Prop Platform=AnyCPU -Symbols
nuget pack ./CommandMessenger.Transport.Serial/CommandMessenger.Transport.Serial.csproj -IncludeReferencedProjects -Prop Configuration=Release -Prop Platform=AnyCPU -Symbols


