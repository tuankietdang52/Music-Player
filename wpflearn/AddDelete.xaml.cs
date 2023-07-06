using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpflearn
{
    public partial class AddDelete : Window
    {
        private readonly string _pictureDir = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public string pathload = "save.txt";
        public string pathnewload = @"anothersave.txt";
        public int addordelete;
        public bool firstline;
        public AddDelete()
        {
            InitializeComponent();
        }
        public void CreateSave()
        {
            Icon = new BitmapImage(new (_pictureDir + "save.png"));
            addordelete = 0;
            CreateLabelSave();
            ButtonConfirm();
        }
        public void CreateDelete()
        {
            Icon = new BitmapImage(new Uri(_pictureDir + "delete.png"));
            addordelete = 1;
            CreateListBoxDelete();
            ButtonConfirm();
        }
        public void ButtonConfirm()
        {
            Button confirm = new Button();
            confirm.Content = "Confirm";
            confirm.Height = 40;
            confirm.Width = 70;
            confirm.Background = new SolidColorBrush(Color.FromRgb(0, 255, 249));
            if (addordelete == 0)
            {
                confirm.Margin = new Thickness(350, 70, 0, 0);
                StackPanel Stacksong = (StackPanel)SongOption.FindName("Stack_Song");
                Stacksong.Children.Add(confirm);
                confirm.Click += SaveFunction;
            }
            else
            {
                confirm.Margin = new Thickness(350, 200, 0, 0);
                Grid griddel = (Grid)SongOption.FindName("Delete_Grid");
                griddel.Children.Add(confirm);
                ListBox delete = (ListBox)SongOption.FindName("ListDelete");
                confirm.Click += DeleteFunction;
            }
        }
        public void CreateLabelSave()
        {
            Title = "Add";
            StackPanel Stacksong = new StackPanel();
            Stacksong.Name = "Stack_Song";
            RegisterName(Stacksong.Name, Stacksong);
            Stacksong.Margin = new Thickness(20, 20, 20, 20);
            SongOption.AddChild(Stacksong);
            Label namesong = new Label();
            namesong.Content = "Name Song: ";
            Stacksong.Children.Add(namesong);
            TextBox write1 = new TextBox();
            write1.Name = "name";
            Stacksong.Children.Add(write1);
            Label songaddress = new Label();
            songaddress.Content = "Song Address (.mp3): ";
            Stacksong.Children.Add(songaddress);
            TextBox write2 = new TextBox();
            write2.Name = "address";
            Stacksong.Children.Add(write2);
            Label songimg = new Label();
            songimg.Content = "Song Image (Source): ";
            Stacksong.Children.Add(songimg);
            TextBox write3 = new TextBox();
            write3.Name = "img";
            Stacksong.Children.Add(write3);
        }
        private void SaveFunction(object sender, RoutedEventArgs e)
        {
            using var read = new StreamReader(pathload);
            if (read.ReadToEnd() == "") firstline = true;
            else firstline = false;
            read.Close();
            StackPanel Stacksong = (StackPanel)SongOption.FindName("Stack_Song");
            TextBox name = (TextBox)Stacksong.Children[1];
            TextBox address = (TextBox)Stacksong.Children[3];
            TextBox img = (TextBox)Stacksong.Children[5];
            if (name.Text != "" && address.Text != "" && img.Text != "")
            {
                using var load = new StreamWriter(pathload, true);
                if (firstline == false) load.WriteLine();
                load.WriteLine(name.Text);
                load.WriteLine(address.Text);
                load.Write(img.Text);
                MessageBox.Show("Da them " + name.Text);
                name.Text = String.Empty;
                address.Text = String.Empty;
                img.Text = String.Empty;
            }
            else
            {
                MessageBox.Show("Nhap duong dan vao");
            }
        }
        private void CreateListBoxDelete()
        {
            Title = "Delete";
            Grid griddel = new Grid();
            griddel.Name = "Delete_Grid";
            RegisterName(griddel.Name, griddel);
            SongOption.AddChild(griddel);
            using var load = new StreamReader(pathload);
            List<ListMusic> list = new List<ListMusic>();
            int line = 0;
            while (!load.EndOfStream)
            {
                if(line % 3 == 0 || line == 0) list.Add(new ListMusic() { songname = load.ReadLine() });
                else
                {
                    string gb = load.ReadLine()!;
                }
                line++;
            }
            ListView delete = new ListView();
            delete.Name = "ListDelete";
            RegisterName(delete.Name, delete);
            griddel.Children.Add(delete);
            for (int i = 0; i < list.Count; i++)
            {
                delete.Items.Add(list[i].songname);
            }
        }
        private void DeleteFunction(object sender, RoutedEventArgs e)
        {
            Grid griddel = (Grid)SongOption.FindName("Delete_Grid");
            ListView delete = (ListView)griddel.FindName("ListDelete");
            if (delete.SelectedIndex != -1)
            {
                using var rewrite = new StreamWriter(pathnewload);
                using var load = new StreamReader(pathload);
                string itemdelete = (delete.SelectedItem as string)!;
                string line;
                List<string> itemname = new List<string>();
                int i = 0;
                while (!load.EndOfStream)
                {
                    if ((line = load.ReadLine()!) == itemdelete)
                    {
                        MessageBox.Show("Da xoa " + line);
                        load.ReadLine();
                        load.ReadLine();
                        continue;
                    }
                    if (i % 3 == 0 || i == 0)
                    {
                        itemname.Add(line);
                        if (i == 0)
                        {
                            rewrite.Write(line);
                            i++;
                            continue;
                        }
                    }
                    rewrite.Write("\n" + line);
                    i++;
                }
                rewrite.Close();
                load.Close();
                File.Copy(pathnewload, pathload, true);
                delete.Items.Clear();
                foreach (var item in itemname)
                {
                    delete.Items.Add(item);
                }
            }
            else
            {
                MessageBox.Show("Chon nhac de xoa");
            }
        }
    }
}