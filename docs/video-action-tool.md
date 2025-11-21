# Video-to-action helper

The `tools/video_action_builder.py` script turns a short MP4 (ideally ~8 seconds) into a Shattered Pixel Dungeon-friendly sprite sheet and manifest. It wraps `ffmpeg` and `ffprobe` to keep the workflow consistent.

## Prerequisites
- Python 3.8+
- `ffmpeg` and `ffprobe` available on your PATH

## Basic usage
```bash
python tools/video_action_builder.py path/to/clip.mp4 \
  --action-name fireball_dash \
  --output build/video-actions \
  --fps 12 \
  --frame-size 16 \
  --max-frames 48 \
  --sheet-columns 8
```

Outputs are placed under `build/video-actions/<action-name>/`:
- `frames/` contains the scaled frame PNGs.
- `sheet.png` stitches the frames into an atlas using the requested column count.
- `action.json` documents the frame count, size, frame rate, and sheet layout for easy wiring into an animation.

## Building a Windows `.exe`
If you want a standalone executable for Windows, install [PyInstaller](https://pyinstaller.org/) and run:

```bash
pip install pyinstaller
python tools/build_video_action_builder_exe.py
```

The build drops `video_action_builder.exe` into `build/exe/`. PyInstaller needs to run on Windows to emit a native `.exe`; the helper script simply standardizes the build options.

## Tips for Shattered Pixel Dungeon assets
- Keep `--frame-size` small (16–24px) to match the pixel density of the game's sprite sheets.
- Lower `--fps` or `--max-frames` if the action feels too long compared to in-game timings.
- The generated `action.json` can be used as-is or adapted to whichever resource pipeline you use for custom actions.
