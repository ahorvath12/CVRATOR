using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ArtManager : MonoBehaviour
{

    private GameObject[] artInventory;
    private string[] resourceFiles;
    private string path;
    private DirectoryInfo dir;

    private void Awake()
    {
        path = Application.streamingAssetsPath + "Artwork";
        dir = new DirectoryInfo(@path);
        OpenInventory(null);
    }

    public void OpenInventory(string path)
    {
        AddResourceFiles(path);
        resourceFiles = AddResourceFiles(path);

        foreach (string s in resourceFiles)
        {

        }

        
    }
    private string[] AddResourceFiles(string path)
    {
        //add to linked list 
        string[] filePaths = GetFileArray(@path, "*.png|*.jpg|*.jpeg", SearchOption.AllDirectories);
        foreach (string f in filePaths)
        {
            Debug.Log(f);
        }
        return filePaths;
    }

    public static string[] GetFileArray(string path, string searchPattern, SearchOption searchOption)
    {
        string[] searchPatterns = searchPattern.Split('|');
        List<string> files = new List<string>();
        foreach (string sp in searchPatterns) // find appropriate files by file type 
        {
            files.AddRange(System.IO.Directory.GetFiles(path, sp, searchOption));
        }
        //now add subdirs
        string[] subDirs = Directory.GetDirectories(path);
        foreach (string sub in subDirs)
        {
            files.Add(sub);
        }
        files.Sort();
        return files.ToArray();
    }

    /// Instantiating all object types

    //turn 2D images to "paintings"
    private GameObject MakeImagesPaintings(string filePath)
    {
        IMG2Sprite imgToSprite = GetComponent<IMG2Sprite>(); //this script is also attached to PocketInventoryManagger

        //make canvasBase (acts as parent object)
        GameObject canvasBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        canvasBase.transform.position = new Vector3(0, 1, 0);
        canvasBase.gameObject.name = Path.GetFileNameWithoutExtension(filePath);
        canvasBase.gameObject.tag = "WallArt";
        canvasBase.gameObject.layer = LayerMask.NameToLayer("Artwork");

        //make painting
        GameObject go = new GameObject(Path.GetFileNameWithoutExtension(filePath));
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        Sprite canvas = imgToSprite.LoadNewSprite(filePath);
        renderer.sprite = canvas;
        renderer.transform.parent = canvasBase.transform;

        var bounds = canvas.bounds;
        var factory = 1 / bounds.size.y;
        var factorx = 1 / bounds.size.x;
        renderer.transform.localScale = new Vector2(factorx, factory);

        canvasBase.transform.localScale = new Vector3(canvas.texture.width / 1000.0f, canvas.texture.height / 1000.0f, 0.055f) * 1.5f;
        renderer.transform.position = new Vector3(canvasBase.transform.position.x - (canvasBase.transform.localScale.x / 2), canvasBase.transform.position.y - (0.5f * canvasBase.transform.position.y), canvasBase.transform.position.z - (canvasBase.transform.localScale.z));

        //canvasBase.AddComponent<BoxCollider>();
        //canvasBase.GetComponent<MeshRenderer>().enabled = false;
        canvasBase.transform.Rotate(0.0f, 180.0f, 0.0f);
        canvasBase.AddComponent<ColliderDetector>();
        canvasBase.AddComponent<Rigidbody>();
        canvasBase.GetComponent<Rigidbody>().useGravity = false;
        canvasBase.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //add painting as material
        //CreatePaintingMaterial(canvasBase.GetComponent<MeshRenderer>().material, Path.GetFileName(filePath), filePath);

        return canvasBase;
    }

    // read .fbx files in their appropriate folders and create texture for objects
    public GameObject MakeSculptures(string path)
    {
        int foundStart = path.IndexOf("StreamingAssets/");
        string pathToUse = path.Substring(foundStart + "StreamingAssets/".Length); //CHECK IF WORKS
        pathToUse = pathToUse.Substring(0, pathToUse.Length - 4);
        Object prefab = Resources.Load(pathToUse);
        GameObject sculpture = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //sculpture = sculpture.transform.GetChild(0).gameObject;

        //add unity details
        sculpture.name = Path.GetFileNameWithoutExtension(path);
        //GameObject child = sculpture.transform.GetChild(0).gameObject;
        sculpture.tag = "FloorArt";
        sculpture.layer = LayerMask.NameToLayer("Artwork");
        sculpture.AddComponent<BoxCollider>();
        //child.AddComponent<MeshCollider>().inflateMesh = true;
        sculpture.AddComponent<ColliderDetector>();
        sculpture.AddComponent<Rigidbody>();
        sculpture.GetComponent<Rigidbody>().useGravity = false;
        sculpture.GetComponent<Rigidbody>().isKinematic = true;
        sculpture.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //assign material
        pathToUse = pathToUse.Remove(pathToUse.IndexOf(sculpture.name)) + "Materials/";
        Material sculptureChildMat = sculpture.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        //Material sculptureMat = sculpture.GetComponent<MeshRenderer>().material;
        CreateMaterial(sculptureChildMat, sculpture.name);

        return sculpture;
    }

    public void MakePrefab(string path)
    {
        int foundStart = path.IndexOf("StreamingAssets/");
        string pathToUse = path.Substring(foundStart + "StreamingAssets/".Length);
        pathToUse = pathToUse.Substring(0, pathToUse.Length - 4);
        Object prefab = Resources.Load(pathToUse);
        GameObject sculpture = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }


    ///////////////////////////////////////////////
    /// Functions to create materials

    public void CreatePaintingMaterial(Material mat, string paintingName, string path)
    {
        WWW localFile = new WWW(path);
        mat.SetTexture("_MainTex", localFile.texture);
    }

    public void CreateMaterial(Material mat, string objName)
    {
        string finalPath;
        WWW localFile;

        // find the textures within the Materials folder
        string pathtest = Application.streamingAssetsPath + "/Materials/";
        DirectoryInfo texPath = new DirectoryInfo(@pathtest);
        FileInfo[] matsInDir = texPath.GetFiles();

        mat.EnableKeyword("_NORMALMAP");
        mat.EnableKeyword("_PARALLAXMAP");
        mat.EnableKeyword("_METALLICGLOSSMAP");

        foreach (FileInfo f in matsInDir)
        {
            string current = f.Name.ToLower();
            if (current.Contains(objName.ToLower()) && !current.Contains(".meta"))
            {
                finalPath = "file://" + Application.streamingAssetsPath + "/Materials/" + f.Name;
                localFile = new WWW(finalPath);
                if (current.Contains("albedo"))
                    mat.SetTexture("_MainTex", localFile.texture);
                if (current.Contains("metallic"))
                    mat.SetTexture("_MetallicGlossMap", localFile.texture);
                if (current.Contains("normal"))
                    mat.SetTexture("_BumpMap", localFile.texture);
            }
        }
    }
}
