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

using Gibbed.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace FC2MFTConverter
{
    public class MFTFormat : MFTStruct
    {
        public static MFTStruct LoadMFT(string inputMFT)
        {
            MFTStruct mft = new();
            // Read header
            var input = File.OpenRead(inputMFT);

            ReadHeader(input, ref mft.generalInfo, ref mft.unk);
            ReadTableCharDesc(input, mft.charDescList, mft.generalInfo.charsCount);
            mft.generalInfo.pagesCount = (ushort)(1 + mft.charDescList.Max(t => t.page));
            ReadTableCharEntry(input, ref mft.idList, mft.generalInfo.charsCount);
            mft.unk.unkFooter = input.ReadBytes((int)(input.Length - input.Position));
            return mft;
        }

        private static void ReadHeader(FileStream input, ref general generalInfo, ref UnknownStuff unk)
        {
            input.Position = 0;

            string magic = input.ReadStringZ();
            if (magic != "Magma Font")
            {
                Console.WriteLine("Invalid MFT Far Cry 2");
                Environment.Exit(0);
            }
            //ushort unk1 = input.ReadValueU16();
            //ushort unk2 = input.ReadValueU16();
            unk.unkHeader1 = input.ReadBytes(4);
            ushort sizeFontname = input.ReadValueU16();
            generalInfo.fontName = input.ReadString(sizeFontname);
            uint sizeVectorPath = input.ReadValueU32();
            generalInfo.vectorPath = input.ReadString((int)sizeVectorPath);
            //uint unk3 = input.ReadValueU32();
            //uint unk4 = input.ReadValueU32();
            //input.ReadValueU32();
            //input.ReadValueU32();
            //uint unk5 = input.ReadValueU32();
            //byte zero1 = input.ReadValueU8();
            unk.unkHeader2 = input.ReadBytes(21);
            //generalInfo.size = input.ReadValueS32();
            generalInfo.size = input.ReadValueU8();
            unk.unkHeader3 = input.ReadBytes(4);
            //byte zero2 = input.ReadValueU8();
            byte sizePixmapFont = input.ReadValueU8(); // = 10
            generalInfo.pixmapFont = input.ReadString(sizePixmapFont);
            generalInfo.charsCount = input.ReadValueU16();
        }
        public static void RebuildHeader(FileStream output, general generalInfo, UnknownStuff unk)
        {
            output.WriteString("Magma Font");
            output.WriteByte(0);
            output.WriteBytes(unk.unkHeader1);
            output.WriteValueU16((ushort)generalInfo.fontName.Length);
            output.WriteString(generalInfo.fontName);
            output.WriteValueU32((uint)generalInfo.vectorPath.Length);
            output.WriteString(generalInfo.vectorPath);
            output.WriteBytes(unk.unkHeader2);
            output.WriteValueU8(generalInfo.size);
            output.WriteBytes(unk.unkHeader3);
            output.WriteValueU8((byte)generalInfo.pixmapFont.Length);
            output.WriteString(generalInfo.pixmapFont);
            //output.WriteValueU16(generalInfo.charsCount);
        }

        private static void ReadTableCharDesc(FileStream input, List<charDesc> charDescList, int charsCount)
        {
            for (int i = 0; i < charsCount; i++)
            {
                charDescList.Add(new charDesc
                {
                    charID = input.ReadValueU16(), // = id
                    widthScale = input.ReadValueS16(), // = width
                    heightScale = input.ReadValueS16(), // = height
                    xoffset = input.ReadValueS16(), // = x-offset
                    yoffsetRev = input.ReadValueS16(), // = y-offset reverse = size - y-offset
                    xadvanceScale = input.ReadValueS16(), // = xadvance
                    UVLeft = input.ReadValueF32(), // = UVLeft
                    UVTop = input.ReadValueF32(), // = UVTop
                    UVRight = input.ReadValueF32(), // = UVRight
                    UVBottom = input.ReadValueF32(), // = UVBottom
                    page = input.ReadValueU16() // = page
                });
                //output.WriteLine(string.Format("id={0,-5} char={10,-5} {1,-5} {2,-5} {3,-5} {4,-5} {5,-8} {6,-8} {7,-8} {8,-8} {9,-8}", id, numb1, numb2, numb3, numb4, numb5, float1 * 256, float2 * 256, float3 * 256, float4 * 256, (char)id));
            }
        }

        public static void RebuildTableCharDesc(FileStream output, List<charDesc> charDescList)
        {
            foreach (charDesc _char in charDescList)
            {
                output.WriteValueU16(_char.charID);
                output.WriteValueS16(_char.widthScale);
                output.WriteValueS16(_char.heightScale);
                output.WriteValueS16(_char.xoffset);
                output.WriteValueS16(_char.yoffsetRev);
                output.WriteValueS16(_char.xadvanceScale);
                output.WriteValueF32(_char.UVLeft);
                output.WriteValueF32(_char.UVTop);
                output.WriteValueF32(_char.UVRight);
                output.WriteValueF32(_char.UVBottom);
                output.WriteValueU16(_char.page);
            }
        }

        private static void ReadTableCharEntry(FileStream input, ref ushort[] idList, int charsCount)
        {
            idList = new ushort[charsCount];
            idList[0] = 127;
            int baseId = 0;
            while (true)
            {
                while (input.ReadByte() == 0)
                {
                    baseId += 256;
                    if (baseId > 0xFFFF)
                        return;
                }
                long start = input.Position;
                ushort i = 0;
                while (input.Position - start < 512)
                {
                    ushort idx = input.ReadValueU16();
                    if (idx != 0)
                    {
                        idList[idx] = (ushort)(i + baseId);
                    }
                    i += 1;
                }
                baseId += 256;
                input.Position = start + 512;
            }
        }

        public static void RebuildTableCharEntry(FileStream output, ushort[] idList)
        {
            ushort[] array = new ushort[0xFFFF + 1];
            Array.Clear(array, 0, array.Length);
            ushort idx = 0;
            foreach (ushort id in idList)
            {
                array[id] = idx++;
            }
            for (int i = 0; i < 0xFFFF; i += 256)
            {
                int sum = 0;
                for (int j = i; j < i + 256; j++)
                {
                    sum += array[j];
                }
                if (sum > 0)
                {
                    output.WriteByte(1);
                    for (int j = i; j < i + 256; j++)
                    {
                        output.WriteValueU16(array[j]);
                    }
                }
                else
                {
                    output.WriteByte(0);
                }
            }
        }
    }
}
