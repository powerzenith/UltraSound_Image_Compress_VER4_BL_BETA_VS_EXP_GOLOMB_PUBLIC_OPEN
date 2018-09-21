using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSC
{
    public  class BL_beta


    {


        public static List<byte> Encode(List<int> code_num, int S)

        {
            StringBuilder r = new StringBuilder();
            int Z = 0;
            int CL = code_num.Count;
            string r2 = "";
            for (int i = 0; i < CL; i++)
            {
                Z = code_num[i];


                int M = (int)Math.Ceiling(Math.Log((Z + Math.Pow(2, S)) / (double)Math.Pow(2, S), 2));
                double K = 0;


                K = (1 + Math.Sqrt(1 + 8 * M)) / (double)2;
                double test_K = Math.Truncate((1 + Math.Sqrt(1 + 8 * M)) / (double)2);
                int K2 = 0;
                if (test_K == K) { K2 = (int)(K - 1); } else { K2 = (int)test_K; }

                int X = M - K2 * (K2 - 1) / 2;
                int suffix = (int)(Z - Math.Pow(2, S) * (Math.Pow(2, (M - 1)) - 1) - 1);



                r.Append('1', X - 1);
                r.Append('0', K2 - (X - 1));
                r.Append('1');

                r2 = Convert.ToString(suffix, 2).PadLeft(S + M - 1, '0');
                //int diff = (S + M - 1) - r2.Length;
                //MessageBox.Show(diff.ToString());
                //StringBuilder r3 = new StringBuilder();
                //r2 = r3.Append('0', diff).ToString() + r2;

                r.Append(r2);




            }

            //string result2 = r.ToString();



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
        public static string  Encode(int code_num, int S)

        {
            StringBuilder r = new StringBuilder();
            int Z = 0;
            //int CL = code_num.Count;
            string r2 = "";
            //for (int i = 0; i < CL; i++)
            //{
                Z = code_num;


               // int M = (int)Math.Ceiling(Math.Log((Z + Math.Pow(2, S)) / (double)Math.Pow(2, S), 2));
               int M = (int)Math.Ceiling(Math.Log((Z + 2) / (double)2, 2));
               double K = 0;


                K = (1 + Math.Sqrt(1 + 8 * M)) / (double)2;
                double test_K = Math.Truncate((1 + Math.Sqrt(1 + 8 * M)) / (double)2);
                int K2 = 0;
                if (test_K == K) { K2 = (int)(K - 1); } else { K2 = (int)test_K; }

                int X = M - K2 * (K2 - 1) / 2;
                //int suffix = (int)(Z - Math.Pow(2, S) * (Math.Pow(2, (M - 1)) - 1) - 1);
                int suffix = (int)(Z - 2 * (Math.Pow(2, (M - 1)) - 1) - 1);



            r.Append('1', X - 1);
                r.Append('0', K2 - (X - 1));
                r.Append('1');

                r2 = Convert.ToString(suffix, 2).PadLeft(S + M - 1, '0');
                //int diff = (S + M - 1) - r2.Length;
                //MessageBox.Show(diff.ToString());
                //StringBuilder r3 = new StringBuilder();
                //r2 = r3.Append('0', diff).ToString() + r2;

                r.Append(r2);




          

            //string result2 = r.ToString();



            //if ((r.Length % 8) != 0)
            //{
            //    char last_bit = r[r.Length - 1];
            //    char add_bit = '1';
            //    if (last_bit == '1') { add_bit = '0'; }
            //    r.Append(add_bit, 8 - (r.Length % 8));

            //}

          
            string rs = r.ToString();


            return rs;

        }
        public static int Decode(string bin, int S)
        {
            //아직 만드는중... 

            string rs = bin.ToString();
            int RS_L = rs.Length;
            int Z = -1;
            for (int i = 0; i < RS_L; i++)
            {


                int p = rs.IndexOf("01", i) + 1;

                if (p == 0) break;

                string r3 = rs.Substring(i, p - i + 1);  //BL-alpha코드부 추출
                int L = r3.Length;
                int K = L - 1;
                int T = r3.IndexOf("10") + 1;
                int M = K * (K - 1) / 2 + T + 1;
                int SL = S + (M - 1);


                string r4 = rs.Substring(p + 1, SL);  //suffix 부 추출
                int Bcode = Convert.ToInt32(r4, 2);
                //Z = Bcode + (int)Math.Pow(2, S) * ((int)Math.Pow(2, M - 1) - 1) + 1;
                Z = Bcode + 2 * ((int)Math.Pow(2, M - 1) - 1) + 1;



                i += L;
                i += SL;
                i--;



            }

            //Z 를 
            return Z;



        }
        public static List<int> Decode(List<byte> encoded_byte, int S)

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


                int p = rs.IndexOf("01", i) + 1;

                if (p == 0) break;

                string r3 = rs.Substring(i, p - i + 1);  //BL-alpha코드부 추출
                int L = r3.Length;
                int K = L - 1;
                int T = r3.IndexOf("10") + 1;
                int M = K * (K - 1) / 2 + T + 1;
                int SL = S + (M - 1);


                string r4 = rs.Substring(p + 1, SL);  //suffix 부 추출
                int Bcode = Convert.ToInt32(r4, 2);
                int Z = Bcode + (int)Math.Pow(2, S) * ((int)Math.Pow(2, M - 1) - 1) + 1;


                myList.Add(Z);
                i += L;
                i += SL;
                i--;



            }




            return myList;




        }

       

    }
}
