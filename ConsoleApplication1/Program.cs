using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class compV
    {
        static Random rnd = new Random();
        int _firewall;
        int firewall//Вероятность излечения/защиты от бот-атаки
        {
            get
            {
                return _firewall;
            }
            set
            {
                if (value >= 0 && value < 100)
                    _firewall = value;
            }
        }
        public bool status = false;//Состояние зараженности true - заражен
        public ushort ID;//произвольное 
        List<compV> lst_neighbor;
        public int countNeighbor
        {
            get
            {
                return lst_neighbor.Count;
            }
        }
        public compV()//Конструктор по итогам создаст несвязную вершину
        {
            firewall = 0;
            Random r = new Random();
            ID = (ushort)r.Next();
            while (used_id.Contains(ID))
                ID = (ushort)r.Next();
            used_id.Add(ID);
            all_vertex.Add(this);
            lst_neighbor = new List<compV>();
        }
        public compV(ushort tID)//Конструктор по итогам создаст несвязную вершину c заданным ID
        {
            firewall = 90;
            if (!used_id.Contains(tID))//Если такой ID 
            {
                ID = tID;
                used_id.Add(tID);
                all_vertex.Add(this);
                lst_neighbor = new List<compV>();
            }
        }
        public compV(ushort tID, int fw)//Конструктор по итогам создаст несвязную вершину c заданным ID и firewall
        {
            firewall = fw;
            if (!used_id.Contains(tID))//Если такой ID 
            {
                ID = tID;
                used_id.Add(tID);
                all_vertex.Add(this);
                lst_neighbor = new List<compV>();
            }
        }
        public void AddEdge(compV vertex)
        {
            if (!lst_neighbor.Contains(vertex))
            {
                lst_neighbor.Add(vertex);
                vertex.lst_neighbor.Add(this);
            }
        }
        public void AddEdge(int ID)
        {
            foreach (compV a in all_vertex)
            {
                if (a.ID == ID)
                {
                    lst_neighbor.Add(a);
                    a.lst_neighbor.Add(this);
                    return;
                }
            }
        }
        public void remove_vertex()
        {
            foreach (var a in lst_neighbor)
            {
                a.lst_neighbor.Remove(this);
            }
            all_vertex.Remove(this);
            used_id.Remove(ID);
        }
        public List<compV> attack()
        {
            List<compV> l = new List<compV>();
            foreach(var a in lst_neighbor)
            {
                int r = rnd.Next(0, 101);//[0,100]
                if (!a.status && r>a.firewall)
                {
                    l.Add(a);
                }
            }
            return l;
        }        
        
        //BFS
        public List<compV> get_Componenta() //найти компоненту связности для текущей
        {
            List<compV> lst = new List<compV>(); //те вершины которые будут искать соседей
            List<compV> used = new List<compV>(); //использованные вершины
            lst.Add(this);
            while (lst.Count != 0)
            {
                compV t = lst[0];
                used.Add(t);
                lst.Remove(t);
                foreach (var a in t.lst_neighbor)
                    if (!used.Contains(a) && !lst.Contains(a))
                        lst.Add(a);
            }
            return used;
        }

        //static:
        public static void remove_Componenta(List<compV> cmp)
        {
            foreach (var vert in cmp)
            {
                vert.remove_vertex();
            }
        }
        public static compV get_vertex(ushort tID)
        {
            foreach (var a in all_vertex)
            {
                if (a.ID == tID)
                    return a;
            }
            return null;
        }
        public static void AddEdge_byID(ushort ID1, ushort ID2)
        {
            if (ID1 != ID2)
            {
                compV v1 = get_vertex(ID1);
                compV v2 = get_vertex(ID2);
                v1.AddEdge(v2);
            }
        }

        public static bool read_graph(string path)
        {
            //число вершин N
            //ID вершин N штук
            //число ребер M
            //Ребра парами a b
            //5                 0 вершин
            //1 2 5 7 9         2 ребра
            //6                 1 5
            //1 2               7 20
            //1 5
            //2 9
            //2 7
            //5 9
            //9 7
            if (!File.Exists(path))
            {
                //Если файла нет;
                return false;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                string str = sr.ReadLine();
                string[] str2;
                ushort[] X=null;
                ushort[][] Y=null;
                int n, m;
                n = int.Parse(str);
                if (n > 0)//Значит хотим добавить вершин
                {
                    str = sr.ReadLine();
                    str2 = str.Split(' ');
                    if (str2.Length != n)
                        throw new IndexOutOfRangeException();
                    X = new ushort[n];
                    for (int i = 0; i < n; i++)
                        X[i] = ushort.Parse(str2[i]);
                    for (int i = 0; i < n; i++)
                    {
                        if (get_vertex(X[i]) != null)
                            return false;
                    }
                }
                str = sr.ReadLine();
                m = int.Parse(str);
                if (m > 0)
                {
                    Y = new ushort[2][];
                    Y[0] = new ushort[m];
                    Y[1] = new ushort[m];
                    for (int i = 0; i < m; i++)
                    {
                        str = sr.ReadLine();
                        str2 = str.Split(' ');
                        if (str2.Length != 2)
                            throw new IndexOutOfRangeException();
                        Y[0][i] = ushort.Parse(str2[0]);
                        Y[1][i] = ushort.Parse(str2[1]);
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    new compV(X[i]);
                }
                for (int i = 0; i < m; i++)
                {
                    AddEdge_byID(Y[0][i], Y[1][i]);
                }
            }
            return true;
        }

        public static List<compV> get_Componenta(compV v) //найти компоненту связности для произвольной вершины
        {
            List<compV> lst = new List<compV>(); //те вершины которые будут искать соседей
            List<compV> used = new List<compV>(); //использованные вершины
            lst.Add(v);
            while (lst.Count != 0)
            {
                compV t = lst[0];
                used.Add(t);
                lst.Remove(t);
                foreach (var a in t.lst_neighbor)
                    if (!used.Contains(a) && !lst.Contains(a))
                        lst.Add(a);
            }
            return used;
        }
        public static List<ushort> used_id = new List<ushort>();//Поле хранит все идентификаторы 
        public static List<compV> all_vertex = new List<compV>();//Список всех вершин графа
    }



    class Program
    {
        static void Main(string[] args)
        {
            compV.read_graph("f.txt");
            for(ushort i=0;i<100;i++)
            {
                compV t = new compV(i);
            }
            Random q = new Random();
            ushort id1, id2;
            //TODO:Почитать закономерность Ребра-вершины            
            for(int i = 0;i<500;i++)
            {
                id1 = (ushort)q.Next(0, 100);
                id2 = (ushort)q.Next(0, 100);
                compV.AddEdge_byID(id1, id2);
            }
            compV start = null;
            List<List<compV>> cmp_lst = new List<List<compV>>();//список компонент связности на перспективу(?)
            while (true)//Пои тогам получим связный граф с одной компонентой связности
            {
                start = compV.all_vertex[0];
                List<compV> componenta = start.get_Componenta();
                if(componenta.Count!=compV.all_vertex.Count)
                {
                    //случай когда есть несколько компонент связности
                    if (componenta.Count >compV.all_vertex.Count/2)
                    {
                        for(int i=0;i< compV.all_vertex.Count;i++)
                        {
                            if(!componenta.Contains(compV.all_vertex[i]))
                            {
                                compV.all_vertex[i].remove_vertex();
                                i--;
                            }
                        }
                    }
                    else
                    {
                        compV.remove_Componenta(componenta);
                    }
                }
                else                    
                    break;//все вершины связанны между собой;
            }

            Console.WriteLine(compV.all_vertex.Count);
            start = compV.all_vertex[0];
            List<compV> botnet = new List<compV>();
            //List<compV> new_bot = new List<compV>();
            botnet.Add(start);
            //new_bot.Add(start);
            start.status = true;
            int step = 1;
            int count=1;
            Console.WriteLine("Start vertex = "+start.ID);
            while (botnet.Count != compV.all_vertex.Count)
            {
                Console.WriteLine("Step " + step + " botnet count = " + botnet.Count + " new= " + count);
                count = 0;
                int c = botnet.Count;
                for (int i = 0; i < c; i++)
                {
                    List<compV> atLst = botnet[i].attack();
                    foreach (var j in atLst)
                    {
                        Console.Write(botnet[i].ID + " -> " + j.ID + " ");
                        j.status = true;
                        botnet.Add(j);
                        count++;
                    }
                }
                step++;
                Console.WriteLine();
            }



            /* Среднее количество ребер
            float sr = 0;
            foreach (var a in compV.all_vertex)
            {
                sr += a.countEdge;
            }
            sr /= 100;
            Console.WriteLine(sr);
            */

            Console.ReadKey();
        }
    }
}
