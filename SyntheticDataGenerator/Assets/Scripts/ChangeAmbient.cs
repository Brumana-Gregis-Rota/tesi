// importazione variabili d'ambiente Unity
using UnityEngine;

// questa classe sarà assegnata a ObjectsToTrain e servirà a cambiare la luce ambientale (ossia diffusa e non proveniente da una fonte specifica) della scena tra tonalità randomiche di bianco, riportandola alla sua colorazione e intensità predefinita quando il ciclo di generazione immagini è terminato
public class ChangeAmbient : MonoBehaviour, IChangeable {
    // realizzazione ChangeRandom(), obbligata dall'implementazione di IChangeable, che cambia la luce ambientale su una tonalità di bianco randomica
    public void ChangeRandom() {
        Color lightColor = Color.white;
        lightColor *= Random.Range(.8f, 1.3f);
        RenderSettings.ambientLight = lightColor;
    }
    // funzione richiamata a fine ciclo di generazione che riporta la luce ambientale alla tonalità di bianco predefinita
    void OnApplicationQuit() {
        RenderSettings.ambientLight = Color.white;
    }
}
