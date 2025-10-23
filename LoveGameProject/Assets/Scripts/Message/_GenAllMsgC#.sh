#!/bin/base
echo "gen start"
basepath=$(cd `dirname $0`; pwd)
cd $basepath/ProtoFiles/Messge/
for file in $(ls ./)
do
	if [ "${file##*.}" = "proto" ];then
		if [ "${file}" != "pigeon.proto" ];then
			echo "gen ${file}"
			$basepath/tool/protoc.exe --csharp_out=$basepath/GenCode ${file} 
		fi
	fi
done

echo "finish"