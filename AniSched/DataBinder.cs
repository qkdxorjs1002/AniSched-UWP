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

            foreach (DataTypes.ListData tmp in targetData)
            {
                AddList(tmp, listToBind);
            }

            targetObject.ItemsSource = listToBind;
        }

        public void DataBind(ListView targetObject, DataTypes.ListData[] targetData, String targetStr)
        {
            List<DataTypes.ListData> listToBind = new List<DataTypes.ListData>();

            if (targetStr == null)
            {
                foreach (DataTypes.ListData tmp in targetData)
                {
                    AddList(tmp, listToBind);
                }

                targetObject.ItemsSource = listToBind;
            }
            else if (targetStr == "%1002&FAVLIST%1002&")
            {
                List<string> idList = new List<string>();

                foreach (DataTypes.ListData tmp in targetData)
                {
                    byte isAdded = 0;

                    foreach (string targetId in MainFunction.favList)
                    {
                        if (tmp.i.ToString() == targetId)
                        {
                            isAdded = 1;
                        }

                    }
                    
                    if (isAdded != 0)
                    {
                        idList.Add(tmp.i.ToString());
                        AddList(tmp, listToBind);
                    }

                }

                targetObject.ItemsSource = listToBind;
                MainFunction.favList = idList;
            }
            else
            {
                foreach (DataTypes.ListData tmp in targetData)
                {
                    if (tmp.s.Contains(targetStr) == false)
                    {
                        continue;
                    }

                    AddList(tmp, listToBind);
                }

                targetObject.ItemsSource = listToBind;
            }

        }

        void AddList(DataTypes.ListData tmp, List<DataTypes.ListData> targetList)
        {
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

    }

}
