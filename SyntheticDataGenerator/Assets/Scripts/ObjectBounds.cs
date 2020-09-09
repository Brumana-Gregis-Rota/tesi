// importazione variabili d'ambiente Unity
using UnityEngine;

// questa classe sarà assegnata a tutti gli oggetti della scena figli di ObjectsToTrain e avrà la funzione di ricalcolarne i bounds, traslandoli dallo spazio reale a quello in funzione della prospettiva della telecamera, partendo dalle informazioni sulla mesh ricevute da MeshUtility.cs
public class ObjectBounds : MonoBehaviour {
    public GUISkin guiSkin;

    Camera cam;
    Rect currBox = new Rect();
    Rect photoRect = new Rect();
    bool showBox = true;

    void Start() {
        cam = Camera.main;
    }
    // funzione che fa apparire la bounding box, se richiesta tramite il parametro booleano, sull'interfaccia utente.
    void OnGUI() {
        if (!showBox) {
            return;
        }
        
        Vector2 min = new Vector2(currBox.xMin, currBox.yMin);
        Vector2 max = new Vector2(currBox.xMax, currBox.yMax);
        Rect rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        
        GUI.skin = guiSkin;
        GUI.Box(rect, "");
    }
    // funzione che ritorna i bounds calcolati, se sono stati calcolati, dell'oggetto passato
    public Rect GetBounds() {
        return photoRect;
    }
    // funzione che fa un update dei bounds dell'oggetto, aprendo alla possibilità di mostrarli o meno in una bounding box i cui limiti sono min e max di x e y, a seconda del parametro booleano che le viene passato. I bounds, inizialmente statici, vengono convertiti sulla base della posizione della telecamera, assicurandosi che non finiscano fuori dal campo visivo di quest'ultima.
    public void UpdateBounds(bool visualize = true) {
        if (!gameObject.activeSelf) {
            return;
        }

        showBox = visualize;
        
        Vector3[] verts = MeshUtility.GetMesh(transform).vertices;

        for (int i = 0; i < verts.Length; i++) {
            verts[i] = cam.WorldToScreenPoint(transform.TransformPoint(verts[i]));
            if (verts[i].x < 0 || verts[i].x > Screen.width || verts[i].y < 0 || verts[i].y > Screen.height) {
                verts[i] = verts[0];
            }
        }

        currBox = new Rect {
            xMin = verts[0].x,
            xMax = verts[0].x,
            yMin = verts[0].y,
            yMax = verts[0].y
        };

        for (int i = 0; i < verts.Length; i++) {
            currBox.xMin = currBox.xMin < verts[i].x ? currBox.xMin : verts[i].x;
            currBox.xMax = currBox.xMax > verts[i].x ? currBox.xMax : verts[i].x;
            currBox.yMin = currBox.yMin < verts[i].y ? currBox.yMin : verts[i].y;
            currBox.yMax = currBox.yMax > verts[i].y ? currBox.yMax : verts[i].y;
        }

        photoRect = currBox;

        currBox.yMin = Screen.height - currBox.yMin;
        currBox.yMax = Screen.height - currBox.yMax;
    }
}
