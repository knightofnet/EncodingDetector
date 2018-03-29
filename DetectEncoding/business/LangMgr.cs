using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.AccessControl;
using System.Text;
using DetectEncoding.constant.texts;

namespace DetectEncoding.business
{
    public class LangMgr
    {
        private static LangMgr _instance;

        public static LangMgr Instance
        {
            get { return _instance ?? (_instance = new LangMgr(CultureInfo.CurrentCulture.TwoLetterISOLanguageName)); }
            private set { _instance = value; }
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

            if (culture.Equals("fr"))
            {
                _currentManager = Resource_fr_Fr.ResourceManager;
            }
            else if (culture.Equals("en"))
            {
                _currentManager = Resource_en_US.ResourceManager;
            }
            else
            {
                _currentManager = Resource_en_US.ResourceManager;
            }

        }



    }
}
