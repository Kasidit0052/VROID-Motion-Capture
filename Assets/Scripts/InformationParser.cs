using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationParser : MonoBehaviour
{
    public string _vrmFilePath;
    private CreateCarousel createCarouselScript;

    // Start is called before the first frame update
    void Start()
    {
        createCarouselScript = GameObject.Find("CarouselModelLoader").GetComponent<CreateCarousel>();
    }

    // Update is called once per frame
    void Update()
    {
        if(createCarouselScript)
        {
            var path = createCarouselScript.carouselObjects[createCarouselScript.ChosenObject].GetComponent<CarouselModelLoaderHandler>().context_path;
            var path_array = path.Split(char.Parse("/"));
            var _vrmFileName = path_array[path_array.Length -1];
            _vrmFilePath = Path.Combine("models",_vrmFileName);
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
