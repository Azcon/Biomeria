using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPBzadanie1;
using System.ServiceModel;


namespace BPBzadanie1
{
    class Program
    {
        static List<ServiceReference1.Point> normalizacja(List<ServiceReference1.Point> lista, int k)
        {
            int j = 0;
            double b = 0.0;
            double a = 1.0;
            double parametr_n = ((double)lista.Count / k);
            ServiceReference1.Point[] punkt = new ServiceReference1.Point[k];
            List<ServiceReference1.Point> lista_pomocnicza = new List<ServiceReference1.Point>(k);
            for (int i = 0; i < k - 2; i++)
            {
                j = (int)Math.Floor(i * parametr_n);
                b = i * parametr_n - j;
                a = 1 - b;
                punkt[i] = new ServiceReference1.Point();
                punkt[i].x = (float)(a * lista[j].x + b * lista[j + 1].x);
                punkt[i].y = (float)(a * lista[j].y + b * lista[j + 1].y);
                punkt[i].force = (byte)(a * lista[j].force + b * lista[j + 1].force);
                lista_pomocnicza.Add(punkt[i]);
            }
            int J = (int)Math.Floor((k - 2) * parametr_n);
            punkt[k - 2] = new ServiceReference1.Point();
            punkt[k - 2].x = (float)0.85 * lista[J].x + (float)0.15 * lista[J-1].x;
            punkt[k - 2].y = (float)0.85 * lista[J].y + (float)0.15 * lista[J - 1].y;
            punkt[k - 2].force = (byte)(0.85 * lista[J].force + 0.15 * lista[J - 1].force);
            lista_pomocnicza.Add(punkt[k - 2]);
            punkt[k - 1] = new ServiceReference1.Point();
            punkt[k - 1].x = lista[J].x;
            punkt[k - 1].y = lista[J].y;
            punkt[k - 1].force = lista[J].force;
            lista_pomocnicza.Add(punkt[k - 1]);
            return lista_pomocnicza;        
        }
        static double distance_euclides(ServiceReference1.Point pierwszy, ServiceReference1.Point drugi)
        {
            return Math.Sqrt(Math.Pow((pierwszy.force - drugi.force), 2) + Math.Pow((pierwszy.x - drugi.x), 2) + Math.Pow((pierwszy.y - drugi.y), 2));
        }
        static double distance_maximus(ServiceReference1.Point pierwszy, ServiceReference1.Point drugi)
        {
            return Math.Max(Math.Abs(pierwszy.force - drugi.force), Math.Max(Math.Abs(pierwszy.x - drugi.x), Math.Abs(pierwszy.y - drugi.y)));
        }
        static double distance_taxi(ServiceReference1.Point pierwszy, ServiceReference1.Point drugi)
        {
            return Math.Abs(pierwszy.force - drugi.force) + Math.Abs(pierwszy.x - drugi.x) + Math.Abs(pierwszy.y - drugi.y);
        }
        static double DTWDistance(List<ServiceReference1.Point> s, List<ServiceReference1.Point> t) //to char to nie jest char tylko lista punktów na które składają się trójki x,y,force
        {
            double[,] DTW = new double[s.Count  , t.Count ];
            int i, j;
            double cost; 
            for (i = 1; i < t.Count; i++)
            {
                DTW[0, i] = int.MaxValue;
            }
            for (i = 1; i < s.Count; i++)
            {
                DTW[i, 0] = int.MaxValue;
            }
            DTW[0, 0] = 0;
            for (i = 1; i < s.Count; i++)
            {
                for (j = 1;j < t.Count; j++)
                {
                    
                    cost = distance_euclides(s[i], t[j]);
                    DTW[i, j] = cost + Math.Min(DTW[i - 1, j], Math.Min(DTW[i, j - 1], DTW[i - 1, j - 1])); //insertion , deletion, match           
                }
            }
            //upychanie do listy indeksow najkrotszej sciezki
            //Musimy isc od konca zeby znalezc sciezke
            List<string> sciezka_najkrotsza = new List<string>();
            sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", s.Count -1 , t.Count - 1));
            int k = s.Count -1 , l = t.Count -1 ;
            while (k != 1 || l != 1)
            {
                if ((DTW[k - 1, l]) == Math.Min(DTW[k - 1, l], Math.Min(DTW[k, l - 1], DTW[k - 1, l - 1])))
                {
                    sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", k - 1, l));
                    k = k - 1;
                }
                else if (DTW[k, l - 1] == Math.Min(DTW[k - 1, l], Math.Min(DTW[k, l - 1], DTW[k - 1, l - 1])))
                {
                    sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", k, l - 1));
                    l = l - 1;
                }
                else
                {
                    sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", k - 1, l - 1));
                    k = k - 1;
                    l = l - 1;
                }
            }  
            //wypisanie indeksow najkrotszej sciezki
            Console.Write("\nNajkrotsza sciezka ma indeksy: \t");
            foreach (string indeks_sciezki in sciezka_najkrotsza)
            {
                Console.Write("{0} ",indeks_sciezki);
            }
            return DTW[s.Count-1, t.Count-1]; //DTW[s.Count, t.Count];
        }

