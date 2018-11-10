#!/bin/bash

cd ../..
cp -r Boa BoaServer
rm -rf BoaServer/Assets
ln -s $(pwd)/Boa/Assets $(pwd)/BoaServer/Assets