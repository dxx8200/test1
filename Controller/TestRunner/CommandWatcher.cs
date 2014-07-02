using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Controller.TestRunner
{
    public class CommandEventArgs : EventArgs
    {
        private string _msg;

        public string Message
        {
            get { return _msg; }
        }

        public CommandEventArgs(string Msg)
        {
            _msg = Msg;
        }
    }

    public class CommandWatcher
    {


        public event EventHandler<CommandEventArgs> CommandHandler;

        #region Fields
        private string fileName;
        private FileSystemWatcher watcher;
        private DateTime lastModifiedTime;
        private long lastFileSize;
        private bool reloadingFile;
        private bool isChangeImmediate;

        #endregion Fields


        public CommandWatcher(string cmdfile)
        {
            this.fileName = cmdfile;
            isChangeImmediate = true;


        }

        //
        // Summary:
        //     Raises the System.IO.FileSystemWatcher.Changed event.
        //
        // Parameters:
        //   e:
        //     A System.IO.FileSystemEventArgs that contains the event data.
        protected void OnChanged(object sender, FileSystemEventArgs e)
        {
            ReloadFile();
        }



        //
        // Summary:
        //     Raises the System.IO.FileSystemWatcher.Error event.
        //
        // Parameters:
        //   e:
        //     An System.IO.ErrorEventArgs that contains the event data.
        protected void OnError(object sender, ErrorEventArgs e)
        {

        }

        //
        // Summary:
        //     Raises the System.IO.FileSystemWatcher.Renamed event.
        //
        // Parameters:
        //   e:
        //     A System.IO.RenamedEventArgs that contains the event data.
        protected void OnRenamed(object sender, RenamedEventArgs e)
        {

        }



        /// <summary>
        /// Hook up for the currently selected monitoring method.
        /// </summary>
        public void Start()
        {
            if (isChangeImmediate)
            {
                // If a file is set and the watcher has not been set up yet then create it
                if (null != fileName && null == watcher)
                {
                    string path = Path.GetDirectoryName(fileName);
                    string baseName = Path.GetFileName(fileName);
                    // Use the FileSystemWatcher class to detect changes to the log file immediately
                    // We must watch for Changed, Created, Deleted, and Renamed events to cover all cases
                    watcher = new FileSystemWatcher(path, baseName);
                    FileSystemEventHandler handler = new FileSystemEventHandler(OnChanged);
                    watcher.Changed += handler;
                    watcher.Created += handler;
                    watcher.Deleted += handler;
                    watcher.Renamed += OnRenamed;
                    // Without setting EnableRaisingEvents nothing happens
                    watcher.EnableRaisingEvents = true;
                }
            }
            this.ReloadFile();
        }

        /// <summary>
        /// Read the log file.
        /// </summary>
        /// <remarks>If the user has some text selected then keep that text selected.</remarks>
        private void ReloadFile()
        {
            if (reloadingFile) return; // already busy doing this
            reloadingFile = true;
            try
            {
                if (null == fileName)
                {
                    // textBoxContents.Text = "";
                }
                else
                {
                    string newFileLines = "";
                    lastModifiedTime = File.GetLastWriteTime(fileName); // Remember when we last read in this file
                    long newLength = 0;  // will be set properly later if all is well
                    bool fileExists = File.Exists(fileName);
                    bool needToClear = !fileExists;
                    if (fileExists)
                    {
                        // It is possible the file will be in use when we read it (especially if using Immediate mode)
                        // So, we will try up to 5 times to access the file
                        int count = 0;
                        bool success = false;
                        while (count < 5 && !success)
                        {
                            try
                            {
                                // Open with the least amount of locking possible
                                using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                                {
                                    newLength = stream.Length;
                                    if (newLength >= lastFileSize)
                                    {
                                        stream.Position = lastFileSize;  // Only read in new lines added
                                    }
                                    else
                                    {
                                        needToClear = true;
                                    }
                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        newFileLines = reader.ReadToEnd();

                                        if (newFileLines != string.Empty)
                                        {
                                            if (this.CommandHandler != null)
                                                this.CommandHandler(this, new CommandEventArgs(newFileLines));
                                        }

                                        //string []tmp = newFileLines.Split(seprater, StringSplitOptions.RemoveEmptyEntries);

                                    }
                                }
                                success = true;
                            }
                            catch (IOException)
                            {
                                System.Threading.Thread.Sleep(50); // Give a little while to free up file
                            }
                            ++count;
                        }
                    }
                    // Remember the current file length so we can read only newly added log lines the next time the log file is read
                    lastFileSize = newLength;
                    if (lastFileSize > 1024 * 1024 * 2)
                    {
                        File.Delete(fileName);
                        File.Create(fileName);
                    }

                    if (0 != newFileLines.Length)
                    {
                        // See if this log file has proper cr/lf and if not convert
                        int lastCr = newFileLines.LastIndexOf('\n');
                        if (-1 != lastCr && 0 < lastCr)
                        {
                            if (newFileLines[lastCr - 1] != '\r')
                            {
                                // OK, this file only has Cr and we need to convert to CrLf
                                newFileLines = newFileLines.Replace("\n", "\r\n");
                            }
                        }
                    }


                }
            }
            finally
            {
                // Put clearing this flag in a finally so we know it will be cleared
                reloadingFile = false;
            }
        }

        /// <summary>
        /// Cleanup the <see cref="FileSystemWatcher"/>
        /// </summary>
        private void ClearWatcher()
        {
            if (null != watcher)
            {
                FileSystemEventHandler handler = new FileSystemEventHandler(OnChanged);
                watcher.Changed -= handler;
                watcher.Created -= handler;
                watcher.Deleted -= handler;
                watcher.Renamed -= OnRenamed;
                watcher.Dispose();
                watcher = null;
            }
        }



    }
}
