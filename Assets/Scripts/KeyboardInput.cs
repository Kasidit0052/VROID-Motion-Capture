using CVVTuber;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CVVTuberExample
{
    public class KeyboardInput : MonoBehaviour
    {
        public int Debug_Mode = 0;
        private GameObject VRMCVVtuber;
        private VRMCVVTuberExample  VRMCVVTuberExampleScript;
        private DlibFaceLandmarkGetter dlibFaceLandmarkGetterScript;

        // Start is called before the first frame update
        void Start()
        {
            VRMCVVtuber = GameObject.FindWithTag("VRMCVVTuber");
            VRMCVVTuberExampleScript = VRMCVVtuber.GetComponent<VRMCVVTuberExample>();
            dlibFaceLandmarkGetterScript = VRMCVVTuberExampleScript.dlibFaceLandmarkGetter;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                Debug_Mode+=1;
                
                if(Debug_Mode > 2)
                {
                    Debug_Mode = 0;
                }

                switch (Debug_Mode)
                {
                case 0:
                    dlibFaceLandmarkGetterScript.hideImage = false;
                    dlibFaceLandmarkGetterScript.isDebugMode = true;
                    break;
                case 1:
                    dlibFaceLandmarkGetterScript.hideImage = true;
                    dlibFaceLandmarkGetterScript.isDebugMode = true;
                    break;
                case 2:
                    dlibFaceLandmarkGetterScript.hideImage = false;
                    dlibFaceLandmarkGetterScript.isDebugMode = false;
                    break;
                default:
                    Debug.Log("Case Not Used");
                    break;
                }
            }
        }
    }
}
