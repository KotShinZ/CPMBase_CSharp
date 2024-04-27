# 第一引数を出力ディレクトリの末尾のパスとして設定
exampleName="$1"

# 出力ディレクトリの設定
outputDir="/workspaces/CPMBase_CSharp/Output/$exampleName"

# 出力ファイル名の設定
outputFile="output_video.mp4"

# フレームレートの設定
framerate=30

# ffmpegコマンドを使用して動画を生成
ffmpeg -y -framerate $framerate -i "$outputDir/%d_image.png" -c:v libx264 -pix_fmt yuv420p "$outputDir/$outputFile"

echo "動画が生成されました: $outputDir/$outputFile"