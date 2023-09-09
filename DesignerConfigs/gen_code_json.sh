#!/bin/zsh

dotnet ./Tools/Luban.ClientServer/Luban.ClientServer.dll -j cfg --\
 -d ./Defines/__root__.xml \
 --input_data_dir ./Excel \
 --output_code_dir ../Assets/Scripts/Table \
 --output_data_dir ../Assets/GameAssets/Configs \
 --gen_types code_cs_unity_json,data_json \
 -s all
'--l10n: input_text_files ./Excel/i18n.xlsx' \
'--l10n: text_field_name zh_cn' \
'--l10n: output_not_translated_text_file NotLocalized_CN.txt'
