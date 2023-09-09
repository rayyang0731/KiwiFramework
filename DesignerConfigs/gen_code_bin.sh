#!/bin/zsh

dotnet ./Tools/Luban.ClientServer/Luban.ClientServer.dll -j cfg --\
 -d ./Defines/__root__.xml \
 --input_data_dir ./Excel \
 --output_code_dir ../Assets/Scripts/Table \
 --output_data_dir ../Assets/GameAssets/Configs \
 --gen_types code_cs_bin,data_bin \
 -s all 
