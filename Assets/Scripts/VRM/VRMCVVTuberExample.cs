using CVVTuber;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVVTuberExample
{
    public class VRMCVVTuberExample : MonoBehaviour
    {
        /// <summary>
        /// The webcam texture mat source getter.
        /// </summary>
        public WebCamTextureMatSourceGetter webCamTextureMatSourceGetter;

        /// <summary>
        /// The dlib face landmark getter.
        /// </summary>
        public DlibFaceLandmarkGetter dlibFaceLandmarkGetter;

        // Use this for initialization
        void Start()
        {
            // Load global settings.
            dlibFaceLandmarkGetter.dlibShapePredictorFileName = CVVTuberExample.dlibShapePredictorFileName;
            dlibFaceLandmarkGetter.dlibShapePredictorMobileFileName = CVVTuberExample.dlibShapePredictorFileName;
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            Destroy(GameObject.Find("InformationParser"));
            SceneManager.LoadScene("Carousel", LoadSceneMode.Single);
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            webCamTextureMatSourceGetter.ChangeCamera();
        }
    }
}