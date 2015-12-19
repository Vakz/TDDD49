using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Game.State
{
    public class SaveHandler
    {
        DateTime lastSave = DateTime.MinValue;
        string filename = Directory.GetCurrentDirectory() + "\\Resources\\save.xml";
        FileSystemWatcher watcher = new FileSystemWatcher();

        public void Save(GameState state)
        {
            // Disable the watcher while saving, to avoid our own save
            // causing the game to reload
            watcher.EnableRaisingEvents = false;
            Saver.Save(filename, state);
            lastSave = File.GetLastWriteTime(filename);
            watcher.EnableRaisingEvents = true;
        }

        public GameState Load()
        {
            return Loader.Load(filename);
        }

        public SaveHandler()
        {
            watcher.Path = Path.GetDirectoryName(filename);
            watcher.Filter = "*.xml";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e) {
            DateTime newEdit = File.GetLastWriteTime(filename);
            if (newEdit.CompareTo(lastSave) > 0)
            {
                lastSave = newEdit.AddSeconds(3);
                if (OnManualSaveChange != null)
                    OnManualSaveChange();
            }
        }

        public Action OnManualSaveChange { get; set; }
    }
}
