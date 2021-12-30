using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrZI5
{
    class TableCreator
    {
        void FillTable(ref char[,] table, int r, int c, ref int m, string message)
        {
            if (m < message.Length)
            {
                table[r, c] = message[m];
                m++;
            }
            else
                table[r, c] = '_';
        }

        public char[,] GetTable(string message, int cRows, int cCols, Fit input)
        {
            char[,] table = new char[cRows, cCols];
            int m = 0;

            if (input == Fit.LeftRight)
                for (int r = 0; r < cRows; r++)
                    for (int c = 0; c < cCols; c++)
                        FillTable(ref table, r, c, ref m, message);

            if (input == Fit.RightLeft)
                for (int r = 0; r < cRows; r++)
                    for (int c = cCols - 1; c >= 0; c--)
                        FillTable(ref table, r, c, ref m, message);


            if (input == Fit.UpDown)
                for (int c = 0; c < cCols; c++)
                    for (int r = 0; r < cRows; r++)
                        FillTable(ref table, r, c, ref m, message);

            if (input == Fit.DownUp)
                for (int c = 0; c < cCols; c++)
                    for (int r = cRows - 1; r >= 0; r--)
                        FillTable(ref table, r, c, ref m, message);

            return table;
        }

        internal string OutputStringFromTable(char[,] table, int rows, int cols, Discharge output)
        {
            string result = "";

            if (output == Discharge.UpDown)
                for (int c = 0; c < cols; c++)
                    for (int r = 0; r < rows; r++)
                        result += table[r, c];

            if (output == Discharge.DownUp)
                for (int c = 0; c < cols; c++)
                    for (int r = rows - 1; r >= 0; r--)
                        result += table[r, c];


            if (output == Discharge.LeftRight)
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        result += table[r, c];

            if (output == Discharge.RightLeft)
                for (int r = 0; r < rows; r++) //(int r = rows-1; r >= 0; r--)
                    for (int c = cols - 1; c >= 0; c--)
                        result += table[r, c];

            if (result == "")
                result = "Smth get wrong in output";

            return result;

        }

        internal char[,] TransposeCols(char[,] table, int rows, int cols, int[] keysCol)
        {
            char[,] newTable = new char[rows, cols];

            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    newTable[r, keysCol[c] - 1] = table[r, c];

            return newTable;
        }

        internal char[,] TransposeRows(char[,] table, int rows, int cols, int[] keysRow)
        {
            char[,] newTable = new char[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    newTable[keysRow[r] - 1, c] = table[r, c];

            return newTable;
        }
    }

    public enum Algorithm { Route, Vertical, Kardano, Double }
    public enum Fit { LeftRight, RightLeft, UpDown, DownUp };
    public enum Discharge { LeftRight, RightLeft, UpDown, DownUp };
    public enum Transpose { Column, Row }
    
    class CipherAlgorithms
    {
        TableCreator mh = new TableCreator();


        public string EncodeTable(char[,] table, int rows, int cols, Discharge output)
        {
            return mh.OutputStringFromTable(table, rows, cols, output);
        }

        public char[,] MakeTable(string message, int rows, int cols, Fit input)
        {
            return mh.GetTable(message, rows, cols, input);
        }

        //вертикальная перестановка
        public char[,] Vertical(char[,] table, int rows, int cols, int[] key)
        {
            char[,] newTable = mh.TransposeCols(table, rows, cols, key);
            return newTable;
        }

        //горизонтальная перестановка
        public char[,] Horizontal(char[,] table, int rows, int cols, int[] key)
        {
            char[,] newTable = mh.TransposeRows(table, rows, cols, key);
            return newTable;
        }

        //перестановка Карадано
        public char[,] Kardano(char[,] table, int rows, int cols, int grad, bool[,] isChosenCell, string message)
        {
            char[,] newTable = table;//берем текущую матрицу

            //без поворота (начальное положение)
            if (grad == 0)
            {
                int m = 0;
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[r, c])
                        {
                            newTable[r, c] = letter;
                            m++;
                        }
                        else
                            table[r, c] = ' ';
                    }
            }

            if (grad == 90)
            {
                int m = 1 * GetCountCells(isChosenCell);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[cols - c - 1, r])
                        {
                            newTable[r, c] = letter;
                            m++;
                        }
                    }
            }


            if (grad == 180)
            {
                int m = 2 * GetCountCells(isChosenCell);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[rows - r - 1, cols - c - 1])
                        {
                            newTable[r, c] = letter;
                            m++;
                        }
                    }
            }

            if (grad == 270)
            {
                int m = 3 * GetCountCells(isChosenCell);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[c, rows - r - 1])
                        {
                            newTable[r, c] = letter;
                            m++;
                        }

                    }
            }
            return newTable;
        }

        int GetCountCells(bool[,] isChosenCel)
        {
            int count = 0;
            foreach (bool b in isChosenCel)
                if (b)
                    count++;
            return count;
        }

        void FindLetter(out char letter, string message, int i)
        {
            if (i < message.Length)
                letter = message[i];
            else
                letter = '_';
        }

    }

    
    
}
