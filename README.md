# UltraSound_Image_Compress
# Ver 4.0
# For more detail, Manual.pptx explains that how to run our algorithm and evaluate the performance.

1.Install
the Visual Studio 2017 Community (VS 2017)

2.Open the Visual studio solution file
UltraSound_Image_Compress.sln file with VS 2017 

3.Run the Form1 by starting project
“Form1.cs”

4.You can see form1 as page 1 in manual.pptx
  #  if you want to compress "Envelop" data for a certain sample ultrasound  with LUT(Look Up Table) method, click
      "Envelope_Data_Compression_LOOKUP" buttton
  #  also, if you want to compress "Envelop" data for a cetramin sample ultrasound with Formular method(Mathmatical method),
      click "Envelope_Data_Compression" button
      
  # To compress "Prebeamformed" data , please click "Prebeamformed_RF_Compression_LOOKUP" or "Preberamformed_RF_Compression"
  # To compress "IQ" data, please click "Prebeamformed_RF_Compression_LOOKUP" or "Preberamformed_RF_Compression" button
  # To compress "Beamformed" data, please click "Beamformed_RF_Compression_LOOKUP" or "Beamformed_RF_Compression" button
      

 
5. Then you can see file selecting diaolog box to select scanline data file for each data tytpe (i.e. Envelope, Prebeamformed, IQ, Beamformed) 
   - Dialog box descripted on page3 in manual.pptx
   - You can select cyst folder and then select "Envelope" folder to compress "Envelope" type data for the cyst sample
     # in Envelope folder, YOU HAVE TO SELECT "Envelope_bin1.txt"
     # Once you select this file, other scanline(2~128) files are automatically compressed by our program.
     if you do not select Envelope_bin1.txt, the program make an exception error.
     # After compression, Compression result files that _TOTAL.txt, _Decode.txt, _Encode.txt, _STAT.txt  are made by our program
   - You can see the detail description on page4 in manual.pptx
   
 6. on page from 6 to 9 in manual.pptx, 
    We explained result file's information line by line 
    
 7. For IQ data compression
    Basically all process is the same as above explained,
    But, If you want to test I type data, then YOU MUST SELECT "I_bin1.txt" not "Q_bin1.txt in  IQ folder
    On the other hand, If you want to test  Q type data, YOU MUST DELCT "Q_bin1.txt" not "I_bin1.txt" in IQ folder
    
    
    
    
    
   
