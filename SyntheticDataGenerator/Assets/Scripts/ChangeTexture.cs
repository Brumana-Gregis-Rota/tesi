// importazione variabili d'ambiente Unity, con l'aggiunta di quelle della UI (user interface)
using UnityEngine;
using UnityEngine.UI;
// richiede che l'oggetto al quale lo script è assegnato abbia un componente di tipo RawImage (questo script sarà assegnato all'Image nel Canvas della Main Camera, quindi la RawImage sarà l'immagine di sfondo dell'immagine che verrà generata)
[RequireComponent(typeof(RawImage))]
// questa classe sarà assegnata a Main Camera -> Canvas -> Image e servirà a cambiare l'immagine di sfondo dell'immagine che verrà generata. Senza lo script RandoTextures.cs 
public class ChangeTexture : MonoBehaviour, IChangeable {

    RawImage backgroundImage;
    
    void Start() {
        backgroundImage = GetComponent<RawImage>();
    }
    // realizzazione ChangeRandom(), obbligata dall'implementazione di IChangeable, che cambia l'immagine di sfondo reperendola dall'unica istanza di RandoTextures (singleton), che la pesca da Assets/Resources/Textures
    public void ChangeRandom() {
        backgroundImage.material.mainTexture = RandoTextures.Instance.GetRandomTexture();
        backgroundImage.SetMaterialDirty();
    }
}
