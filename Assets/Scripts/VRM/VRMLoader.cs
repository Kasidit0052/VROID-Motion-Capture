using System;
using System.Collections;
using System.IO;
using UnityEngine;
using VRM;

namespace CVVTuber.VRM
{
    public class VRMLoader : MonoBehaviour
    {
        [Tooltip("A filename for runtime loading from StreamingAssets folder. (e.g. \"VRM/AliciaSolid.vrm\")")]
        public string vrmFileName;

        [Space(5)]

        public VRMMeta meta;

        [Space(10)]

        public LookTarget faceCamera;

        public VRMFirstPerson firstPerson;

        public VRMBlendShapeProxy blendShape;

        public VRMLookAtHead lookAtHead;

        public Animator animator;

        public virtual bool isDone { get; protected set; }

        public virtual bool isError { get; protected set; }

        protected VRMImporterContext context;

        public virtual void LoadMeta(VRMMeta meta)
        {
            SetupTarget(meta);
        }

        public virtual void LoadVRMSync(string fileName)
        {
            if (context != null)
                Dispose();
            ImportVRMSync(OpenCVForUnity.UnityUtils.Utils.getFilePath(vrmFileName));
        }

        public virtual IEnumerator LoadVRMAsync(string fileName)
        {
            yield return OpenCVForUnity.UnityUtils.Utils.getFilePathAsync(vrmFileName, (result) =>
            {
                if (context != null)
                    Dispose();
                ImportVRMAsync(result);
            });

            while (!isDone)
            {
                yield return null;
            }
        }

        public virtual void Dispose()
        {
            isDone = isError = false;

            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }

        protected virtual void ImportVRMSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(".vrm file does not exist.");
                isDone = isError = true;
                return;
            }

            var bytes = File.ReadAllBytes(path);

            context = new VRMImporterContext();

            context.ParseGlb(bytes);

            var metaObject = context.ReadMeta(false);
            Debug.LogFormat("meta: title:{0}", metaObject.Title);
            Debug.LogFormat("meta: version:{0}", metaObject.Version);
            Debug.LogFormat("meta: author:{0}", metaObject.Author);
            Debug.LogFormat("meta: exporterVersion:{0}", metaObject.ExporterVersion);

            context.Load();

            OnLoaded(context);
        }

        protected virtual void ImportVRMAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(".vrm file does not exist.");
                isDone = isError = true;
                return;
            }

            var bytes = File.ReadAllBytes(path);

            context = new VRMImporterContext();

            context.ParseGlb(bytes);

            var metaObject = context.ReadMeta(false);
            Debug.LogFormat("meta: title:{0}", metaObject.Title);
            Debug.LogFormat("meta: version:{0}", metaObject.Version);
            Debug.LogFormat("meta: author:{0}", metaObject.Author);
            Debug.LogFormat("meta: exporterVersion:{0}", metaObject.ExporterVersion);

            context.LoadAsync(() =>
            {
                OnLoaded(context);
            }, OnError);
        }

        protected virtual void OnLoaded(VRMImporterContext context)
        {
            var root = context.Root;
            //root.transform.SetParent(transform, false);
            context.ShowMeshes();

            meta = root.GetComponent<VRMMeta>();
            SetupTarget(meta);

            meta.transform.localPosition = new Vector3(0, 0, 0);
            //meta.transform.localRotation = new Quaternion(0, 180, 0, 0);

            isDone = true;
            isError = false;
        }

        protected virtual void OnError(Exception e)
        {
            Debug.LogError(e);
            Dispose();
            isDone = true;
        }

        public void SetupTarget(VRMMeta meta)
        {
            blendShape = meta.GetComponent<VRMBlendShapeProxy>();

            firstPerson = meta.GetComponent<VRMFirstPerson>();

            animator = meta.GetComponent<Animator>();
            if (animator != null)
            {
                firstPerson.Setup();

                if (faceCamera != null)
                {
                    faceCamera.Target = animator.GetBoneTransform(HumanBodyBones.Head);
                }
            }

            lookAtHead = meta.GetComponent<VRMLookAtHead>();
        }
    }
}
