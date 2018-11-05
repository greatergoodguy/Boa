cd ../..
xcopy /e/h Boa BoaServer
rmdir /S /Q BoaServer\Assets
mklink /j BoaServer\Assets Boa\Assets
pause