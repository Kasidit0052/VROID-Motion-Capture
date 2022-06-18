using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselModelLoader : MonoBehaviour
{
    public Transform modelParent;
    public GameObject VRMLoaderPref;
    private CreateCarousel createCarouselScript;
    public List<GameObject> Instantiated_prefab;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Canvas").transform.Find("RotateRightBtn").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("RotateLeftBtn").gameObject.SetActive(false); 

        createCarouselScript = this.gameObject.GetComponent<CreateCarousel>();

        string path_to_model_folder = Path.Combine(Application.streamingAssetsPath, "models");
        string[] filePaths = Directory.GetFiles(path_to_model_folder,"*.vrm");
        if(filePaths.Length != 0)
        {

            createCarouselScript.carouselObjects = new GameObject[filePaths.Length];

            var i = 0;
            foreach (string path in filePaths)
            {
                Debug.Log(path);
                GameObject model = Instantiate(VRMLoaderPref, new Vector3(0, 0, 0), Quaternion.identity);
                var path_array = path.Split(char.Parse("/"));
                model.name = path_array[path_array.Length -1].Replace(".vrm","");
                model.transform.SetParent(modelParent, false);
                createCarouselScript.carouselObjects[i] = model;
                model.GetComponent<CarouselModelLoaderHandler>().CarouselOpenVRM(path); 
                i++;
                Instantiated_prefab.Add(model);
            }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
