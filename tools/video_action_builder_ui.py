#!/usr/bin/env python3
"""Simple Tkinter UI wrapper around video_action_builder."""

from __future__ import annotations

import threading
import tkinter as tk
from pathlib import Path
from tkinter import filedialog, messagebox, scrolledtext

import video_action_builder


class VideoActionBuilderUI(tk.Tk):
    def __init__(self) -> None:
        super().__init__()
        self.title("Video Action Builder")
        self.resizable(False, False)

        self._build_form()
        self._build_log()
        self._build_actions()

        self._worker: threading.Thread | None = None

    def _build_form(self) -> None:
        form = tk.Frame(self)
        form.pack(padx=12, pady=12, fill=tk.X)

        self.video_var = tk.StringVar()
        self.action_var = tk.StringVar(value="custom_action")
        self.output_var = tk.StringVar(value=str(Path("build") / "video-actions"))
        self.fps_var = tk.StringVar(value="12")
        self.frame_size_var = tk.StringVar(value="16")
        self.max_frames_var = tk.StringVar(value="48")
        self.sheet_cols_var = tk.StringVar(value="8")

        self._add_file_picker(form, "Video (MP4)", self.video_var, self._pick_video)
        self._add_entry(form, "Action name", self.action_var)
        self._add_file_picker(form, "Output dir", self.output_var, self._pick_output, directory=True)
        self._add_entry(form, "FPS", self.fps_var, width=6)
        self._add_entry(form, "Frame size", self.frame_size_var, width=6)
        self._add_entry(form, "Max frames", self.max_frames_var, width=6)
        self._add_entry(form, "Sheet columns", self.sheet_cols_var, width=6)

    def _add_entry(self, parent: tk.Widget, label: str, variable: tk.StringVar, width: int = 32) -> None:
        row = tk.Frame(parent)
        row.pack(fill=tk.X, pady=2)
        tk.Label(row, text=label, width=15, anchor="w").pack(side=tk.LEFT)
        tk.Entry(row, textvariable=variable, width=width).pack(side=tk.LEFT, fill=tk.X, expand=True)

    def _add_file_picker(
        self,
        parent: tk.Widget,
        label: str,
        variable: tk.StringVar,
        command,
        directory: bool = False,
    ) -> None:
        row = tk.Frame(parent)
        row.pack(fill=tk.X, pady=2)
        tk.Label(row, text=label, width=15, anchor="w").pack(side=tk.LEFT)
        tk.Entry(row, textvariable=variable, width=32).pack(side=tk.LEFT, fill=tk.X, expand=True)
        tk.Button(row, text="Browse", command=command).pack(side=tk.LEFT, padx=4)

    def _build_log(self) -> None:
        log_frame = tk.LabelFrame(self, text="Log")
        log_frame.pack(padx=12, pady=(0, 8), fill=tk.BOTH, expand=True)
        self.log = scrolledtext.ScrolledText(log_frame, width=64, height=10, state=tk.DISABLED)
        self.log.pack(fill=tk.BOTH, expand=True, padx=6, pady=6)

    def _build_actions(self) -> None:
        action_frame = tk.Frame(self)
        action_frame.pack(padx=12, pady=(0, 12), fill=tk.X)
        self.status_var = tk.StringVar(value="Idle")
        tk.Label(action_frame, textvariable=self.status_var).pack(side=tk.LEFT)
        tk.Button(action_frame, text="Run", command=self._start_build).pack(side=tk.RIGHT)

    def _pick_video(self) -> None:
        path = filedialog.askopenfilename(filetypes=[("MP4 files", "*.mp4"), ("All files", "*.*")])
        if path:
            self.video_var.set(path)

    def _pick_output(self) -> None:
        path = filedialog.askdirectory()
        if path:
            self.output_var.set(path)

    def _append_log(self, text: str) -> None:
        self.log.configure(state=tk.NORMAL)
        self.log.insert(tk.END, text + "\n")
        self.log.see(tk.END)
        self.log.configure(state=tk.DISABLED)

    def _start_build(self) -> None:
        if self._worker and self._worker.is_alive():
            messagebox.showinfo("Video Action Builder", "A build is already running.")
            return

        try:
            video_path = Path(self.video_var.get()).expanduser()
            action_name = self.action_var.get().strip() or "custom_action"
            output_dir = Path(self.output_var.get()).expanduser()
            fps = int(self.fps_var.get())
            frame_size = int(self.frame_size_var.get())
            max_frames = int(self.max_frames_var.get())
            sheet_cols = int(self.sheet_cols_var.get())
        except ValueError:
            messagebox.showerror("Video Action Builder", "FPS, frame size, max frames, and sheet columns must be numbers.")
            return

        if not video_path:
            messagebox.showerror("Video Action Builder", "Please pick an input video.")
            return

        self.log.configure(state=tk.NORMAL)
        self.log.delete(1.0, tk.END)
        self.log.configure(state=tk.DISABLED)

        self.status_var.set("Working...")
        self._append_log(f"Input video: {video_path}")
        self._append_log(f"Action name: {action_name}")
        self._append_log(f"Output dir: {output_dir}")

        self._worker = threading.Thread(
            target=self._run_build,
            args=(video_path, action_name, output_dir, fps, frame_size, max_frames, sheet_cols),
            daemon=True,
        )
        self._worker.start()

    def _run_build(
        self,
        video_path: Path,
        action_name: str,
        output_dir: Path,
        fps: int,
        frame_size: int,
        max_frames: int,
        sheet_cols: int,
    ) -> None:
        try:
            result = video_action_builder.build_action(
                video_path,
                action_name=action_name,
                output=output_dir,
                fps=fps,
                frame_size=frame_size,
                max_frames=max_frames,
                sheet_columns=sheet_cols,
            )
        except Exception as exc:  # noqa: BLE001
            self.after(0, self._finish_build, None, exc)
            return

        self.after(0, self._finish_build, result, None)

    def _finish_build(self, result: dict | None, error: Exception | None) -> None:
        if error:
            self.status_var.set("Failed")
            self._append_log(str(error))
            messagebox.showerror("Video Action Builder", str(error))
            return

        assert result is not None
        self.status_var.set("Done")
        self._append_log(f"Frames saved to: {result['frames_dir']}")
        self._append_log(f"Sheet: {result['sheet_path']}")
        self._append_log(f"Manifest: {result['manifest_path']}")
        self._append_log(f"Frames rendered: {result['frame_count']}")
        messagebox.showinfo("Video Action Builder", "Build finished. Assets are ready.")


def main() -> int:
    app = VideoActionBuilderUI()
    app.mainloop()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
