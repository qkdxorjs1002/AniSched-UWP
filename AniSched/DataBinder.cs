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

            if (targetData.Length == 0)
            {
                targetData = new DataTypes.ListData[] { new DataTypes.ListData { s = "목록 없음", d = "원인", n = "제작자가 없습니다." } };

            }

            AddList(targetObject, targetData, listToBind, null);

        }

        public void DataBind(ListView targetObject, DataTypes.ListData[] targetData, String targetStr)
        {
            List<DataTypes.ListData> listToBind = new List<DataTypes.ListData>();

            AddList(targetObject, targetData, listToBind, targetStr);

        }

        void AddList(ListView targetObject, DataTypes.ListData[] targetData, List<DataTypes.ListData> targetList, String targetStr)
        {
            foreach (DataTypes.ListData tmp in targetData)
            {
                if (targetStr != null && tmp.s.Contains(targetStr) == false)
                {
                    continue;

                }

                targetList.Add(new DataTypes.ListData
                {
                    iid = tmp.iid
                    ,
                    iis = tmp.iis
                    ,
                    i = tmp.i
                    ,
                    s = tmp.s
                    ,
                    t = ((tmp.iid == 7 || tmp.iid == 8) ? tmp.sd : tmp.t)
                    ,
                    g = tmp.g
                    ,
                    l = tmp.l
                    ,
                    a = tmp.a
                    ,
                    sd = tmp.sd
                    ,
                    ed = tmp.ed
                    ,
                    d = tmp.d
                    ,
                    n = tmp.n
                });

            }

            targetObject.ItemsSource = targetList;

        }

    }

}
