using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTask_2
{
    public partial class Form1 : Form
    {
        List<string> Adresses = new List<string>();
        int currentIndex = -1;
        string currentListViewAdress = "";

        bool stopsignal = false;

        int folderCount = 0;
        int fileCount = 0;

        int findfileCount = 0;
        int analysFilesCount = 0;


        int seconds = 0;

        DateTime t1, t2;

        public Form1()
        {
            InitializeComponent();

            

            //StreamReader fstream = new StreamReader("data.txt");
            //textBox2.Text = fstream.ReadLine();
            //currentListViewAdress = fstream.ReadLine();
            //fstream.Close();


            abort.Enabled = false;
            stopsearch.Enabled = false;

            listView1.View = View.Details;

            listView1.ColumnClick += new ColumnClickEventHandler(ClickOnColumn);
            ColumnHeader c = new ColumnHeader();
            c.Text = "Имя";
            c.Width = c.Width + 80;
            ColumnHeader c2 = new ColumnHeader();
            c2.Text = "Размер";
            c2.Width = c2.Width + 60;
            ColumnHeader c3 = new ColumnHeader();
            c3.Text = "Тип";
            c3.Width = c3.Width + 60;
            ColumnHeader c4 = new ColumnHeader();
            c4.Text = "Изменен";
            c4.Width = c4.Width + 60;
            listView1.Columns.Add(c);
            listView1.Columns.Add(c2);
            listView1.Columns.Add(c3);
            listView1.Columns.Add(c4);



            string[] str = Environment.GetLogicalDrives();
            int n = 1;
            foreach (string s in str)
            {
                try
                {
                    TreeNode tn = new TreeNode();
                    tn.Name = s;
                    tn.Text = "Диск " + s;
                    treeView1.Nodes.Add(tn.Name, tn.Text);
                    FileInfo f = new FileInfo(@s);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@s);
                    foreach (string s2 in str2)
                    {
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        treeView1.Nodes[n - 1].Nodes.Add(s2, t);
                    }
                }
                catch { }
                n++;
            }

            try
            {
                StreamReader fstream = new StreamReader("data.txt");
                textBox1.Text = fstream.ReadLine();
                currentListViewAdress = fstream.ReadLine();
                fstream.Close();

                FileInfo f = new FileInfo(currentListViewAdress);
                string[] str2 = Directory.GetDirectories(currentListViewAdress);
                string[] str3 = Directory.GetFiles(currentListViewAdress);

                DrawListView(f, str2, str3);
            }
            catch { 
            }

        }

        private void ClickOnColumn(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                if (listView1.Sorting == SortOrder.Descending)
                    listView1.Sorting = SortOrder.Ascending;
                else
                    listView1.Sorting = SortOrder.Descending;
            }
        }

        private void DrawListView(FileInfo f, string[] str2, string[] str3)
        {
            try
            {
                string t = "";

                ListViewItem lw = new ListViewItem();

                foreach (string s2 in str2)
                {
                    f = new FileInfo(@s2);
                    string type = "папка";
                    t = s2.Substring(s2.LastIndexOf('\\') + 1);
                    lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                    lw.Name = s2;
                    listView1.Items.Add(lw);
                    folderCount++;
                }

                foreach (string s3 in str3)
                {
                    f = new FileInfo(@s3);
                    string type = "файл";
                    t = s3.Substring(s3.LastIndexOf('\\') + 1);
                    lw = new ListViewItem(new string[] { t, f.Length.ToString() + "байт", type, f.LastWriteTime.ToString() }, 1);
                    lw.Name = s3;
                    listView1.Items.Add(lw);
                    fileCount++;
                }

            }
            catch { }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            folderCount = 0;
            fileCount = 0;

            string strtmp = "";
            if (Adresses.Count != 0)
            {
                strtmp = ((string)Adresses[Adresses.Count - 1]);
                Adresses.Clear();
                Adresses.Add(strtmp);
                currentIndex = 0;
            }
            Adresses.Add(e.Node.Name);
            currentIndex++;

            listView1.Items.Clear();
            currentListViewAdress = e.Node.Name;
            textBox1.Text = currentListViewAdress;

            label1.Text = currentListViewAdress;


            try
            {
                string[] str2 = Directory.GetDirectories(e.Node.Name);
                string[] str3 = Directory.GetFiles(@e.Node.Name);
                FileInfo f = new FileInfo(e.Node.Name);
                DrawListView(f, str2, str3);
            }
            catch { }

            label5.Text = "Кол-во папок: " + folderCount;
            label5.Text += "  Кол-во файлов: " + fileCount;

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems[0].Text.IndexOf('.') == -1)
            {
                folderCount = 0;
                fileCount = 0;

                Adresses.Add(listView1.SelectedItems[0].Name);
                currentIndex++;
                currentListViewAdress = ((string)Adresses[currentIndex]);
                currentListViewAdress = listView1.SelectedItems[0].Name;
                textBox1.Text = currentListViewAdress;
                
                try
                {
                    FileInfo f = new FileInfo(@listView1.SelectedItems[0].Name);

                    string[] str2 = Directory.GetDirectories(listView1.SelectedItems[0].Name);
                    string[] str3 = Directory.GetFiles(@listView1.SelectedItems[0].Name);
                    listView1.Items.Clear();

                    DrawListView(f, str2, str3);
                }
                catch { }


                label5.Text = "Кол-во папок: " + folderCount;
                label5.Text += "  Кол-во файлов: " + fileCount;
            }
            else
            {
                //обработка нажатия на файл(его запуска)
                System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                MyProc.StartInfo.FileName = @listView1.SelectedItems[0].Name;
                MyProc.Start();
            }
        }


        //public void searchProc(string adress, string name)
        //{
        //    Regex regex = new Regex(@"\w*" + name + @"\w*");
        //    try
        //    {
        //        string foldername = adress.Substring(adress.LastIndexOf('\\') + 1);
        //        MatchCollection match = regex.Matches(foldername);

        //        if (match.Count != 0)
        //        {
        //            FileInfo f = new FileInfo(adress);
        //            string type = "папка";
        //            ListViewItem lw = new ListViewItem(new string[] { foldername, "", type, f.LastWriteTime.ToString() }, 0);
        //            lw.Name = adress;
        //            Invoke(new Action(() => listView1.Items.Add(lw)));
        //            folderCount++;
        //            Invoke(new Action(() => label1.Text = "\nНайдено папок: " + folderCount + "\nНайдено файлов: " + fileCount));
        //            //Invoke(new Action(() => node.Nodes.Add(foldername)));
        //            //string[] str2 = Directory.GetDirectories(adress);
        //            //foreach (string s2 in str2)
        //            //{
        //            //    t = s2.Substring(s2.LastIndexOf('\\') + 1);
        //            //    node.Nodes[node.Nodes.Count-1].Nodes.Add(s2, t, 0);
        //            //}

        //        }
        //    }
        //    catch { }

        //    TreeNode tn_buff = new TreeNode(adress);

        //    string[] str3 = null;
        //    try
        //    {
        //        str3 = Directory.GetFiles(adress);
        //    }
        //    catch { }


        //    if (str3 != null)
        //    {
        //        foreach (string s in str3)
        //        {
        //            try
        //            {
        //                string filename = s.Substring(s.LastIndexOf('\\') + 1);
        //                MatchCollection matches = regex.Matches(filename);

        //                if (matches.Count != 0)
        //                {

        //                    FileInfo f = new FileInfo(@s);
        //                    string type = "файл";
        //                    ListViewItem lw = new ListViewItem(new string[] { filename, f.Length.ToString() + "байт", type, f.LastWriteTime.ToString() }, 1);
        //                    lw.Name = s;
        //                    Invoke(new Action(() => listView1.Items.Add(lw)));
        //                    fileCount++;
        //                    Invoke(new Action(() => label1.Text = "\nНайдено папок: " + folderCount + "\nНайдено файлов: " + fileCount));
        //                    //Invoke(new Action(() => node.Nodes.Add(filename)));
        //                }
        //            }
        //            catch { }
        //        }
        //    }

        //}

        //public void searchFile(List<string> adresses, string name/*, ref TreeNode node*/)
        //{
        //    foreach (string a in adresses)
        //    {
        //        searchProc(a, name);
        //    }
        //    foreach (string a in adresses)
        //    {
        //        List<string> AllAdresses = new List<string>();
        //        try
        //        {
        //            string[] str2 = Directory.GetDirectories(a);

        //            foreach (string s2 in str2)
        //            {
        //                AllAdresses.Add(s2);
        //            }

        //            searchFile(AllAdresses, name);
        //        }
        //        catch { }
        //    }


        //TreeNode tek_tn = new TreeNode(Path.GetFileName(StartDir));

        //foreach (string a in Directory.GetDirectories(StartDir))
        //{
        //    try
        //    {
        //        foreach (string file in Directory.GetFiles(a, name))
        //        {
        //            string searchedfile = Path.GetFileName(file);
        //            Invoke(new Action(() => tek_tn.Nodes.Add(file)));
        //            //tek_tn.Nodes.Add(file);
        //        }
        //        searchFile(a, name, ref tek_tn);
        //    }
        //    catch (Exception Error)
        //    {
        //        MessageBox.Show(Error.Message);
        //    }
        //    //searchProc(a, name, node);
        //}
        //if (tek_tn.Nodes.Count > 0)
        //{
        //    //node.Nodes.Add(tek_tn);
        //    node.Nodes.Add(tek_tn);
        //}
        //foreach (string a in adresses)
        //{
        //    List<string> AllAdresses = new List<string>();
        //    try
        //    {
        //        string[] str2 = Directory.GetDirectories(a);

        //        foreach (string s2 in str2)
        //        {
        //            AllAdresses.Add(s2);
        //        }

        //        searchFile(AllAdresses, name);
        //    }
        //    catch { }
        //}
        //}
        static ManualResetEventSlim _event = new ManualResetEventSlim(false); //создаем в несигнальном состоянии
        Thread searchThread;

        private void s1_test(string name, string dir, TreeNode n)
        {
            
            _event.Wait();
            Regex regex = new Regex(@"\w*" + name + @"\w*");
            Invoke(new Action(() => label4.Text = "Поиск в " + dir));

            string foldername = dir.Substring(dir.LastIndexOf('\\') + 1);


            //try
            //{
            //    MatchCollection match = regex.Matches(foldername);

            //    if (match.Count != 0)
            //    {
            //        FileInfo f = new FileInfo(dir);
            //        string type = "папка";
            //        ListViewItem lw = new ListViewItem(new string[] { foldername, "", type, f.LastWriteTime.ToString() }, 0);
            //        lw.Name = dir;
            //        Invoke(new Action(() => listView1.Items.Add(lw)));
            //    }
            //}
            //catch { }


            TreeNode tek_tn = new TreeNode(foldername);
            try
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    analysFilesCount++;
                    string filename = file.Substring(file.LastIndexOf('\\') + 1);
                    MatchCollection matches = regex.Matches(filename);

                    if (matches.Count != 0)
                    {
                        tek_tn.Nodes.Add(filename);
                        findfileCount++;

                        //FileInfo f = new FileInfo(@file);
                        //string type = "файл";
                        //ListViewItem lw = new ListViewItem(new string[] { filename, f.Length.ToString() + "байт", type, f.LastWriteTime.ToString() }, 1);
                        //lw.Name = file;
                        //Invoke(new Action(() => listView1.Items.Add(lw)));

                    }
                }
                if (tek_tn.Nodes.Count > 0)
                {
                    Invoke(new Action(() => n.Nodes.Add(tek_tn)));
                    
                }
                Invoke(new Action(() => label2.Text = "Найдено файлов: " + findfileCount + "\nПроанализировано файлов: " + analysFilesCount));
            }
            catch (Exception Error)
            {
                //MessageBox.Show(Error.Message);
            }
            
        }

        private void SearchFile(string name, string startDir, TreeNode tn, bool firststep=false)
        {
            string foldername;

            TreeNode tek_tn;
            if (firststep)
            {
                foldername = startDir;
                tek_tn = new TreeNode(foldername);
                Regex regex = new Regex(@"\w*" + name + @"\w*");
                try
                {
                    foreach (string file in Directory.GetFiles(startDir))
                    {
                        analysFilesCount++;
                        string filename = file.Substring(file.LastIndexOf('\\') + 1);
                        MatchCollection matches = regex.Matches(filename);

                        if (matches.Count != 0)
                        {
                            Invoke(new Action(() => tek_tn.Nodes.Add(filename)));
                            findfileCount++;
                            
                        }

                    }
                }
                catch (Exception Error) {
                    //MessageBox.Show(Error.Message);
                }
            }
            else
            {
                foldername = startDir.Substring(startDir.LastIndexOf('\\') + 1);
                tek_tn = new TreeNode(foldername);
            }
            if (tek_tn.Nodes.Count > 0)
            {
                Invoke(new Action(() => tn.Nodes.Add(tek_tn)));
                Invoke(new Action(() => label2.Text = "Найдено файлов: " + findfileCount + "\nПроанализировано файлов: " + analysFilesCount));
            }
            try
            {
                string[] dires = Directory.GetDirectories(startDir);
                foreach (string dir in dires)
                {
                    s1_test(name, dir, tek_tn);
                }
            }
            catch (Exception Error)
            {
                
            }

            try
            {
                string[] dires = Directory.GetDirectories(startDir);
                foreach (string dir in dires)
                {
                    SearchFile(name, dir, tek_tn);
                }
            }
            catch (Exception Error)
            {
                
            }

        }

        //private void Test(string name, string[] startDires, TreeNode tn)
        //{
        //    foreach (string dir in startDires)
        //    {
        //        SearchFile(name, dir, tn);
        //    }
        //    foreach (string dir in startDires)
        //    {
        //        try
        //        {
        //            string[] str2 = Directory.GetDirectories(dir);
        //            Test(name, str2, tn);
        //        }
        //        catch { }
        //    }
        //}


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                folderCount = 0;
                fileCount = 0;

                try
                {
                    string[] str2 = Directory.GetDirectories(textBox1.Text);
                    string[] str3 = Directory.GetFiles(@textBox1.Text);

                    currentIndex++;
                    currentListViewAdress = textBox1.Text;
                    Adresses.Add(textBox1.Text);

                    listView1.Items.Clear();

                    FileInfo f = new FileInfo(@textBox1.Text);

                    DrawListView(f, str2, str3);

                }
                catch
                {
                    listView1.Items.Clear();
                    textBox1.Text = "Такого адреса не существует!";
                }

                label5.Text = "Кол-во папок: " + folderCount;
                label5.Text += "  Кол-во файлов: " + fileCount;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView1.SelectedItems)
            {
                label1.Text = i.Name;
            }
        }

        private void stopsearch_Click(object sender, EventArgs e)
        {
            if (_event.IsSet)
            {
                _event.Reset();
                timer1.Stop();
                stopsearch.Text = "Возобновить";
            }
            else
            {
                _event.Set();
                t1 = DateTime.Now;
                timer1.Start();
                stopsearch.Text = "Остановить";
            }
            
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
            findfileCount = 0;
            analysFilesCount = 0;
            seconds = 0;
            //listView1.Items.Clear();
            treeView2.Nodes.Clear();

            string filename = textBox2.Text;

            if (searchThread != null)
            {
                if (searchThread.IsAlive)
                    searchThread.Abort();
            }


            TreeNode tn = treeView2.Nodes.Add("Поиск");
            _event.Set();
            stopsearch.Text = "Остановить";
            abort.Enabled = true;
            stopsearch.Enabled = true;
            searchThread = new Thread(() => SearchFile(filename, currentListViewAdress, tn, true)) { IsBackground = true };
            searchThread.Start();
        }

        private void abort_Click(object sender, EventArgs e)
        {
            if (searchThread.IsAlive)
            {
                stopsignal = true;
                searchThread.Abort();
                _event.Reset();
                abort.Enabled = false;
                stopsearch.Enabled = false;
                timer1.Stop();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StreamWriter fstream = new StreamWriter("data.txt");
            fstream.WriteLine(textBox2.Text);
            fstream.WriteLine(currentListViewAdress);
            fstream.Close();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (searchThread.IsAlive)
            {
                seconds++;
                label3.Text = "Прошло: " + seconds + " секунд";
            }
        }


    }

  
}

