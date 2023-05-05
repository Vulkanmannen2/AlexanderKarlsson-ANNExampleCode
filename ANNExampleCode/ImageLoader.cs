using System;
using System.IO;
using System.Resources;

namespace ANN_test
{
	public static class ImageLoader
    {
        // First single is the width of the image
        // Rest is grayscale value of each pixle
        // Asumes the file is in right format
        public static float[] LoadImageAsArray(string path, int width, int height)
        {
            TestManager.TestInt(width);
            TestManager.TestInt(height);

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fs);

            List<float> imageAsList = new List<float>();

            float imageWidth = reader.ReadSingle();
            TestManager.CompareInts((int)imageWidth, width);

            imageAsList.Add(imageWidth);

            for (int i = 0; i < width * height; ++i)
            {
                imageAsList.Add(reader.ReadSingle());
            }

            reader.Close();

            float[] imageAsArray = imageAsList.ToArray();

            TestManager.TestFloats(imageAsArray, 0f, 1f, 1);

            return imageAsArray;
        }
    }
}

