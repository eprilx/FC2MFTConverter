/*
MIT License

Copyright (c) 2021 eprilx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using BMFONT;
using Gibbed.IO;
using System;
using System.IO;
using System.Linq;
using UlitiesFunction;

namespace FC2MFTConverter
{
    class MFTFunction
    {
        public static void ConvertMFTtoFNT(string inputMFT, string output)
        {
            // load mft
            MFTStruct mft = MFTFormat.LoadMFT(inputMFT);

            // create bmfont
            BMFontStruct bmf = new();
            // convert infoMFT 2 infoBMF
            bmf.generalInfo.face = mft.generalInfo.fontName;
            bmf.generalInfo.size = mft.generalInfo.size;
            bmf.generalInfo.charsCount = mft.generalInfo.charsCount;
            bmf.generalInfo.pages = mft.generalInfo.pagesCount;

            for (int i = 0; i < bmf.generalInfo.pages; i++)
            {
                bmf.generalInfo.idImg.Add(i);
                bmf.generalInfo.fileImg.Add(mft.generalInfo.fontName + "_" + i.ToString());
            }

            // Get width/height image font from user
            (bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg) = GetWidthHeightImageFont();

            //convert charDescMFT 2 charDescBMF
            foreach (MFTStruct.charDesc charMFT in mft.charDescList)
            {
                (float x, float y, float width, float height) = Ulities.getPointFromUVmapping(charMFT.UVLeft, charMFT.UVTop, charMFT.UVRight, charMFT.UVBottom, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);
                int yoffset = Ulities.YoffsetReverseToNormal(charMFT.yoffsetRev, mft.generalInfo.size);

                BMFontStruct.charDesc charBMF = new();
                charBMF.id = charMFT.charID;
                charBMF.x = x;
                charBMF.y = y;
                charBMF.width = width;
                charBMF.height = height;
                charBMF.xoffset = charMFT.xoffset;
                charBMF.yoffset = yoffset;
                charBMF.xadvance = charMFT.xadvanceScale;
                charBMF.page = charMFT.page;
                bmf.charDescList.Add(charBMF);
            }

            BMFontFormat.CreateText(output, bmf);
        }
        private static (int, int) GetWidthHeightImageFont()
        {
            int width = 0;
            int height = 0;
            Console.WriteLine("Please input width, height of image fonts:");
            Console.Write("Width = ");
            Int32.TryParse(Console.ReadLine(), out width);
            Console.Write("Height = ");
            Int32.TryParse(Console.ReadLine(), out height);
            return (width, height);
        }

        public static void CreateMFTfromFNT(string inputMFT, string BMFpath, string outputPath)
        {
            var output = File.Create(outputPath);
            // load mft
            MFTStruct mft = MFTFormat.LoadMFT(inputMFT);

            // load bmfont
            BMFontStruct bmf = BMFontFormat.Load(BMFpath);
            bmf.SortCharDescListById();

            // convert infoBMF 2 infoMFT
            //mft.generalInfo.fontName = bmf.generalInfo.face;
            bmf.generalInfo.size = Math.Abs(bmf.generalInfo.size);
            mft.generalInfo.size = (byte)bmf.generalInfo.size;
            mft.generalInfo.charsCount = (ushort)bmf.generalInfo.charsCount;

            MFTFormat.RebuildHeader(output, mft.generalInfo, mft.unk);
            //convert charDescBMF 2 charDescMFT
            mft.charDescList = new();

            foreach (BMFontStruct.charDesc charBMF in bmf.charDescList)
            {
                (float UVLeft, float UVTop, float UVRight, float UVBottom) = Ulities.getUVmappingFromPoint(charBMF.x, charBMF.y, charBMF.width, charBMF.height, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);
                int yoffsetRev = Ulities.YoffsetNormalToReverse(charBMF.yoffset, mft.generalInfo.size);

                MFTStruct.charDesc charMFT = new();
                charMFT.charID = (ushort)charBMF.id;

                charMFT.widthScale = (short)(charBMF.width <= 1 ? 0 : (charBMF.width - 1));
                charMFT.heightScale = (short)(charBMF.height <= 1 ? 0 : (charBMF.height - 1));
                charMFT.xoffset = (short)charBMF.xoffset;
                charMFT.yoffsetRev = (short)yoffsetRev;
                charMFT.xadvanceScale = (short)(charBMF.xadvance * 1);
                charMFT.UVLeft = UVLeft;
                charMFT.UVTop = UVTop;
                charMFT.UVRight = UVRight;
                charMFT.UVBottom = UVBottom;
                charMFT.page = (ushort)charBMF.page;
                mft.charDescList.Add(charMFT);
            }
            // id = 127
            MFTStruct.charDesc charMFT127 = new();
            int index127 = mft.charDescList.FindIndex(t => t.charID == 127);
            int index32 = mft.charDescList.FindIndex(t => t.charID == 32);
            int index;
            if (index127 >= 0)
            {
                index = index127;
            }
            else
            {
                index = index32;
            }

            charMFT127.charID = 127;
            charMFT127.widthScale = mft.charDescList[index].widthScale;
            charMFT127.heightScale = mft.charDescList[index].heightScale;
            charMFT127.xoffset = mft.charDescList[index].xoffset;
            charMFT127.yoffsetRev = mft.charDescList[index].yoffsetRev;
            charMFT127.xadvanceScale = mft.charDescList[index].xadvanceScale;
            charMFT127.UVLeft = mft.charDescList[index].UVLeft;
            charMFT127.UVTop = mft.charDescList[index].UVTop;
            charMFT127.UVRight = mft.charDescList[index].UVRight;
            charMFT127.UVBottom = mft.charDescList[index].UVBottom;
            charMFT127.page = mft.charDescList[index].page;

            if(index127 >= 0)
                mft.charDescList.RemoveAt(index127);
            mft.charDescList.Insert(0, charMFT127);

            output.WriteValueU16((ushort)mft.charDescList.Count);
            MFTFormat.RebuildTableCharDesc(output, mft.charDescList);

            // get mft.idlist
            mft.generalInfo.charsCount = (ushort)mft.charDescList.Count();
            mft.idList = new ushort[mft.generalInfo.charsCount];

            for (int i = 0; i < mft.generalInfo.charsCount; i++)
            {
                mft.idList[i] = (ushort)mft.charDescList[i].charID;
            }

            MFTFormat.RebuildTableCharEntry(output, mft.idList);

            output.WriteBytes(mft.unk.unkFooter);
            output.Close();
        }
    }
}
