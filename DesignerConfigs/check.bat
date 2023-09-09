set GEN_CLIENT=.\Tools\Luban.ClientServer\Luban.ClientServer.exe

%GEN_CLIENT% -j cfg --generateonly --^
 -d .\Defines\__root__.xml ^
 --input_data_dir .\Excel ^
 --output_data_dir dummy ^
 --gen_types data_json ^
 -s all
pause