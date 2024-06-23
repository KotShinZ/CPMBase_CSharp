#!/bin/bash

# Check if ffmpeg command exists
if ! command -v ffmpeg &> /dev/null; then
    echo "Error: ffmpeg command not found. Please install ffmpeg before running this script."
    exit 1
fi

# 引数の確認
if [ -z "$1" ]; then
    echo "出力ディレクトリのパスを指定してください。"
    echo "例: ./create_video.sh /path/to/output"
    exit 1
fi



# 出力ディレクトリの設定
outputDir="$1"

# 出力ファイル名の設定
outputFile="output_video.mp4"

# フレームレートの設定
framerate=30

file="image%d.png"
full_path="$outputDir/$file"

echo "$outputDir/image%d.png"

# ffmpegコマンドを使用して動画を生成
ffmpeg -y -framerate $framerate -i "$outputDir/image%d.png" -c:v libx264 -pix_fmt yuv420p "$outputDir/$outputFile"

echo "動画が生成されました: $outputDir/$outputFile"
