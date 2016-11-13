using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace AniSched
{

    class JsonParser
    {

        public DataTypes.ListData[] Parse(string jsonData)
        {
            jsonData = ((jsonData.Replace("[", "")).Replace("]", "")).Replace("},{", "}{");

            IList<DataTypes.ListData> dataListCollection = new List<DataTypes.ListData>();
            JsonTextReader jsonReader = new JsonTextReader(new StringReader(jsonData));
            jsonReader.SupportMultipleContent = true;

            while (jsonReader.Read())
            {
                JsonSerializer Serializer = new JsonSerializer();
                DataTypes.ListData dataList = Serializer.Deserialize<DataTypes.ListData>(jsonReader);

                dataListCollection.Add(dataList);
            }

            return dataListCollection.ToArray<DataTypes.ListData>();
        }

    }

}
