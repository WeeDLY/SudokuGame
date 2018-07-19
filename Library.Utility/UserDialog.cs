using Library.Log;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Library.Utility
{
    /// <summary>
    /// Class to show UserDialog boxes to the user.
    /// </summary>
    public class UserDialog
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="UserDialog"/> class from being created.
        /// </summary>
        private UserDialog()
        {

        }

        /// <summary>
        /// Shows the message dialog asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        public static async void ShowMessageDialogAsync(string title, string message)
        {
            try
            {
                var dialog = new MessageDialog(message, title);
                await dialog.ShowAsync();
            }
            catch(Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
        }

        /// <summary>
        /// Shows the message dialog options asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task<UserDialogResponse> ShowMessageDialogOptionsAsync(string title, string message)
        {
            try
            {
                var dialog = new MessageDialog(message, title)
                {
                    Options = MessageDialogOptions.AcceptUserInputAfterDelay
                };
                dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
                dialog.Commands.Add(new UICommand("No") { Id = 1 });
                dialog.Commands.Add(new UICommand("Cancel") { Id = 2 });
                var result = await dialog.ShowAsync();
                return (UserDialogResponse)result.Id;
            }
            catch(ArgumentOutOfRangeException e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return UserDialogResponse.Cancel;
        }
    }
}