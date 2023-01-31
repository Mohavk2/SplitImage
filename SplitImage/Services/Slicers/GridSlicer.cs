using SplitImage.Services.Slicers.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace SplitImage.Services.Slicers
{
    internal class GridSlicer : ISlicer
    {
        public const int MinRows = 1;
        public const int MaxRows = 100;
        public const int MinColumns = 1;
        public const int MaxColumns = 100;

        public string[] SupportedImageFormats { get; } = { "png" };

        int rows = 1;
        /// <summary>
        /// The number of rows into which the picture can be sliced. Min = 1, Max = 100
        /// </summary>
        public int Rows
        {
            get => rows;
            set
            {
                rows = value < MinRows ? MinRows 
                     : value > MaxRows ? MaxRows : value;
            }
        }

        int columns = 1;
        /// <summary>
        /// The number of columns into which the picture can be sliced. Min = 1, Max = 100
        /// </summary>
        public int Columns
        {
            get => columns;
            set
            {
                columns = value < MinColumns ? MinColumns 
                        : value > MaxColumns ? MaxColumns : value;
            }
        }

        public Bitmap[,] Slice(Bitmap target)
        {   
            int difX = target.Width % columns;
            int difY = target.Height % rows;

            int width = target.Width / columns;
            int height = target.Height / rows;

            Bitmap[,] pieces = new Bitmap [rows, columns];

            int y = 0;
            for (int i = 0; i < rows; i++)
            {
                int h;
                if (i == 0)
                    h = height + difY / 2;

                else if (i == rows - 1)
                    h = height + difY / 2 + difY % 2;

                else
                    h = height;

                int x = 0;
                for (int j = 0; j < columns; j++)
                {
                    int w;
                    if (j == 0) 
                        w = width + difX / 2;

                    else if (j == columns - 1)
                        w = width + difX / 2 + difX % 2; 

                    else
                        w = width;

                    pieces[i, j] = target.Clone(new Rectangle(x, y, w, h), target.PixelFormat);
                    x += w;
                }
                y += h;
            }
            return pieces;
        }
    }
}
