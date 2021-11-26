﻿using GenShin_LauncherDIY.Config;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace GenShin_LauncherDIY
{
    /// <summary>
    /// setWindow.xaml 的交互逻辑
    /// </summary>
    public partial class setWindow : MetroWindow
    {
        public setWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            GamePath.Text = Config.IniGS.gamePath;//游戏路径
            if (!Config.IniGS.isAutoSize)//全屏
                FullF.IsChecked = true;
            else
                FullT.IsChecked = true;
            GHeight.Text = Config.IniGS.Height.ToString();//高度
            GWidth.Text = Config.IniGS.Width.ToString();//宽度
            if (Config.IniGS.BiOrMi == 2)
            { //读取服务器
                BIliS.IsChecked = true;
                GlobalS.IsEnabled = false;
            }
            else if (Config.IniGS.BiOrMi == 1)
            {
                MiS.IsChecked = true;
                GlobalS.IsEnabled = false;
            }
            else
                GlobalS.IsChecked = true; 
            if(GamePath.Text == "")
                ToGlobal.IsEnabled = false;
            IsGlobal();
            IsSDK();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (bqload.Visibility == Visibility.Hidden)
            {
                e.Cancel = false;
            }
            else
            {
                this.ShowMessageAsync("警告", "请等待文件解压缩完毕，否则可能出现致命错误！！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                e.Cancel = true;
            }
        }



        private void setSave_Click(object sender, RoutedEventArgs e)
        {
            //游戏路径保存
            if (string.IsNullOrWhiteSpace(GamePath.Text))
            {
                this.ShowMessageAsync("警告", "游戏路径为空,请输入游戏路径！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                return;
            }
            else if (Directory.Exists(GamePath.Text) == false)
            {
                this.ShowMessageAsync("错误", "游戏路径不存在,请输入正确的游戏路径！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                return;
            }
            else
            {
                Config.IniGS.gamePath = GamePath.Text;
                Config.Settings.GamePath = GamePath.Text;
            }
            //游戏分辨率保存
            if (!Utils.checkTool.IsNumber(GWidth.Text) || !Utils.checkTool.IsNumber(GHeight.Text))
            {
                this.ShowMessageAsync("警告", "窗口大小错误, 请输入正确的分辨率！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                return;
            }
            else
            {
                Settings.Height = GHeight.Text;
                Settings.Width = GWidth.Text;
                Config.IniGS.Width = Convert.ToUInt16(GWidth.Text);
                Config.IniGS.Height = Convert.ToUInt16(GHeight.Text);
            }
            //是否全屏
            if (FullF.IsChecked == true)
            {
                Settings.FullS = "0";
                Config.IniGS.isAutoSize = false;
            }
            else if (FullT.IsChecked == true)
            {
                Settings.FullS = "1";
                Config.IniGS.isAutoSize = true;
            }
            //B服或官服
            if (MiS.IsChecked == true)
            {
                Config.IniGS.BiOrMi = 1;
                BOM.Sub_channel = 1;
                BOM.Channel = 1;
            }
            else if(BIliS.IsChecked==true)
            {
                Config.IniGS.BiOrMi = 2;
                BOM.Sub_channel = 0;
                BOM.Channel = 14;
            }
            else
            {
                Config.IniGS.BiOrMi = 3;
                BOM.Sub_channel = 0;
                BOM.Channel = 1;
            }
            Config.setConfig.checkini();
            this.Close();
        }

        private void Button_Click21_9(object sender, RoutedEventArgs e)
        {
            int x = (int)SystemParameters.WorkArea.Width - 80;
            int y = (int)(SystemParameters.WorkArea.Width - 80) * 9 / 21;
            GWidth.Text = Convert.ToString(x);
            GHeight.Text = Convert.ToString(y);
        }

        private void Button_Click4_3(object sender, RoutedEventArgs e)
        {
            int x = (int)(SystemParameters.WorkArea.Height - 80) * 4 / 3;
            int y = (int)SystemParameters.WorkArea.Height - 80;
            GWidth.Text = Convert.ToString(x);
            GHeight.Text = Convert.ToString(y);
        }
        private void IsSDK()
        {
            if (GamePath.Text == "")
            {
                SDKlive.Content = "SDK:未知";
                Fixbtn.IsEnabled = false;
            }
            else
            {
                if (File.Exists(GamePath.Text + "\\Genshin Impact Game\\YuanShen_Data\\Plugins\\PCGameSDK.dll") == true)
                {
                    SDKlive.Content = "SDK:存在";
                    Fixbtn.IsEnabled = false;
                }
                else if (File.Exists(GamePath.Text + "\\Genshin Impact Game\\GenshinImpact_Data\\Plugins\\PCGameSDK.dll") == true)
                {
                    SDKlive.Content = "SDK:存在";
                    Fixbtn.IsEnabled = false;
                }
                else
                {
                    SDKlive.Content = "SDK:缺失";
                    Fixbtn.IsEnabled = true;
                }
            }
        }
        private void Fix_Click(object sender, RoutedEventArgs e)
        {
            var sdkUri = "pack://application:,,,/Res/mihoyosdk.dll";
            var uri = new Uri(sdkUri, UriKind.RelativeOrAbsolute);
            var stream = Application.GetResourceStream(uri).Stream;
            Utils.UtilsTools.StreamToFile(stream, GamePath.Text + "\\Genshin Impact Game\\YuanShen_Data\\Plugins\\PCGameSDK.dll");
            this.ShowMessageAsync("提示", "MihoyoSDK修复成功", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            IsSDK();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ToGlobal.Content == "转换")
            {
                Thread StartUn = new Thread(() => UnFile());
                bqload.Visibility = Visibility.Visible;
                setSave.IsEnabled = false;
                ToGlobal.IsEnabled = false;
                TimeStatus.Content = "当前状态：正在解压资源";
                StartUn.Start();
            }
            else
            {
                Thread StartRe = new Thread(() => ReCnGame());
                bqload.Visibility = Visibility.Visible;
                setSave.IsEnabled = false;
                ToGlobal.IsEnabled = false;
                TimeStatus.Content = "当前状态：正在还原游戏";
                StartRe.Start();
            }
        }
        private void UnFile()
        {
            if (Utils.UtilsTools.UnZip(@"GlobalFile.pkg", @""))
            {
                this.Dispatcher.Invoke(new Action(delegate ()
                {
                    bool error=false;
                    for (int i = 0; i < Settings.globalfiles.Length; i++)
                    {
                        if (File.Exists(@"GlobalFile//" + Settings.globalfiles[i]) == false)
                        {                           
                            LogBox.Content = Settings.globalfiles[i] + "文件不存在，请重新下载资源包或尝试重新操作";
                            error = true;
                            break;
                        }
                        LogBox.Content = Settings.globalfiles[i] + "存在"; 
                    }
                    if(!error)
                    {
                        //开始转换                    
                        MoveFile();
                        //
                        bqload.Visibility = Visibility.Hidden;
                        setSave.IsEnabled = true;
                    }
                    else
                    {
                        this.ShowMessageAsync("提示", "资源解压完成但不完整\r\n请重新下载", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                        bqload.Visibility = Visibility.Hidden;
                        setSave.IsEnabled = true;
                    }
                }));

            }
            else
            {
                this.Dispatcher.Invoke(new Action(delegate ()
                {
                    this.ShowMessageAsync("提示", "没有找到资源[GlobalFile.pkg]或解压失败\r\n请重试或前往下载转换资源包", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                    bqload.Visibility = Visibility.Hidden;
                    ToGlobal.IsEnabled = true;
                    TimeStatus.Content = "当前状态：未找到资源包";
                    setSave.IsEnabled = true;
                }));               
            }
        }
        private void MoveFile()
        {
            Computer redir = new Computer();
            this.Dispatcher.Invoke(new Action(delegate ()
            {
                TimeStatus.Content = "当前状态：正在备份原文件";
                for (int a = 0; a < Settings.cnfiles.Length; a++)
                {
                    String newFileName = System.IO.Path.GetFileNameWithoutExtension(GamePath.Text + "//Genshin Impact Game//" + Settings.cnfiles[a]) + System.IO.Path.GetExtension(GamePath.Text + "//Genshin Impact Game//" + Settings.cnfiles[a]);
                    redir.FileSystem.RenameFile(GamePath.Text + "//Genshin Impact Game//" + Settings.cnfiles[a], newFileName + ".bak");
                    LogBox.Content = newFileName+"备份成功";
                }
            }));
            this.Dispatcher.Invoke(new Action(delegate ()
            {
                TimeStatus.Content = "当前状态：开始替换资源";
                redir.FileSystem.RenameDirectory(GamePath.Text + "//Genshin Impact Game//YuanShen_Data", "GenshinImpact_Data");
                for (int i = 0; i < Settings.globalfiles.Length; i++)
                {
                    File.Copy(@"GlobalFile//" + Settings.globalfiles[i], GamePath.Text + "//Genshin Impact Game//" + Settings.globalfiles[i], true);
                    LogBox.Content = Settings.globalfiles[i] + "替换成功";
                }
                IsGlobal();
                TimeStatus.Content = "当前状态：无状态";
                Config.IniGS.BiOrMi = 3;
                BOM.Sub_channel = 0;
                BOM.Channel = 1;
                ToGlobal.IsEnabled = true;
                this.ShowMessageAsync("提示", "转换完毕，尽情享受吧！~", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            }));
        }
        private void IsGlobal()
        {
            if (File.Exists(Config.Settings.GamePath + "//Genshin Impact Game//YuanShen.exe") == true)
            {
                ToGlobal.Content = "转换";
                GlobalS.IsEnabled = false;
                BIliS.IsEnabled = true;
                MiS.IsEnabled = true;
                MiS.IsChecked = true;

            }
            else if (File.Exists(Config.Settings.GamePath + "//Genshin Impact Game//GenshinImpact.exe") == true)
            {
                ToGlobal.Content = "复原";
                GlobalS.IsChecked = true;
                GlobalS.IsEnabled = true;
                BIliS.IsEnabled = false;
                MiS.IsEnabled = false;
            }
        }
        private void ReCnGame()
        {
            Computer redir = new Computer();
            this.Dispatcher.Invoke(new Action(delegate ()
            {
                TimeStatus.Content = "当前状态：清理现存文件";
                for (int i = 0; i < Settings.globalfiles.Length; i++)
                {
                    File.Delete(GamePath.Text + "//Genshin Impact Game//" + Settings.globalfiles[i]);
                    LogBox.Content = Settings.globalfiles[i] + "清理完毕";
                }
                TimeStatus.Content = "当前状态：正在还原文件";
                redir.FileSystem.RenameDirectory(GamePath.Text + "//Genshin Impact Game//GenshinImpact_Data", "YuanShen_Data");
                for (int a = 0; a < Settings.cnfiles.Length; a++)
                {
                    String newFileName = System.IO.Path.GetFileNameWithoutExtension(GamePath.Text + "//Genshin Impact Game//" + Settings.cnfiles[a])+ System.IO.Path.GetExtension(GamePath.Text + "//Genshin Impact Game//" + Settings.cnfiles[a]); ;
                    redir.FileSystem.RenameFile(GamePath.Text + "//Genshin Impact Game//" + Settings.cnfiles[a] + ".bak", newFileName);
                    LogBox.Content = Settings.cnfiles[a] + "还原成功";
                }
                GlobalS.IsChecked = false;
                GlobalS.IsEnabled = false;
                MiS.IsChecked = true;
                BIliS.IsEnabled = true;
                MiS.IsEnabled = true;
                TimeStatus.Content = "当前状态：无状态";
                Config.IniGS.BiOrMi = 1;
                BOM.Sub_channel = 1;
                BOM.Channel = 1;
                Config.setConfig.checkini();
                bqload.Visibility = Visibility.Hidden;
                setSave.IsEnabled = true;
                ToGlobal.Content = "转换";
                ToGlobal.IsEnabled = true;
                this.ShowMessageAsync("提示", "转换完毕，尽情享受吧！~", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            }));

        }

        private void DownRes_Click(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("提示", "访问密码：etxd\r\n已复制到剪切板", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            Clipboard.SetText("etxd");
            Process.Start("https://pan.baidu.com/s/1-5zQoVfE7ImdXrn8OInKqg");
        }
    }
}