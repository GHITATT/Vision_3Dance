import argparse
import threading
import subprocess
import time
import cv2
import mediapipe as mp
from flask import Flask, jsonify, Response

latest_pose = []
latest_lock = threading.Lock()
app = Flask(__name__)

@app.route("/")
def index():
    html = """
    <!doctype html>
    <html>
    <head>
      <meta charset="utf-8">
      <title>Pose</title>
      <style>
        body { background:#111; color:#eee; font-family:sans-serif; }
        pre { white-space: pre-wrap; word-wrap: break-word; }
      </style>
      <script>
        async function poll() {
          try {
            const r = await fetch('/pose');
            const d = await r.json();
            document.getElementById('p').textContent = JSON.stringify(d);
          } catch(e) {
            document.getElementById('p').textContent = 'Error';
          }
          setTimeout(poll, 100);
        }
        window.onload = poll;
      </script>
    </head>
    <body>
      <h1>Pose</h1>
      <pre id="p"></pre>
    </body>
    </html>
    """
    return Response(html, mimetype="text/html")

@app.route("/pose")
def pose_endpoint():
    with latest_lock:
        return jsonify(latest_pose)

def run_flask(port):
    app.run(host="0.0.0.0", port=port, debug=False, use_reloader=False)

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("video", nargs="?", default=None)
    parser.add_argument("--port", type=int, default=None)
    parser.add_argument("--scale", type=float, default=1.0)
    args = parser.parse_args()

    # Use different default ports for video vs webcam
    if args.port is None:
        port = 5001 if args.video else 5002
    else:
        port = args.port

    threading.Thread(target=run_flask, args=(port,), daemon=True).start()

    if args.video:
        proc = subprocess.Popen(
            ["ffplay", "-autoexit", "-loglevel", "warning", args.video]
        )
        cap = cv2.VideoCapture(args.video)
        is_video = True
    else:
        proc = None
        cap = cv2.VideoCapture(0)
        is_video = False
    if not cap.isOpened():
        return

    fps = cap.get(cv2.CAP_PROP_FPS)
    if fps <= 0:
        fps = 30
    dt_frame = 1.0 / fps

    pose = mp.solutions.pose.Pose()
    last = time.time()

    # Create window for webcam display if no video file
    if not args.video:
        cv2.namedWindow("Webcam", cv2.WINDOW_AUTOSIZE)

    while True:
        ok, frame = cap.read()
        if not ok:
            break
        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        r = pose.process(rgb)

        if r.pose_landmarks:
            landmarks = r.pose_landmarks.landmark
            
            # # Normalize by head-to-feet distance
            # head_y = landmarks[0].y  # nose
            # left_foot_y = landmarks[27].y  # left ankle
            # right_foot_y = landmarks[28].y  # right ankle
            # foot_y = (left_foot_y + right_foot_y) / 2
            # height = abs(head_y - foot_y)
            
            # Calculate feet Y position to shift body so feet are at y=0
            left_foot_y = landmarks[27].y  # left ankle
            right_foot_y = landmarks[28].y  # right ankle
            foot_y = (left_foot_y + right_foot_y) / 2
            
            if is_video:
                # Invert x-coordinate for video to match Unity's coordinate system
                pts = [[(1.0 - lm.x) * args.scale, (lm.y - foot_y) * args.scale, lm.z] for lm in landmarks]
            else:
                pts = [[lm.x * args.scale, (lm.y - foot_y) * args.scale, lm.z] for lm in landmarks]
        else:
            pts = []

        with latest_lock:
            latest_pose[:] = pts

        # Display webcam feed if no video file
        if not args.video:
            cv2.imshow("Webcam", frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

        now = time.time()
        d = now - last
        if d < dt_frame:
            time.sleep(dt_frame - d)
        last = time.time()

    cap.release()
    if not args.video:
        cv2.destroyAllWindows()
    if proc:
        proc.wait()

if __name__ == "__main__":
    main()
