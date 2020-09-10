// importazione variabili d'ambiente Unity
using UnityEngine;
// questa classe sarà assegnata a ObjectsToTrain -> hand e cambierà la sua posizione e la sua rotazione ogni volta che verrà invocato il suo metodo ChangeRandom()
    public class ChangeTransform : MonoBehaviour, IChangeable {
    // la rotazione sarà al massimo di 20°. È stata ricercata una rotazione maggiore, di modo da avere anche il dorso della mano visibile nel dataset, ma i risultati non sono stati quelli previsti
    const int MAX_ANGLE = 20;

    Vector3 startPosition;
    Vector3 startAngle;

    void Start() {
        startPosition = transform.localPosition;
        startAngle = transform.localEulerAngles;
    }

    // realizzazione ChangeRandom(), obbligata dall'implementazione di IChangeable, che cambia rotazione e posizione dell'oggetto al quale lo script è legato, mantenendolo comunque all'interno della futura immagine
    public void ChangeRandom() {
        // cambio posizione in maniera randomica ma entro range di valori che assicurino la visibilità della mano
        Vector3 randScreenPos = new Vector3(Random.Range(.1f, .9f), Random.Range(.6f, .9f), startPosition.z + Random.Range(.5f, 4.5f));
        transform.position = Camera.main.ViewportToWorldPoint(randScreenPos);

        // cambio rotazione
        Vector3 randAngle;
        randAngle.x = startAngle.x + Random.Range(-MAX_ANGLE, MAX_ANGLE);
        randAngle.y = startAngle.y + Random.Range(-MAX_ANGLE, MAX_ANGLE);
        randAngle.z = startAngle.z + Random.Range(-MAX_ANGLE, MAX_ANGLE);
        transform.localEulerAngles = randAngle;
    }
}
