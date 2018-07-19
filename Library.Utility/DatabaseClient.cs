using Library.Log;
using Library.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Library.Utility
{
    /// <summary>
    /// DatabaseClient class
    /// </summary>
    public class DatabaseClient
    {
        /// <summary>
        /// The Http Client
        /// </summary>
        private HttpClient Client = new HttpClient();

        /// <summary>
        /// The base URL
        /// </summary>
        private const string BaseUrl = "http://localhost:59590/";

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseClient"/> class.
        /// </summary>
        public DatabaseClient(int timeout = 10)
        {
            Client.Timeout = TimeSpan.FromSeconds(timeout);
        }


        /// <summary>
        /// Gets the register user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<RegisterUser> GetRegisterUserById(int id)
        {
            string response = await GetRequestAsync($"api/RegisterUsers/{id}");
            return await JsonDeserializeAsync<RegisterUser>(response);
        }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public async Task<int> GetUserIdAsync(string username)
        {
            string response = await GetRequestAsync($"api/RegisterUsers/exists/{username}");
            if (response != null)
            {
                RegisterUser user = await JsonDeserializeAsync<RegisterUser>(response);
                if (user == null)
                    return -1;

                return user.UserId;
            }
            return -1;
        }

        /// <summary>
        /// Authenticates the user asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Authenticated true : User exists(may be wrong password) Dependent on UserId
        /// Authenticated false: User does not exists
        /// Authenticated null : No response from server
        /// 
        /// UserId UserId = authenticated, -1 = not authenticated
        /// </returns>
        public async Task<Session> AuthenticateUserAsync(string username, string password)
        {
            var response = await GetRequestDetailAsync($"api/RegisterUsers/exists/{username}");
            string content = response.Content;
            int? status = response.StatusCode;
            
            // User does not exist with that username
            if(status == 204) { return new Session(-1, username, false); }

            // Api can't contact database
            if (status >= 500 && status <= 599) { return new Session(-1, username, false); }

            // Timeout
            if (status == 0) { return new Session(-1, username, null); }

            if (content != null)
            {
                RegisterUser user = await JsonDeserializeAsync<RegisterUser>(content);
                if (user == default(RegisterUser))
                    return new Session(-1, username, null);

                string hashedPassword = Hashing.GetSha256Hash(password, user.Salt);
                if (user.Password == hashedPassword)
                    return new Session(user.UserId, username, true);
                else
                    return new Session(-1, username, true);
            }
            else
                return new Session(-1, username, null);
        }

        /// <summary>
        /// Gets the request detail asynchronous.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <returns></returns>
        private async Task<HttpResponse> GetRequestDetailAsync(string api)
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}{api}");
                string content = await response.Content.ReadAsStringAsync();
                return new HttpResponse(content, (int)response.StatusCode);
            }
            catch (TimeoutException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
            catch (HttpRequestException e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return new HttpResponse(null, null);
        }

        /// <summary>
        /// Gets the request asynchronous.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <returns></returns>
        private async Task<string> GetRequestAsync(string api)
        {
            try
            {
                return await Client.GetStringAsync($"{BaseUrl}{api}");
            }
            catch (TimeoutException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
            catch (HttpRequestException e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return null;
        }

        /// <summary>
        /// Gets the register user exists.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// true: Username is taken
        /// null: No response from server, request timed out.
        /// false: Username is available
        /// </returns>
        public async Task<bool?> GetRegisterUserExistsAsync(string username)
        {
            HttpResponse resp = await GetRequestDetailAsync($"api/RegisterUsers/exists/{username}");
            if (resp.StatusCode == 200)
                return true;
            else if (resp.StatusCode == null)
                return null;
            else
                return false;
        }

        /// <summary>
        /// Posts the request asynchronous.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        private async Task<HttpResponse> PostRequestAsync(string api, HttpContent content)
        {
            try
            {
                HttpResponseMessage response = await Client.PostAsync($"{BaseUrl}{api}", content);
                string responseContent = await response.Content.ReadAsStringAsync();
                return new HttpResponse(responseContent, (int)response.StatusCode);
            }
            catch (TimeoutException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
            catch (HttpRequestException e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return new HttpResponse(null, null);
        }

        /// <summary>
        /// Deletes the request asynchronous.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <returns></returns>
        private async Task<bool> DeleteRequestAsync(string api)
        {
            try
            {
                HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}{api}");
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 200 || statusCode <= 299)
                    return true;
                else
                    return false;
            }
            catch (TimeoutException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
            catch (HttpRequestException e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return false;
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<bool> RegisterUserAsync(RegisterUser user)
        {
            StringContent content = await JsonSerializeAsync(user);
            if (content == null)
                return false;

            HttpResponse response = await PostRequestAsync($"api/RegisterUsers/{user.UserId}", content);
            if (response.StatusCode == 201)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Puts the request asynchronous.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        private async Task<HttpResponse> PutRequestAsync(string api, HttpContent content)
        {
            try
            {
                HttpResponseMessage response = await Client.PutAsync($"{BaseUrl}{api}", content);
                string responseContent = await response.Content.ReadAsStringAsync();
                return new HttpResponse(responseContent, (int)response.StatusCode);
            }
            catch (TimeoutException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
            catch (HttpRequestException e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return new HttpResponse(null, null);
        }

        /// <summary>
        /// Registers the puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<bool> RegisterPuzzleAsync(SudokuPuzzle puzzle, int userId)
        {
            StringContent sudokuPuzzleContent = await JsonSerializeAsync(puzzle);
            if (sudokuPuzzleContent == null)
                return false;

            HttpResponse response = await PostRequestAsync($"api/SudokuPuzzles/add/{userId}", sudokuPuzzleContent);
            return (response.StatusCode == 200) ? true : false;
        }

        /// <summary>
        /// Gets the user sudoku puzzles.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<List<SudokuPuzzle>> GetUserSudokuPuzzlesAsync(int userId)
        {
            string content;
            try
            {
                content = await GetRequestAsync($"api/SudokuPuzzles/Users/{userId}");
                content = content.Substring(1, content.Length - 2);
            }
            catch(ArgumentOutOfRangeException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
                return null;
            }

            return await JsonDeserializeAsync<List<SudokuPuzzle>>(content);
        }

        /// <summary>
        /// Deletes the user asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await DeleteRequestAsync($"api/RegisterUsers/{id}");
        }

        /// <summary>
        /// Deletes the puzzles asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeletePuzzlesAsync(int userId)
        {
            var userPuzzles = await GetUserSudokuPuzzlesAsync(userId);
            try
            {
                List<SudokuPuzzle> puzzles = (List<SudokuPuzzle>)userPuzzles;
                if (puzzles.Count > 0)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (SudokuPuzzle puzzle in puzzles)
                    {
                        tasks.Add(Task.Run(() => DeleteRequestAsync($"api/SudokuPuzzles/{puzzle.SudokuPuzzleId}")));
                    }
                    await Task.WhenAll(tasks);
                }
                return true;
            }
            catch (TimeoutException e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
            catch (HttpRequestException e)
            {
                await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return false;
        }

        /// <summary>
        /// Puts the change username asynchronous.
        /// A check if the user exists is done beforehand
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="newUsername">The new username.</param>
        /// <returns></returns>
        public async Task<bool> PutChangeUsernameAsync(Session session, string newUserName)
        {
            HttpResponse response = await PutRequestAsync($"api/RegisterUsers/{session.UserId}/{newUserName}", null);
            if (response.StatusCode == 200)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Jsons the deserialize asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObj">The json object.</param>
        /// <returns></returns>
        private async Task<T> JsonDeserializeAsync<T>(string jsonObj)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonObj);
            }
            catch(JsonSerializationException e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return default(T);
        }

        /// <summary>
        /// Jsons the serialize asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private async Task<StringContent> JsonSerializeAsync<T>(T obj)
        {
            try
            {
                return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            }
            catch (JsonSerializationException e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
            }
            return null;
        }
    }
}