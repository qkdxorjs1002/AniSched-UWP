using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AniSched
{
    class OnnadaThumb
    {
        const string URL_IMGSOURCE = "http://anisched.moeru.me/";
        const string onnada = "http://onnada.com/search/?q=";

        HttpClient httpClient = new HttpClient();
        HttpResponseMessage httpResponse = new HttpResponseMessage();
        

        public async Task<Uri> Request(string title, int id)
        {
            string htmlDocument;
            try
            {
                httpResponse = await httpClient.GetAsync(new Uri(onnada + title));
                htmlDocument = await httpResponse.Content.ReadAsStringAsync();

                var startIndex = htmlDocument.LastIndexOf("http://data5.onnada.com/anime");
                var length = htmlDocument.LastIndexOf("' align='absmiddle' border='0'") - startIndex;

                return new Uri(htmlDocument.Substring(startIndex, length).Replace("thumb300x400_", ""), UriKind.Absolute);
            }
            catch (Exception)
            {
                return new Uri(URL_IMGSOURCE + id.ToString(), UriKind.Absolute);
            }
        }
    }
}
