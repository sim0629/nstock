using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace XingPrice
{
    class Query
    {
        private XAQuery query;
        private int year, month;
        private string code;

        private string in_block_name = "t8412InBlock";
        private string out_block_name = "t8412OutBlock1";

        private int BlockCount
        {
            get
            {
                return query.GetBlockCount(out_block_name);
            }
        }

        private string GetFieldData(string field_name, int index)
        {
            return query.GetFieldData(out_block_name, field_name, index);
        }

        public Query(string code, int year, int month)
        {
            query = (XAQuery)Activator.CreateInstance(Type.GetTypeFromProgID("XA_DataSet.XAQuery"));
            query.ResFileName = @"C:\eBEST\xingAPI\Res\t8412.res";
            query.SetFieldData(in_block_name, "shcode", 0, code);
            query.SetFieldData(in_block_name, "ncnt", 0, "10"); // 10분
            query.SetFieldData(in_block_name, "comp_yn", 0, "N");
            query.ReceiveData += Receive_Handler;
            this.year = year;
            this.month = month;
            this.code = code;
        }

        public int Run()
        {
            query.SetFieldData(in_block_name, "sdate", 0, String.Format("{0}{1}01", year, month.ToString("D2")));
            query.SetFieldData(in_block_name, "edate", 0, String.Format("{0}{1}31", year, month.ToString("D2")));
            return query.Request(false);
        }

        private void Receive_Handler(string szTrCode)
        {
            var fn = String.Format("{0}-{1}{2}.txt", code, year, month.ToString("D2"));
            using (var writer = new StreamWriter(fn, false, Encoding.ASCII))
            {
                var prevDate = "";
                var count = BlockCount;
                for (var i = 0; i < count; i++)
                {
                    var date = GetFieldData("date", i);
                    var time = GetFieldData("time", i);
                    var close = GetFieldData("close", i);
                    if (date != prevDate)
                    {
                        var open = GetFieldData("open", i);
                        var hours = Int32.Parse(time.Substring(0, 2));
                        var minutes = Int32.Parse(time.Substring(2, 2));
                        var val = hours * 60 + minutes - 10;
                        hours = val / 60; minutes = val % 60;
                        writer.WriteLine("{0} {1}{2}00 {3}", date, hours.ToString("D2"), minutes.ToString("D2"), open);
                        prevDate = date;
                    }
                    writer.WriteLine("{0} {1} {2}", date, time, close);
                }
            }
            Console.WriteLine(fn);
        }
    }
}
