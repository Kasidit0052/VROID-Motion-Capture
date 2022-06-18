using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class CarouselModelLoaderHandler : MonoBehaviour
{
        VRMImporterContext m_context;
        public string context_path;
        public void CarouselOpenVRM(string path)
        {
            context_path = path;
            var des_path = "file:///" + context_path;
            if (des_path.Length != 0)
            {
                GameObject.Find("Canvas").transform.Find("btn_Load").gameObject.SetActive(false);
                StartCoroutine(LoadVRMCoroutine(des_path));
            }
        }

        IEnumerator LoadVRMCoroutine(string path)
        {
            var www = new WWW(path);
            yield return www;

            // GLB形式のperse
            m_context = new VRMImporterContext();
            m_context.ParseGlb(www.bytes);

            // meta情報を読み込む
            bool createThumbnail=true;
            var meta = m_context.ReadMeta(createThumbnail);
            ModelLoad();
        }

        private void ModelLoad()
        {
            var now = Time.time;
            m_context.LoadAsync(_ =>
                {
                    m_context.ShowMeshes();
                    var go = m_context.Root;
                    // load完了
                    var delta = Time.time - now;
                    Debug.LogFormat("LoadVrmAsync {0:0.0} seconds", delta);
                    OnLoaded(go);
                },
                Debug.LogError);
        }

        void OnLoaded(GameObject root)
        {
            // 設置先 hierarchy を決める
            GameObject.Find("Canvas").transform.Find("btn_Load").gameObject.SetActive(true);
            root.transform.SetParent(transform, false);
        }
}
