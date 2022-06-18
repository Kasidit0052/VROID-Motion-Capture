using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;
public class ModelDownloader : MonoBehaviour
{
    public Transform _modelParent;
    public GameObject _VRMLoaderPref;
    private CreateCarousel createCarouselScript;
    private CarouselModelLoader carouselModelLoaderScript;
    void Start()
    {
        createCarouselScript = GameObject.Find("CarouselModelLoader").GetComponent<CreateCarousel>();
        carouselModelLoaderScript = GameObject.Find("CarouselModelLoader").GetComponent<CarouselModelLoader>();
    }
    public void DownloadVRM()
    {
        // Open file with filter
        var extensions = new [] {
            new ExtensionFilter("VRM Files", "vrm")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if(paths.Length != 0)
        {
            var path = paths[0];

            string[] splitString = path.Split(char.Parse("/"));
            string des_path = Path.Combine(Application.streamingAssetsPath,"models",splitString[splitString.Length - 1]);
            File.Copy(path, des_path, true);
            Debug.Log(des_path);

            GameObject model = Instantiate(_VRMLoaderPref, new Vector3(0, 0, 0), Quaternion.identity);
            model.name = splitString[splitString.Length - 1].Replace(".vrm","");
            model.transform.SetParent(_modelParent, false);
            
            Array.Resize(ref createCarouselScript.carouselObjects, createCarouselScript.carouselObjects.Length + 1);
            createCarouselScript.carouselObjects[createCarouselScript.carouselObjects.Length - 1] = model;
            model.GetComponent<CarouselModelLoaderHandler>().CarouselOpenVRM(des_path); 
            carouselModelLoaderScript.Instantiated_prefab.Add(model);

            // Show and Hide Rotate Button
            if(createCarouselScript.carouselObjects.Length > 1)
            {
                GameObject.Find("Canvas").transform.Find("RotateRightBtn").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("RotateLeftBtn").gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("RotateRightBtn").gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.Find("RotateLeftBtn").gameObject.SetActive(false); 
            }

            createCarouselScript.CarouselStart();
        }
    }

    // Error Here
    public void DeleteVRM()
    {   
        string path_to_delete = createCarouselScript.carouselObjects[createCarouselScript.ChosenObject].GetComponent<CarouselModelLoaderHandler>().context_path;

        // Destroy And Remove Instantanious Prefab
        Destroy(createCarouselScript.carouselObjects[createCarouselScript.ChosenObject]);
        carouselModelLoaderScript.Instantiated_prefab.RemoveAt(createCarouselScript.ChosenObject);

        // Modify Carousel Object Array
        createCarouselScript.carouselObjects = new GameObject[carouselModelLoaderScript.Instantiated_prefab.Count];
        for(int i=0; i < carouselModelLoaderScript.Instantiated_prefab.Count; i++) 
        {
            createCarouselScript.carouselObjects[i] = carouselModelLoaderScript.Instantiated_prefab[i];
        }

        // Delete File
        File.Delete(path_to_delete);

        // Show and Hide Rotate Button
        if(createCarouselScript.carouselObjects.Length > 1)
        {
            GameObject.Find("Canvas").transform.Find("RotateRightBtn").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("RotateLeftBtn").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("RotateRightBtn").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("RotateLeftBtn").gameObject.SetActive(false); 
        }

        createCarouselScript.CarouselStart();
        Debug.Log("Delete VRM at" + path_to_delete);
    }
}
