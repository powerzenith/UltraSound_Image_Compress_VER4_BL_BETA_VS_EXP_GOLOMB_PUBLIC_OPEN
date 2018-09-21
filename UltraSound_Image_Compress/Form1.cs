using BLSC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltraSound_Image_Compress
{
    public partial class Form1 : Form
    {

        public void Env_compress_LOOKUP()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult DR = fileDialog.ShowDialog();
            FILE_NAME_TEXT.Text = fileDialog.FileName;
            string data_type = "envelope";

            string initial_fn = FILE_NAME_TEXT.Text;
            initial_fn = initial_fn.Replace("_bin1", "_bin");
            initial_fn = initial_fn.Replace(".txt", "");
            string fn = "";
            string c_fn = "";
            string d_fn = "";
            string r_fn = "";
            string tr_fn = "";
            int total_ori_size = 0;
            tr_fn = initial_fn + "_" + data_type + "_TOTAL.txt";
            StreamWriter sw_tr = File.CreateText(tr_fn);
            Stopwatch timer = new Stopwatch();

            long sum_tick_BL_encode = 0;
            long sum_tick_BL_decode = 0;
            long sum_tick_EG_encode = 0;
            long sum_tick_EG_decode = 0;

            BL_beta_LUT.setup_code();
            Exponential_Golomb_LUT.setup_code();


            for (int i = 1; i <= 128; i++)
            {
                fn = initial_fn + (i.ToString() + ".txt");
                c_fn = initial_fn + (i.ToString() + "_" + data_type + "_Encode.txt");
                d_fn = initial_fn + (i.ToString() + "_" + data_type + "_Decode.txt");
                r_fn = initial_fn + (i.ToString() + "_" + data_type + "_STAT.txt");

                StreamWriter sw = File.CreateText(c_fn);
                StreamWriter sw_r = File.CreateText(r_fn);
                StreamWriter sw_d = File.CreateText(d_fn);




                string[] lines = System.IO.File.ReadAllLines(fn);

                int cc = 0;
                double ratio_sum = 0;
                double ratio_sum2 = 0;
                //const int L= 16;
                sw_r.WriteLine("HEAD TITLE DESCRPITON BELOW");
                sw_r.WriteLine("original_size ==> compressed_size  / compression ratio  %  ********+ Exponential Golomb size /  exponential golomb compression ration %  ******* exact decoding check flag / original code-num ");
                sw_r.WriteLine("===============================================================================================================================================================================================");


                foreach (string line_F in lines)
                {

                    int ori_size = 0;
                    int comp_size = 0;
                    int EG_comp_size = 0;

                    string line = line_F.Trim();
                    ori_size += line.Length;   // 원본길이 저장
                    total_ori_size += line.Length;

                    int L = line.Length;




                    //LOWER_B에 대해서 십진수로 변환한뒤, BL-beta로 압축
                    int code_num = Convert.ToInt16(line, 2);


                    timer.Reset();
                    timer.Start();
                    string comp_result = BL_beta_LUT.Encode_LOOKUP(code_num);
                    timer.Stop();
                    long ET_BL_ENCODE = timer.ElapsedTicks;


                    timer.Reset();
                    timer.Start();
                    //string EG_comp_result = Exponential_Golomb.Encode(code_num, 0); //Exponential Golomb code로 코딩(k=0)
                    string EG_comp_result = Exponential_Golomb_LUT.Encode_LookUp(code_num); //Exponential Golomb code로 코딩(k=0)
                    timer.Stop();
                    long ET_EG_ENCODE = timer.ElapsedTicks;


                    comp_size += comp_result.Length;
                    EG_comp_size = +EG_comp_result.Length;


                    sw.WriteLine(comp_result);

                    //다시 결과를 decode 해보자
                    timer.Reset();
                    timer.Start();
                    //int code_num_decomp = BL_beta.Decode(comp_result, 1) - 1;  //실제 code-num은  디코딩값에서 1을 뺀값임.
                    int code_num_decomp = BL_beta_LUT.Decode_LOOKUP(comp_result) -1 ;


                    timer.Stop();
                    long ET_BL_DECODE = timer.ElapsedTicks;

                    timer.Reset();
                    timer.Start();
                    //int EG_code_num_decomp = Exponential_Golomb.Decode(EG_comp_result, 0) - 1;
                    int EG_code_num_decomp = Exponential_Golomb_LUT.Decode_LookUp(EG_comp_result)-1;



                    timer.Stop();
                    long ET_EG_DECODE = timer.ElapsedTicks;

                    sum_tick_BL_encode += ET_BL_ENCODE;
                    sum_tick_BL_decode += ET_BL_DECODE;
                    sum_tick_EG_encode += ET_EG_ENCODE;
                    sum_tick_EG_decode += ET_EG_DECODE;


                    string decomp_result = Convert.ToString(code_num_decomp, 2).PadLeft(16, '0');
                    string EG_decomp_result = Convert.ToString(EG_code_num_decomp, 2).PadLeft(16, '0');

                    // sw_r.WriteLine("TICK : " + "BL_ENCODE==>" + ET_BL_ENCODE + "/" + "EG_ENCODE==> " + ET_EG_ENCODE + " ****** " + "BL_DECODE==> " + ET_BL_DECODE + "/" + "EG_DECODE==> " + ET_EG_DECODE);

                    string chk = "";
                    if (decomp_result == line && EG_decomp_result == line)
                    {
                        sw_d.WriteLine("original ==>" + line + "/ compressed_BL ==>" + comp_result + "/ decompressed_BL ==>" + decomp_result + "/ MATCH" );
                        sw_d.WriteLine("original ==>" + line + "/ compressed_EG ==>" + EG_comp_result + "/ decompressed_EG ==>" + EG_decomp_result + "/ MATCH");
                        chk = "OK";
                    }

                    else

                    {

                        chk = "NOT_OK";
                        MessageBox.Show("There is some problem with exact decoding");

                    }

                    double ratio = 100 - ((double)comp_size / (double)ori_size * 100.0);
                    double ratio2 = 100 - ((double)EG_comp_size / (double)ori_size * 100.0);

                    sw_r.WriteLine(ori_size + "==>" + comp_size + " / " + ratio.ToString() + "%" + "********" + EG_comp_size + " / " + ratio2.ToString() + "%" + "*******" + chk + "/" + Convert.ToString(code_num, 2).Length);

                    ratio_sum += ratio;
                    ratio_sum2 += ratio2;
                    cc++;



                }



                sw.Close();
                sw_r.Close();
                sw_d.Close();



                sw_tr.WriteLine(fn + "/" + Math.Round((ratio_sum / cc), 2).ToString() + "/" + Math.Round((ratio_sum2 / cc), 2).ToString());
                sw_tr.WriteLine("TICK ENCODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_encode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_encode / cc), 2).ToString());
                sw_tr.WriteLine("TICK DECODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_decode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_decode / cc), 2).ToString());

                sum_tick_BL_encode = 0;
                sum_tick_BL_decode = 0;
                sum_tick_EG_encode = 0;
                sum_tick_EG_decode = 0;
                cc = 0;

            }
            sw_tr.Close();

            MessageBox.Show("Finished");




        }


        public void Env_compress()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult DR = fileDialog.ShowDialog();
            FILE_NAME_TEXT.Text = fileDialog.FileName;
            string data_type = "envelope";

            string initial_fn = FILE_NAME_TEXT.Text;
            initial_fn = initial_fn.Replace("_bin1", "_bin");
            initial_fn = initial_fn.Replace(".txt", "");
            string fn = "";
            string c_fn = "";
            string d_fn = "";
            string r_fn = "";
            string tr_fn = "";
            int total_ori_size = 0;
            tr_fn = initial_fn + "_" + data_type + "_TOTAL.txt";
            StreamWriter sw_tr = File.CreateText(tr_fn);
            Stopwatch timer = new Stopwatch();

            long sum_tick_BL_encode = 0;
            long sum_tick_BL_decode = 0;
            long sum_tick_EG_encode = 0;
            long sum_tick_EG_decode = 0;


            for (int i = 1; i <= 128; i++)
            {
                fn = initial_fn + (i.ToString() + ".txt");
                c_fn = initial_fn + (i.ToString() + "_" + data_type + "_Encode.txt");
                d_fn = initial_fn + (i.ToString() + "_" + data_type + "_Decode.txt");
                r_fn = initial_fn + (i.ToString() + "_" + data_type + "_STAT.txt");

                StreamWriter sw = File.CreateText(c_fn);
                StreamWriter sw_r = File.CreateText(r_fn);
                StreamWriter sw_d = File.CreateText(d_fn);




                string[] lines = System.IO.File.ReadAllLines(fn);
                
                int cc = 0;
                double ratio_sum = 0;
                double ratio_sum2 = 0;
                //const int L= 16;
                sw_r.WriteLine("HEAD TITLE DESCRPITON BELOW");
                sw_r.WriteLine("original_size ==> compressed_size  / compression ratio  %  ********+ Exponential Golomb size /  exponential golomb compression ration %  ******* exact decoding check flag / original code-num ");
                sw_r.WriteLine("===============================================================================================================================================================================================");
             

                foreach (string line_F in lines)
                {

                    int ori_size = 0;
                    int comp_size = 0;
                    int EG_comp_size = 0;

                    string line = line_F.Trim();
                    ori_size += line.Length;   // 원본길이 저장
                    total_ori_size += line.Length;

                    int L = line.Length;




                    //LOWER_B에 대해서 십진수로 변환한뒤, BL-beta로 압축
                    int code_num = Convert.ToInt16(line, 2) + 1;


                    timer.Reset();
                    timer.Start();
                    string comp_result = BL_beta.Encode(code_num, 1);
                    timer.Stop();
                     long ET_BL_ENCODE = timer.ElapsedTicks;


                    timer.Reset();
                    timer.Start();
                    string EG_comp_result = Exponential_Golomb.Encode(code_num, 0); //Exponential Golomb code로 코딩(k=0)
                    timer.Stop();
                    long ET_EG_ENCODE = timer.ElapsedTicks;


                    comp_size += comp_result.Length;
                    EG_comp_size = +EG_comp_result.Length;


                    sw.WriteLine(comp_result);

                    //다시 결과를 decode 해보자
                    timer.Reset();
                    timer.Start();
                    int code_num_decomp = BL_beta.Decode(comp_result, 1) - 1;  //실제 code-num은  디코딩값에서 1을 뺀값임.
                    timer.Stop();
                    long ET_BL_DECODE = timer.ElapsedTicks;

                    timer.Reset();
                    timer.Start();
                    int EG_code_num_decomp = Exponential_Golomb.Decode(EG_comp_result, 0) - 1;
                    timer.Stop();
                    long ET_EG_DECODE = timer.ElapsedTicks;

                    sum_tick_BL_encode += ET_BL_ENCODE;
                    sum_tick_BL_decode += ET_BL_DECODE;
                    sum_tick_EG_encode += ET_EG_ENCODE;
                    sum_tick_EG_decode += ET_EG_DECODE;


                    string decomp_result = Convert.ToString(code_num_decomp, 2).PadLeft(16, '0');
                    string EG_decomp_result = Convert.ToString(EG_code_num_decomp, 2).PadLeft(16, '0');

                   // sw_r.WriteLine("TICK : " + "BL_ENCODE==>" + ET_BL_ENCODE + "/" + "EG_ENCODE==> " + ET_EG_ENCODE + " ****** " + "BL_DECODE==> " + ET_BL_DECODE + "/" + "EG_DECODE==> " + ET_EG_DECODE);

                    string chk = "";
                    if (decomp_result == line && EG_decomp_result == line)
                    {

                        sw_d.WriteLine("original ==>" + line + "/ compressed_BL ==>" + comp_result + "/ decompressed_BL ==>" + decomp_result + "/ MATCH");
                        sw_d.WriteLine("original ==>" + line + "/ compressed_EG ==>" + EG_comp_result + "/ decompressed_EG ==>" + EG_decomp_result + "/ MATCH");

                        chk = "OK";
                    }

                    else

                    {

                        chk = "NOT_OK";
                        MessageBox.Show("There is some problem with exact decoding");

                    }

                    double ratio = 100 - ((double)comp_size / (double)ori_size * 100.0);
                    double ratio2 = 100 - ((double)EG_comp_size / (double)ori_size * 100.0);

                    sw_r.WriteLine(ori_size + "==>" + comp_size + " / " + ratio.ToString() + "%" + "********" + EG_comp_size + " / " + ratio2.ToString() + "%"  + "*******" + chk + "/" + Convert.ToString(code_num, 2).Length);

                    ratio_sum += ratio;
                    ratio_sum2 += ratio2;
                    cc++;



                }



                sw.Close();
                sw_r.Close();



                sw_tr.WriteLine(fn + "/" + Math.Round((ratio_sum / cc), 2).ToString() + "/" + Math.Round((ratio_sum2 / cc), 2).ToString());
                sw_tr.WriteLine("TICK ENCODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_encode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_encode / cc), 2).ToString());
                sw_tr.WriteLine("TICK DECODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_decode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_decode / cc), 2).ToString());

                sum_tick_BL_encode = 0;
                sum_tick_BL_decode = 0;
                sum_tick_EG_encode = 0;
                sum_tick_EG_decode = 0;
                cc = 0;

            }
            sw_tr.Close();

            MessageBox.Show("Finished");




        }

        public  void RF_compress(string data_type)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult DR = fileDialog.ShowDialog();
            FILE_NAME_TEXT.Text = fileDialog.FileName;

            string initial_fn = FILE_NAME_TEXT.Text;
            ;
            if (data_type.IndexOf("prebeamformed") < 0)
            {
                initial_fn = initial_fn.Replace("_bin1", "_bin");
            }

            else

            {



            }
            initial_fn = initial_fn.Replace(".txt", "");
            string fn = "";
            string c_fn = "";
            string d_fn = "";
            string r_fn = "";
            string tr_fn = "";
            int total_ori_size = 0;
            tr_fn = initial_fn + "_" + data_type + "_TOTAL.txt";
            StreamWriter sw_tr = File.CreateText(tr_fn);
            Stopwatch timer = new Stopwatch();

            long sum_tick_BL_encode = 0;
            long sum_tick_BL_decode = 0;
            long sum_tick_EG_encode = 0;
            long sum_tick_EG_decode = 0;


            for (int i = 1; i <= 128; i++)
            {

                

                if (data_type.IndexOf("prebeamformed") < 0)
                {
                    fn = initial_fn + (i.ToString() + ".txt");
                    c_fn = initial_fn + (i.ToString() + "_" + data_type + "_Encode.txt");
                    d_fn = initial_fn + (i.ToString() + "_" + data_type + "_Decode.txt");
                    r_fn = initial_fn + (i.ToString() + "_" + data_type + "_STAT.txt");

                }

                else

                {
                    ;
                    string[] si_fn= initial_fn.Split('_');

                    ;
                    fn = si_fn[0]+"_"+si_fn[1]+"_"+si_fn[2]+"_"+si_fn[3]+ "_" + i.ToString() +   ".txt";


                    c_fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + (i.ToString() + "_" + data_type + "_Encode.txt");
                    d_fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + (i.ToString() + "_" + data_type + "_Decode.txt");
                    r_fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + (i.ToString() + "_" + data_type + "_STAT.txt");


                }



                StreamWriter sw = File.CreateText(c_fn);
                StreamWriter sw_r = File.CreateText(r_fn);
                StreamWriter sw_d = File.CreateText(d_fn);



                string[] lines = System.IO.File.ReadAllLines(fn);
               
                string HB = "";
                string UPPER_B = "";
                string LOWER_B = "";
                int cc = 0;
                double ratio_sum = 0;
                double ratio_sum2 = 0;
                //const int L= 16;

                sw_r.WriteLine("HEAD TITLE DESCRPITON BELOW");
                sw_r.WriteLine("original_size ==> compressed_size  / compression ratio  %  ********+ Exponential Golomb size /  exponential golomb compression ration %  ******* exact decoding check flag / origianl code-num ");
                sw_r.WriteLine("===============================================================================================================================================================================================");

                long ET_BL_ENCODE = 0;
                long ET_EG_ENCODE = 0;
                long ET_BL_DECODE = 0;
                long ET_EG_DECODE = 0;



                foreach (string line_F in lines)
                {

                    int ori_size = 0;
                    int comp_size = 0;
                    int EG_comp_size = 0;

                    string line = line_F.Trim();
                    ori_size += line.Length;   // 원본길이 저장
                    total_ori_size += line.Length;

                    int L = line.Length;



                    //header = line.Substring(0, 8);
                    //tail = line.Substring(8, 8);
                    int code_num = -1;
                    string comp_result = "";
                    string EG_comp_result = "";
                    if (line.Substring(0, 2) == "01")
                    {
                        HB = "0";

                        //최초로 10 이 나오는 위치 확인
                        int pp = line.IndexOf("10", 1);

                        if (pp >= 0)

                        {
                            //0111111010100101
                            //011111 1010100101    pp= 6, L=16, 
                            UPPER_B = line.Substring(0, pp - 1); //011111  //UPPER_B는 검증용이지 실제로 압축데이터에 고려되는것음 아님.
             

                            LOWER_B = line.Substring(pp, L - pp); //L-pp = 10;      1010100101
             

                        }

                        else

                        {
                            UPPER_B = line.Substring(0, L);
                            LOWER_B = "0";



                        }

                        //HB를 보고 압축해제 시나리오를 확인하고 나서,이어서 해당 비트를 읽어서,
                        //LOWER_B에 대해서 십진수로 변환한뒤, BL-beta로 압축
                        code_num = Convert.ToInt16(LOWER_B, 2) + 1;
                        timer.Reset();
                        timer.Start();
                        comp_result = HB + BL_beta.Encode(code_num, 1);
                        timer.Stop();
                        ET_BL_ENCODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
                        EG_comp_result = HB + Exponential_Golomb.Encode(code_num, 0);
                        timer.Stop();
                        ET_EG_ENCODE = timer.ElapsedTicks;


                    }
                    else if (line.Substring(0, 2) == "10")
                    {
                        HB = "10";
                        // '10000000000011110011...
                        //최초로 01 이 나오는 위치 확인
                        int pp = line.IndexOf("01", 1);

                        if (pp >= 0)

                        {
                            
                            UPPER_B = line.Substring(0, pp - 1); //100000000000
                            LOWER_B = line.Substring(pp, L - pp); //L-pp = 10;  11110011
                           
                  
                            


                        }

                        else

                        {
                            UPPER_B = line.Substring(0, L);
                            LOWER_B = "0";



                        }


                        code_num = Convert.ToInt16(LOWER_B, 2) + 1;

                        timer.Reset();
                        timer.Start();
                        comp_result = HB + BL_beta.Encode(code_num, 1);
                        timer.Stop();
                         ET_BL_ENCODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
                        EG_comp_result = HB + Exponential_Golomb.Encode(code_num, 0);
                        timer.Stop();
                         ET_EG_ENCODE = timer.ElapsedTicks;


                    }
                    else
                    {
                        //10 이나 01로 시작안하는 00 이나 11로 시작하는데
                        //그외의 소수의 경우 그대로 BL-beta로 간다. 이때는 HB로 인하여 2비트 손해인데
                        HB = "11";
                        LOWER_B = line.Substring(1, L-1); //이때 어차피 HB라 압축데이터에 붙으므로, 1100 , 1111 ==> 110 / 111 로  1비트 정도로 손해를 줄일수있음

                        comp_result = HB + LOWER_B;
                        EG_comp_result = HB + LOWER_B;





                    }

               
                    

                    comp_size += comp_result.Length;
                    EG_comp_size += EG_comp_result.Length;

                    sw.WriteLine(comp_result + "/" + EG_comp_result);

                    string LOWER_B_decomp = "";
                    string EG_LOWER_B_decomp = "";

                    string decomp_string = "";
                    string EG_decomp_string = "";

                    int code_num_decomp = -1;
                    int EG_code_num_decomp = -1;

                    //다시 결과를 decode 해보자
                    //comp_result의 header를 분석
                    if (comp_result.Substring(0,1)=="0")
                    {

                        LOWER_B_decomp = comp_result.Substring(1, comp_result.Length - 1); //처음 1비트를 제외하고 나머지를 읽는다.
                        EG_LOWER_B_decomp = EG_comp_result.Substring(1, EG_comp_result.Length - 1); //처음 1비트를 제외하고 나머지를 읽는다.

                        timer.Reset();
                        timer.Start();
                        code_num_decomp = BL_beta.Decode(LOWER_B_decomp, 1) - 1;  //실제 code-num은  디코딩값에서 1을 뺀값임.
                        timer.Stop();
                        ET_BL_DECODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
                        EG_code_num_decomp = Exponential_Golomb.Decode(EG_LOWER_B_decomp, 0) - 1;
                        timer.Stop();
                        ET_EG_DECODE = timer.ElapsedTicks;


                        if (code_num_decomp  != EG_code_num_decomp ) { MessageBox.Show("There is some problem with exact decoding"); }

                        if (code_num_decomp == 0 )

                        {
                            decomp_string = "0";

                            for (int kk = 0; kk < L - 1; kk++)
                            {
                                decomp_string += "1";
                            }

                        }

                        else

                        {
                            string filler = "";
                            string code_num_string = Convert.ToString(code_num_decomp, 2);
                            int filler_size = L - 1 - code_num_string.Length;
                            for (int j=0; j<filler_size;j++)
                            {
                                filler += "1";
                            }
                            decomp_string = "0" + filler + code_num_string;

                            //decomp_string = Convert.ToString(code_num_decomp, 2).PadLeft(L, '0');



                        }
                        



                    } else if (comp_result.Substring(0,2)=="10")

                        {

                            LOWER_B_decomp = comp_result.Substring(2, comp_result.Length - 2);
                         EG_LOWER_B_decomp = EG_comp_result.Substring(2, EG_comp_result.Length - 2);

                        timer.Reset();
                        timer.Start();
                        code_num_decomp = BL_beta.Decode(LOWER_B_decomp, 1) - 1;  //실제 code-num은  디코딩값에서 1을 뺀값임.
                        timer.Stop();
                        ET_BL_DECODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
                        EG_code_num_decomp = Exponential_Golomb.Decode(EG_LOWER_B_decomp, 0) - 1;
                        timer.Stop();
                        ET_EG_DECODE = timer.ElapsedTicks;


                        if (code_num_decomp != EG_code_num_decomp) { MessageBox.Show("There is some problem with exact decoding"); }

                        if (code_num_decomp == 0)

                        {
                            decomp_string = "1";

                            for (int kk=0; kk<L-1;kk++)
                            {
                                decomp_string += "0";
                            }


                        }

                        else

                        {
                            string filler = "";
                            string code_num_string = Convert.ToString(code_num_decomp, 2);
                            int filler_size = L - 1 - code_num_string.Length;
                            for (int j = 0; j < filler_size; j++)
                            {
                                filler += "0";
                            }
                            decomp_string = "1" + filler + code_num_string;

                            //decomp_string = Convert.ToString(code_num_decomp, 2).PadLeft(L, '0');



                        }


                         

                    }
                    else if (comp_result.Substring(0,2)=="11")
                    {

                               if (comp_result.Substring(0, 3) == "110")
                                {
                                    decomp_string = "0" + comp_result.Substring(2, comp_result.Length - 2);

                                }
                               else
                                {
                                    decomp_string = "1" + comp_result.Substring(2, comp_result.Length - 2);

                                }

                                



                    }
                



                    double ratio = 100 - ((double)comp_size / (double)ori_size * 100.0);
                    double ratio2 = 100 - ((double)EG_comp_size / (double)ori_size * 100.0);
                    string chk = "";
                    if (line == decomp_string ) { chk = "OK";  } else
                    { chk = "NOT_OK";
                        MessageBox.Show("There is some problem with exact decoding");

                    }

                    sw_r.WriteLine(ori_size + "==>" + comp_size + " / " + ratio.ToString() + "%" + "  " + chk + "/" + Convert.ToString(code_num, 2).Length);

                    sw_d.WriteLine("original ==>" + line + "/ compressed_BL ==>" + comp_result + "/ decompressed_BL ==>" + decomp_string + "/ MATCH");
                    sw_d.WriteLine("original ==>" + line + "/ compressed_EG ==>" + EG_comp_result + "/ decompressed_EG ==>" + decomp_string + "/MATCH");

                    sum_tick_BL_encode += ET_BL_ENCODE;
                    sum_tick_BL_decode += ET_BL_DECODE;
                    sum_tick_EG_encode += ET_EG_ENCODE;
                    sum_tick_EG_decode += ET_EG_DECODE;



                    ratio_sum += ratio;
                    ratio_sum2 += ratio2;
                    cc++;



                }



                sw.Close();
                sw_r.Close();
                sw_d.Close();



                sw_tr.WriteLine(fn + "/" + Math.Round((ratio_sum / cc), 2).ToString() + "/" + Math.Round((ratio_sum2 / cc), 2).ToString());
                sw_tr.WriteLine("TICK ENCODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_encode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_encode / cc), 2).ToString());
                sw_tr.WriteLine("TICK DECODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_decode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_decode / cc), 2).ToString());

                sum_tick_BL_encode = 0;
                sum_tick_BL_decode = 0;
                sum_tick_EG_encode = 0;
                sum_tick_EG_decode = 0;
                cc = 0;

            }
            sw_tr.Close();

            MessageBox.Show("Finished");




        }

        public void RF_compress_LOOKUP(string data_type)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult DR = fileDialog.ShowDialog();
            FILE_NAME_TEXT.Text = fileDialog.FileName;

            string initial_fn = FILE_NAME_TEXT.Text;
            ;
            if (data_type.IndexOf("prebeamformed") < 0)
            {
                initial_fn = initial_fn.Replace("_bin1", "_bin");
            }

            else

            {



            }
            initial_fn = initial_fn.Replace(".txt", "");
            string fn = "";
            string c_fn = "";
            string d_fn = "";
            string r_fn = "";
            string tr_fn = "";
            int total_ori_size = 0;
            tr_fn = initial_fn + "_" + data_type + "_TOTAL.txt";
            StreamWriter sw_tr = File.CreateText(tr_fn);
            Stopwatch timer = new Stopwatch();

            long sum_tick_BL_encode = 0;
            long sum_tick_BL_decode = 0;
            long sum_tick_EG_encode = 0;
            long sum_tick_EG_decode = 0;

            Exponential_Golomb_LUT.setup_code();
            BL_beta_LUT.setup_code();


            for (int i = 1; i <= 128; i++)
            {



                if (data_type.IndexOf("prebeamformed") < 0)
                {
                    fn = initial_fn + (i.ToString() + ".txt");
                    c_fn = initial_fn + (i.ToString() + "_" + data_type + "_Encode.txt");
                    d_fn = initial_fn + (i.ToString() + "_" + data_type + "_Decode.txt");
                    r_fn = initial_fn + (i.ToString() + "_" + data_type + "_STAT.txt");

                }

                else

                {
                    ;
                    string[] si_fn = initial_fn.Split('_');

                    ;
                    fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + i.ToString() + ".txt";


                    c_fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + (i.ToString() + "_" + data_type + "_Encode.txt");
                    d_fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + (i.ToString() + "_" + data_type + "_Decode.txt");
                    r_fn = si_fn[0] + "_" + si_fn[1] + "_" + si_fn[2] + "_" + si_fn[3] + "_" + (i.ToString() + "_" + data_type + "_STAT.txt");


                }



                StreamWriter sw = File.CreateText(c_fn);
                StreamWriter sw_r = File.CreateText(r_fn);
                StreamWriter sw_d = File.CreateText(d_fn);


                string[] lines = System.IO.File.ReadAllLines(fn);

                string HB = "";
                string UPPER_B = "";
                string LOWER_B = "";
                int cc = 0;
                double ratio_sum = 0;
                double ratio_sum2 = 0;
                //const int L= 16;

                sw_r.WriteLine("HEAD TITLE DESCRPITON BELOW");
                sw_r.WriteLine("original_size ==> compressed_size  / compression ratio  %  ********+ Exponential Golomb size /  exponential golomb compression ration %  ******* exact decoding check flag / origianl code-num ");
                sw_r.WriteLine("===============================================================================================================================================================================================");

                long ET_BL_ENCODE = 0;
                long ET_EG_ENCODE = 0;
                long ET_BL_DECODE = 0;
                long ET_EG_DECODE = 0;



                foreach (string line_F in lines)
                {

                    int ori_size = 0;
                    int comp_size = 0;
                    int EG_comp_size = 0;

                    string line = line_F.Trim();
                    ori_size += line.Length;   // 원본길이 저장
                    total_ori_size += line.Length;

                    int L = line.Length;

                    
                    int code_num = -1;
                    string comp_result = "";
                    string EG_comp_result = "";
                    if (line.Substring(0, 2) == "01")
                    {
                        HB = "0";

                        //최초로 10 이 나오는 위치 확인
                        int pp = line.IndexOf("10", 1);

                        if (pp >= 0)

                        {
                            //0111111010100101
                            //011111 1010100101    pp= 6, L=16, 
                            UPPER_B = line.Substring(0, pp - 1); //011111  //UPPER_B는 검증용이지 실제로 압축데이터에 고려되는것음 아님.


                            LOWER_B = line.Substring(pp, L - pp); 

                        }

                        else

                        {
                            UPPER_B = line.Substring(0, L);
                            LOWER_B = "0";



                        }

                        //HB를 보고 압축해제 시나리오를 확인하고 나서,이어서 해당 비트를 읽어서,
                        //LOWER_B에 대해서 십진수로 변환한뒤, BL-beta로 압축
                        code_num = Convert.ToInt16(LOWER_B, 2) ;
                        timer.Reset();
                        timer.Start();
           
                        comp_result = HB + BL_beta_LUT.Encode_LOOKUP(code_num);




                        timer.Stop();
                        ET_BL_ENCODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
             
                        EG_comp_result = HB + Exponential_Golomb_LUT.Encode_LookUp(code_num);

                        timer.Stop();
                        ET_EG_ENCODE = timer.ElapsedTicks;


                    }
                    else if (line.Substring(0, 2) == "10")
                    {
                        HB = "10";
                        // '10000000000011110011...
                        //최초로 01 이 나오는 위치 확인
                        int pp = line.IndexOf("01", 1);

                        if (pp >= 0)

                        {

                            UPPER_B = line.Substring(0, pp - 1); //100000000000
                            LOWER_B = line.Substring(pp, L - pp); //L-pp = 10;  11110011





                        }

                        else

                        {
                            UPPER_B = line.Substring(0, L);
                            LOWER_B = "0";



                        }


                        code_num = Convert.ToInt16(LOWER_B, 2);

                        timer.Reset();
                        timer.Start();
                 
                        comp_result = HB + BL_beta_LUT.Encode_LOOKUP(code_num);

                        timer.Stop();
                        ET_BL_ENCODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
           
                        EG_comp_result = HB + Exponential_Golomb_LUT.Encode_LookUp(code_num);


                        timer.Stop();
                        ET_EG_ENCODE = timer.ElapsedTicks;


                    }
                    else
                    {
                        //10 이나 01로 시작안하는 00 이나 11로 시작하는데
                        //그외의 소수의 경우 그대로 BL-beta로 간다. 이때는 HB로 인하여 2비트 손해인데
                        HB = "11";
                        LOWER_B = line.Substring(1, L - 1); //이때 어차피 HB라 압축데이터에 붙으므로, 1100 , 1111 ==> 110 / 111 로  1비트 정도로 손해를 줄일수있음

                        comp_result = HB + LOWER_B;
                        EG_comp_result = HB + LOWER_B;





                    }




                    comp_size += comp_result.Length;
                    EG_comp_size += EG_comp_result.Length;

                    sw.WriteLine(comp_result + "/" + EG_comp_result);

                    string LOWER_B_decomp = "";
                    string EG_LOWER_B_decomp = "";

                    string decomp_string = "";
                    string EG_decomp_string = "";

                    int code_num_decomp = -1;
                    int EG_code_num_decomp = -1;

                    
                    //comp_result  header analysis
                    if (comp_result.Substring(0, 1) == "0")
                    {

                        LOWER_B_decomp = comp_result.Substring(1, comp_result.Length - 1); //except first bit, and read another bits
                        EG_LOWER_B_decomp = EG_comp_result.Substring(1, EG_comp_result.Length - 1); //except first bit, and read another bits

                        timer.Reset();
                        timer.Start();
         
                        code_num_decomp = BL_beta_LUT.Decode_LOOKUP(LOWER_B_decomp)-1;

                        timer.Stop();
                        ET_BL_DECODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
             
                        EG_code_num_decomp = Exponential_Golomb_LUT.Decode_LookUp(EG_LOWER_B_decomp)-1;

                        timer.Stop();
                        ET_EG_DECODE = timer.ElapsedTicks;


                        if (code_num_decomp != EG_code_num_decomp) { MessageBox.Show("There is some problem with exact decoding"); }

                        if (code_num_decomp == 0)

                        {
                            decomp_string = "0";

                            for (int kk = 0; kk < L - 1; kk++)
                            {
                                decomp_string += "1";
                            }

                        }

                        else

                        {
                            string filler = "";
                            string code_num_string = Convert.ToString(code_num_decomp, 2);
                            int filler_size = L - 1 - code_num_string.Length;
                            for (int j = 0; j < filler_size; j++)
                            {
                                filler += "1";
                            }
                            decomp_string = "0" + filler + code_num_string;

                      



                        }




                    }
                    else if (comp_result.Substring(0, 2) == "10")

                    {

                        LOWER_B_decomp = comp_result.Substring(2, comp_result.Length - 2);
                        EG_LOWER_B_decomp = EG_comp_result.Substring(2, EG_comp_result.Length - 2);

                        timer.Reset();
                        timer.Start();
      
                        code_num_decomp = BL_beta_LUT.Decode_LOOKUP(LOWER_B_decomp)-1;

                        timer.Stop();
                        ET_BL_DECODE = timer.ElapsedTicks;

                        timer.Reset();
                        timer.Start();
       
                        EG_code_num_decomp = Exponential_Golomb_LUT.Decode_LookUp(EG_LOWER_B_decomp)-1;

                        timer.Stop();
                        ET_EG_DECODE = timer.ElapsedTicks;


                        if (code_num_decomp != EG_code_num_decomp) { MessageBox.Show("There is some problem with exact decoding"); }

                        if (code_num_decomp == 0)

                        {
                            decomp_string = "1";

                            for (int kk = 0; kk < L - 1; kk++)
                            {
                                decomp_string += "0";
                            }


                        }

                        else

                        {
                            string filler = "";
                            string code_num_string = Convert.ToString(code_num_decomp, 2);

                            int filler_size = L - 1 - code_num_string.Length;
                            for (int j = 0; j < filler_size; j++)
                            {
                                filler += "0";
                            }
                            decomp_string = "1" + filler + code_num_string;

                



                        }




                    }
                    else if (comp_result.Substring(0, 2) == "11")
                    {

                        if (comp_result.Substring(0, 3) == "110")
                        {
                            decomp_string = "0" + comp_result.Substring(2, comp_result.Length - 2);

                        }
                        else
                        {
                            decomp_string = "1" + comp_result.Substring(2, comp_result.Length - 2);

                        }





                    }




                    double ratio = 100 - ((double)comp_size / (double)ori_size * 100.0);
                    double ratio2 = 100 - ((double)EG_comp_size / (double)ori_size * 100.0);
                    string chk = "";
                    if (line == decomp_string) { chk = "OK"; }
                    else
                    {
                        chk = "NOT_OK";
                        MessageBox.Show("There is some problem with exact decoding");

                    }

                    sw_r.WriteLine(ori_size + "==>" + comp_size + " / " + ratio.ToString() + "%" + "  " + chk + "/" + Convert.ToString(code_num, 2).Length);

                    sw_d.WriteLine("original ==>" + line + "/ compressed_BL ==>" + comp_result + "/ decompressed_BL ==>" + decomp_string + "/ MATCH");
                    sw_d.WriteLine("original ==>" + line + "/ compressed_EG ==>" + EG_comp_result + "/ decompressed_EG ==>" + decomp_string + "/MATCH");
                    sum_tick_BL_encode += ET_BL_ENCODE;
                    sum_tick_BL_decode += ET_BL_DECODE;
                    sum_tick_EG_encode += ET_EG_ENCODE;
                    sum_tick_EG_decode += ET_EG_DECODE;



                    ratio_sum += ratio;
                    ratio_sum2 += ratio2;
                    cc++;



                }



                sw.Close();
                sw_r.Close();
                sw_d.Close();


                sw_tr.WriteLine(fn + "/" + Math.Round((ratio_sum / cc), 2).ToString() + "/" + Math.Round((ratio_sum2 / cc), 2).ToString());
                sw_tr.WriteLine("TICK ENCODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_encode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_encode / cc), 2).ToString());
                sw_tr.WriteLine("TICK DECODE: " + fn + "/" + Math.Round(((double)sum_tick_BL_decode / cc), 2).ToString() + "/" + Math.Round(((double)sum_tick_EG_decode / cc), 2).ToString());

                sum_tick_BL_encode = 0;
                sum_tick_BL_decode = 0;
                sum_tick_EG_encode = 0;
                sum_tick_EG_decode = 0;
                cc = 0;

            }
            sw_tr.Close();

            MessageBox.Show("Finished");




        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            RF_compress("beamformed");


        }

        private void button3_Click(object sender, EventArgs e)
        {
            RF_compress("prebeamformed");


        }

        private void button4_Click(object sender, EventArgs e)
        {
            RF_compress("IQ");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Env_compress();
        }

        private void button5_Click(object sender, EventArgs e)
        {

         
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
           
        }

        private void button7_Click(object sender, EventArgs e)
        {

         
        }

        private void button8_Click(object sender, EventArgs e)
        {
   

        }

        private void button9_Click(object sender, EventArgs e)
        {

           

        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            Env_compress_LOOKUP();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            RF_compress_LOOKUP("beamformed");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            RF_compress_LOOKUP("prebeamformed");
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            RF_compress_LOOKUP("IQ"); 

        }
    }
}
