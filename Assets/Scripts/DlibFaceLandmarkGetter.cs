using DlibFaceLandmarkDetector;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CVVTuber
{
    public class DlibFaceLandmarkGetter : CVVTuberProcess, IFaceLandmarkGetter
    {
        [Header("[Input]")]

        [SerializeField, InterfaceRestriction(typeof(IMatSourceGetter))]
        protected CVVTuberProcess matSourceGetter;

        protected IMatSourceGetter _matSourceGetterInterface = null;

        protected IMatSourceGetter matSourceGetterInterface
        {
            get
            {
                if (matSourceGetter != null && _matSourceGetterInterface == null)
                    _matSourceGetterInterface = matSourceGetter.GetComponent<IMatSourceGetter>();
                return _matSourceGetterInterface;
            }
        }

        [SerializeField, InterfaceRestriction(typeof(IFaceRectGetter))]
        protected CVVTuberProcess faceRectGetter;

        protected IFaceRectGetter _faceRectGetterInterface = null;

        protected IFaceRectGetter faceRectGetterInterface
        {
            get
            {
                if (faceRectGetter != null && _faceRectGetterInterface == null)
                    _faceRectGetterInterface = faceRectGetter.GetComponent<IFaceRectGetter>();
                return _faceRectGetterInterface;
            }
        }

        [Header("[Setting]")]

        public string dlibShapePredictorFileName;

        public string dlibShapePredictorMobileFileName;

        [Header("[Debug]")]

        public RawImage screen;

        public bool isDebugMode;

        public bool hideImage;

        protected Mat debugMat;

        protected Texture2D debugTexture;

        protected Color32[] debugColors;

        protected List<Vector2> faceLandmarkPoints;

        // Add public variable to export face landmarks
        public List<Vector2> exported_faceLandmarkPoints;

        protected bool didUpdateFaceLanmarkPoints;

        protected FaceLandmarkDetector faceLandmarkDetector;

        protected static readonly string DLIB_SHAPEPREDICTOR_FILENAME_PRESET = "sp_human_face_68.dat";

        protected static readonly string DLIB_SHAPEPREDICTOR_MOBILE_FILENAME_PRESET = "sp_human_face_68_for_mobile.dat";

        protected string dlibShapePredictorFilePath;

#if UNITY_WEBGL
        protected IEnumerator getFilePath_Coroutine;
#endif


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Get face landmark points from MatSourceGetter.";
        }

        public override void Setup()
        {
            NullCheck(matSourceGetterInterface, "matSourceGetter");

            if (string.IsNullOrEmpty(dlibShapePredictorFileName))
                dlibShapePredictorFileName = DLIB_SHAPEPREDICTOR_FILENAME_PRESET;

            if (string.IsNullOrEmpty(dlibShapePredictorMobileFileName))
                dlibShapePredictorMobileFileName = DLIB_SHAPEPREDICTOR_MOBILE_FILENAME_PRESET;

#if UNITY_WEBGL
            getFilePath_Coroutine = DlibFaceLandmarkDetector.UnityUtils.Utils.getFilePathAsync(dlibShapePredictorMobileFileName, (result) =>
            {
                getFilePath_Coroutine = null;

                dlibShapePredictorFilePath = result;
                if (string.IsNullOrEmpty(dlibShapePredictorFilePath))
                {
                    Debug.LogError("shape predictor file does not exist. Please copy from “DlibFaceLandmarkDetector/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
                }
                Run();
            });
            StartCoroutine(getFilePath_Coroutine);
#else
#if UNITY_ANDROID || UNITY_IOS
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.UnityUtils.Utils.getFilePath(dlibShapePredictorMobileFileName);
#else
            dlibShapePredictorFilePath = DlibFaceLandmarkDetector.UnityUtils.Utils.getFilePath(dlibShapePredictorFileName);
#endif
            if (string.IsNullOrEmpty(dlibShapePredictorFilePath))
            {
                Debug.LogError("shape predictor file does not exist. Please copy from “DlibFaceLandmarkDetector/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
            }
            Run();
#endif
        }

        //Attach KalmanScript to update the dlib calculation
        private KalmanScript kalman_script;
        public List<Vector2> imported_faceLandmarkPoints;
        public void Start()
        {
            kalman_script = GameObject.Find("KalmanCalculator").GetComponent<KalmanScript>();
        }

        public override void UpdateValue()
        {
            if (faceLandmarkDetector == null)
                return;

            if (matSourceGetterInterface == null)
                return;

            didUpdateFaceLanmarkPoints = false;

            Mat rgbaMat = matSourceGetterInterface.GetMatSource();
            Mat downScaleRgbaMat = matSourceGetterInterface.GetDownScaleMatSource();
            if (rgbaMat != null)
            {
                if (isDebugMode && screen != null)
                {

                    if (debugMat != null && (debugMat.width() != rgbaMat.width() || debugMat.height() != rgbaMat.height()))
                    {
                        debugMat.Dispose();
                        debugMat = null;
                    }
                    debugMat = debugMat ?? new Mat(rgbaMat.rows(), rgbaMat.cols(), rgbaMat.type());

                    if (hideImage)
                    {
                        debugMat.setTo(new Scalar(0, 0, 0, 255));
                    }
                    else
                    {
                        rgbaMat.copyTo(debugMat);
                    }

                    if (debugTexture != null && (debugTexture.width != debugMat.width() || debugTexture.height != debugMat.height()))
                    {
                        Texture2D.Destroy(debugTexture);
                        debugTexture = null;
                    }
                    if (debugTexture == null)
                    {
                        debugTexture = new Texture2D(debugMat.width(), debugMat.height(), TextureFormat.RGBA32, false, false);

                        Vector2 size = screen.rectTransform.sizeDelta;
                        screen.rectTransform.sizeDelta = new Vector2(size.x, size.x * (float)debugMat.height() / (float)debugMat.width());
                    }

                    if (debugColors != null && debugColors.Length != debugMat.width() * debugMat.height())
                    {
                        debugColors = new Color32[debugMat.width() * debugMat.height()];
                    }
                    screen.texture = debugTexture;
                    screen.enabled = true;
                }
                else
                {
                    if (screen != null)
                        screen.enabled = false;
                }


                if (faceRectGetterInterface != null)
                {
                    UnityEngine.Rect faceRect = faceRectGetterInterface.GetFaceRect();

                    if (faceRect != UnityEngine.Rect.zero)
                    {
                        // correct the deviation of the detection result of the face rectangle of OpenCV and Dlib.
                        faceRect = new UnityEngine.Rect(
                            faceRect.x + (faceRect.width * 0.05f),
                            faceRect.y + (faceRect.height * 0.1f),
                            faceRect.width * 0.9f,
                            faceRect.height * 0.9f);

                        OpenCVForUnityUtils.SetImage(faceLandmarkDetector, rgbaMat);
                        List<Vector2> points = faceLandmarkDetector.DetectLandmark(faceRect);

                        faceLandmarkPoints = points;
                        //add public variable to export faceLandmark
                        exported_faceLandmarkPoints = points;
                        // override old points
                        if(kalman_script.filtered_faceLandmarkPoints.Count != 0){imported_faceLandmarkPoints = kalman_script.filtered_faceLandmarkPoints;}


                        didUpdateFaceLanmarkPoints = true;

                        if (isDebugMode && screen != null)
                            OpenCVForUnityUtils.DrawFaceLandmark(debugMat, points, new Scalar(0, 255, 0, 255), 2);
                            // draw kalman filtered face landmark (add)
                            if(kalman_script.filtered_faceLandmarkPoints.Count != 0){OpenCVForUnityUtils.DrawFaceLandmark(debugMat, imported_faceLandmarkPoints, new Scalar(0, 0, 255, 255), 2);}
                    }
                }
                else
                {
                    //Define CLAHE
                    CLAHE clahe = Imgproc.createCLAHE(3,new Size(8,8));

                    // Define Gray Mat
                    Mat grayMat = new Mat();
                    // Convert BGR to Gray Mat
                    Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
                    // Apply Contrast Limited Histogram Equalization
                    clahe.apply(grayMat, grayMat);

                    // Define Downscale Gray Mat
                    Mat downScaleGrayMat = new Mat();
                    // Convert Downscale BGR Mat to Downscale Gray Mat
                    Imgproc.cvtColor(downScaleRgbaMat, downScaleGrayMat, Imgproc.COLOR_RGBA2GRAY);
                    // Apply Contrast Limited Histogram Equalization
                    clahe.apply(downScaleGrayMat, downScaleGrayMat);

                    //detect Face Rect from Downscale Matrix
                    OpenCVForUnityUtils.SetImage(faceLandmarkDetector, downScaleGrayMat);
                    List<UnityEngine.Rect> detectResult = faceLandmarkDetector.Detect();

                    if (detectResult.Count > 0)
                    {

                        // restore to original size rect
                        UnityEngine.Rect r = detectResult[0];
                        float downscaleRatio = matSourceGetterInterface.GetDownScaleRatio();
                        UnityEngine.Rect rect = new UnityEngine.Rect(
                                                    r.x * downscaleRatio,
                                                    r.y * downscaleRatio,
                                                    r.width * downscaleRatio,
                                                    r.height * downscaleRatio
                                                );

                        // detect landmark points from Original Matrix
                        OpenCVForUnityUtils.SetImage(faceLandmarkDetector, grayMat);
                        List<Vector2> points = faceLandmarkDetector.DetectLandmark(rect);

                        faceLandmarkPoints = points;
                        //add public variable to export faceLandmark
                        exported_faceLandmarkPoints = points;
                        // override old points
                        if(kalman_script.filtered_faceLandmarkPoints.Count != 0){imported_faceLandmarkPoints = kalman_script.filtered_faceLandmarkPoints;}

                        didUpdateFaceLanmarkPoints = true;

                        if (isDebugMode && screen != null)
                            OpenCVForUnityUtils.DrawFaceLandmark(debugMat, points, new Scalar(0, 255, 0, 255), 2);
                            // draw kalman filtered face landmark (add)
                            if(kalman_script.filtered_faceLandmarkPoints.Count != 0){OpenCVForUnityUtils.DrawFaceLandmark(debugMat, imported_faceLandmarkPoints, new Scalar(0, 0, 255, 255), 2);}
                    }
                }

                //Imgproc.putText (debugMat, "W:" + debugMat.width () + " H:" + debugMat.height () + " SO:" + Screen.orientation, new Point (5, debugMat.rows () - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);

                if (isDebugMode && screen != null)
                {
                    OpenCVForUnity.UnityUtils.Utils.matToTexture2D(debugMat, debugTexture, debugColors);
                }
            }
        }

        public override void Dispose()
        {
            if (faceLandmarkDetector != null)
                faceLandmarkDetector.Dispose();

            if (debugMat != null)
            {
                debugMat.Dispose();
                debugMat = null;
            }

            if (debugTexture != null)
            {
                Texture2D.Destroy(debugTexture);
                debugTexture = null;
            }

#if UNITY_WEBGL
            if (getFilePath_Coroutine != null)
            {
                StopCoroutine(getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose();
            }
#endif
        }

        #endregion


        protected virtual void Run()
        {
            faceLandmarkDetector = new FaceLandmarkDetector(dlibShapePredictorFilePath);

            didUpdateFaceLanmarkPoints = false;
        }


        #region IFaceLandmarkGetter

        public virtual List<Vector2> GetFaceLanmarkPoints()
        {
            if (didUpdateFaceLanmarkPoints)
            {
                if(kalman_script.filtered_faceLandmarkPoints.Count == 0)
                    return faceLandmarkPoints;
                else
                    return imported_faceLandmarkPoints;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}