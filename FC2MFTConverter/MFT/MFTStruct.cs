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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC2MFTConverter
{
    public class MFTStruct
    {
        public general generalInfo = new();
        public List<charDesc> charDescList = new();
        public ushort[] idList;
        public UnknownStuff unk = new();
        public class general
        {
            public string fontName;
            public ushort charsCount;
            //public uint WidthImg;
            //public uint HeightImg;
            public int size;
            public string vectorPath;
            public string pixmapFont; // = PixmapFont
            public ushort pagesCount;
        }
        public class charDesc
        {
            public ushort charID;
            public short widthScale;
            public short heightScale;
            public short xoffset;
            public short yoffsetRev;
            public short xadvanceScale;
            public float UVLeft;
            public float UVTop;
            public float UVRight;
            public float UVBottom;
            public ushort page;
        }
        public class UnknownStuff
        {
            public byte[] unkHeader1;
            public byte[] unkHeader2;
            public byte[] unkFooter;
        }
    }
}
