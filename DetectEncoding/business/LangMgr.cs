using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;


namespace DetectEncoding.business
{
    public class LangMgr
    {
        private static LangMgr _instance;

        private static readonly Dictionary<string, ResourceManager> ResourceManagerByCulture =
            new Dictionary<string, ResourceManager>(1);

        private static string _dftCulture;

        public static void AddResourceManager(string cultureTwoLetterRm, ResourceManager resourceManager)
        {
            ResourceManagerByCulture.Add(cultureTwoLetterRm, resourceManager);

        }

        public static void ForceCulture(string twoLetterCulture)
        {
            if (ResourceManagerByCulture.ContainsKey(twoLetterCulture))
            {
                _dftCulture = twoLetterCulture;
            }
            else
            {
                throw new Exception("There is no culture with two letter '" + twoLetterCulture + " that is set");
            }
        }

        public static LangMgr Instance => GetInstance();

        private static LangMgr GetInstance()
        {
            if (_instance == null)
            {
                if (ResourceManagerByCulture.Count == 0)
                {
                    throw new Exception("LangMgr has no ResourceManager set");
                }

                _instance = new LangMgr(_dftCulture ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            }
            return _instance;
        }


        public string LangangeTwoLetter { get; private set; }

        public string this[string str] => _currentManager.GetString(str);

        private readonly ResourceManager _currentManager;



        private LangMgr(string culture)
        {
            LangangeTwoLetter = culture;

            if (ResourceManagerByCulture.TryGetValue(culture, out ResourceManager ressMgr))
            {
                _currentManager = ressMgr;

            }


        }
    }
}
