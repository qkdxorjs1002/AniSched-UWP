using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AniSched
{

    class DataBinder
    {

        public void DataBind(ListView targetObject, DataTypes.ListData[] targetData)
        {
            List<DataTypes.ListData> listToBind = new List<DataTypes.ListData>();
            foreach (DataTypes.ListData tmp in targetData)
            {
                listToBind.Add(new DataTypes.ListData { i = tmp.i, s = tmp.s, t = tmp.t, g = tmp.g, l = tmp.l, a = tmp.a, sd = tmp.sd, ed = tmp.ed, d = tmp.d, n = tmp.n });
            }
            targetObject.ItemsSource = listToBind;

        }
    }

}
