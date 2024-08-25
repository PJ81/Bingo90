using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Bingo90 {

    public class Grid {

        public int[,] grid;
        public int count;

        public Grid() {
            grid = new int[9, 3];
            count = 0;
        }

        public void set(int c, int r, int v) {
            grid[c, r] = v;
            count++; 
        }

        public int rows(int r) {
            int f = 0;
            for(int e = 0; e < 9; e++) {
                if (grid[e, r] > 0) f++;
            }

            return f;
        }

        public void printGrid() {
            for (int j = 0; j < grid.GetLength(1); j++) {
                for (int i = 0; i < grid.GetLength(0); i++) {
                    Console.Write($"{grid[i, j].ToString("D2")}, ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(count);
            Console.WriteLine();
        }

    }

    public class Bingo90 {

        public Grid[] card;
        public string hash;
        public int error;

        public Bingo90() {
            card = new Grid[6];

        }

        private int isArrayEmpty(int[] arr) {
     
            for (int y = 0; y < arr.Length; y++) {
                if (arr[y] != 0) return y;
            }

            return -1;
        }

        private int isColumnFull(Grid c, int col) {

            for(int y = 0; y < 3; y++) {
                if (c.grid[col, y] == 0) return y;
            }

            return -1;
        }

        private void swap(int q, int v, int grd) {
            int ngrd = (grd + 1) % 6;

            if (card[ngrd].grid[q,1] == 0) {
                card[ngrd].grid[q, 1] = v;
            } else if (card[ngrd].grid[q, 2] == 0) {
                card[ngrd].grid[q, 2] = v;
            }

            for (int d = 1; d < 3; d++) {
                for (int r = 1; r < 8; r++) {
                    if (card[ngrd].grid[r, d] != 0 && card[grd].grid[r, d] == 0) {
                        card[grd].grid[r, d] = card[ngrd].grid[r, d];
                        card[ngrd].grid[r, d] = 0;
                        return;
                    } 
                }
            }

        }

        public void createCard(Random rand) {
            for (int j = 0; j < card.Length; j++) {
                card[j] = new Grid();
            }

            // all numbers we need to fill the 6 grids
            int[] setA = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] setB = { 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };

            int[][] setC = new int[7][];
            setC[0] = new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            setC[1] = new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
            setC[2] = new int[] { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
            setC[3] = new int[] { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
            setC[4] = new int[] { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };
            setC[5] = new int[] { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69 };
            setC[6] = new int[] { 70, 71, 72, 73, 74, 75, 76, 77, 78, 79 };

            int r, s;

            // picks a random number inside of the array
            int chooseNumber(int[] arr) {
                while(true) {
                    r = rand.Next(arr.Length);

                    if(arr[r] != 0) {
                        s = arr[r];
                        arr[r] = 0;
                        return s;
                    }
                }
            }

            // fills all first rows (all grids, all columns)
            for (int v = 0; v < 6; v++) {
                for (int w = 0; w < 9; w++) {
                    if(w == 0) {
                        card[v].set(w, 0, chooseNumber(setA));
                    } else if (w == 8) {
                        card[v].set(w, 0, chooseNumber(setB));
                    } else {
                        card[v].set(w, 0, chooseNumber(setC[w - 1]));
                    }

                }
            }

            // fills column 9 (just 5 grids)
            int[] used = { -1, -1, -1, -1, -1 };
            for (int v = 0; v < 5; v++) {
                while(true) {
                    r = rand.Next(6);
                    if(!used.Contains(r)) {
                        used[v] = r;
                        card[r].set(8, 1, chooseNumber(setB));
                        break;
                    }
                    
                }

            }

            // fills column 1 (just 3 grids)
            used = new int[] { -1, -1, -1 };
            for (int v = 0; v < 3; v++) {
                while (true) {
                    r = rand.Next(6);
                    if (!used.Contains(r)) {
                        used[v] = r;
                        card[r].set(0, 1, chooseNumber(setA));
                        break;
                    }

                }
            }

            // fills the rest of the columns (2 - 7) of all grids 
            int count = 28, grid, row;
            while(count > 0) {

                List<Tuple<int, List<Tuple<int, int>>>> uncompleted = new List<Tuple<int, List<Tuple<int, int>>>>();

                for(int grd = 0; grd < 6; grd++) {
                    List<Tuple<int, int>> cols = new List<Tuple<int, int>>();

                    if (card[grd].count < 15) {

                        for(int col = 1; col < 8; col++) {
                            row = isColumnFull(card[grd], col);
                            if (row != -1 && isArrayEmpty(setC[col - 1]) != -1) {
                                cols.Add(new Tuple<int, int>(col, row));
                            }
                        }

                        if (cols.Count > 0) {
                            uncompleted.Add(new Tuple<int, List<Tuple<int, int>>>(grd, cols));
                        } else {
                            int q = 0, g = 0;
                            for( ; q < 7; q++) {
                                g = isArrayEmpty(setC[q]);
                                if (g != -1) break;
                            }

                            swap(q + 1, setC[q][g], grd);

                            error = grd;

                            return;

                        }
                    }
                }

                s = rand.Next(uncompleted.Count);
                grid = uncompleted[s].Item1;
                Tuple<int, int> col_row = uncompleted[s].Item2[rand.Next(uncompleted[s].Item2.Count)];

                card[grid].set(col_row.Item1, col_row.Item2, chooseNumber(setC[col_row.Item1 - 1]));
                count--;
            }
        }

        public void rearrangeRows(Random rand) {

            int c = 0, r = 0, i;
            bool found = true;

            while(found) {

                found = false;
                for (int v = 0; v < 6; v++) {

                    List<int> empty = new List<int>();
                    for (int q = 0; q < 9; q++) {
                        if (card[v].grid[q, r + 1] == 0) empty.Add(q);
                    }

                    while (card[v].rows(r) > 5 && empty.Count > 0) {
                        i = rand.Next(empty.Count);
                        c = empty[i];
                        empty.RemoveAt(i);

                        card[v].grid[c, r + 1] = card[v].grid[c, r];
                        card[v].grid[c, r] = 0;

                        found = true;
                    }

                }

                r++;
                if (r > 1) r = 0;
            }

        }

        public void sortColumns() {
            int tmp;

            bool swap(int v, int r, int a, int b) {

                if (card[v].grid[r, b] == 0 || card[v].grid[r, b] == 0) return false;
                if (card[v].grid[r, a] > card[v].grid[r, b]) {
                    tmp = card[v].grid[r, a];
                    card[v].grid[r, a] = card[v].grid[r, b];
                    card[v].grid[r, b] = tmp;
                    return true;
                }
                return false;
            }

            for(int v = 0; v < 6; v++) {
                for(int r = 0; r < 9; r++) {
                    bool t;
                    do {
                        t = false;
                        if (swap(v, r, 0, 1)) t = true;
                        if (swap(v, r, 1, 2)) t = true;
                        if (swap(v, r, 0, 2)) t = true;
                    } while (t);
                    
                }
            }
        }

        public void printCard() {
            for(int j = 0; j < card.Length; j++) {
                card[j].printGrid();
            }
        }

        public void calcHash() {

            byte[] getHash(string str) {
                using (HashAlgorithm algorithm = HMACSHA1.Create())
                    return algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
            }

            string h = "";
            for(int v = 0; v < 6; v++) {
                for(int j = 0; j < 3; j++) {
                    for(int i = 0; i < 9; i++) {
                        h += card[v].grid[i, j].ToString();
                    }
                }
            }

            hash = BitConverter.ToString(getHash(h)).Replace("-", String.Empty); ;
        }
    }

    class Program {

        static Bingo90[] bingoCard;
        static Random rand = new Random();

        private static void createPDF() {

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Bingo90Cards";

            XUnitPt REC_SIZE = 26, MARGIN = 2, BIG_RECT_MARGIN = 4, TOT_WID = REC_SIZE * 9 + 10 * MARGIN, TOT_HEI = REC_SIZE * 3 + 4 * MARGIN;

            XFont font = new XFont("verdana", REC_SIZE / 2);
            XFont fontN = new XFont("verdana", REC_SIZE * 1.5, XFontStyleEx.Bold);
            XFont fontS = new XFont("verdana", REC_SIZE * .3);
            XBrush xbr = new XSolidBrush(XColor.FromArgb(128, 15, 159, 255));
            XBrush nul = new XSolidBrush(XColor.FromArgb(128, 200, 200, 200));
            XPen xpn = new XPen(XColor.FromArgb(255, 6, 64, 102), 2);
            XPen npn = new XPen(XColor.FromArgb(128, 160, 160, 160), 1);
            XPen bpn = new XPen(XColor.FromArgb(255, 8, 80, 128), 1);


            XUnitPt startX, startY;

            int pageCount = (int)Math.Ceiling((double)(bingoCard.Length / 2.0)),
                idx, crd_idx = 0;

            for (int page = 0; page < pageCount; page++) {

                document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(document.Pages[document.Pages.Count - 1]);

                gfx.DrawString("* B I N G O  9 0 *", fontN, XBrushes.Black, new XRect(0, 30, gfx.PageSize.Width, 80), XStringFormats.Center);

                for (int crd = crd_idx; crd < crd_idx + 2; crd++) {

                    if (crd >= bingoCard.Length) continue;

                    // output card
                    Bingo90 sheet = bingoCard[crd];

                    for (int h = 0; h < 2; h++) {

                        startX = 30 + h * 280;
                        startY = 140 + 350 * (crd % 2);

                        for (int g = 0; g < 3; g++) {

                            idx = g + h * 3;

                            for (int r = 0; r < 3; r++) {
                                for (int c = 0; c < 9; c++) {

                                    XUnitPt x = startX + REC_SIZE * c + c * MARGIN, y = startY + REC_SIZE * r + r * MARGIN;
                                    XRect rect = new XRect(x, y, REC_SIZE, REC_SIZE);

                                    if(sheet.card[idx].grid[c, r] == 0) {
                                        gfx.DrawRoundedRectangle(nul, rect, new XSize(2, 2));
                                        gfx.DrawRoundedRectangle(npn, rect, new XSize(2, 2));
                                    } else {
                                        gfx.DrawRoundedRectangle(xbr, rect, new XSize(2, 2));
                                        gfx.DrawRoundedRectangle(bpn, rect, new XSize(2, 2));
                                        gfx.DrawString(sheet.card[idx].grid[c, r].ToString(), font, XBrushes.Black, rect, XStringFormats.Center);
                                    }
                                    
                                }
                            }

                            XRect b = new XRect(startX - BIG_RECT_MARGIN, startY - BIG_RECT_MARGIN, TOT_WID + BIG_RECT_MARGIN, TOT_HEI + BIG_RECT_MARGIN);
                            gfx.DrawRoundedRectangle(xpn, b, new XSize(2, 2));

                            startY += REC_SIZE * 4;
                        }
                    }
                }

                crd_idx += 2;
            }
            
            document.Save("bingo90Cards.pdf");
            
        }

        static void Main(string[] args) {
            const int cnt = 9;

            bingoCard = new Bingo90[cnt];

            for (int x = 0; x < cnt; x++) {
                bingoCard[x] = new Bingo90();
                bingoCard[x].createCard(rand);
                bingoCard[x].rearrangeRows(rand);
                bingoCard[x].sortColumns();
                bingoCard[x].calcHash();
                Console.Write(bingoCard[x].hash);
                Console.WriteLine(bingoCard[x].error > 0 ? $" - Error grid {bingoCard[x].error}" : "");
            }

            createPDF();

            //Console.ReadKey();
        }
    }
}
