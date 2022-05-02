#!/bin/sh
dotnet publish SharpMC.Server -p:PublishSingleFile=true -o dist --self-contained -r linux-x64
