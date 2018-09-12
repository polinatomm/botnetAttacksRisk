using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Курсовая
{
    /// <summary>
    /// Логика взаимодействия для GraphEditor.xaml
    /// </summary>
    public partial class GraphEditor : Window
    {
        public GraphEditor()
        {
            InitializeComponent();
            refresh();
        }
        public void refresh()
        {
            lb_vertex.Items.Clear();
            for (int i = 0; i < compV.used_id.Count; i++)
            {
                lb_vertex.Items.Add(compV.used_id[i]);
            }
        }

        private void Button_Click_addvertex(object sender, RoutedEventArgs e)
        {
            int t;
            if(int.TryParse(tb_vertexID.Text,out t))
            {
                if (compV.used_id.Contains(t))
                    MessageBox.Show("Данная вершина уже есть в графе", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    new compV(t);
                    refresh();
                }
            }
        }

        public void update_neighbor()
        {
            int sel = lb_vertex.SelectedIndex;
            if (sel != -1)
            {
                compV sV = compV.all_vertex[sel];
                tb_firewall.Text = sV.firewall.ToString();
                cb_neigbor.Items.Clear();
                lb_neigbor.Items.Clear();
                foreach (var a in compV.all_vertex)
                {
                    if (a != sV && !sV.isNeighbor(a))
                        cb_neigbor.Items.Add(a.ID);
                    else
                        if (a != sV)
                        lb_neigbor.Items.Add(a.ID);
                }
                if (cb_neigbor.Items.Count != 0)
                    cb_neigbor.SelectedIndex = 0;
            }
        }

        private void lb_vertex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            update_neighbor();
        }

        private void Button_Click_addneighbor(object sender, RoutedEventArgs e)
        {
            int sel = lb_vertex.SelectedIndex;
            if (sel != -1 && cb_neigbor.Items.Count>0)
            {
                compV sV = compV.all_vertex[sel];
                int id2 = int.Parse(cb_neigbor.SelectedItem.ToString());
                compV.AddEdge_byID(sV.ID, id2);
                update_neighbor();
            }
        }

        private void Button_Click_deleteneighbor(object sender, RoutedEventArgs e)
        {
            int sel = lb_vertex.SelectedIndex;
            if (sel != -1 &&  lb_neigbor.SelectedIndex!=-1)
            {
                compV sV = compV.all_vertex[sel];
                int id2 = int.Parse(lb_neigbor.SelectedItem.ToString());
                compV.remove_Edge(sV.ID, id2);
                update_neighbor();
            }
        }

        private void Button_Click_madeAndAdd(object sender, RoutedEventArgs e)
        {
            int start = -1;
            int end = -1;
            int c_edges = -1;
            int firewall = -1;
            try
            {
                start = int.Parse(tb_start.Text);
                end = int.Parse(tb_end.Text);
                c_edges = int.Parse(tb_edges.Text);
                firewall = int.Parse(tb_generation_fw.Text);
            }
            catch
            {
                MessageBox.Show("Некорректно заданы поля", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            for(int i = start; i <= end; i++)
            {
                if (compV.used_id.Contains(i))
                {
                    MessageBox.Show("Вершины из этого диапазона уже есть в графе", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            for (int i = start; i <= end; i++)            
                new compV(i,firewall);
            Random q = new Random();
            for (int i = 0; i < c_edges; i++)
            {
                int id1 = (int)q.Next(start, end+1);
                int id2 = (int)q.Next(start, end+1);
                compV.AddEdge_byID(id1, id2);
            }
            refresh();
            update_neighbor();
        }

        private void Button_Click_checkTwoID(object sender, RoutedEventArgs e)
        {
            int id1 = -1;
            int id2 = -1;
            try
            {
                id1 = int.Parse(tb_id1.Text);
                id2 = int.Parse(tb_id2.Text);
                if (id1 == id2 || !compV.used_id.Contains(id1) || !compV.used_id.Contains(id2))
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Некорректно заданы поля", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            compV st = compV.get_vertex(id1);
            List<compV> componenta = st.get_Componenta();
            foreach(var a in componenta)
                if(id2 == a.ID)
                {
                    MessageBox.Show("Вершины связанны между собой", "Результат работы", MessageBoxButton.OK);
                    return;
                }
            MessageBox.Show("Вершины не связанны между собой", "Результат работы", MessageBoxButton.OK);
        }

        private void Button_Click_countComp(object sender, RoutedEventArgs e)
        {
            int id1 = -1;
            try
            {
                id1 = int.Parse(tb_find_id.Text);
                if ( !compV.used_id.Contains(id1) )
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Некорректно заданы поля", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            compV st = compV.get_vertex(id1);
            List<compV> componenta = st.get_Componenta();
            tb_count_comp.Text = componenta.Count.ToString();
        }

        private void Button_Click_deleteOtherComp(object sender, RoutedEventArgs e)
        {
            int id1 = -1;
            try
            {
                id1 = int.Parse(tb_find_id.Text);
                if (!compV.used_id.Contains(id1))
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Некорректно заданы поля");
                return;
            }
            compV st = compV.get_vertex(id1);
            List<compV> componenta = st.get_Componenta();
            for (int i = 0; i < compV.all_vertex.Count; i++)
            {
                if (!componenta.Contains(compV.all_vertex[i]))
                {
                    compV.all_vertex[i].remove_vertex();
                    i--;
                }
            }
            refresh();
            update_neighbor();
        }

        private void Button_Click_deletevertex(object sender, RoutedEventArgs e)
        {
            int sel = lb_vertex.SelectedIndex;
            if (sel != -1)
            {
                compV sV = compV.all_vertex[sel];
                sV.remove_vertex();
            }
            refresh();
            update_neighbor();
        }

        private void Button_Click_firewall(object sender, RoutedEventArgs e)
        {
            try
            {
                int sel = lb_vertex.SelectedIndex;
                if (sel != -1)
                {
                    compV.all_vertex[sel].firewall = int.Parse(tb_firewall.Text);
                }
            }
            catch
            {
                MessageBox.Show("Некорректное значение");
            }

        }
    }
}
