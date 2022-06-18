using System;
using System.Collections;
using UnityEngine;

namespace CVVTuber.VRM
{
    [RequireComponent(typeof(VRMLoader))]
    public class VRMCVVTuberControllManager : CVVTuberControllManager
    {
        public RuntimeAnimatorController animatorController;

        public bool useRuntimeLoader;

        protected VRMLoader vrmLoader;

        protected IEnumerator coroutine;

        // Use this for initialization
        protected override IEnumerator Start()
        {
            enabled = false;

            yield return null;

            processOrderList = GetComponent<CVVTuberProcessOrderList>().GetProcessOrderList();
            if (processOrderList == null)
                yield break;

            vrmLoader = GetComponent<VRMLoader>();
            if (vrmLoader != null)
            {
                if (useRuntimeLoader)
                {
                    // Add Information Parser
                    vrmLoader.vrmFileName = GameObject.Find("InformationParser").GetComponent<InformationParser>()._vrmFilePath;

                    if (!string.IsNullOrEmpty(vrmLoader.vrmFileName))
                    {
                        coroutine = vrmLoader.LoadVRMAsync(vrmLoader.vrmFileName);
                        yield return coroutine;
                        coroutine = null;
                    }
                    else
                    {
                        Debug.LogWarning("vrmLoader.vrmFileName is null or empty.");
                    }
                }
                else
                {
                    if (vrmLoader.meta != null)
                    {
                        vrmLoader.LoadMeta(vrmLoader.meta);
                    }
                    else
                    {
                        Debug.LogWarning("vrmLoader.meta is null.");
                    }
                }
            }

            if (vrmLoader != null && (vrmLoader.isError || vrmLoader.meta == null))
                yield break;

            if (animatorController != null)
            {
                if (vrmLoader.animator != null && vrmLoader.animator.runtimeAnimatorController == null)
                    vrmLoader.animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorController;
            }

            foreach (var item in processOrderList)
            {
                if (item == null)
                    continue;

                //Debug.Log("Setup : "+item.gameObject.name);

                if (item is HeadRotationController)
                {
                    if (vrmLoader.lookAtHead != null)
                        ((HeadRotationController)item).target = vrmLoader.lookAtHead.Head;
                }
                if (item is HeadLookAtIKController)
                {
                    if (vrmLoader.animator != null)
                        ((HeadLookAtIKController)item).target = vrmLoader.animator;

                    var lookAtLoot = GameObject.Find("LookAtRoot").transform;
                    if (lookAtLoot != null)
                    {
                        ((HeadLookAtIKController)item).lookAtRoot = lookAtLoot;
                        var lookAtTarget = lookAtLoot.transform.Find("LookAtTarget").transform;
                        if (lookAtTarget != null)
                        {
                            ((HeadLookAtIKController)item).lookAtTarget = lookAtTarget;
                        }
                    }
                }
                if (item is VRMFaceBlendShapeController)
                {
                    if (vrmLoader.blendShape != null)
                        ((VRMFaceBlendShapeController)item).blendShapeProxy = vrmLoader.blendShape;
                }
                if (item is VRMKeyInputFaceBlendShapeController)
                {
                    if (vrmLoader.blendShape != null)
                        ((VRMKeyInputFaceBlendShapeController)item).target = vrmLoader.blendShape;
                }

                item.Setup();
            }

            enabled = true;
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (vrmLoader != null && vrmLoader.meta == null)
                return;

            base.Update();
        }

        // Update is called once per frame
        protected override void LateUpdate()
        {
            if (vrmLoader != null && vrmLoader.meta == null)
                return;

            base.LateUpdate();
        }

        public override void Dispose()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                ((IDisposable)coroutine).Dispose();
                coroutine = null;
            }
        }
    }
}