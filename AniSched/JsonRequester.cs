using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace AniSched
{

    class JsonRequester
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage httpResponse = new HttpResponseMessage();

        string baseUrl, listUrl, endUrl, captionUrl;

        public JsonRequester(string baseUrl, string listUrl, string endUrl, string captionUrl)
        {
            this.baseUrl = baseUrl;
            this.listUrl = listUrl;
            this.endUrl = endUrl;
            this.captionUrl = captionUrl;

        }
        
        public async Task<DataTypes.HttpData> Request(JsType jsType, int id)
        {
            string uri = baseUrl;

            switch (jsType)
            {
                case JsType.List:
                    uri += listUrl;
                    break;
                case JsType.End:
                    uri += endUrl;
                    break;
                case JsType.Capton:
                    uri += captionUrl;
                    break;
            }

            httpResponse = await httpClient.GetAsync(new Uri(uri + id));

            return new DataTypes.HttpData(httpResponse.StatusCode.GetHashCode()
                , await httpResponse.Content.ReadAsStringAsync());
        }

    }

}
