using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.VideoModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

namespace CVVTuber
{
    public class KalmanScript : MonoBehaviour
    {

        /// <summary>
        /// The kalman filter.
        /// </summary>
        KalmanFilter KF;

        /// <summary>
        /// The measurement.
        /// </summary>
        Mat[] measurement;

        // variable to stored face landmark point
        private bool didUpdate;
        private List<Vector2> imported_faceLandmarkPoints;
        public List<Vector2> filtered_faceLandmarkPoints;
        public DlibFaceLandmarkGetter DlibFaceLandmarkGetterscript;

        // declare public variable to stored kalman filters
        public KalmanFilter[] kalmanFilter_array;

        // Use this for initialization
        void Start ()
        {
            // initializing kalman filter array
            kalmanFilter_array = new KalmanFilter[68];
            measurement = new Mat[68];
        }

        // Update is called once per frame
        void Update ()
        {
            Point predictedPt;
            Point estimatedPt;

            // import face landmarks
            imported_faceLandmarkPoints = DlibFaceLandmarkGetterscript.exported_faceLandmarkPoints;

            // Set initial state estimate for dlib.
            if(!didUpdate && imported_faceLandmarkPoints.Count != 0)
            {
                
                for(int i = 0; i < imported_faceLandmarkPoints.Count; i++)
                {
                    kalmanFilter_array[i] = new KalmanFilter (4, 2, 0, CvType.CV_32FC1);

                    // intialization of KF...
                    Mat transitionMat = new Mat (4, 4, CvType.CV_32F);
                    transitionMat.put (0, 0, new float[] { 1, 0, 1, 0,   0, 1, 0, 1,  0, 0, 1, 0,  0, 0, 0, 1 });
                    kalmanFilter_array[i].set_transitionMatrix (transitionMat);

                    measurement[i] = new Mat (2, 1, CvType.CV_32FC1);
                    measurement[i].setTo (Scalar.all (0));

                    // Set initial state estimate.
                    Mat statePreMat = kalmanFilter_array[i].get_statePre ();
                    statePreMat.put (0, 0, new float[] {(float) imported_faceLandmarkPoints[i].x, (float)imported_faceLandmarkPoints[i].y});
                    Mat statePostMat = kalmanFilter_array[i].get_statePost ();
                    statePostMat.put (0, 0, new float[] {(float) imported_faceLandmarkPoints[i].x, (float)imported_faceLandmarkPoints[i].y});

                    Mat measurementMat = new Mat (2, 4, CvType.CV_32FC1);
                    Core.setIdentity (measurementMat);
                    kalmanFilter_array[i].set_measurementMatrix (measurementMat);

                    Mat processNoiseCovMat = new Mat (4, 4, CvType.CV_32FC1);
                    Core.setIdentity (processNoiseCovMat, Scalar.all (1e-4));
                    kalmanFilter_array[i].set_processNoiseCov (processNoiseCovMat);

                    Mat measurementNoiseCovMat = new Mat (2, 2, CvType.CV_32FC1);
                    Core.setIdentity (measurementNoiseCovMat, Scalar.all (10));
                    kalmanFilter_array[i].set_measurementNoiseCov (measurementNoiseCovMat);

                    Mat errorCovPostMat = new Mat (4, 4, CvType.CV_32FC1);
                    Core.setIdentity (errorCovPostMat, Scalar.all (.1));
                    kalmanFilter_array[i].set_errorCovPost (errorCovPostMat);

                }
                didUpdate = true;
            }
            // late update
            else
            {
                filtered_faceLandmarkPoints = new List<Vector2>();
                for(int i = 0; i < imported_faceLandmarkPoints.Count; i++)
                {
                    // First predict, to update the internal statePre variable.
                    using (Mat prediction = kalmanFilter_array[i].predict ()) {
                        predictedPt = new Point (prediction.get (0, 0) [0], prediction.get (1, 0) [0]);
                    }

                    // Measurement Detection
                    measurement[i].put (0, 0, new float[] { (float) imported_faceLandmarkPoints[i].x, (float)imported_faceLandmarkPoints[i].y });
                    Point measurementPt = new Point (measurement[i].get (0, 0) [0], measurement[i].get (1, 0) [0]);

                    // The update phase.
                    using (Mat estimated = kalmanFilter_array[i].correct (measurement[i])) {
                        estimatedPt = new Point (estimated.get (0, 0) [0], estimated.get (1, 0) [0]);
                        filtered_faceLandmarkPoints.Add(new Vector2((int)estimatedPt.x, (int)estimatedPt.y));
                    }
                }
            }
            //Debug.Log("P2_Real:"+ imported_faceLandmarkPoints[2] + "P2_Estimated:" + filtered_faceLandmarkPoints[2] + "P3_Real:" + imported_faceLandmarkPoints[3] + "P3_Estimated:" + filtered_faceLandmarkPoints[3] );
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy ()
        {
            if (measurement != null)
                measurement = new Mat[68];
            kalmanFilter_array = new KalmanFilter[68];
        }
    }
}
