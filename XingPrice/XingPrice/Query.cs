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

        private string code = "084990";
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

        public Query(int year, int month)
        {
            query = (XAQuery)Activator.CreateInstance(Type.GetTypeFromProgID("XA_DataSet.XAQuery"));
            query.ResFileName = @"C:\eBEST\xingAPI\Res\t8412.res";
            query.SetFieldData(in_block_name, "shcode", 0, code); // 바이로메드
            query.SetFieldData(in_block_name, "ncnt", 0, "10");
            query.SetFieldData(in_block_name, "comp_yn", 0, "N");
            query.ReceiveData += Receive_Handler;
            this.year = year;
            this.month = month;
        }

        public int Run()
        {
            query.SetFieldData(in_block_name, "sdate", 0, String.Format("{0}{1}01", year, month.ToString("D2")));
            query.SetFieldData(in_block_name, "edate", 0, String.Format("{0}{1}31", year, month.ToString("D2")));
            return query.Request(false);
        }

        private void Receive_Handler(string szTrCode)
        {
            using (var writer = new StreamWriter(String.Format("{0}-{1}{2}.txt", code, year, month.ToString("D2")), false, Encoding.UTF8))
            {
                var count = BlockCount;
                for (var i = 0; i < count; i++)
                {
                    var date = GetFieldData("date", i);
                    var time = GetFieldData("time", i);
                    var open = GetFieldData("open", i);
                    writer.WriteLine("{0} {1} {2}", date, time, open);
                }
            }
        }
    }
}
