using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;


namespace DetectEncoding.business
{
    public class LangMgr
    {
        private static LangMgr _instance;

        private static readonly Dictionary<String, ResourceManager> _resourceManagerByCulture =
            new Dictionary<string, ResourceManager>(1);

        private static string _dftCulture;

        public static void AddResourceManager(String cultureTwoLetterRm, ResourceManager resourceManager)
        {
            _resourceManagerByCulture.Add(cultureTwoLetterRm, resourceManager);

        }

        public static void ForceCulture(string twoLetterCulture)
        {
            if (_resourceManagerByCulture.ContainsKey(twoLetterCulture))
            {
                _dftCulture = twoLetterCulture;
            }
            else
            {
                throw new Exception("There is no culture with two letter '" + twoLetterCulture + " that is set");
            }
        }

        public static LangMgr Instance
        {
            get { return GetInstance(); }
            private set { _instance = value; }
        }

        private static LangMgr GetInstance()
        {
            if (_instance == null)
            {
                if (_resourceManagerByCulture.Count == 0)
                {
                    throw new Exception("LangMgr has no ResourceManager set");
                }

                _instance = new LangMgr(_dftCulture ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            }
            return _instance;
        }


        public String LangangeTwoLetter { get; private set; }

        public String this[string str]
        {
            get { return _currentManager.GetString(str); }

        }

        private readonly ResourceManager _currentManager;



        private LangMgr(String culture)
        {
            LangangeTwoLetter = culture;

            if (_resourceManagerByCulture.ContainsKey(culture))
            {
                _currentManager = _resourceManagerByCulture[culture];
            }


        }
    }
}
