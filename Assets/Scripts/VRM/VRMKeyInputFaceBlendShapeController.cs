using UnityEngine;
using VRM;

namespace CVVTuber.VRM
{
    public class VRMKeyInputFaceBlendShapeController : CVVTuberProcess
    {
        [Header("[Target]")]

        public VRMBlendShapeProxy target;


        #region CVVTuberProcess

        public override string GetDescription()
        {
            return "Update face BlendShape of VRM using KeyInput.";
        }

        public override void Setup()
        {
            NullCheck(target, "target");
        }

        public override void UpdateValue()
        {
            float value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), value);

            /*
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft), value);

            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E), value);
            value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O)) - 0.5f);
            target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O), value);
            */

            if (Input.GetKey(KeyCode.Z))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun), value);
            }
            if (Input.GetKey(KeyCode.X))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry), value);
            }
            if (Input.GetKey(KeyCode.C))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy), value);
            }
            if (Input.GetKey(KeyCode.V))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), value);
            }

            /*
            if (Input.GetKey(KeyCode.UpArrow))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp), value);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown), value);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight), value);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft), value);
            }

            if (Input.GetKey(KeyCode.A))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), value);
            }
            if (Input.GetKey(KeyCode.I))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I), value);
            }
            if (Input.GetKey(KeyCode.U))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U), value);
            }
            if (Input.GetKey(KeyCode.E))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E), value);
            }
            if (Input.GetKey(KeyCode.O))
            {
                value = Mathf.Clamp01(target.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O)) + 1.0f);
                target.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O), value);
            }
            */
        }

        public override void LateUpdateValue()
        {
            target.Apply();
        }

        #endregion
    }
}
