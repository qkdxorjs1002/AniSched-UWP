using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniSched
{

    class DataRefiner
    {

        public void ListDataRefine(DataTypes.ListData[] listData)
        {
            int toDay = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

            foreach (DataTypes.ListData ld in listData)
            {
                try
                {
                    int startDay = Convert.ToInt32(ld.sd);
                    int endDay = Convert.ToInt32(ld.ed);

                    ld.s = ld.s.Replace("&lt;", "<");
                    ld.s = ld.s.Replace("&gt;", ">");
                    ld.s = ld.s.Replace("&amp;", "&");
                    ld.t = ld.t.Insert(2, ":");
                    ld.l = ld.l.Trim();
                    ld.l = ld.l.Insert(0, "http://");
                    ld.a = (ld.a == "true" ? (toDay.CompareTo(endDay) > 0 && endDay != 0 ? "#FF455A64" : (toDay.CompareTo(startDay) < 0 && startDay != 0 ? "#FF00B0FF" : "#FF00E676")) : "#FFFF1744");
                    ld.sd = ld.sd.Insert(4, "/");
                    ld.sd = ld.sd.Insert(7, "/");
                    ld.ed = ld.ed.Insert(4, "/");
                    ld.ed = ld.ed.Insert(7, "/");

                } catch { }

            }

        }

        public void CaptionDataRefine(DataTypes.ListData[] listData)
        {
            foreach (DataTypes.ListData ld in listData)
            {
                try
                {
                    if (ld.s.Length == 5)
                    {
                        ld.s = ld.s.Insert(4, ".");
                        ld.s = ((Convert.ToSingle(ld.s)).ToString()).Replace(".0", "") + "화";
                        ld.s = (ld.s == "9999화" ? "TVA/BD" : ld.s);

                    }

                    ld.d = ld.d.Remove(0, 4);
                    ld.d = ld.d.Insert(2, "/");
                    ld.d = ld.d.Insert(5, "-");
                    ld.d = ld.d.Insert(8, ":");
                    ld.d = ld.d.Insert(11, ":");
                    ld.a = (ld.a.Trim()).Insert(0, "http://");
                    ld.n = (ld.n == null ? "제작자 정보 없음" : ld.n);

                } catch { }

            }

        }

    }

}