        static void Main(string[] args)
        {
            ServiceReference1.PPBServiceClient client = new ServiceReference1.PPBServiceClient();
            var x = client.GetSignatures("fake", "a1dae9c55c60c73276f5e7fe9306e5da", 6298);
            var y = client.GetSignatures("fake", "a1dae9c55c60c73276f5e7fe9306e5da", 6301);
            /* osobisty podpis nie działa
            var x = client.GetSignatures("as21614@st.amu.edu.pl", "0c429600f40074205c023f3a876fe2a6", 8510); 
           */
            /* RegionsWithStrokesList[0] to podpis w pierwszym oknie
               Strokes[0] to kreska pierwsza w oknie
               inquiryPoints.pointsList[0] to pierwszy punkt w kresce
            */

            // wybieramy dwa okna ktore porownujemy

            //punkty podpisow fakowych1 [2-6]
            List<ServiceReference1.Point> fake1_lista2 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake1_lista3 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake1_lista4 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake1_lista5 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake1_lista6 = new List<ServiceReference1.Point>();
            //punkty podpisow fakowych2 [2-6]
            List<ServiceReference1.Point> fake2_lista2 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake2_lista3 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake2_lista4 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake2_lista5 = new List<ServiceReference1.Point>();
            List<ServiceReference1.Point> fake2_lista6 = new List<ServiceReference1.Point>();

            for (int i = 0; i < x.RegionsWithStrokesList[2].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[2].Strokes[i].inquiryPoints.pointsList.Length)
                {              
                    fake1_lista2.Add(x.RegionsWithStrokesList[2].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < x.RegionsWithStrokesList[3].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[3].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    x.RegionsWithStrokesList[3].Strokes[i].inquiryPoints.pointsList[j].x -= 637;
                    fake1_lista3.Add(x.RegionsWithStrokesList[3].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < x.RegionsWithStrokesList[4].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[4].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    x.RegionsWithStrokesList[4].Strokes[i].inquiryPoints.pointsList[j].y -= 185;
                    fake1_lista4.Add(x.RegionsWithStrokesList[4].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < x.RegionsWithStrokesList[5].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    x.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList[j].x -= 637;
                    x.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList[j].y -= 185;
                    fake1_lista5.Add(x.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < x.RegionsWithStrokesList[6].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[6].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    x.RegionsWithStrokesList[6].Strokes[i].inquiryPoints.pointsList[j].y -= 370;
                    fake1_lista6.Add(x.RegionsWithStrokesList[6].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            //to samo dla fakow2
            for (int i = 0; i < y.RegionsWithStrokesList[2].Strokes.Length; i++)
            {
                int j = 0;
                while (j < y.RegionsWithStrokesList[2].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    fake2_lista2.Add(y.RegionsWithStrokesList[2].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < y.RegionsWithStrokesList[3].Strokes.Length; i++)
            {
                int j = 0;
                while (j < y.RegionsWithStrokesList[3].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    y.RegionsWithStrokesList[3].Strokes[i].inquiryPoints.pointsList[j].x -= 637;
                    fake2_lista3.Add(y.RegionsWithStrokesList[3].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < y.RegionsWithStrokesList[4].Strokes.Length; i++)
            {
                int j = 0;
                while (j < y.RegionsWithStrokesList[4].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    y.RegionsWithStrokesList[4].Strokes[i].inquiryPoints.pointsList[j].y -= 185;
                    fake2_lista4.Add(y.RegionsWithStrokesList[4].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < y.RegionsWithStrokesList[5].Strokes.Length; i++)
            {
                int j = 0;
                while (j < y.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    y.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList[j].x -= 637;
                    y.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList[j].y -= 185;
                    fake2_lista5.Add(y.RegionsWithStrokesList[5].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            for (int i = 0; i < y.RegionsWithStrokesList[6].Strokes.Length; i++)
            {
                int j = 0;
                while (j < y.RegionsWithStrokesList[6].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    y.RegionsWithStrokesList[6].Strokes[i].inquiryPoints.pointsList[j].y -= 370;
                    fake2_lista6.Add(y.RegionsWithStrokesList[6].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            //--------------------------------------------------------------------------------------------------------------
            Console.Write("Podaj ktore okno bierzesz pod uwage (od 0 do 8): ");
            int nr_listy0 = int.Parse(Console.ReadLine());
            List<ServiceReference1.Point> lista0 = new List<ServiceReference1.Point>();
            Console.Write("Podaj ktore okno bierzesz pod uwage (od 0 do 8): ");
            int nr_listy1 = int.Parse(Console.ReadLine());
            List<ServiceReference1.Point> lista1 = new List<ServiceReference1.Point>();

            //upychanie punktow do listy0 i listy1

            for (int i = 0; i < x.RegionsWithStrokesList[nr_listy0].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    lista0.Add(x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;

                }

            }
            for (int i = 0; i < x.RegionsWithStrokesList[nr_listy1].Strokes.Length; i++)
            {
                int j = 0;
                while (j < x.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList.Length)
                {
                    lista1.Add(x.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList[j]);
                    j++;
                }
            }
            //ilosc punktow w listach 1 i 2
            Console.WriteLine("\nLista0 ma {0} punktow. Lista1 ma {1} punktow.", lista0.Count, lista1.Count);
            //wyswietlanie force, x, y dla punktow w tablicy 1
            
            Console.WriteLine("\n Punkt(\"force\",x,y)\n");

            foreach (ServiceReference1.Point punkcik in lista0)
            {
                Console.Write("({0}, {1}, {2})**", punkcik.force, punkcik.x, punkcik.y);
            }
            Console.WriteLine("\n\n");
            foreach (ServiceReference1.Point punkcik in lista1)
            {
                Console.Write("({0}, {1}, {2})**", punkcik.force, punkcik.x, punkcik.y);
            }
                //liczenie wagi dla listy0 i listy1
                List<double> wagi_ostatecznelista_fake1 = new List<double>();
            double minimalny_fejk1 = 100000;
            int indeks_minimalny_fejk1 = 1000;
            double Waga = DTWDistance(lista0, lista1);
            //Console.WriteLine("\nWaga niepodobienstwa to : {0}\n", Waga);
            int punkty_lista0 = 0;
            for (int i = 0; i < x.RegionsWithStrokesList[nr_listy0].Strokes.Length; i++)
            {
                punkty_lista0 = punkty_lista0 + x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList.Length;
            }
            int punkty_lista1 = 0;
            for (int i = 0; i < x.RegionsWithStrokesList[nr_listy1].Strokes.Length; i++)
            {
                punkty_lista1 = punkty_lista1 + x.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList.Length;
            }
            double Waga_ostateczna = Waga / (punkty_lista0 + punkty_lista1);
            //Console.WriteLine("Waga ostateczna to {0}", Waga_ostateczna);
            wagi_ostatecznelista_fake1.Add(Waga_ostateczna);



            for (int i = 0; i < wagi_ostatecznelista_fake1.Count; i++)
            {
                if (wagi_ostatecznelista_fake1[i] < minimalny_fejk1 && wagi_ostatecznelista_fake1[i] > 0)
                {
                    minimalny_fejk1 = wagi_ostatecznelista_fake1[i];
                    indeks_minimalny_fejk1 = i;
                }
            }
            // wypisanie liczby elementow z kazdego z podpisow [2-6] zestawu fake1 i fake2
            Console.WriteLine("\nFake1 Lista 2 ma {0} punktów", fake1_lista2.Count);
            Console.WriteLine("Fake1 Lista 3 ma {0} punktów", fake1_lista3.Count);
            Console.WriteLine("Fake1 Lista 4 ma {0} punktów", fake1_lista4.Count); // posiada najwiecej punktow tj. 178
            Console.WriteLine("Fake1 Lista 5 ma {0} punktów", fake1_lista5.Count);
            Console.WriteLine("Fake1 Lista 6 ma {0} punktów", fake1_lista6.Count);
            Console.WriteLine("Fake2 Lista 2 ma {0} punktów", fake2_lista2.Count);
            Console.WriteLine("Fake2 Lista 3 ma {0} punktów", fake2_lista3.Count);
            Console.WriteLine("Fake2 Lista 4 ma {0} punktów", fake2_lista4.Count);
            Console.WriteLine("Fake2 Lista 5 ma {0} punktów", fake2_lista5.Count);
            Console.WriteLine("Fake2 Lista 6 ma {0} punktów", fake2_lista6.Count);
            Console.WriteLine("---normalizacja---");

            // wypisanie elementow podpisu z okna [2] z zestawu fake2 przed normalizacja
            foreach (ServiceReference1.Point p in fake2_lista2)
            {
                Console.Write("[{0}; {1}; {2}]**", p.force, p.x, p.y);
            }
            // wypisanie elementow podposi z okna [2] z zestawu fake2 po normalizacji
            Console.WriteLine("\n");
            foreach (ServiceReference1.Point q in normalizacja(fake2_lista2, 178))
            {
                Console.Write("[{0}; {1}; {2}]**", q.force, q.x, q.y);
            }
            // liczba elementow znormalizowanego podpisu z okna [2] fake2 po normalizacji
            // Console.WriteLine("Fake2 Lista 2 ma po normalizacji {0} punktów", normalizacja(fake2_lista2, 178).Count);
            fake1_lista2 = normalizacja(fake1_lista2,178);
            fake1_lista3 = normalizacja(fake1_lista3, 178);
            fake1_lista5 = normalizacja(fake1_lista5, 178);
            fake1_lista6 = normalizacja(fake1_lista6, 178);
            fake2_lista2 = normalizacja(fake2_lista2, 178);
            fake2_lista3 = normalizacja(fake2_lista3, 178);
            fake2_lista4 = normalizacja(fake2_lista4, 178);
            fake2_lista5 = normalizacja(fake2_lista5, 178);
            fake2_lista6 = normalizacja(fake2_lista6, 178);

            Console.WriteLine("\nFake1 Lista 2 ma {0} punktów", fake1_lista2.Count);
            Console.WriteLine("Fake1 Lista 3 ma {0} punktów", fake1_lista3.Count);
            Console.WriteLine("Fake1 Lista 4 ma {0} punktów", fake1_lista4.Count);
            Console.WriteLine("Fake1 Lista 5 ma {0} punktów", fake1_lista5.Count);
            Console.WriteLine("Fake1 Lista 6 ma {0} punktów", fake1_lista6.Count);
            Console.WriteLine("Fake2 Lista 2 ma {0} punktów", fake2_lista2.Count);
            Console.WriteLine("Fake2 Lista 3 ma {0} punktów", fake2_lista3.Count);
            Console.WriteLine("Fake2 Lista 4 ma {0} punktów", fake2_lista4.Count);
            Console.WriteLine("Fake2 Lista 5 ma {0} punktów", fake2_lista5.Count);
            Console.WriteLine("Fake2 Lista 6 ma {0} punktów", fake2_lista6.Count);



            /*
            int ilosc_punktow_fake1_lista7 = 0;
            for (int i = 0; i < x.RegionsWithStrokesList[7].Strokes.Length; i++)
            {
                ilosc_punktow_fake1_lista7 = ilosc_punktow_fake1_lista7 + x.RegionsWithStrokesList[7].Strokes[i].inquiryPoints.pointsList.Length;
            }
            int ilosc_punktow_fake1_lista8 = 0;
            for (int i = 0; i < x.RegionsWithStrokesList[8].Strokes.Length; i++)
            {
                ilosc_punktow_fake1_lista8 = ilosc_punktow_fake1_lista8 + x.RegionsWithStrokesList[8].Strokes[i].inquiryPoints.pointsList.Length;
            }
            int ilosc_punktow_fake2_lista7 = 0;
            for (int i = 0; i < y.RegionsWithStrokesList[7].Strokes.Length; i++)
            {
                ilosc_punktow_fake2_lista7 = ilosc_punktow_fake2_lista7 + y.RegionsWithStrokesList[7].Strokes[i].inquiryPoints.pointsList.Length;
            }
            int ilosc_punktow_fake2_lista8 = 0;
            for (int i = 0; i < y.RegionsWithStrokesList[8].Strokes.Length; i++)
            {
                ilosc_punktow_fake2_lista8 = ilosc_punktow_fake2_lista8 + y.RegionsWithStrokesList[8].Strokes[i].inquiryPoints.pointsList.Length;
            }
            Console.WriteLine("Fake1 Lista 7 ma {0} punktów", ilosc_punktow_fake1_lista7);
            Console.WriteLine("Fake2 Lista 8 ma {0} punktów", ilosc_punktow_fake1_lista8);
            Console.WriteLine("Fake2 Lista 7 ma {0} punktów", ilosc_punktow_fake2_lista7);
            Console.WriteLine("Fake2 Lista 8 ma {0} punktów", ilosc_punktow_fake2_lista8);
            */

            /*
            for (int i = 0; i < 9; i++)
            {
                ServiceReference1.Point punkt = x.RegionsWithStrokesList[i].Strokes[0].inquiryPoints.pointsList[0];
                string rozmiar_okna_x = x.RegionsWithStrokesList[i].Region.Width.ToString();
                string rozmiar_okna_y = x.RegionsWithStrokesList[i].Region.Height.ToString();
                Console.WriteLine("\n Rozmiar okna to: (szerokosc){0} na (wysokosc){1}, współrzędne 1 punktu to({2}; {3}; {4})", rozmiar_okna_x, rozmiar_okna_y, punkt.force, punkt.x, punkt.y);
            }
            Console.Write("------------------------------------------------------");
            for (int i = 0; i < 9; i++)
            {
                ServiceReference1.Point punkt = y.RegionsWithStrokesList[i].Strokes[0].inquiryPoints.pointsList[0];
                string rozmiar_okna_x = y.RegionsWithStrokesList[i].Region.Width.ToString();
                string rozmiar_okna_y = y.RegionsWithStrokesList[i].Region.Height.ToString();
                Console.WriteLine("\n Rozmiar okna to: (szerokosc){0} na (wysokosc){1}, współrzędne 1 punktu to({2}; {3}; {4})", rozmiar_okna_x, rozmiar_okna_y, punkt.force, punkt.x, punkt.y);
            }
            */

            Console.WriteLine("\nKONIEC");
            Console.ReadKey();
        }
        /*
List<double> wagi_ostatecznelista_fake1 = new List<double>();
List<double> wagi_ostatecznelista_fake2 = new List<double>();
double minimalny_fejk1 = 100000;
int indeks_minimalny_fejk1 = 1000;
double minimalny_fejk2 = 100000;
int indeks_minimalny_fejk2 = 1000;
//wszystkie mozliwe kombinacje okien, jest 9 okien (od 0 do 8) bierzemy pierwsze 8 okien i porownojemy z kazdym z 9
for (int m = 0; m < 9; m++)
{
for (int n = 0; n < 9; n++)
{
    List<ServiceReference1.Point> lista0 = new List<ServiceReference1.Point>();
    List<ServiceReference1.Point> lista1 = new List<ServiceReference1.Point>();
    int nr_listy0 = m;
    int nr_listy1 = n;
    for (int i = 0; i < x.RegionsWithStrokesList[nr_listy0].Strokes.Length; i++)
    {
        int j = 0;
        while (j < x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList.Length)
        {
            lista0.Add(x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList[j]);
            j++;
        }
    }
    for (int i = 0; i < x.RegionsWithStrokesList[nr_listy1].Strokes.Length; i++)
    {
        int j = 0;
        while (j < x.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList.Length)
        {
            lista1.Add(x.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList[j]);
            j++;
        }
    }
    double Waga = DTWDistance(lista0, lista1);
    Console.WriteLine("\nOkno {0} i {1} fejk1", m, n);
    Console.WriteLine("Waga niepodobienstwa to : {0}", Waga);
    int punkty_lista0 = 0;
    for (int i = 0; i < x.RegionsWithStrokesList[nr_listy0].Strokes.Length; i++)
    {
        punkty_lista0 = punkty_lista0 + x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList.Length;
    }
    int punkty_lista1 = 0;
    for (int i = 0; i < x.RegionsWithStrokesList[nr_listy1].Strokes.Length; i++)
    {
        punkty_lista1 = punkty_lista1 + x.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList.Length;
    }
    double Waga_ostateczna = Waga / (punkty_lista0 + punkty_lista1);
    Console.WriteLine("Waga ostateczna to {0}", Waga_ostateczna);
    wagi_ostatecznelista_fake1.Add(Waga_ostateczna);
}

}
for (int i = 0; i < wagi_ostatecznelista_fake1.Count; i++)
{
if (wagi_ostatecznelista_fake1[i] < minimalny_fejk1 && wagi_ostatecznelista_fake1[i] > 0)
{
    minimalny_fejk1 = wagi_ostatecznelista_fake1[i];
    indeks_minimalny_fejk1 = i;
}              
}
//-----------------------------------------------------------------------

for (int m = 0; m < 9; m++)
{
for (int n = 0; n < 9; n++)
{
    List<ServiceReference1.Point> lista0 = new List<ServiceReference1.Point>();
    List<ServiceReference1.Point> lista1 = new List<ServiceReference1.Point>();
    int nr_listy0 = m;
    int nr_listy1 = n;
    for (int i = 0; i < x.RegionsWithStrokesList[nr_listy0].Strokes.Length; i++)
    {
        int j = 0;
        while (j < x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList.Length)
        {
            lista0.Add(x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList[j]);
            j++;
        }
    }
    for (int i = 0; i < y.RegionsWithStrokesList[nr_listy1].Strokes.Length; i++)
    {
        int j = 0;
        while (j < y.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList.Length)
        {
            lista1.Add(y.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList[j]);
            j++;
        }
    }
    double Waga = DTWDistance(lista0, lista1);
    Console.WriteLine("\nOkno {0} i {1} fejk2", m, n);
    Console.WriteLine("Waga niepodobienstwa to : {0}", Waga);
    int punkty_lista0 = 0;
    for (int i = 0; i < x.RegionsWithStrokesList[nr_listy0].Strokes.Length; i++)
    {
        punkty_lista0 = punkty_lista0 + x.RegionsWithStrokesList[nr_listy0].Strokes[i].inquiryPoints.pointsList.Length;
    }
    int punkty_lista1 = 0;
    for (int i = 0; i < y.RegionsWithStrokesList[nr_listy1].Strokes.Length; i++)
    {
        punkty_lista1 = punkty_lista1 + y.RegionsWithStrokesList[nr_listy1].Strokes[i].inquiryPoints.pointsList.Length;
    }
    double Waga_ostateczna = Waga / (punkty_lista0 + punkty_lista1);
    Console.WriteLine("Waga ostateczna to {0}", Waga_ostateczna);
    wagi_ostatecznelista_fake2.Add(Waga_ostateczna);

    for (int i = 0; i < wagi_ostatecznelista_fake2.Count; i++)
    {
        if (wagi_ostatecznelista_fake2[i] < minimalny_fejk2 && wagi_ostatecznelista_fake2[i] > 0)
        {
            minimalny_fejk2 = wagi_ostatecznelista_fake2[i];
            indeks_minimalny_fejk2 = i;
        }
    }
}
}
Console.WriteLine("\nPorownywanie podpisow z 1 fejka");
Console.WriteLine("Najmniejsza waga ostateczna to: {0}", minimalny_fejk1);
Console.WriteLine("Indeks listy najmniejszego elementu to: {0}", indeks_minimalny_fejk1);
int pierwsze_okno_fejk1 = indeks_minimalny_fejk1 / 9;
int drugie_okno_fejk1 = indeks_minimalny_fejk1 % 9;
Console.WriteLine("Najbardziej podobne jest okno {0} i {1}", pierwsze_okno_fejk1, drugie_okno_fejk1);

Console.WriteLine("\nPorownywanie podpisow miedzy 1 a 2 fejkiem");
Console.WriteLine("Najmniejsza waga ostateczna to: {0}", minimalny_fejk2);
Console.WriteLine("Indeks listy najmniejszego elementu to: {0}", indeks_minimalny_fejk2);
int pierwsze_okno_fejk2 = indeks_minimalny_fejk2 / 9;
int drugie_okno_fejk2 = indeks_minimalny_fejk2 % 9;
Console.WriteLine("Najbardziej podobne jest okno {0} i {1}", pierwsze_okno_fejk2, drugie_okno_fejk2);

*/

        //---------------------------------------------------------------------
        /*
        List<ServiceReference1.Point> punkty = new List<ServiceReference1.Point>();
        for (int i = 0; i < x.RegionsWithStrokesList[0].Strokes.Length; i++)
        {
            for (int j = 0; i < x.RegionsWithStrokesList[0].Strokes[i].inquiryPoints.pointsList.Length; j++)
            {
                punkty.AddRange(x.RegionsWithStrokesList[0].Strokes[i].inquiryPoints.pointsList.ToList());
            }
        }           
        */






    }
}

