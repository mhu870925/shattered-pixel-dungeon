# Video-to-action helper

The `tools/video_action_builder.py` script turns a short MP4 (ideally ~8 seconds) into a Shattered Pixel Dungeon-friendly sprite sheet and manifest. It wraps `ffmpeg` and `ffprobe` to keep the workflow consistent.

The defaults assume a typical 1280×720 source clip and downscale each sampled frame into a small square (16×16 by default) while preserving the original aspect ratio and padding to fit the target size.

Prefer a GUI? Run `python tools/video_action_builder_ui.py` to open a simple desktop window where you can browse for the MP4, tweak the parameters, and launch the conversion without the command line. The UI surfaces the same defaults as the CLI and displays the output locations when the build finishes.

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

For a 1280×720 clip, the command above will automatically shrink and center the frames into the 16×16 tiles while keeping the 12 fps cadence (up to 48 frames total).

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

> **Where is the `.exe` created?** The helper always writes the executable to `build/exe/video_action_builder.exe` relative to the repo root and prints the path after a successful build.

## Tips for Shattered Pixel Dungeon assets
- Keep `--frame-size` small (16–24px) to match the pixel density of the game's sprite sheets.
- Lower `--fps` or `--max-frames` if the action feels too long compared to in-game timings.
- The generated `action.json` can be used as-is or adapted to whichever resource pipeline you use for custom actions.

## How to download and run
You can either run the Python script directly or build a Windows-friendly executable.

### 1) Get the sources
- Clone the repo: `git clone https://github.com/YourOrg/shattered-pixel-dungeon.git`
- Or download a ZIP of the repo from GitHub, then extract it.

### 2) Run with Python (any OS)
1. Install Python 3.8+ and make sure `ffmpeg`/`ffprobe` are on your PATH.
2. From the repo root, run the tool (example values shown):
   ```bash
   python tools/video_action_builder.py path/to/clip.mp4 \
     --action-name fireball_dash \
     --output build/video-actions \
     --fps 12 \
     --frame-size 16 \
     --max-frames 48 \
     --sheet-columns 8
   ```
3. Find the generated frames, sprite sheet, and `action.json` under `build/video-actions/<action-name>/`.

### 3) Build and run the Windows `.exe`
1. On Windows, install Python and `ffmpeg`, then install PyInstaller: `pip install pyinstaller`.
2. From the repo root, run `python tools/build_video_action_builder_exe.py`. The executable is written to `build/exe/video_action_builder.exe`.
3. Invoke the executable the same way you would the Python script. Example from PowerShell:
   ```powershell
   .\build\exe\video_action_builder.exe .\path\to\clip.mp4 \
     --action-name fireball_dash \
     --output .\build\video-actions \
     --fps 12 \
     --frame-size 16 \
     --max-frames 48 \
     --sheet-columns 8
   ```
