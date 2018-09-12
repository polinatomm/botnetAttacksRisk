using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace Курсовая
{
    public class compV
    {
        static Random rnd = new Random();
        private int _firewall = 0;//Значение firewall
        public int firewall//Вероятность излечения/защиты от бот-атаки
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
        public int ID;//произвольное 

        public compV()//Конструктор по итогам создаст несвязную вершину
        {
            firewall = 0;
            Random r = new Random();
            ID = r.Next();
            while (used_id.Contains(ID))
                ID = r.Next();
            used_id.Add(ID);
            all_vertex.Add(this);
            lst_neighbor = new List<compV>();
        }
        public compV(int tID)//Конструктор по итогам создаст несвязную вершину c заданным ID
        {
            firewall = 0;
            if (!used_id.Contains(tID))//Если такой ID 
            {
                ID = tID;
                used_id.Add(tID);
                all_vertex.Add(this);
                lst_neighbor = new List<compV>();
            }
        }
        public compV(int tID, int fw)//Конструктор по итогам создаст несвязную вершину c заданным ID и firewall
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


        List<compV> lst_neighbor;
        public int countNeighbor//посчитать соседей
        {
            get
            {
                return lst_neighbor.Count;
            }
        }
        public bool isNeighbor(int ID1)//является ли соседом через ID
        {
            compV t = get_vertex(ID1);
            if (lst_neighbor.Contains(t))
                return true;
            else
                return false;
        }
        public bool isNeighbor(compV t)//является ли соседом через вершину
        {
            if (lst_neighbor.Contains(t))
                return true;
            else
                return false;
        }
        public void AddEdge(compV vertex)//добавить ребро
        {
            if (!lst_neighbor.Contains(vertex))
            {
                lst_neighbor.Add(vertex);
                vertex.lst_neighbor.Add(this);
            }
        }
        public void AddEdge(int ID)//добавить ребро через ID
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
        public void remove_vertex()//удалить вершину
        {
            foreach (var a in lst_neighbor)
            {
                a.lst_neighbor.Remove(this);
            }
            all_vertex.Remove(this);
            used_id.Remove(ID);
        }
        public List<compV> attack()//атака
        {
            List<compV> l = new List<compV>();
            foreach (var a in lst_neighbor)
            {
                if (!a.status)
                {
                    if (rnd.Next(0, 101) > a.firewall)
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
        public static void remove_Componenta(List<compV> cmp)//удалить компоненту
        {
            foreach (var vert in cmp)
            {
                vert.remove_vertex();
            }
        }
        public static compV get_vertex(int tID)//возвращение вершины с заданным ID
        {
            foreach (var a in all_vertex)
            {
                if (a.ID == tID)
                    return a;
            }
            return null;
        }
        public static void AddEdge_byID(int ID1, int ID2)//добавление ребра при помощи двух ID
        {
            if (ID1 != ID2)
            {
                compV v1 = get_vertex(ID1);
                compV v2 = get_vertex(ID2);
                v1.AddEdge(v2);
            }
        }
        public static void remove_Edge(int ID1, int ID2)//удаление ребра при помощи двух ID
        {
            compV v1 = get_vertex(ID1);
            compV v2 = get_vertex(ID2);
            remove_Edge(v1, v2);
        }
        public static void remove_Edge(compV v1, compV v2)//удаление ребра при помощи двух вершин
        {
            v1.lst_neighbor.Remove(v2);
            v2.lst_neighbor.Remove(v1);
        }
        public static int count_Edges()//вычисление количества ребер
        {
            int count = 0;
            for (int i = 0; i < all_vertex.Count; i++)
            {
                count += all_vertex[i].countNeighbor;
            }
            return count;
        }
        public static void refreshGraph()//перезапись графа как незараженного
        {
            foreach( compV a in all_vertex)
            {
                a.status = false;
            }
        }
        public static void Clear()//очищение
        {
            all_vertex.Clear();
            used_id.Clear();
        }
        public static bool read_graph(string path)//чтение графа из текстового файла
        {
            //число вершин N
            //ID вершин N штук
            //число ребер M
            //Ребра парами a b
            //5                 
            //1_98 2_23 5_99 7_70 9_90         Номер вершины_значение firewall
            //6                
            //1 2              
            //1 5
            //2 9
            //2 7
            //5 9
            //9 7
            if (!File.Exists(path))
            {
                //Если файла нет;
                MessageBox.Show("Файлаа не существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                try
                {
                    string str = sr.ReadLine();
                    string[] str2;
                    int[][] X = new int[2][];
                    int[][] Y = null;
                    int n, m;
                    n = int.Parse(str);
                    if (n > 0)//Значит хотим добавить вершин
                    {
                        str = sr.ReadLine();
                        str = str.Trim();
                        str2 = str.Split(' ', '_');
                        if (str2.Length != n * 2)
                            throw new IndexOutOfRangeException();
                        X[0] = new int[n];
                        X[1] = new int[n];
                        for (int i = 0; i < n; i++)
                        {
                            X[0][i] = int.Parse(str2[i * 2]);
                            X[1][i] = int.Parse(str2[i * 2 + 1]);
                        }
                        for (int i = 0; i < n; i++)
                        {
                            if (get_vertex(X[0][i]) != null)
                                return false;
                        }
                    }
                    str = sr.ReadLine();
                    m = int.Parse(str);
                    if (m > 0)
                    {
                        Y = new int[2][];
                        Y[0] = new int[m];
                        Y[1] = new int[m];
                        for (int i = 0; i < m; i++)
                        {
                            str = sr.ReadLine();
                            str2 = str.Split(' ');
                            if (str2.Length != 2)
                                throw new IndexOutOfRangeException();
                            Y[0][i] = int.Parse(str2[0]);
                            Y[1][i] = int.Parse(str2[1]);
                        }
                    }

                    for (int i = 0; i < n; i++)
                    {
                        new compV(X[0][i], X[1][i]);
                    }
                    for (int i = 0; i < m; i++)
                    {
                        AddEdge_byID(Y[0][i], Y[1][i]);
                    }
                }
                catch
                {
                    MessageBox.Show("Проблемы с файлом", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return true;
        }
        public static void save_graph(string path)//сохрание графа в файл
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                try
                {
                    //число вершин N
                    //ID вершин N штук
                    //число ребер M
                    //Ребра парами a b
                    sw.WriteLine(used_id.Count);

                    for (int i = 0; i < all_vertex.Count; i++)
                    {
                        sw.Write(all_vertex[i].ID + "_" + all_vertex[i].firewall + " ");
                    }
                    if (used_id.Count != 0)
                        sw.WriteLine("\r\n" + count_Edges());
                    else
                        sw.WriteLine(count_Edges());
                    for (int i = 0; i < all_vertex.Count; i++)
                    {
                        foreach (var a in all_vertex[i].lst_neighbor)
                        {
                            sw.WriteLine(all_vertex[i].ID + " " + a.ID);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Проблемы с файлом", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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

        public static List<int> used_id = new List<int>();//Поле хранит все идентификаторы 
        public static List<compV> all_vertex = new List<compV>();//Список всех вершин графа
    }

        /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //layout.insertvertex("1");
        //    //layout.insertvertex("2");
        //    //layout.insertedge("1","2");
        //    compV.Clear();
        //    log_box.Clear();
        //    int countV=0;
        //    int countE = 0;
        //    try
        //    {
        //        countV = int.Parse(tb_countVertex.Text);
        //        countE = int.Parse(tb_countEdges.Text);
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }
        //    //compV.read_graph("f.txt");
        //    //Генерируем 100 вершин
        //    for (int i = 0; i < countV; i++)
        //    {
        //        compV t = new compV(i);
        //    }
        //    Random q = new Random();
        //    int id1, id2;
        //    //TODO:Почитать закономерность Ребра-вершины            
        //    for (int i = 0; i < countE; i++)
        //    {
        //        id1 = (int)q.Next(0, countV);
        //        id2 = (int)q.Next(0, countV);
        //        compV.AddEdge_byID(id1, id2);
        //    }
        //    compV start = null;

        //    #region Шаманство с компонентой
        //    List<List<compV>> cmp_lst = new List<List<compV>>();//список компонент связности на перспективу(?)
        //    while (true)//Пои тогам получим связный граф с одной компонентой связности
        //    {
        //        start = compV.all_vertex[0];
        //        List<compV> componenta = start.get_Componenta();
        //        if (componenta.Count != compV.all_vertex.Count)
        //        {
        //            //случай когда есть несколько компонент связности
        //            if (componenta.Count > compV.all_vertex.Count / 2)
        //            {
        //                for (int i = 0; i < compV.all_vertex.Count; i++)
        //                {
        //                    if (!componenta.Contains(compV.all_vertex[i]))
        //                    {
        //                        compV.all_vertex[i].remove_vertex();
        //                        i--;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                compV.remove_Componenta(componenta);
        //            }
        //        }
        //        else
        //            break;//все вершины связанны между собой;
        //    }
        //    #endregion

        //    log_box.Text+="Количество вершин" + compV.all_vertex.Count+"\r\n";
        //    start = compV.all_vertex[0];
        //    List<compV> botnet = new List<compV>();
        //    //List<compV> new_bot = new List<compV>();
        //    botnet.Add(start);
        //    //new_bot.Add(start);
        //    start.status = true;
        //    int step = 1;
        //    int count = 1;
        //    log_box.Text+="Start vertex = " + start.ID + "\r\n";
        //    //TODO:Количество шагов задать
        //    while (botnet.Count != compV.all_vertex.Count && step<1000)
        //    {
        //        log_box.Text+="Step " + step +":\r\n";
        //        count = 0;
        //        int c = botnet.Count;
        //        for (int i = 0; i < c; i++)
        //        {
        //            List<compV> atLst = botnet[i].attack();
        //            foreach (var j in atLst)
        //            {
        //                log_box.Text += (botnet[i].ID + " -> " + j.ID + " ");
        //                j.status = true;
        //                botnet.Add(j);
        //                count++;
        //            }
        //        }
        //        step++;
        //        if(count !=0)
        //            log_box.Text +="\r\nbotnet count = " + botnet.Count + " new= " + count + "\r\n";
        //    }
        //}

        private void Button_Click_save_graph(object sender, RoutedEventArgs e)
        {
            string filepath;
            OpenFileDialog opn = new OpenFileDialog();
            Nullable<bool> result = opn.ShowDialog();
            if (result == true)
            {
                filepath = opn.FileName;
                compV.save_graph(filepath);
                return;
            }
        }

        private void Button_Click_read_graph(object sender, RoutedEventArgs e)
        {
            string filepath;
            OpenFileDialog opn = new OpenFileDialog();
            Nullable<bool> result = opn.ShowDialog();
            if (result == true)
            {
                filepath = opn.FileName;
                compV.read_graph(filepath);
                tb_count_edges.Text = "Количество вершин = " + compV.all_vertex.Count;
                cb_start_vertex.Items.Clear();
                foreach (var a in compV.used_id)
                    cb_start_vertex.Items.Add(a);
                if (cb_start_vertex.Items.Count != 0)
                    cb_start_vertex.SelectedIndex = 0;
                return;
            }
            //compV.read_graph("graph.txt");
            //tb_count_edges.Text = "Количество вершин = " + compV.all_vertex.Count;
            //cb_start_vertex.Items.Clear();
            //foreach (var a in compV.used_id)
            //    cb_start_vertex.Items.Add(a);
            //if (cb_start_vertex.Items.Count != 0)
            //    cb_start_vertex.SelectedIndex = 0;
        }

        private void Button_Click_start(object sender, RoutedEventArgs e)
        {
            if (cb_start_vertex.SelectedIndex == -1)
                return;
            log_box.Clear(); 
            compV start = compV.get_vertex(int.Parse(cb_start_vertex.SelectedItem.ToString()));
            List<compV> componenta = start.get_Componenta();
            if (componenta.Count != compV.all_vertex.Count)
            {
                MessageBox.Show("Граф не связен.","Внимание!");
                return;
            }
            int count_algs = 0;
            int max_count_step = 0;
            try
            {
                count_algs = int.Parse(tb_count_algs.Text);
                max_count_step = int.Parse(tb_count_step.Text);
                if (count_algs < 0 || max_count_step < 0)
                    MessageBox.Show("Некорретно задано поле.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("Некорретно задано поле.", "Ошибка!",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            List<compV> botnet = new List<compV>();
            int total_step=0;
            int botnet_fail = 0;

            for (int k = 0; k < count_algs; k++)
            {
                compV.refreshGraph();
                botnet.Clear();
                botnet.Add(start);
                start.status = true;
                int step = 1;
                int count = 1;
                log_box.Text += (k + 1) + "  PASS" + "\r\n\r\n";
                log_box.Text += "Start vertex = " + start.ID + "\r\n";
                while (botnet.Count != compV.all_vertex.Count && max_count_step>step)
                {
                    log_box.Text += "Step " + step + ":\r\n";
                    count = 0;
                    int c = botnet.Count;
                    for (int i = 0; i < c; i++)
                    {
                        List<compV> atLst = botnet[i].attack();
                        foreach (var j in atLst)
                        {
                            log_box.Text += (botnet[i].ID + " -> " + j.ID + " ");
                            j.status = true;
                            botnet.Add(j);
                            count++;
                        }
                    }
                    if (count != 0)
                        log_box.Text += "\r\nbotnet count = " + botnet.Count + " new= " + count + "\r\n";
                    step++;
                }
                log_box.Text += "\r\n\r\n________________________________________________________________\r\n\r\n";
                if (max_count_step == step)
                    botnet_fail++;
                total_step += step;
            }
            tb_botnet_fail.Text = "Успешных защит = " + botnet_fail;
            tb_sred_step.Text = "Среднее количество шагов = " + (float)total_step / count_algs;
            cb_numAlg.Items.Clear();
            for (int i = 0; i < count_algs; ++i)
                cb_numAlg.Items.Add(i+1);
            if (cb_start_vertex.Items.Count != 0)
                cb_numAlg.SelectedIndex = 0;
        }

        private void MC_redact(object sender, RoutedEventArgs e)
        {
            GraphEditor f = new GraphEditor();
            f.ShowDialog();
            tb_count_edges.Text = "Количество вершин = " + compV.all_vertex.Count;
            cb_start_vertex.Items.Clear();
            foreach (var a in compV.used_id)
                cb_start_vertex.Items.Add(a);
            if (cb_start_vertex.Items.Count != 0)
                cb_start_vertex.SelectedIndex = 0;
        }

        private void MC_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_NumAlg(object sender, RoutedEventArgs e)
        {
            int count_algs = int.Parse(tb_count_algs.Text);
            string[] strArr = new string[count_algs];
            strArr = log_box.Text.Split("________________________________________________________________".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (cb_numAlg.SelectedIndex == -1)
                return;
            int per = int.Parse(cb_numAlg.Text);
            log_box_small.Text = strArr[per - 1];
        }

        private void Button_Click_AboutProg(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Полное наименование программы – «Программа оценки риска бот-атаки».\r\n\r\nПриказ НИУ ВШЭ №2.3-02/0812-01 от 08.12.2016. Программа выполнена в рамках темы курсовой работы «Программа «Оценка риска бот-атаки»» (факультет компьютерных наук, отделение программной инженерии), в соответствии с учебным планом подготовки бакалавров по направлению 09.03.04 ”Программная инженерия”.\r\n\r\nРазработчик - Томм Полина", "О программе");
        }
    }
}
