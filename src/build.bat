@echo off
dotnet publish SharpMC.Server -p:PublishSingleFile=true -o dist --self-contained -r win-x64
