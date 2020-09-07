// importazione language integrated queries (LINQ), editor Unity e variabili d'ambiente Unity
using System.Linq;
using UnityEditor;
using UnityEngine;
// questa classe sarà assegnata ad ObjectsToTrain e ha la funzione di ridimensionare la finestra di esecuzione di Unity: ridimensionare quest'ultima, infatti, implica anche ridimensionare le immagini che verranno generate, in quanto queste ultime saranno fittate alle dimensioni della GUI di esecuzione
public class ChangeWindow : MonoBehaviour, IChangeable {
    // realizzazione ChangeRandom(), obbligata dall'implementazione di IChangeable, che cambia le dimensioni della finestra all'interno della quale vengono esposte le immagini che saranno poi salvate. La finestra viene passata dalla funzione GetMainView() e l'altezza che gli viene attribuita durante il ridimensionamento è funzione della larghezza attribuitagli randomicamente mediante il medesimo processo
    public void ChangeRandom() {
        Rect R = GetMainGameView().position;
        R.width = Random.Range(720,1440);
        R.height = R.width / Random.Range(1.25f, 2.5f);
        GetMainGameView().position = R;
    }
    // ritorna una EditorWindow di Unity, più precisamente la prima o quella settata di default il cui titolo contiene la parola "Game"
   EditorWindow GetMainGameView() {
        EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        return windows.FirstOrDefault(e => e.titleContent.text.Contains("Game"));
    }
}
