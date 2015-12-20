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
                    //upychanie do listy indeksow najkrotszej sciezki
                    //Musimy isc od konca zeby znalezc sciezke
                }
            }
            
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
                /*
            for (i = s.Count - 1; i > 1; i--)
            {
                for (j = t.Count - 1 ; j > 1; j--)
                {
                    if ((DTW[i - 1, j]) == Math.Min(DTW[i - 1, j], Math.Min(DTW[i, j - 1], DTW[i - 1, j - 1])))
                    {
                        sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", i - 1, j));
                    }
                    else if (DTW[i, j - 1] == Math.Min(DTW[i - 1, j], Math.Min(DTW[i, j - 1], DTW[i - 1, j - 1])))
                    {
                        sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", i, j - 1));
                    }
                    else
                    {
                        sciezka_najkrotsza.Add(string.Format("[{0}, {1}]", i - 1, j - 1));
                    }
                }
            }*/
            
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
            /*
            var x = client.GetSignatures("as21614@st.amu.edu.pl", "0c429600f40074205c023f3a876fe2a6", 8510);
            var y = client.GetSignatures("as21614@st.amu.edu.pl", "0c429600f40074205c023f3a876fe2a6", 8510);
           */
            /* RegionsWithStrokesList[0] to podpis w pierwszym oknie
               Strokes[0] to kreska pierwsza w oknie
               inquiryPoints.pointsList[0] to pierwszy punkt w kresce
            */

            /*-----------------------------------------------------------------
            int ilosc_lini = x.RegionsWithStrokesList[0].Strokes.Length;
            int ilosc_punktow_linia = x.RegionsWithStrokesList[0].Strokes[0].inquiryPoints.pointsList.Length;
            Console.WriteLine("Ilosc lini: {0}", ilosc_lini);
            Console.WriteLine("Ilosc punktow: {0}\n", ilosc_punktow_linia);


            for (int i = 0; i < x.RegionsWithStrokesList.Length; i++)
            {
                Console.WriteLine("Okno {0}", i);
                Console.WriteLine("Ilosc lini w oknie {0} wynosi {1}", i, x.RegionsWithStrokesList[i].Strokes.Length);
                for (int j = 0; j < x.RegionsWithStrokesList[i].Strokes.Length; j++)
                {
                    Console.WriteLine("Linia {0}, sklada sie z {1} punktow", j, x.RegionsWithStrokesList[i].Strokes[j].inquiryPoints.pointsList.Length);
                }
                Console.WriteLine("\n");
            }-----------------------------------------------------------------------------
            */
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
            

            // wybieramy dwa okna ktore porownujemy

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
            Console.WriteLine(lista0.Count);
            Console.WriteLine(lista1.Count);
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
            Console.WriteLine("\nKONIEC");
            Console.ReadKey();
        }
            
            //liczenie ilosci punktow dla listy0 i listy1
            /*
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
            */
            //wyswietlanie ilosci punktow w listach0 i 1
            /*
            Console.WriteLine("\nLista0 ma {0} punktow. Lista1 ma {1} punktow.", punkty_lista0, punkty_lista1);
            */
            //waga ostateczna dla listy0 i listy1
            /*
            double Waga_ostateczna = Waga / (punkty_lista0 + punkty_lista1);
            Console.WriteLine("Waga ostateczna to {0}", Waga_ostateczna);
            */



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

