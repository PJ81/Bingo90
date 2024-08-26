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

            rearrangeRows(rand);
            sortColumns();
            calcHash();
        }

        private void rearrangeRows(Random rand) {

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

        private void sortColumns() {

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

        public bool isValid() {

            int r = 0, u, l;
            
            for(int d = 0; d < 6; d++) {
                int[] cols = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (int t = 0; t < 3; t++) {
                    l = 0;
                    for(int z = 0; z < 9; z++) {
                        u = card[d].grid[z, t];
                        if (u > 0) {
                            l++;
                            cols[z]++;
                        }
                        r += u;
                    }
                    if (l != 5) return false;
                }

                if (cols.Contains(0)) return false;
            }

            return r == 4095;
        }

        private void calcHash() {

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

        static List<Bingo90> bingoCard = new List<Bingo90>();
        static Random rand = new Random();

        private static void createPDF() {

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Bingo90Cards";

            XColor[] colorsL = {
                XColor.FromArgb(255, 228, 3, 3), 
                XColor.FromArgb(255, 255, 140, 0), 
                XColor.FromArgb(255, 217, 202, 0), 
                XColor.FromArgb(255, 0, 128, 38), 
                XColor.FromArgb(255, 0, 77, 255), 
                XColor.FromArgb(255, 117, 7, 135), 
            };

            XColor[] colorsD = {
                XColor.FromArgb(255, 135, 0, 0),
                XColor.FromArgb(255, 169, 51, 0),
                XColor.FromArgb(255, 144, 123, 0),
                XColor.FromArgb(255, 0, 42, 4),
                XColor.FromArgb(255, 0, 15, 169),
                XColor.FromArgb(255, 36, 0, 47)
            };

            XColor[] colorsDD = {
                XColor.FromArgb(255, 46, 0, 0),
                XColor.FromArgb(255, 112, 10, 0),
                XColor.FromArgb(255, 87, 64, 0),//XColor.FromArgb(255, 95, 70, 0),
                XColor.FromArgb(255, 0, 16, 0),
                XColor.FromArgb(255, 0, 1, 112),
                XColor.FromArgb(255, 20, 0, 24)
            };

            XUnitPt REC_SIZE = 26, MARGIN = 2, BIG_RECT_MARGIN = 4, TOT_WID = REC_SIZE * 9 + 10 * MARGIN, TOT_HEI = REC_SIZE * 3 + 4 * MARGIN;
            XFont font = new XFont("verdana", REC_SIZE / 2);
            XFont fontN = new XFont("verdana", REC_SIZE * 1.5, XFontStyleEx.Bold);
            XBrush nul = new XSolidBrush(XColor.FromArgb(128, 220, 220, 220));
            XPen npn = new XPen(XColor.FromArgb(128, 160, 160, 160), 1);
            XUnitPt startX, startY;
            string bng = " * B I N G O  9 0 * ";

            int pageCount = (int)Math.Ceiling((double)(bingoCard.Count / 2.0)),
                idx, crd_idx = 0;

            for (int page = 0; page < pageCount; page++) {

                document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(document.Pages[document.Pages.Count - 1]);

                XSize sw = gfx.MeasureString(bng, fontN);
                XUnitPt l = gfx.PageSize.Width / 2 - sw.Width / 2;
                XRect tr = new XRect(l, 60, sw.Width, sw.Height);

                gfx.DrawRoundedRectangle(new XPen(colorsDD[4], 2), tr, new XSize(2, 2));
                gfx.DrawString(bng, fontN, new XSolidBrush(colorsDD[4]), tr, XStringFormats.Center);

                for (int crd = crd_idx; crd < crd_idx + 2; crd++) {

                    if (crd >= bingoCard.Count) continue;

                    // output card
                    Bingo90 sheet = bingoCard[crd];

                    for (int h = 0; h < 2; h++) {

                        startX = 30 + h * 280;
                        startY = 150 + 350 * (crd % 2);

                        for (int g = 0; g < 3; g++) {

                            idx = g + h * 3;

                            XBrush xbr = new XSolidBrush(colorsL[idx]);
                            XBrush xbw = new XSolidBrush(colorsDD[idx]);
                            XPen xpn = new XPen(colorsD[idx], 2);
                            XPen bpn = new XPen(colorsD[idx], 1);

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
                                        gfx.DrawString(sheet.card[idx].grid[c, r].ToString(), font, xbw, rect, XStringFormats.Center);
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
            const int cnt = 20;

            Bingo90 card, f;

            for (int x = 0; x < cnt; x++) {

                while (true) {

                    card = new Bingo90();
                    card.createCard(rand);
                    
                    f = bingoCard.Find(c => c.hash == card.hash);

                    if (f == null && card.isValid()) {
                        bingoCard.Add(card);
                        Console.Write(card.hash);
                        Console.WriteLine(card.error > 0 ? $" - Grid {card.error} was edited!" : card.error < 0 ? $" - Grid {card.error} not all numbers!" : "");
                        break;
                    }
                }
            }

            if (bingoCard.Count < 1) return;

            createPDF();

            Console.ReadKey();
        }
    }
}
