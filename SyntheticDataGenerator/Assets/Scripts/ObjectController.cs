// importazione raccolte generiche di sistema, language integrated queries (LINQ) e variabili d'ambiente Unity
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// classe, derivante da singleton, che si occupa di risolvere eventuali overlap tra oggetti nell'immagine, ammettendo una sovrapposizione massima del 15% (regolabile)
public class ObjectController : Singleton<ObjectController> {
    const float PERCENT_OVERLAP = .15f;
    // funzione che attiva tutti gli oggetti figli di tutte le transform
    public void ActivateObjects() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }
    // funzione che ritorna un dizionario contenente gli oggetti attivi tramite la cooperazione con le funzioni CheckForOverlap() e GetActiveObjects()
    public Dictionary<GameObject, Rect> GetObjects() {
        CheckForOverlap();
        return GetActiveObjects();
    }
    // funzione che ritorna un dizionario degli oggetti attivi e delle rispettive bounding box
    Dictionary<GameObject, Rect> GetActiveObjects() {
        Dictionary<GameObject, Rect> currObjects = new Dictionary<GameObject, Rect>();

        foreach (Transform child in transform) {
            if (child.gameObject.activeSelf) {
                currObjects.Add(child.gameObject, child.GetComponent<ObjectBounds>().GetBounds());
            }
        }

        return currObjects;
    }
    // funzione che prende il dizionario di GetActiveObjects(), lo mescola e rende invisibili gli elementi che sono sovrapposti per una percentuale più ampia di quella definita come soglia
    void CheckForOverlap() {
        Dictionary<GameObject, Rect> currObjects = GetActiveObjects();

        System.Random rand = new System.Random();
        currObjects = currObjects.OrderBy(x => rand.Next())
          .ToDictionary(item => item.Key, item => item.Value);

        foreach (KeyValuePair<GameObject, Rect> obj1 in currObjects) {
            if (obj1.Key.activeSelf) {
                foreach (KeyValuePair<GameObject, Rect> obj2 in currObjects) {
                    if (obj1.Key != obj2.Key && obj1.Key.activeSelf && isOverlapping(obj1.Value, obj2.Value)) {
                        obj2.Key.SetActive(false);
                    }
                }
            }
        }
    }
    // funzione utilizzata per verificare se i bounds di due oggetti sono sovrapposti per più di una certa percentuale
    bool isOverlapping(Rect rect1, Rect rect2) {
        if (rect1.Overlaps(rect2)) {

            float area1 = Mathf.Abs(rect1.min.x - rect1.max.x) * Mathf.Abs(rect1.min.y - rect1.max.y);
            float area2 = Mathf.Abs(rect2.min.x - rect2.max.x) * Mathf.Abs(rect2.min.y - rect2.max.y);

            float areaI = (Mathf.Min(rect1.max.x, rect2.max.x) - Mathf.Max(rect1.min.x, rect2.min.x)) * (Mathf.Min(rect1.max.y, rect2.max.y) - Mathf.Max(rect1.min.y, rect2.min.y));
            float percentOverlap = areaI / Mathf.Min(area1, area2);

            return percentOverlap > PERCENT_OVERLAP;
        } else {
            return false;
        }
    }
}
