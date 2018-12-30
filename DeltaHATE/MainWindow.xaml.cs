using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.IO.Pipes;
using Microsoft.Win32;
//using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Shapes;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace DeltaHATE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region some vars
        public UndertaleData Data { get; set; }
        //public UndertaleData OldData { get; set; }
        public string FilePath { get; set; }
        public Random rng { get; set; }
        #endregion

        public int RandomSeed()
        {
            Random rRandom = new Random();
            string characters = "0123456789";
            StringBuilder result = new StringBuilder(6);
            for (int i = 0; i < 6; i++)
            {
                result.Append(characters[rRandom.Next(characters.Length)]);
            }
            return Convert.ToInt32(Convert.ToString(result));
        }

        public bool EnsureDataLoaded()
        {
            if (Data == null)
            {
                return false;
            }
            else return true;
        }

        private void SetStatics()
        {
            Values.DoSprite = (bool)check_csprites.IsChecked;
            Values.DoText = (bool)check_cstrs.IsChecked;
            Values.DoSound = (bool)check_csnds.IsChecked;
            Values.DoFont = (bool)check_cfnts.IsChecked;
            Values.DoBackground = (bool)check_cbackground.IsChecked;

            if((bool)radio_shufflebytpi.IsChecked)
            {
                Values.SpriteType = true;
            } else { Values.SpriteType = false; }
        }

        private bool IsDelta()
        {
            var scr_debug = Data.Scripts.ByName("scr_debug")?.Code;
            if (scr_debug != null)
                return true;
            else
                return false;
        }

        #region savenstuff
        private async Task<bool> DoOpenDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = "win";
            dlg.Filter = "Game Maker Studio data files (.win, .unx, .ios)|*.win;*.unx;*.ios|All files|*";

            if (dlg.ShowDialog() == true)
            {
                await LoadFile(dlg.FileName);
                return true;
            }
            return false;
        }

        private async Task<bool> DoSaveDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.DefaultExt = "win";
            dlg.Filter = "Game Maker Studio data files (.win, .unx, .ios)|*.win;*.unx;*.ios|All files|*";
            dlg.FileName = FilePath;

            if (dlg.ShowDialog() == true)
            {
                await SaveFile(dlg.FileName);
                return true;
            }
            return false;
        }

        private async Task LoadFile(string filename)
        {
            LoadDlg dialogue = new LoadDlg { Owner = this };

            Task t = Task.Run(() =>
            {
                Data = null;
                UndertaleData data = null;
                try
                {
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        data = UndertaleIO.Read(stream, warning =>
                        {
                            MessageBox.Show(warning, "Loading warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        });
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error occured while trying to load:\n" + e.Message, "Load error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Dispatcher.Invoke(() =>
                {
                    //if (data != null)
                    //{
                        this.Data = data;
                        this.FilePath = filename;
                    //}
                    dialogue.Close();
                });
            });
            dialogue.ShowDialog();
            await t;
        }

        private async Task SaveFile(string filename)
        {
            if (Data == null || Data.UnsupportedBytecodeVersion) { return; }
            SetStatics();
            LoadDlg dialogue = null;
            Task t = null;
            bool thingy = await DoCorrupt.DoThing(Data, Values.DoSprite, Values.DoBackground, Values.DoSound, Values.DoText, Values.DoFont, Values.SpriteType, FilePath, rng, IsDelta());

            if (thingy)
            {
                MessageBox.Show("Corrupted succesfully! Probably!", "Egg?!?!?!?!??!", MessageBoxButton.OK, MessageBoxImage.None);
                dialogue = new LoadDlg { Owner = this };

                t = Task.Run(() =>
                {
                    //OldData = Data;
                    //MessageBox.Show("Data old set", "", MessageBoxButton.OK, MessageBoxImage.None);
                    using (var stream = new FileStream(filename, FileMode.Create))
                    {
                        UndertaleIO.Write(stream, Data);
                    }

                    Dispatcher.Invoke(() =>
                    {
                        dialogue.Close();
                        //Data = OldData;
                        //MessageBox.Show("New data set", "", MessageBoxButton.OK, MessageBoxImage.None);
                    });
                });
                dialogue.ShowDialog();
                await t;
            }
        }
        #endregion

        private void button_loadfile_Click(object sender, RoutedEventArgs e)
        {
            DoOpenDialog();
            label_dataname.Content = FilePath;
        }

        private void button_randomseed_Click(object sender, RoutedEventArgs e)
        {
            tbox_seedcamp.Text = Convert.ToString(RandomSeed());
            rng = new Random(Convert.ToInt32(tbox_seedcamp.Text));
        }

        private void lewindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbox_seedcamp.Text = Convert.ToString(RandomSeed());
            rng = new Random(Convert.ToInt32(tbox_seedcamp.Text));
        }

        private void button_corrupt_Click(object sender, RoutedEventArgs e)
        {
            UndertaleData OldData = new UndertaleData();
            rng = new Random(Convert.ToInt32(tbox_seedcamp.Text));

            if (!EnsureDataLoaded())
            {
                MessageBox.Show("Load the game first!", "Egg!!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DoSaveDialog();
        }

        private void quitter_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void minimize_bt_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void creditbt_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DeltaHATE GUI version:\nby mike309\nOriginal DeltaHATE & UndertaleModLib:\nby krzys_h\nOriginal HATE:\nby RedSpah", "Egg", MessageBoxButton.OK, MessageBoxImage.Information);
            //MessageBox.Show("Current sprite exceptions:\nSprites that start with 'spr_kris' (only on unstable sprite shuffle  mode)", "Egg: The long awaited sequel", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ops_Click(object sender, RoutedEventArgs e)
        {
            //button_corrupt.IsEnabled = false;
            NewOptions newopt = new NewOptions { Owner = this };
            newopt.ShowDialog();
        }
    }

    public static class DoCorrupt
    {
        public static MainWindow mw = new MainWindow();

        public static List<UndertaleTexturePageItem> SpriteList = new List<UndertaleTexturePageItem>();

        public static List<UndertaleTexturePageItem> TileList = new List<UndertaleTexturePageItem>();

        public static List<UndertaleTexturePageItem> BGTList = new List<UndertaleTexturePageItem>();

        //public static UndertaleData Data = mw.Data;

        //krzys' code
        static void Shuffle<T>(this IList<T> list, Random MyRng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = MyRng.Next(n + 1);

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        static void ShuffleOnlySelected<T>(this IList<T> list, IList<int> selected, Action<int, int> swapFunc, Random MyRng)
        {
            int n = selected.Count;
            while (n > 1)
            {
                n--;
                int k = MyRng.Next(n + 1);

                swapFunc(selected[n], selected[k]);

                int idx = selected[k];
                selected[k] = selected[n];
                selected[n] = idx;
            }
        }

        static void ShuffleOnlySelected<T>(this IList<T> list, IList<int> selected, Random MyRng)
        {
            list.ShuffleOnlySelected(selected, (n, k) => {
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            },MyRng);
        }

        static void SelectSome(this IList<int> list, Random MyRng)
        {
            int toRemove = (int)(list.Count * (1 - (float)mw.slider_power.Value));
            for (int i = 0; i < toRemove; i++)
                list.RemoveAt(MyRng.Next(list.Count));
        }
        static void SelectSomeTPI(this IList<UndertaleTexturePageItem> list, Random MyRng)
        {
            int toRemove = (int)(list.Count * (1 - (float)mw.slider_power.Value));
            for (int i = 0; i < toRemove; i++)
                list.RemoveAt(MyRng.Next(list.Count));
        }

        public static async Task<bool> DoThing(UndertaleData Data, bool DoSprs, bool DoBGs, bool DoSnds, bool DoStrs, bool DoFnts, bool CorrupType, string FilePath, Random MyRng, bool DeltaGame)
        {
            //set sprites to shuffle without backgrounds
            //nevermind i cant do that

            //foreach (var sprs in Data.Sprites)
            //{
                //if (sprs.Name.Content.StartsWith("bg_"))
                //{
                //    continue;
                //}

                //get bamboozled i actually can
                //actually no again im a fucking idiot
                for (int i = 0; i < Data.TexturePageItems.Count; i++)
                {
                    //var curspr = Data.Sprites[i];
                    //if (curspr.Name.Content.StartsWith("bg_"))
                    //{
                        SpriteList.Add(Data.TexturePageItems[i]);
                    //}
                }
            //}

            if (DeltaGame)
            {
                foreach (var sp in Data.Sprites)
                {
                    if (sp.Name.Content.Contains("tiles"))
                    {
                        TileList.Add(sp.Textures[0].Texture);
                    }
                }

                foreach (var sp in Data.Sprites)
                {
                    if (sp.Name.Content.StartsWith("bg_") && !sp.Name.Content.Contains("tiles"))
                    {
                        BGTList.Add(sp.Textures[0].Texture);
                    }
                }
            }

            if (DoSprs)
            {
                if (CorrupType)
                {
                    //texture page shuff
                    foreach (var sprite in Data.Sprites)
                    {
                        /*if (!DoBGs)
                        {
                            if (sprite.Name.Content.StartsWith("bg_"))
                            {
                                continue;
                            }
                        }*/
                        if (DeltaGame)
                        {
                            if (sprite.Name.Content.StartsWith("bg_") || sprite.Name.Content.Contains("tiles"))
                            {
                                continue;
                            }
                        }
                        foreach (var tex in sprite.Textures)
                        {
                            //List<UndertaleTexturePageItem> SpriteSome = SpriteList;
                            //SpriteSome.SelectSomeTPI(MyRng);
                            int r = MyRng.Next(SpriteList.Count);
                            tex.Texture = SpriteList[r];
                        }
                    }
                }
                else
                {
                    //unstable reeeeeeeeeeeeeeeeeeeee
                    List<int> Whichs = new List<int>();
                    for (int i = 0; i < Data.Sprites.Count; i++)
                    {
                        var sprite = Data.Sprites[i];
                        if (DeltaGame)
                        {
							if (!Values.DoKris) {
								if (sprite.Name.Content.StartsWith("spr_kris"))
								{
									continue;
								}
							}
							
							if (!Values.SuseiChalkScene)
							{
								if (sprite.Name.Content.Contains("spr_susie_eatchalk"))
								{
									continue;
								}
							}
							
							if (!Values.SantaLancer)
							{
								/*if (sprite.Name.Content.Contains("spr_kris"))
								{
									continue;
								}*/
							}
                            /*if (!DoBGs)
                            {
                                if (sprite.Name.Content.StartsWith("bg_"))
                                {
                                    continue;
                                }
                            }*/
							
							//ehhhhhhhhhhh
							if (sprite.Name.Content.StartsWith("bg_") || sprite.Name.Content.Contains("tiles"))
                            {
                                continue;
                            }
                        }
                        Whichs.Add(i);
                    }
                    //Whichs.SelectSome(MyRng);
                    Data.Sprites.ShuffleOnlySelected(Whichs, MyRng);
                }
            }

            if (DoBGs)
            {
                Data.Backgrounds.Shuffle(MyRng);
                //bad code, doesnt shuffle tilesets
                /*List<int> Whichs = new List<int>();
                for (int i = 0; i < Data.Backgrounds.Count; i++)
                {
                    Whichs.Add(i);
                }
                Whichs.SelectSome(MyRng);
                Data.Backgrounds.ShuffleOnlySelected(Whichs, MyRng);*/
                if (DeltaGame)
                {
                    if (CorrupType)
                    {
                        foreach (var sprite in Data.Sprites)
                        {
                            if (!(sprite.Name.Content.StartsWith("bg_")))
                            {
                                continue;
                            }
                            foreach (var tex in sprite.Textures)
                            {
                                //List<UndertaleTexturePageItem> SpriteSome = BGTList;
                                //SpriteSome.SelectSomeTPI(MyRng);
                                int r = MyRng.Next(BGTList.Count);
                                tex.Texture = BGTList[r];
                            }
                        }

                        foreach (var sprite in Data.Sprites)
                        {
                            if (!(sprite.Name.Content.Contains("tiles")))
                            {
                                continue;
                            }
                            foreach (var tex in sprite.Textures)
                            {
                                //List<UndertaleTexturePageItem> SpriteSome = TileList;
                                //SpriteSome.SelectSomeTPI(MyRng);
                                int r = MyRng.Next(TileList.Count);
                                tex.Texture = TileList[r];
                            }
                        }
                    }
                    else
                    {
                        List<int> Whichs = new List<int>();
                        List<int> tWhichs = new List<int>();
                        for (int i = 0; i < Data.Sprites.Count; i++)
                        {
                            var sprite = Data.Sprites[i];
                            if (sprite.Name.Content.StartsWith("bg_"))
                            {
                                Whichs.Add(i);
                            }

                            if (sprite.Name.Content.Contains("tiles"))
                            {
                                tWhichs.Add(i);
                            }
                        }
                        //Whichs.SelectSome(MyRng);
                        Data.Sprites.ShuffleOnlySelected(Whichs, MyRng);
                        Data.Sprites.ShuffleOnlySelected(tWhichs, MyRng);
                    }
                }
            }

            if (DoSnds)
            {
                List<int> Whichs = new List<int>();
                for (int i = 0; i < Data.Sounds.Count; i++)
                {
                    Whichs.Add(i);
                }
                //Whichs.SelectSome(MyRng);
                Data.Sounds.ShuffleOnlySelected(Whichs, MyRng);
            }

            if (DoFnts)
            {
                List<int> Whichs = new List<int>();
                for (int i = 0; i < Data.Fonts.Count; i++)
                {
                    Whichs.Add(i);
                }
                //Whichs.SelectSome(MyRng);
                Data.Fonts.ShuffleOnlySelected(Whichs, MyRng);
            }//aa

            if (DoStrs)
            {
                //krzys' code
                if (DeltaGame)
                {
                    Dictionary<string, string> translations = new Dictionary<string, string>();
                    foreach (string line in File.ReadAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FilePath), "lang/lang_en.json")))
                    {
                        // Yeah. No JSON support in scripts. Deal with it.
                        string[] a = line.Split(new char[] { ':' }, 2);
                        if (a.Length != 2)
                            continue;
                        a[0] = a[0].Trim();
                        a[0] = a[0].Substring(1, a[0].Length - 2);
                        a[1] = a[1].Trim();
                        a[1] = a[1].Substring(1, a[1].Length - 3);
                        if (a[0] == "date")
                            continue;
                        if (a[1] == "||") // This breaks the auto-line-break badly, why is this string even localized
                            continue;
                        translations.Add(a[0], a[1]);
                    }
                    // Splitting the strings into groups like this is necessary to prevent crashes and the game freezing because of waiting on input it can't get
                    List<int> choicer_old_lines = new List<int>();
                    List<int> choicer_neo_2_lines = new List<int>();
                    List<int> choicer_neo_3_lines = new List<int>();
                    List<int> choicer_neo_4_lines = new List<int>();
                    List<int> final_lines = new List<int>();
                    List<int> continue_lines = new List<int>();
                    List<int> waiting_lines = new List<int>();
                    List<int> waiting_final_lines = new List<int>();
                    List<int> waiting_continue_lines = new List<int>();
                    List<int> dash_whatever_that_is = new List<int>();
                    List<int> other_lines = new List<int>();
                    for (int i = 0; i < Data.Strings.Count; i++)
                    {
                        var id = Data.Strings[i].Content;
                        if (translations.ContainsKey(id))
                        {
                            var str = translations[id];
                            if (str.Contains("\\\\C1"))
                                choicer_old_lines.Add(i);
                            else if (str.Contains("\\\\C2"))
                                choicer_neo_2_lines.Add(i);
                            else if (str.Contains("\\\\C3"))
                                choicer_neo_3_lines.Add(i);
                            else if (str.Contains("\\\\C4"))
                                choicer_neo_4_lines.Add(i);
                            else if (str.EndsWith("/%%"))
                                waiting_final_lines.Add(i);
                            else if (str.EndsWith("/%"))
                                waiting_continue_lines.Add(i);
                            else if (str.EndsWith("/"))
                                waiting_lines.Add(i);
                            else if (str.EndsWith("%%"))
                                final_lines.Add(i);
                            else if (str.EndsWith("%"))
                                continue_lines.Add(i);
                            else if (str.EndsWith("-"))
                                dash_whatever_that_is.Add(i);
                            else
                                other_lines.Add(i);
                        }
                    }
                    // We have to swap the contents because UndertaleModTool is too smart :P
                    void StringSwap(int n, int k)
                    {
                        string value = Data.Strings[k].Content;
                        Data.Strings[k].Content = Data.Strings[n].Content;
                        Data.Strings[n].Content = value;
                    }
                    /*choicer_old_lines.SelectSome(MyRng);
                    choicer_neo_2_lines.SelectSome(MyRng);
                    choicer_neo_3_lines.SelectSome(MyRng);
                    choicer_neo_4_lines.SelectSome(MyRng);
                    waiting_final_lines.SelectSome(MyRng);
                    waiting_continue_lines.SelectSome(MyRng);
                    waiting_lines.SelectSome(MyRng);
                    final_lines.SelectSome(MyRng);
                    continue_lines.SelectSome(MyRng);
                    dash_whatever_that_is.SelectSome(MyRng);
                    other_lines.SelectSome(MyRng);*/

                    Data.Strings.ShuffleOnlySelected(choicer_old_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(choicer_neo_2_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(choicer_neo_3_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(choicer_neo_4_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(waiting_final_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(waiting_continue_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(waiting_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(final_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(continue_lines, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(dash_whatever_that_is, StringSwap, MyRng);
                    Data.Strings.ShuffleOnlySelected(other_lines, StringSwap, MyRng);
                }
            }

            return true;
        }
    }
}
