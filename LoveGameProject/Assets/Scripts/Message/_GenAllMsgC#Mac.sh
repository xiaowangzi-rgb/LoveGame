#!/bin/base
echo "gen start"
basepath=$(cd `dirname $0`; pwd)
cd $basepath/MM_ProtoFiles/Messge/
for file in $(ls ./)
do
	if [ "${file##*.}" = "proto" ];then
		if [ "${file}" != "pigeon.proto" ];then
			echo "gen ${file}"
			if [ "${file}" = "common.proto" -o "${file}" = "ability.proto" -o "${file}" = "logupload.proto" -o "${file}" = "levelInfo.proto" -o "${file}" = "notify.proto" -o "${file}" = "userDataNative.proto" -o "${file}" = "abtest.proto" -o "${file}" = "checkversion.proto" -o "${file}" = "levelabconfig.proto" -o "${file}" = "productorder.proto" -o "${file}" = "skin.proto" -o "${file}" = "tag.proto" -o "${file}" = "leveldifficultydynamic.proto" -o "${file}" = "racelevelactivity.proto" -o "${file}" = "battlepass.proto" -o "${file}" = "guildgift.proto" -o "${file}" = "configs.proto" -o "${file}" = "rankactivity.proto" -o "${file}" = "activity.proto" -o "${file}" = "whitelist.proto" -o "${file}" = "cheat.proto" -o "${file}" = "ability.proto" -o "${file}" = "lightingrush.proto" -o "${file}" = "spacemissionactivity.proto" -o "${file}" = "battlepass.proto" -o "${file}" = "rank.proto" -o "${file}" = "user.proto"  -o "${file}" = "guildBase.proto" ];then
				/usr/local/bin/protoc --csharp_out=$basepath/../../Assets/MethodScrpit/Message/ProtoBuf ${file}
			else
				/usr/local/bin/protoc --csharp_out=$basepath/../NetWork/ProtoBuf ${file}
			fi
		fi
	fi
done

echo "finish"