using System.Drawing;
using System.IO;

namespace _3DSExplorer.Utils
{

    public static class ImageUtil
    {
        //Decode RGB5A4 Taken from the dolphin project
        private static readonly int[] Convert5To8 = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
                                                0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
                                                0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
                                                0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };
        //Convert4To8 is just multiplication by 0x11
        //private static readonly int[] Convert3To8 = { 0x00, 0x24, 0x48, 0x6D, 0x91, 0xB6, 0xDA, 0xFF };

        private static readonly byte[] TempBytes = new byte[4];
        public enum PixelFormat
        {                   //System.Drawing.Imaging.PixelFormat equivalent
            RGBA8 = 0,      //Format32bppArgb   (-should be Rgba)           rrrrrrrr gggggggg bbbbbbbb aaaaaaaa
            RGB8 = 1,       //Format24bppRgb                                rrrrrrrr gggggggg bbbbbbbb
            RGBA5551 = 2,   //Format16bppArgb1555 (-should be Rgba5551)     rrrrrggg ggbbbbba
            RGB565 = 3,     //Format16bppRgb565                             rrrrrggg gggbbbbb
            RGBA4 = 4,      //                                              rrrrgggg bbbbaaaa                   
            LA8 = 5,        //                                              llllllll aaaaaaaa
            HILO8 = 6,
            L8 = 7,         //                                              llllllll
            A8 = 8,         //                                              aaaaaaaa
            LA4 = 9,        //                                              llllaaaa
            L4 = 10,        //                                              llll
            ETC1 = 11,      //Ericsson Texture Compression //http://www.khronos.org/registry/gles/extensions/OES/OES_compressed_ETC1_RGB8_texture.txt
            ETC1A4 = 12     //Ericsson Texture Compression
        }

        private static Color DecodeColor(int val, PixelFormat pixelFormat)
        {
            int alpha = 0xFF, red, green, blue;
            switch (pixelFormat)
            {
                case PixelFormat.RGBA8:
                    red = (val >> 24) & 0xFF;
                    green = (val >> 16) & 0xFF;
                    blue = (val >> 8) & 0xFF;
                    alpha = val & 0xFF;
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGB8:
                    red = (val >> 16) & 0xFF;
                    green = (val >> 8) & 0xFF;
                    blue = val & 0xFF;
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGBA5551:
                    red = Convert5To8[(val >> 11) & 0x1F];
                    green = Convert5To8[(val >> 6) & 0x1F];
                    blue = Convert5To8[(val >> 1) & 0x1F];
                    alpha = (val & 0x0001) == 1 ? 0xFF : 0x00;
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGB565:
                    red = Convert5To8[(val >> 11) & 0x1F];
                    green = ((val >> 5) & 0x3F) * 4;
                    blue = Convert5To8[val & 0x1F];
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGBA4:
                    alpha = 0x11 * (val & 0xf);
                    red = 0x11 * ((val >> 12) & 0xf);
                    green = 0x11 * ((val >> 8) & 0xf);
                    blue = 0x11 * ((val >> 4) & 0xf);
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.LA8:
                    red = val >> 8;
                    alpha = val & 0xFF;
                    return Color.FromArgb(alpha, red, red, red);
                case PixelFormat.HILO8: //use only the HI
                    red = val >> 8;
                    return Color.FromArgb(alpha, red, red, red);
                case PixelFormat.L8:
                    return Color.FromArgb(alpha, val, val, val);
                case PixelFormat.A8:
                    return Color.FromArgb(val, alpha, alpha, alpha);
                case PixelFormat.LA4:
                    red = val >> 4;
                    alpha = val & 0x0F;
                    return Color.FromArgb(alpha, red, red, red);
                default:
                    return Color.White;
            }
        }

        private static void DecodeTile(int iconSize, int tileSize, int ax, int ay, Bitmap bmp, Stream fs, PixelFormat pixelFormat)
        {
            if (tileSize == 0)
            {
                fs.Read(TempBytes, 0, 2);
                bmp.SetPixel(ax, ay, DecodeColor((TempBytes[1] << 8) + TempBytes[0], pixelFormat));
            }
            else
                for (var y = 0; y < iconSize; y += tileSize)
                    for (var x = 0; x < iconSize; x += tileSize)
                        DecodeTile(tileSize, tileSize / 2, x + ax, y + ay, bmp, fs, pixelFormat);
        }

        public static Bitmap ReadImageFromStream(Stream fs, int width, int height, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(width, height);
            for (var y = 0; y < height; y += 8)
                for (var x = 0; x < width; x += 8)
                    DecodeTile(8, 8, x, y, bmp, fs, pixelFormat);
            return bmp;
        }
    }

}
