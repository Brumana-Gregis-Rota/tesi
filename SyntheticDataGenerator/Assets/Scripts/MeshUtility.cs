// importazione variabili d'ambiente Unity
using UnityEngine;
// classe utility, sfruttata da ObjectBounds.cs, che serve a ritornare, tramite la sua funzione getMesh(transform), tutte le mesh dei discendenti dell'oggetto passatogli come riferimento
public class MeshUtility : MonoBehaviour {
    public static Mesh GetMesh(Transform transform) {
        MeshFilter meshFilter = transform.GetComponentInChildren<MeshFilter>();
        return meshFilter.mesh;
    }
}
