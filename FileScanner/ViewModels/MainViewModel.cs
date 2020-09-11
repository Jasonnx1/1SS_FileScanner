using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();


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

        public ObservableCollection<string> FolderItems { 
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

            foreach (var item in Directory.EnumerateFiles(dir, "*"))
            {
                 FolderItems.Add(item);
            }

            StopWatch = watch.ElapsedMilliseconds;

        }


        ObservableCollection<string> GetDirFiles(string dir)
        {
            ObservableCollection<string> infos = new ObservableCollection<String>();

            foreach (var d in Directory.EnumerateDirectories(dir, "*"))
            {
                infos.Add(d);

                foreach (var f in Directory.EnumerateFiles(d, "*"))
                {
                    infos.Add(f);
                }

            }

            return infos;
        }

        ///TODO : Tester avec un dossier avec beaucoup de fichier
        ///TODO : Rendre l'application asynchrone
        ///TODO : Ajouter un try/catch pour les dossiers sans permission


    }
}
