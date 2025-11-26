# Vision_3Dance

Détection de pose avec **MediaPipe en Python**, envoi des 33 articulations à **Unity** via HTTP.

---

## Installation Python

Depuis le dossier `python/` :

```bash
pip install -r requirements.txt
python3 pose_server.py
```

Le serveur tourne sur :
**[http://127.0.0.1:5000/pose](http://127.0.0.1:5000/pose)**

---

## Installation Unity

Dans Unity, ajouter les scripts :

* `PoseFetcher.cs`
* `PoseGizmos.cs`
* `SimpleJSON.cs`

Créer un GameObject, ajouter :

* `PoseFetcher`
* `PoseGizmos` (référence sur PoseFetcher)

Play → les joints apparaissent en Gizmos dans la vue Scene.

---

## Structure recommandée

```
project/
 ├── python/
 │    ├── pose_server.py
 │    └── requirements.txt
 └── unity/
      └── Assets/Scripts/
```