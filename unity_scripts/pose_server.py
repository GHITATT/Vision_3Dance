from flask import Flask, jsonify
import cv2
import mediapipe as mp

app = Flask(__name__)
pose = mp.solutions.pose.Pose()
cap = cv2.VideoCapture(0)

@app.get("/pose")
def get_pose():
    ok, frame = cap.read()
    if not ok:
        return jsonify([])

    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    res = pose.process(rgb)

    if not res.pose_landmarks:
        return jsonify([])

    pts = []
    for lm in res.pose_landmarks.landmark:
        pts.append([lm.x, lm.y, lm.z])

    return jsonify(pts)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
