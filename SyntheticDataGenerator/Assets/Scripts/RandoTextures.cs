// importazione variabili d'ambiente Unity
using UnityEngine;

// questa classe, singleton, fungerà da tool per ChangeTexture.cs, fornendogli immagini provenienti da Assets/Resources/Textures
public class RandoTextures : Singleton<RandoTextures> {

    Object[] textures;
    int texNum;
    // override della funzione Awake() di Singleton, ne ripropone completamente il comportamento, aggiungendo una chiamata alla funzione Shuffle passando come parametro la cartella Textures presente in Resources
    protected override void Awake() {
        base.Awake();
        textures = Resources.LoadAll("Textures", typeof(Texture));
        Shuffle(textures);
    }
    // ritorna, partendo dalla numero 0, tutte le immagini presenti in Assets/Resources/Textures indicizzate e opportunamente randomizzate nell'ordine dalla funzione Shuffle
    public Texture GetRandomTexture() {
        if (texNum < textures.Length - 1) {
            texNum++;
        } else {
            texNum = 0;
        }
        print(textures[texNum].name);
        return textures[texNum] as Texture;
    }
    // mescola l'ordine delle immagini della cartella Textures passata da Awake()
    void Shuffle(Object[] items) {
        for (int t = 0; t < items.Length; t++) {
            Object tmp = items[t];
            int r = Random.Range(t, items.Length);
            items[t] = items[r];
            items[r] = tmp;
        }
    }
}
