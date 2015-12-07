using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public class ALocalization : MonoBehaviour
    {
        static ALocalization mInstance;

        static public ALocalization instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = Object.FindObjectOfType(typeof(ALocalization)) as ALocalization;

                    if (mInstance == null)
                    {
                        GameObject go = new GameObject("_Localization");
                        DontDestroyOnLoad(go);
                        mInstance = go.AddComponent<ALocalization>();
                    }
                }
                return mInstance;
            }
        }

        public string startingLanguage = "English";

        public TextAsset[] languages;

        Dictionary<string, string> mDictionary = new Dictionary<string, string>();
        string mLanguage;

        public string currentLanguage
        {
            get
            {
                return mLanguage;
            }
            set
            {
                if (mLanguage != value)
                {
                    startingLanguage = value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (languages != null)
                        {
                            for (int i = 0, imax = languages.Length; i < imax; ++i)
                            {
                                TextAsset asset = languages[i];

                                if (asset != null && asset.name == value)
                                {
                                    Load(asset);
                                    return;
                                }
                            }
                        }

                        TextAsset txt = Resources.Load(value, typeof(TextAsset)) as TextAsset;

                        if (txt != null)
                        {
                            Load(txt);
                            return;
                        }
                    }

                    PlayerPrefs.DeleteKey("Language");
                }
            }
        }

        void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this;
                DontDestroyOnLoad(gameObject);

                currentLanguage = PlayerPrefs.GetString("Language", startingLanguage);

                if (string.IsNullOrEmpty(mLanguage) && (languages != null && languages.Length > 0))
                {
                    currentLanguage = languages[0].name;
                }
            }
            else Destroy(gameObject);
        }

        void OnEnable() { if (mInstance == null) mInstance = this; }

        void OnDestroy() { if (mInstance == this) mInstance = null; }

        void Load(TextAsset asset)
        {
            mLanguage = asset.name;
            PlayerPrefs.SetString("Language", mLanguage);
            ByteReader reader = new ByteReader(asset);
            mDictionary = reader.ReadDictionary();
            //UIRoot.Broadcast("OnLocalize", this);
        }

        public string Get(string key)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return key;
#endif
            string val;
#if UNITY_IPHONE || UNITY_ANDROID
		if (mDictionary.TryGetValue(key + " Mobile", out val)) return val;
#endif

#if UNITY_EDITOR
            if (mDictionary.TryGetValue(key, out val)) return val;
            Debug.LogWarning("Localization key not found: '" + key + "'");
            return key;
#else
		return (mDictionary.TryGetValue(key, out val)) ? val : key;
#endif
        }

        static public string Localize(string key) { return (instance != null) ? instance.Get(key) : key; }
    }
}


