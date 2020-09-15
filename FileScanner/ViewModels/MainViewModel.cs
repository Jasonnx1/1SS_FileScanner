using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using FileScanner.Models;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<Item> folderItems;



        private float stopwatch { get; set; }
        public float StopWatch {
            get => stopwatch; 
            set
            {
                stopwatch = value;
                OnPropertyChanged();
            }
        }
         



        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }




        public ObservableCollection<Item> FolderItems { 
            get => folderItems;
            set 
            { 
                folderItems = value;
                OnPropertyChanged();
            }
        }




        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }




        public MainViewModel()
        {
            FolderItems = new ObservableCollection<Item>();
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolder, CanExecuteScanFolder);
            StopWatch = 0;
        }




        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }





        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }





        private async void ScanFolder(string dir)
        {
            var watch = Stopwatch.StartNew();

            FolderItems = await Task.Run(() => GetDirFiles(dir));

            StopWatch = watch.ElapsedMilliseconds;

        }






        ObservableCollection<Item> GetDirFiles(string dir)
        {
            ObservableCollection<Item> infos = new ObservableCollection<Item>();

            try
            {

                foreach (var d in Directory.EnumerateDirectories(dir, "*"))
                {
                    infos.Add(new Item { Value = d, Type = "/images/folder.png" });

                    foreach (var f in Directory.EnumerateFiles(d, "*"))
                    {
                        infos.Add(new Item { Value = f, Type = "/images/file.png" });
                    }

                }

                foreach (var d in Directory.EnumerateFiles(dir, "*"))
                {
                    infos.Add(new Item { Value = d, Type = "/images/file.png" });

                }

            }
            catch (Exception e) { System.Windows.MessageBox.Show(e.Message,"Error", MessageBoxButton.OK); }


            return infos;
        }

        ///TODO : Tester avec un dossier avec beaucoup de fichier
        ///TODO : Rendre l'application asynchrone
        ///TODO : Ajouter un try/catch pour les dossiers sans permission


    }
}
