@echo off
setlocal

REM 第一引数から画像ディレクトリのパスを取得
if "%~1"=="" (
    echo 画像ディレクトリのパスを指定してください。
    echo 例: create_video.bat C:\path\to\images
    exit /b 1
)
set "image_dir=%~1"

REM 出力する動画ファイルの名前を設定
set "output_video=output_video.mp4"

REM 出力ファイルの名前を入力フォルダと同じ場所に設定
set "output_video=%image_dir%\output_video.mp4"


REM フレームレートを設定（1秒あたりの画像数）
set "frame_rate=30"

REM FFmpegコマンドを実行して画像を動画に変換
ffmpeg -y -framerate %frame_rate% -i "%image_dir%\%%d_image.png" -c:v libx264 -pix_fmt yuv420p "%output_video%"

echo 動画が作成されました: %output_video%
pause

endlocal