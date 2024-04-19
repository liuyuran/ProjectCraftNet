using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using Avalonia.Controls;
using CoreMod.generator;
using ModManager.game.generator;
using ModManager.utils;
using static System.Drawing.Imaging.ImageFormat;

namespace LandSimulator;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public partial class MainWindow : Window
{
    private IChunkGenerator _generator = new DefaultChunkGenerator();
    private const int ChunkSize = 64;

    public MainWindow()
    {
        InitializeComponent();
        var drawingBitmap = new Bitmap(ChunkSize, ChunkSize);
        GeneratePreview(drawingBitmap, new IntVector3(0, 0, 0));
        // 创建一个内存流
        using var memoryStream = new MemoryStream();
        // 将System.Drawing.Bitmap保存到内存流中
        drawingBitmap.Save(memoryStream, Png); // 或者使用其他格式，如Jpeg
        // 重置流的位置到开始
        memoryStream.Position = 0;
        ImageControl.Source = new Avalonia.Media.Imaging.Bitmap(memoryStream);
    }

    private static int GetBlockIndex(int x, int y, int z, int size)
    {
        if (x > size) x %= size;
        if (y > size) y %= size;
        if (z > size) z %= size;
        return z * size * size + y * size + x;
    }
    
    private void GeneratePreview(Bitmap bitmap, IntVector3 chunkPosition)
    {
        var data = _generator.GenerateChunkBlockData(ChunkSize, chunkPosition);
        for (var i = 0; i < ChunkSize; i++)
        {
            for (var j = 0; j < ChunkSize; j++)
            {
                var height = 0;
                for (var k = 0; k < ChunkSize; k++)
                {
                    if (data[GetBlockIndex(i, j, k, ChunkSize)].BlockId <= 0) continue;
                    height = k;
                    break;
                }

                var percent = (float)height / ChunkSize;
                bitmap.SetPixel(i, j, Color.FromArgb(
                    (int)(255 * percent),
                    (int)(255 * percent),
                    (int)(255 * percent)
                ));
            }
        }
    }
}