using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
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
using static System.Formats.Asn1.AsnWriter;

namespace wpflearn
{
    public partial class MainWindow : Window
    {
        private MediaPlayer music = new MediaPlayer();
        public int playstop = 0;
        public string loadpath = "save.txt";
        public List<ListMusic> listMusic = new List<ListMusic>();
        public int startapp = 0;
        public TimeSpan pos;
#if RELEASE
        private readonly string _pictureDir = $"{Directory.GetCurrentDirectory()}\\Picture\\";
#else
        private readonly string _pictureDir = $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\Picture\\";
#endif

        #region MIAvoimasa s
        public int rewind = 0;
        public MainWindow()
        {
            InitializeComponent();
            LoadMusic();
            music.MediaEnded += MusicEnd!;
        }
        private void LoadMusic()
        {
            ComboBox cb = (ComboBox)Kiet.FindName("MusicChoice");
            using var load = new StreamReader(loadpath);
            while (!load.EndOfStream)
            {
                listMusic.Add(new ListMusic() { songname = load.ReadLine(), songaddress = load.ReadLine(), songimg = load.ReadLine() });
            }
            foreach (var music in listMusic)
            {
                cb.Items.Add(music.songname);
            }
        }
        #endregion
        private void CreateButtonPlay()
        {
            Button PlayPause = new Button();
            PlayPause.Name = "PlayPause";
            RegisterName(PlayPause.Name, PlayPause);
            PlayPause.HorizontalAlignment = HorizontalAlignment.Center;
            PlayPause.VerticalAlignment = VerticalAlignment.Top;
            PlayPause.Margin = new Thickness(0, 297, 0, 0);
            PlayPause.Height = 54;
            PlayPause.Width = 60;

            PlayPause.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir + "stop.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };

            PlayPause.MouseEnter += PlayPauseEnter;
            PlayPause.MouseLeave += PlayPauseLeave;
            PlayPause.Click += PlayPauseClick;
            Kiet.Children.Add(PlayPause);
        }
        private void CreateButtonSkipPrev()
        {
            Button Skip = new Button();
            Skip.Name = "Skip";
            RegisterName(Skip.Name, Skip);
            Skip.Margin = new Thickness(190, 297, 0, 0);
            Skip.Height = 54;
            Skip.Width = 60;
            Skip.HorizontalAlignment = HorizontalAlignment.Center;
            Skip.VerticalAlignment = VerticalAlignment.Top;
            Skip.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir  + "skip1.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Skip.MouseEnter += SkipEnter;
            Skip.MouseLeave += SkipLeave;
            Skip.Click += SkipClick;

            Button Prev = new Button();
            Prev.Name = "Prev";
            RegisterName(Prev.Name, Prev);
            Prev.Margin = new Thickness(0, 297, 190, 0);
            Prev.Height = 54;
            Prev.Width = 60;
            Prev.HorizontalAlignment = HorizontalAlignment.Center;
            Prev.VerticalAlignment = VerticalAlignment.Top;
            Prev.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir + "prev1.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Prev.MouseLeave += PrevLeave;
            Prev.MouseEnter += PrevEnter;
            Prev.Click += PrevClick;
            Kiet.Children.Add(Skip);
            Kiet.Children.Add(Prev);
        }
        private void SkipEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir + "skip2.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
        private void SkipLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir + "skip1.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
        private void PrevEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir + "prev2.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
        private void PrevLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Content = new Image
            {
                Source = new BitmapImage(new (_pictureDir + "prev1.png")),
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
        private void PlayPauseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (playstop == 0)
            {
                button.Content = new Image
                {
                    Source = new BitmapImage(new (_pictureDir + "stop2.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }
            if (playstop == 1)
            {
                button.Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/OOjs_UI_icon_play-ltr.svg/1200px-OOjs_UI_icon_play-ltr.svg.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }
        }
        private void PlayPauseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (playstop == 0)
            {
                button.Content = new Image
                {
                    Source = new BitmapImage(new (_pictureDir + "stop.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }
            if (playstop == 1)
            {
                button.Content = new Image
                {
                    Source = new BitmapImage(new (_pictureDir + "play.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }
        }
        private void PlayPauseClick(object sender, RoutedEventArgs e)
        {
            if (playstop == 0)
            {
                playstop = 1;
                pos = music.Position;
                music.Stop();
                Button button = (Button)sender;
                button.Content = new Image
                {
                    Source = new BitmapImage(new (_pictureDir + "play.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }
            else
            {
                music.Position = pos;
                playstop = 0;
                music.Play();
                Button button = (Button)sender;
                button.Content = new Image
                {
                    Source = new BitmapImage(new (_pictureDir + "play.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }
        }
        private void SkipClick(object sender, RoutedEventArgs e)
        {
            if (MusicChoice.SelectedIndex < MusicChoice.Items.Count - 1)
            {
                MusicChoice.SelectedIndex++;
            }
            else
            {
                MusicChoice.SelectedIndex = 0;
            }
        }
        private void PrevClick(object sender, RoutedEventArgs e)
        {
            if (MusicChoice.SelectedIndex == 0)
            {
                rewind = 1;
            }
            MusicChoice.SelectedIndex--;
            if (MusicChoice.SelectedIndex <= -1)
            {
                MusicChoice.SelectedIndex = MusicChoice.Items.Count - 1;
            }
        }
        
        private void Choice(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (startapp == 0)
            {
                ComboBox addndelete = (ComboBox)Kiet.FindName("AddDelete");
                addndelete.Visibility = Visibility.Hidden;
                Home();
                Kiet.Height = 450;
                Kiet.Width = 860;
                TextBlock title = (TextBlock)Kiet.FindName("TitleApp");
                title.Visibility = Visibility.Hidden;
                cb.HorizontalAlignment = HorizontalAlignment.Left;
                cb.VerticalAlignment = VerticalAlignment.Top;
                cb.Margin = new Thickness(0, 0, 0, 0);
                CreateButtonPlay();
                CreateButtonSkipPrev();
                startapp = 1;
            }
            if (cb.SelectedIndex != -1)
            {
                int indexchoice = MusicChoice.SelectedIndex;
                music.Open(new Uri(listMusic[indexchoice].songaddress!));
                Kiet.Background = new ImageBrush(new BitmapImage(new Uri(listMusic[indexchoice].songimg!)));
                music.Play();
                music.Volume = 1;
            }
            else
            {
                if (rewind != 1) startapp = 0;
                else rewind = 0;
            }
        }
        private void MusicEnd(object sender, EventArgs e)
        {
            if (MusicChoice.SelectedIndex < MusicChoice.Items.Count - 1)
            {
                MusicChoice.SelectedIndex++;
            }
            else
            {
                MusicChoice.SelectedIndex = 0;
            }
        }
        private void Home()
        {
            Button home = new Button();
            home.Name = "Home";
            home.Margin = new Thickness(790, 0, 0, 390);
            home.Height = 54;
            home.Width = 60;
            home.Content = new Image
            {
                Source = new BitmapImage(new Uri(@"https://i.kym-cdn.com/entries/icons/original/000/026/653/triangle.jpg")),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Kiet.Children.Add(home);
            home.Click += Comeback;
        }
        private void Comeback(object sender, RoutedEventArgs e)
        {
            Button home = (Button)sender;
            Button previous = (Button)Kiet.FindName("Prev");
            Button skip = (Button)Kiet.FindName("Skip");
            Button PlayPause = (Button)Kiet.FindName("PlayPause");
            ComboBox addndel = (ComboBox)Kiet.FindName("AddDelete");
            ComboBox cb = (ComboBox)Kiet.FindName("MusicChoice");
            TextBlock title = (TextBlock)Kiet.FindName("TitleApp");
            music.Stop();
            cb.SelectedIndex = -1;
            Kiet.Background = new ImageBrush(new BitmapImage(new Uri(@"https://i.kym-cdn.com/entries/icons/original/000/026/653/triangle.jpg")));
            cb.HorizontalAlignment = HorizontalAlignment.Center;
            cb.VerticalAlignment = VerticalAlignment.Top;
            cb.Margin = new Thickness(0, 205, 60, 0);
            home.Visibility = Visibility.Hidden;
            addndel.Visibility = Visibility.Visible;
            title.Visibility = Visibility.Visible;
            addndel.Margin = new Thickness(780, 0, 0, 0);
            Kiet.Children.Remove(previous);
            Kiet.Children.Remove(skip);
            Kiet.Children.Remove(PlayPause);
            UnregisterName(previous.Name);
            UnregisterName(skip.Name);
            UnregisterName(PlayPause.Name);
        }
        private void AddorDelete(object sender, SelectionChangedEventArgs e)
        {
            ComboBox addndel = (ComboBox)sender;
            ComboBox cb = (ComboBox)Kiet.FindName("MusicChoice");
            if (addndel.SelectedIndex == 0)
            {
                AddDelete add = new AddDelete();
                add.CreateSave();
                add.ShowDialog();
                using var load = new StreamReader(loadpath);
                listMusic.Clear();
                while (!load.EndOfStream)
                {
                    listMusic.Add(new ListMusic() { songname = load.ReadLine(), songaddress = load.ReadLine(), songimg = load.ReadLine() });
                }
                cb.Items.Clear();
                int i = 0;
                while (i < listMusic.Count)
                {
                    cb.Items.Add(listMusic[i].songname);
                    i++;
                }
            }
            if (addndel.SelectedIndex == 1)
            {
                AddDelete delete = new AddDelete();
                delete.CreateDelete();
                delete.ShowDialog();
                using var load = new StreamReader(loadpath);
                listMusic.Clear();
                while (!load.EndOfStream)
                {
                    listMusic.Add(new ListMusic() { songname = load.ReadLine(), songaddress = load.ReadLine(), songimg = load.ReadLine() });
                }
                cb.Items.Clear();
                int i = 0;
                while (i < listMusic.Count)
                {
                    cb.Items.Add(listMusic[i].songname);
                    i++;
                }
            }
            addndel.SelectedIndex = -1;
        }
    }
}