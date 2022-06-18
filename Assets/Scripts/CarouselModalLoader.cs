using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using VRM;
namespace VRMLoader
{

    public class CarouselModalLoader : MonoBehaviour
    {
        [SerializeField, Header("GUI")]
        Canvas m_canvas;

        [SerializeField]
        GameObject m_modalWindowPrefab;

        [SerializeField]
        Dropdown m_language;

        VRMImporterContext m_context;

        public CreateCarousel create_carousel_script;

        void Start()
        {
            create_carousel_script =  GameObject.Find("CarouselModelLoader").GetComponent<CreateCarousel>();
        }

        public void OpenModalVRM()
        {
            var path = "file:///" + create_carousel_script.carouselObjects[create_carousel_script.ChosenObject].GetComponent<CarouselModelLoaderHandler>().context_path;
            StartCoroutine(LoadModalVRMCoroutine(path));
        }
        
        IEnumerator LoadModalVRMCoroutine(string path)
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
        public void ModelLoad()
        {
            SceneManager.LoadScene("VRMCVVTuberExample", LoadSceneMode.Single);
        }
    }

}