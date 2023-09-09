#!/bin/zsh

dotnet ./Tools/Luban.ClientServer/Luban.ClientServer.dll -j cfg --generateonly --\
 -d ./Defines/__root__.xml \
 --input_data_dir ./Excel \
 --output_data_dir ./GenerateDatas/json \
 --gen_types data_bin \
 -s all 
