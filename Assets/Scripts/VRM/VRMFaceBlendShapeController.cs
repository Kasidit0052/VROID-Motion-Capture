using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace CVVTuber.VRM
{
    public class VRMFaceBlendShapeController : FaceAnimationController
    {
        [Header("[Target]")]

        public VRMBlendShapeProxy blendShapeProxy;

        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Update face BlendShape of VRM using FaceLandmarkGetter.";
        }

        public override void LateUpdateValue()
        {
            if (blendShapeProxy == null)
                return;

            blendShapeProxy.Apply();
        }

        #endregion


        #region FaceAnimationController

        public override void Setup()
        {
            base.Setup();

            NullCheck(blendShapeProxy, "blendShapeProxy");
        }

        public override void UpdateValue()
        {
            if (blendShapeProxy == null)
                return;

            base.UpdateValue();
        }

        protected override void UpdateFaceAnimation(List<Vector2> points)
        {
            if (enableEye)
            {
                float eyeOpen = (GetLeftEyeOpenRatio(points) + GetRightEyeOpenRatio(points)) / 2.0f;
                //Debug.Log("eyeOpen " + eyeOpen);

                if (eyeOpen >= 0.4f)
                {
                    eyeOpen = 1.0f;
                }
                else
                {
                    eyeOpen = 0.0f;
                }
                EyeParam = Mathf.Lerp(EyeParam, 1.0f - eyeOpen, eyeLeapT);

                blendShapeProxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), EyeParam);
            }

            if (enableMouth)
            {
                float mouthOpen = GetMouthOpenYRatio(points);
                //Debug.Log("mouthOpen " + mouthOpen);

                if (mouthOpen >= 0.90f)
                {
                    mouthOpen = 1.0f;
                }
                else if (mouthOpen >= 0.75f)
                {
                    mouthOpen = 0.50f;
                }
                else if (mouthOpen >= 0.60f)
                {
                    mouthOpen = 0.15f;
                }
                else
                {
                    mouthOpen = 0.0f;
                }
                MouthOpenParam = Mathf.Lerp(MouthOpenParam, mouthOpen, mouthLeapT);

                blendShapeProxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), MouthOpenParam);
                blendShapeProxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O), MouthOpenParam * 0.7f);


                float mouthSize = GetMouthOpenXRatio(points);
                //Debug.Log("mouthSize " + mouthSize);

                if (mouthSize >= 0.90f)
                {
                    mouthSize = 0.5f;
                }
                else if (mouthSize >= 0.75f)
                {
                    mouthSize = 0.25f;
                }
                else if (mouthSize >= 0.60f)
                {
                    mouthSize = 0.05f;
                }
                else
                {
                    mouthSize = 0.0f;
                }
                MouthSizeParam = Mathf.Lerp(MouthSizeParam, mouthSize, mouthLeapT);

                blendShapeProxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I), MouthSizeParam);
            }
        }

        #endregion
    }
}