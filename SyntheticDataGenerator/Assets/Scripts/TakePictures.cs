// importazione raccolte generiche di sistema, strumenti per lettura e scrittura di files e variabili d'ambiente Unity
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// questa classe predispone la cartella UnityStuff con le cartelle train e TFUtils e i files train.txt e labelmap.pbtxt e procede con la generazione e la memorizzazione di immagini, sfruttando ogni script contenuto nella cartella Scripts
public class TakePictures : MonoBehaviour {
    const bool SHOW_BOXES = false;
    const int TOTAL_IMAGES = 2;

    int imageNum = 1;
    string parentPath;

    string trainImagePath;
    string trainLabelPath;
    // all'avvio dello script, vengono chiamate le funzioni qui elencate
    void Start() {
        CopyUtils();
        CreateDirectories();
        CreateLabelFiles();
        CreateLabelMap();
    }
    // alla chiamata di questo script, parte la coroutine (funzione i cui risultati intermedi vengono mostrati nei frame, a differenza di quanto succederebbe con una funzione normale) DelayPicture()
    void Update() {
        if (PictureRoutine == null) {
            PictureRoutine = StartCoroutine(DelayPicture());
        }
    }
    // funzione che copia il contenuto di SyntheticDataGenerator/TFUtils, contenente tutto l'occorrente per il training, in Assets/StreamingAssets/UnityStuff/TFUtils
    void CopyUtils() {
        DirectoryInfo srcPath = new DirectoryInfo("./TFUtils");
        DirectoryInfo destPath =new DirectoryInfo(Application.streamingAssetsPath + "/UnityStuff/TFUtils");
        CopyAll(srcPath, destPath);

    }
    // funzione ausiliaria che copia quanto contenuto nella directory passata come parametro "source" in quella passata come parametro "target", creando il percorso di target mediante cartelle nel caso questo non esistesse
    void CopyAll(DirectoryInfo source, DirectoryInfo target) {
        Directory.CreateDirectory(target.FullName);

        foreach (FileInfo fi in source.GetFiles()) {
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }
    // funzione che crea le cartelle UnityStuff e UnityStuff/train (che conterrà tutte le immagini), nel caso queste non esistessero
    void CreateDirectories() {
        parentPath = Application.streamingAssetsPath + "/UnityStuff";
        if (!File.Exists(parentPath)) {
            Directory.CreateDirectory(parentPath);
        }

        trainImagePath = parentPath + "/train";
        if (!File.Exists(trainImagePath)) {
            Directory.CreateDirectory(trainImagePath);
        }
    }
    // funzione che crea il file train.txt all'interno di UnityStuff, file che conterrà tutte le informazioni di labelling
    void CreateLabelFiles() {
        trainLabelPath = parentPath + "/train.txt";
        File.WriteAllText(trainLabelPath, "");
    }
    // funzione che crea il file labelmap.pbtxt, compilandolo in un formato leggibile per TensorFlow, associando un id numerico e un nome ad ogni oggetto presente nella scena (eccetto la telecamera) di modo che TensorFlow possa distinguerli e utilizzare quel nome in output per evidenziarne la detection
    void CreateLabelMap() {
        string labelMapPath = parentPath + "/labelmap.pbtxt";
        string labelMap = "";
        List<string> uniqueNames = new List<string>();
        foreach (Transform child in transform) {
            if (!uniqueNames.Contains(child.gameObject.name)) {
                uniqueNames.Add(child.gameObject.name);
                labelMap += "item {\n" +
                "\tid: " + (child.GetSiblingIndex() + 1) + "\n" +
                "\tname: '" + child.gameObject.name + "'\n" +
                "}\n";
            }
        }
        File.WriteAllText(labelMapPath, labelMap);
    }

    Coroutine PictureRoutine;
    // funzione che effettua una randomizzazione della scena con ricalcolo dei bounds, compila il file di labelling sruttando WriteObjectsToFile(...), fa uno screen di quanto appare nell'interfaccia utente (immagine + eventuali bounding boxes) e aumenta il conto delle fotografie scattate, terminando il processo quando viene raggiunto il conteggio prefissato.
    IEnumerator DelayPicture() {
        ChangeAllItems();
        yield return new WaitForEndOfFrame();

        foreach (ObjectBounds bounds in FindObjectsOfType<ObjectBounds>()) {
            bounds.UpdateBounds(SHOW_BOXES);
        }

        WriteObjectsToFile(ObjectController.Instance.GetObjects());

        yield return new WaitForEndOfFrame();

        TakeFullScreenPicture();

        if (imageNum == TOTAL_IMAGES) {
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Training data collected!");
        } else {
            imageNum++;
            PictureRoutine = null;
        }
    }
    // funzione che scrive nel file di labelling le informazioni degli oggetti contenuti nell'immagine che andrà a finire nel dataset seguendo il formato numero immagine, larghezza immagine, altezza immagine, label, [xmin box, xmax box, ymin box, ymax box] => in un formato di coordinate leggibile da TF
    void WriteObjectsToFile(Dictionary<GameObject, Rect> objects) {
        string line = imageNum + "," + Screen.width + "," + Screen.height;
        foreach (KeyValuePair<GameObject,Rect> obj in objects) {
            Rect tfRect = ConvertUnityRectToTensorflow(obj.Value);

            if (tfRect.xMin != tfRect.xMax && tfRect.yMin != tfRect.xMax) {
                line += "," + obj.Key.name + "," + tfRect.xMin + ","
                    + tfRect.xMax + "," + tfRect.yMin + "," + tfRect.yMax;
            }
         }

        StreamWriter writer;
        writer = new StreamWriter(trainLabelPath, true);
        writer.WriteLine(line);
        writer.Close();
    }
    // funzione che converte le coordinate dei bounds in un formato leggibile da TF, ossia in numeri interi
    Rect ConvertUnityRectToTensorflow(Rect unityRect) {
        return new Rect {
            xMin = Mathf.RoundToInt(unityRect.xMin),
            xMax = Mathf.RoundToInt(unityRect.xMax),
            yMin = Screen.height - Mathf.RoundToInt(unityRect.yMax),
            yMax = Screen.height - Mathf.RoundToInt(unityRect.yMin)
        };
    }
    // funzione che fa uno screen di quanto contenuto nell'interfaccia utente, memorizzandolo in un file numeroprogressivo.jpg
    void TakeFullScreenPicture() {
        Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        photo.Apply();
        byte[] data = photo.EncodeToJPG(75);
        DestroyImmediate(photo);

        File.WriteAllBytes(trainImagePath + "/" + imageNum + ".jpg", data);
    }
    // funzione che chiama tutte le funzioni ChangeRandom() di tutti gli script che implementano IChangeable associati a tutti gli oggetti della scena, cambiando totalmente la scena in tutti i parametri pilotati dagli script il cui nome contiene la parola "Change"
    void ChangeAllItems() {
        foreach (GameObject item in FindObjectsOfType<GameObject>()) {
            IChangeable[] changeableItems = item.GetComponents<IChangeable>();
            foreach (IChangeable changeable in changeableItems) {
                if (changeable != null) {
                    changeable.ChangeRandom();
                }
            }
        }
        ObjectController.Instance.ActivateObjects();
    }
}