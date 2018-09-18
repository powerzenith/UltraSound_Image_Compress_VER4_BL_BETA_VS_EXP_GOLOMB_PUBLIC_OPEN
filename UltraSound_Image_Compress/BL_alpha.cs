using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSC
{
    public class BL_alpha
    {
        public static List<byte> Encode(List<int> code_num)

        {
            StringBuilder r = new StringBuilder();
            int Z = 0;
            int CL = code_num.Count;

            for (int i = 0; i < CL; i++)
            {
                Z = code_num[i];

                //BL_alpha 코드

                double K = (1 + Math.Sqrt(1 + 8 * Z)) / (double)2;
                double test_K = Math.Truncate((1 + Math.Sqrt(1 + 8 * Z)) / (double)2);
                int K2 = 0;
                if (test_K == K) { K2 = (int)(K - 1); } else { K2 = (int)test_K; }


                int X = Z - K2 * (K2 - 1) / 2;

                r.Append('1', X - 1);
                r.Append('0', K2 - (X - 1));
                r.Append('1');




            }

            if ((r.Length % 8) != 0)
            {
                char last_bit = r[r.Length - 1];
                char add_bit = '1';
                if (last_bit == '1') { add_bit = '0'; }
                r.Append(add_bit, 8 - (r.Length % 8));

            }

            //이제 바이트 배열로 전환
            int bc = 0;
            int BL = r.Length;
            List<byte> result = new List<byte>();
            string rs = r.ToString();

            for (int pp = 0; pp < BL; pp += 8)
            {
                result.Add(Convert.ToByte((rs.Substring(pp, 8)), 2));


            }
            return result;

        }


        public static List<int> Decode(List<byte> encoded_byte)

        {


            List<int> myList = new List<int>();
            StringBuilder r = new StringBuilder();
            int EBL = encoded_byte.Count;


            //1바이트씩 모두 2진화 함.
            for (int i = 0; i < EBL; i++)
            {
                //r.Append(Convert.ToString(encoded_byte[i], 2));
                r.Append(Convert.ToString(encoded_byte[i], 2).PadLeft(8, '0'));
            }

            string rs = r.ToString();
            int RS_L = rs.Length;
            for (int i = 0; i < RS_L; i++)
            {


                int p1 = rs.IndexOf("01", i) + 1;
                if (p1 == 0) break;

                int L = p1 - i + 1;

                string temp_r = rs.Substring(i, L);
                int p = temp_r.IndexOf("10") + 1;   //없으면 -1 을 가르켜 버린다. VB랑 다름.


                int K = L - 1;

                int T = p;

                int Z = K * (K - 1) / 2 + T + 1;
                myList.Add(Z);

                i = p1;


            }




            return myList;




        }


    }
}
