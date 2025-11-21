#!/usr/bin/env python3
"""Helper to produce a Windows-friendly executable of video_action_builder."""

from __future__ import annotations

import shutil
import subprocess
import sys
from pathlib import Path


def main() -> int:
    if shutil.which("pyinstaller") is None:
        raise SystemExit("PyInstaller is required. Install with `pip install pyinstaller`.")

    repo_root = Path(__file__).resolve().parent.parent
    script_path = repo_root / "tools" / "video_action_builder.py"
    dist_dir = repo_root / "build" / "exe"
    work_dir = dist_dir / "_pyinstaller"

    dist_dir.mkdir(parents=True, exist_ok=True)

    cmd = [
        "pyinstaller",
        "--clean",
        "--noconfirm",
        "--onefile",
        "--name",
        "video_action_builder",
        f"--distpath={dist_dir}",
        f"--workpath={work_dir}",
        str(script_path),
    ]

    subprocess.run(cmd, check=True)
    executable = dist_dir / "video_action_builder.exe"
    print(f"Executable written to: {executable}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
