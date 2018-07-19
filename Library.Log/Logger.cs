using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Library.Log
{
    /// <summary>
    /// Logger class
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The local folder
        /// </summary>
        private static StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        /// The log file name
        /// </summary>
        private const string LogFileName = "SudokuApp_log.log";

        /// <summary>
        /// The log file
        /// </summary>
        private static StorageFile LogFile;

        /// <summary>
        /// The ready to log
        /// </summary>
        private static bool ReadyToLog = false;

        /// <summary>
        /// Sets up logger.
        /// </summary>
        public static void SetUpLogger()
        {
            ReadyToLog = Task.Run(async () => await CreateLogFile()).Result;
        }

        /// <summary>
        /// Creates the log file.
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> CreateLogFile()
        {
            try
            {
                LogFile = (StorageFile)await LocalFolder.TryGetItemAsync(LogFileName);
                if(LogFile == null)
                {
                    LogFile = await LocalFolder.CreateFileAsync(LogFileName);
                }
                return true;
            }
            catch(UnauthorizedAccessException)
            {
                Debug.WriteLine("Not authorized to create log file");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task LogAsync(LogLevel level, string message)
        {
            if (!ReadyToLog)
                SetUpLogger();

            try
            {
                Debug.WriteLine($"{DateTime.Now}[{level.ToString()}]: {message}");
                await FileIO.AppendTextAsync(LogFile, $"{DateTime.Now}[{level.ToString()}]: {message}{Environment.NewLine}");
            }
            catch(FileNotFoundException)
            {
                bool logFileExists = await CreateLogFile();
                if (logFileExists)
                    await LogAsync(level, message);
            }
            catch(FormatException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}