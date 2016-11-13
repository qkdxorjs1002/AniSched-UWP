using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniSched
{
    public enum JsType { List, End, Capton };
    public enum Days { Sun, Mon, Tue, Wed, Thu, Fri, Sat, Ova, New };
    
    class DataTypes
    {

        public class HttpData
        {
            public int statusCode { get; set; }
            public string jsonData { get; set; }

            public HttpData(int statusCode, string jsonData)
            {
                this.statusCode = statusCode;
                this.jsonData = jsonData;

            }

        }

        public class ListData
        {
            public int i { get; set; }      // ID
            public string s { get; set; }   // Title / Chapter
            public string t { get; set; }   // Time
            public string g { get; set; }   // Genre
            public string l { get; set; }   // Site
            public string a { get; set; }   // Status / Link
            public string sd { get; set; }  // Start
            public string ed { get; set; }  // End
            public string d { get; set; }   // Date / RefreshDate
            public string n { get; set; }   // Publisher

        }

    }

}
