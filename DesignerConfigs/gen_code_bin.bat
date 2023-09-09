set GEN_CLIENT=.\Tools\Luban.ClientServer\Luban.ClientServer.exe

%GEN_CLIENT% -j cfg --^
 -d .\Defines\__root__.xml ^
 --input_data_dir .\Excel ^
 --output_code_dir ..\Assets\Scripts\Table ^
 --output_data_dir ..\Assets\GameAssets\Configs ^
 --gen_types code_cs_unity_bin,data_bin ^
 -s all 

pause