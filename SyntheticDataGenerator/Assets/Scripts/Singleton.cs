// importazione variabili d'ambiente Unity
using UnityEngine;

// classe che implementa il pattern "Singleton" (per un oggetto di tipo T) della Gang of Four descritta nel libro Design Patterns, che serve ad assicurare che la classi figlie di quella contenuta in questo file siano istanziate una e una sola volta, fornendo un unico punto di accesso a quella istanza
public abstract class Singleton<T> : MonoBehaviour where T : Component {
    // il set dell'istanza di tipo T è posto privato, consentendo solo di ottenere l'istanza tramite la chiamata di questo metodo statico
    public static T Instance { get; private set; }
    // il metodo virtuale Awake() definisce l'istanza nel caso non fosse ancora stata definita, rendendola unica e elimina le nuove istanze che si cerca di creare
    protected virtual void Awake() {
        if (Instance == null) {
            Instance = this as T;
        } else {
            Destroy(gameObject);
        }
    }
}