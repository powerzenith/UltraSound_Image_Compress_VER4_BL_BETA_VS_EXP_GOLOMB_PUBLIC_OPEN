using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSC
{
    public  class Exponential_Golomb


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



                int M = (int)Math.Ceiling(Math.Log((Z + 1) / Math.Log(2)));

                r.Append('0', M - 1);
                r.Append('1', 1);

                r.Append('1');

                int suffix = (int)(Z - (Math.Pow(2, (M - 1))));


                r2 = Convert.ToString(suffix, 2).PadLeft(M - 1, '0');


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
            // S 는  exponential golomb 코드에서는 k 임 (k=0)

            StringBuilder r = new StringBuilder();
            int Z = 0;
            //int CL = code_num.Count;
            string r2 = "";
            //for (int i = 0; i < CL; i++)
            //{
                Z = code_num;
            if (Z==1 ) { return "1"; }

            int M = 0;
                double Lp = Math.Log(Z + 1) / Math.Log(2);
            if (Lp == Math.Truncate(Lp))
            {

                M = (int)Lp;
            }

            else

            {

                M = (int)(Math.Truncate(Lp) + 1);
            }

            //M = (int)Math.Floor(Math.Log((Z + 1) / Math.Log(2)));

            r.Append('0', M - 1);
                r.Append('1',1);

               
               int suffix = (int)(Z - (Math.Pow(2, (M - 1))));


                r2 = Convert.ToString(suffix, 2).PadLeft(M - 1, '0');
                

                r.Append(r2);

            
          
            string rs = r.ToString();


            return rs;

        }

        public static int Decode(string bin, int S)
        {


            string rs = bin.ToString();
            int RS_L = rs.Length - 1;
            
            int MM = 0;
            int INFO = 0;
            int ZR = 0;

            

            for (int kk = 0; kk <= RS_L; kk++)
            {
                string chk = rs.Substring(kk, 1);
                if (chk != "1" )
                {
                    //처음으로 1을 만났다면 여기에서 
                    MM++;
                    continue;

                    
                }

                if (kk==RS_L)
                {
                    INFO = 0;

                }
                else
                {

                    string suffix = rs.Substring(kk + 1, MM);
                    INFO = Convert.ToInt16(suffix, 2);




                }


                ZR = (int)(Math.Pow(2, MM) - 1 + INFO + 1);

                return ZR;
                


            }


            return -1;  //이경우 error임.



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
