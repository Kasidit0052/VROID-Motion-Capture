using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using SFB;

namespace VRMLoader
{
    public class ModelLoaderLegacy : MonoBehaviour
    {
        [SerializeField, Header("GUI")]
        Canvas m_canvas;

        [SerializeField]
        GameObject m_modalWindowPrefab;

        [SerializeField]
        Dropdown m_language;

        VRMImporterContext m_context;

        public void OpenVRM()
        {
            // Open file with filter
            var extensions = new [] {
                new ExtensionFilter("VRM Files", "vrm")
            };
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
            var path = "file:///" + paths[0];
            if (path.Length != 0)
            {
                StartCoroutine(LoadVRMCoroutine(path));
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

            // ファイル読み込みモーダルウィンドウの呼び出し
            GameObject modalObject = Instantiate(m_modalWindowPrefab, m_canvas.transform) as GameObject;

            // 言語設定を取得・反映する
            var modalLocale = modalObject.GetComponentInChildren<VRMPreviewLocale>();
            modalLocale.SetLocale(m_language.captionText.text);

            // meta情報の反映
            var modalUI = modalObject.GetComponentInChildren<VRMPreviewUI>();
            modalUI.setMeta(meta);

            // ファイルを開くことの許可
            // ToDo: ファイルの読み込み許可を制御する場合はここで
            modalUI.setLoadable(true);

            modalUI.m_ok.onClick.AddListener(ModelLoad);
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
            root.transform.SetParent(transform, false);
        }
    }
}