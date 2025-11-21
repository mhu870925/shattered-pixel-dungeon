#!/usr/bin/env python3
"""
Utility for turning a short MP4 clip into a Shattered Pixel Dungeon-friendly sprite sheet
and action metadata.

The script expects `ffmpeg` and `ffprobe` to be installed. It samples frames from the
video, scales/pads them to a square tile size, assembles a sprite sheet using ffmpeg's
`tile` filter, and writes a small JSON manifest describing the produced asset.
"""

from __future__ import annotations

import argparse
import json
import math
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Iterable, List


class CommandError(RuntimeError):
    """Raised when an external command fails."""


def _run(cmd: List[str]) -> subprocess.CompletedProcess[str]:
    result = subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    if result.returncode != 0:
        raise CommandError("Command failed: {}\n{}".format(" ".join(cmd), result.stderr.strip()))
    return result


def _require_binaries() -> None:
    missing = [binary for binary in ("ffmpeg", "ffprobe") if not shutil.which(binary)]
    if missing:
        raise SystemExit(
            "Missing required binaries: {}. Install ffmpeg to continue.".format(", ".join(missing))
        )


def _probe_duration(video_path: Path) -> float:
    result = _run(
        [
            "ffprobe",
            "-v",
            "error",
            "-show_entries",
            "format=duration",
            "-of",
            "default=noprint_wrappers=1:nokey=1",
            str(video_path),
        ]
    )
    try:
        return float(result.stdout.strip())
    except ValueError as exc:
        raise CommandError("Unable to read video duration from ffprobe output") from exc


def _extract_frames(video_path: Path, dest_dir: Path, fps: int, frame_size: int, max_frames: int) -> List[Path]:
    dest_dir.mkdir(parents=True, exist_ok=True)
    vf_filter = "".join(
        (
            f"fps={fps},",
            f"scale={frame_size}:{frame_size}:force_original_aspect_ratio=decrease,",
            f"pad={frame_size}:{frame_size}:(ow-iw)/2:(oh-ih)/2:color=0x00000000",
        )
    )
    cmd = [
        "ffmpeg",
        "-y",
        "-i",
        str(video_path),
        "-vf",
        vf_filter,
        "-vframes",
        str(max_frames),
        str(dest_dir / "frame-%03d.png"),
    ]
    _run(cmd)
    return sorted(dest_dir.glob("frame-*.png"))


def _build_sheet(frame_paths: Iterable[Path], cols: int, output_path: Path) -> None:
    frame_list = list(frame_paths)
    if not frame_list:
        raise CommandError("No frames were extracted; cannot build sprite sheet")

    rows = math.ceil(len(frame_list) / cols)
    tile_filter = f"tile={cols}x{rows}"
    cmd = [
        "ffmpeg",
        "-y",
        "-i",
        str(frame_list[0].parent / "frame-%03d.png"),
        "-vf",
        tile_filter,
        str(output_path),
    ]
    _run(cmd)


def _write_manifest(output_dir: Path, action_name: str, frame_size: int, fps: int, frame_count: int, columns: int) -> None:
    rows = math.ceil(frame_count / columns)
    manifest = {
        "action": action_name,
        "frame_size": frame_size,
        "frame_rate": fps,
        "frame_count": frame_count,
        "sheet": output_dir.name + "/" + "sheet.png",
        "sheet_columns": columns,
        "sheet_rows": rows,
        "frames": [f"frame-{idx:03d}.png" for idx in range(1, frame_count + 1)],
    }
    manifest_path = output_dir / "action.json"
    manifest_path.write_text(json.dumps(manifest, indent=2), encoding="utf-8")


def parse_args(argv: Iterable[str]) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("video", type=Path, help="Path to an 8-second MP4 clip")
    parser.add_argument("--action-name", default="custom_action", help="Name for the generated action")
    parser.add_argument(
        "--output",
        type=Path,
        default=Path("build") / "video-actions",
        help="Root directory where assets will be written",
    )
    parser.add_argument("--fps", type=int, default=12, help="Sampling rate for frames")
    parser.add_argument("--frame-size", type=int, default=16, help="Square sprite size in pixels")
    parser.add_argument(
        "--max-frames",
        type=int,
        default=48,
        help="Upper bound on frames to keep from the clip to avoid oversized sheets",
    )
    parser.add_argument(
        "--sheet-columns", type=int, default=8, help="How many frames to place per row in the sprite sheet"
    )
    return parser.parse_args(argv)


def main(argv: Iterable[str]) -> int:
    args = parse_args(argv)
    _require_binaries()

    if not args.video.exists():
        raise SystemExit(f"Input video not found: {args.video}")

    duration = _probe_duration(args.video)
    frame_budget = min(args.max_frames, math.ceil(duration * args.fps))

    output_dir = args.output / args.action_name
    frames_dir = output_dir / "frames"
    sheet_path = output_dir / "sheet.png"

    try:
        frame_paths = _extract_frames(args.video, frames_dir, args.fps, args.frame_size, frame_budget)
        if not frame_paths:
            raise SystemExit("No frames produced from the input video")

        columns = max(1, args.sheet_columns)
        _build_sheet(frame_paths, columns, sheet_path)
        _write_manifest(output_dir, args.action_name, args.frame_size, args.fps, len(frame_paths), columns)
    except CommandError as exc:
        raise SystemExit(str(exc)) from exc

    print(f"Created sprite sheet: {sheet_path}")
    print(f"Frames saved to: {frames_dir}")
    print(f"Manifest written to: {output_dir / 'action.json'}")
    print("Tip: copy the sheet and manifest into your assets directory and reference the frame metadata for animations.")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
