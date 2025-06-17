using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Web;
using TestConsole.Helper;

namespace TestConsole.Programs
{
    public class Ezugi : IProgram
    {
        private readonly HttpHelper _httpHelper;
        private int _operatorId = 11061001;
        private long _previousRoundId;
        private string _previousTransactionId;
        private double _previousStake;
        private double _previousBalance;
        private string _transactionToken;
        private string _previousCreditTransactionId;
        private string _baseUrl = "https://capi-uat-ezugi.techbodia.dev";
        private List<BetReport> _reports = new List<BetReport>();
        private string _username = "";
        private string _currency = "";
        private List<double> _expectedBalances = new List<double>();

        //private string _baseUrl = "https://localhost:7128";

        public Ezugi()
        {
            _httpHelper = new HttpHelper();
        }

        public async Task TestFinancial()
        {
            Console.Clear();
            var loginURL = await GetLoginUrl();
            await RunProviderTest(loginURL);
        }
        private async Task RunProviderTest(string loginToken)
        {
            var awaitSecond = TimeSpan.FromSeconds(0);
            var testSteps = new List<Func<string, Task<bool>>>
           {
               NormalAuthentication,
               RepeatedAuthentication,
               Debit,
               RetryForDebit,
               Rollback,
               RetryForRollback,
               RollbackBeforeDebit,
               DebitAfterRollback,
               Debit,
               Credit,
               RetryCredit,
               DebitWith0Amount,
               CreditWith0Amount1,
               InsufficientFunds,
               DebitWithWrongToken,
               DebitWithUnknowUser,
               DebitWithNegativeDebitAmount,
               DebitAllPlayerBallance,
               Rollback,
               Debit,
               RollbackWithWrongAmount,
               Rollback,
               Debit,
               CreditWithoutDebitTransactionId,
               Debit,
               CreditWithDebitTransactionIdWhichNeverWasProcessed,
               Debit,
               Credit,
               CreditWithDebitTransactionIdWhichAlreadyWasProcessed,
               InvalidHash,
               RequestWithNoHash,
               DebitWithMissingMandatoryParametersInRequest,
               InvalidCurrency,
           };

            foreach (var step in testSteps)
            {
                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine($"Running test step: {step.Method.Name}");
                if (!await step(loginToken))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Test step {step.Method.Name} failed. Await for {awaitSecond}seconds");
                    Console.ResetColor();
                    return;
                }
                Console.WriteLine($"Test step {step.Method.Name} completed successfully.| Balance: {_previousBalance} Await for {awaitSecond}seconds");
                _previousBalance = Get2DigitsAfterDecimalPoint(_previousBalance);
                await Task.Delay(awaitSecond);
            }
        }

        private async Task<bool> DebitAllPlayerBallance(string arg)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = _previousBalance,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;
            _previousStake = request.DebitAmount;

            var response = await PlaceBet(request);

            //PushResultToReport(response, request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != Get2DigitsAfterDecimalPoint(_previousBalance - request.DebitAmount))
            {
                Console.WriteLine($"Balance should be {Get2DigitsAfterDecimalPoint(_previousBalance - request.DebitAmount)}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine($"Timestamp should responce bigger than request Timestamp response: {response.Timestamp}, request: {request.Timestamp}. smaller by: {(DateTimeOffset.FromUnixTimeMilliseconds(response.Timestamp) - DateTimeOffset.FromUnixTimeMilliseconds(request.Timestamp)).TotalMilliseconds}ms");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private async Task<bool> InvalidCurrency(string arg)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = "USD",
                DebitAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 24)
            {
                Console.WriteLine($"ErrorCode should be 24 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Request currency does not match session currency")
            {
                Console.WriteLine($"Error description should be: \"Request currency does not match session currency\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != 0)
            {
                Console.WriteLine($"Balance should be {0}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> DebitWithMissingMandatoryParametersInRequest(string arg)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 1.99,
                GameId = 1,
                //OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be 1 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Mandatory parameter missing from requestBody")
            {
                Console.WriteLine($"Error description should be: \"Mandatory parameter missing from requestBody\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.TransactionId != null || response.RoundId != 0 || response.Currency != null)
            {
                Console.WriteLine($"should responce with base response {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> RequestWithNoHash(string arg)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request, "asdfghjkjhgfdfghjfdsfsd", false);

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be 1 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Invalid Hash")
            {
                Console.WriteLine($"Error description should be: \"Invalid Hash\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != 0)
            {
                Console.WriteLine($"Balance should be {0}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> InvalidHash(string arg)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request, "asdfghjkjhgfdfghjfdsfsd");

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be 1 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Invalid Hash")
            {
                Console.WriteLine($"Error description should be: \"Invalid Hash\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != 0)
            {
                Console.WriteLine($"Balance should be {0}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private double Get2DigitsAfterDecimalPoint(double amount)
        {
            var numberParts = amount.ToString().Split('.');
            if (numberParts.Length == 2 && numberParts[1].Length >= 2)
            {
                numberParts[1] = numberParts[1].Substring(0, 2);
            }
            if(numberParts.Length > 1)
                return Convert.ToDouble($"{numberParts[0]}.{numberParts[1]}");
            return amount;
        }

        private async Task<bool> CreditWithDebitTransactionIdWhichAlreadyWasProcessed(string arg)
        {
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = "c" + (GetTransactionId()),
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = _previousTransactionId,
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var response = await Settle(request);

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be = 1 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Debit transaction already processed")
            {
                Console.WriteLine($"Error description should be: \"Debit transaction already processed\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> CreditWithDebitTransactionIdWhichNeverWasProcessed(string arg)
        {
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = "c" + (_previousTransactionId.Substring(1)),
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = _previousCreditTransactionId+"adf",
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var response = await Settle(request);

            if (response.ErrorCode != 9)
            {
                Console.WriteLine($"ErrorCode should be = 9 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Debit transaction ID not found")
            {
                Console.WriteLine($"Error description should be: \"Debit transaction ID not found\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> CreditWithoutDebitTransactionId(string arg)
        {
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = "c" + (_previousTransactionId.Substring(1)),
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = "",
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var response = await Settle(request);

            if (response.ErrorCode != 9)
            {
                Console.WriteLine($"ErrorCode should be = 9 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Debit transaction ID not found")
            {
                Console.WriteLine($"Error description should be: \"Debit transaction ID not found\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> RollbackWithWrongAmount(string loginToken)
        {
            var request = new ProviderCancelRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                RollbackAmount = _previousStake + 10,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = _previousTransactionId,
                Uid = _username,
            };

            var response = await Cancel(request);

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be 1, response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Invalid amount")
            {
                Console.WriteLine($"Error description should be: \"Invalid amount\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<string> GetLoginUrl()
        {
            Console.WriteLine("Select player credentials: [1] [2] [3]");
            var jsonRequest = "";
            while (string.IsNullOrEmpty(jsonRequest))
            {
                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    jsonRequest = GetPlayerCredential1();
                }
                else if (choice == "2")
                {
                    jsonRequest = GetPlayerCredential2();
                }
                else if (choice == "3")
                {
                    jsonRequest = GetPlayerCredential3();
                }
                else
                { 
                    Console.WriteLine("Invalid choice. Select player credentials: [1] [2] [3]");
                }
            }

            var url = $"{_baseUrl}/Provider/Login";
             
            var response = await _httpHelper.PostAsync(url, jsonRequest);
            var jsonDoc = JsonDocument.Parse(response);
            if (jsonDoc.RootElement.TryGetProperty("result", out var resultElement))
            {
                var result = resultElement.GetString();
                // Extract query part of the URL
                var uri = new Uri(result);
                var query = uri.Query;

                // Parse query parameters
                var queryParams = HttpUtility.ParseQueryString(query);
                string token = queryParams["token"];

                Console.WriteLine($"Login Token: {token}");
                return token;
            }
            else
            {
                Console.WriteLine("Token not found in the response.");
                return string.Empty;
            }
        }

        private string GetPlayerCredential3()
        {
            return @"{""User"":""2468sbons520169"",""ProcessLoginModel"":{""GameId"":0,""GpId"":1088,""Lang"":1,""IsPlayForReal"":true,""Device"":""d"",""IsApp"":false,""IsLoginToSpecificGame"":false,""GameCode"":"""",""GameHall"":"""",""HomeUrl"":"""",""BetCode"":"""",""Ip"":""163.47.15.15"",""MarsDomain"":""lmd-uat.gaolitsai.com"",""UserAgent"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36 Edg/136.0.0.0""},""PlayerLoginInfoModel"":{""CustomerId"":520169,""IsTest"":0,""PlayerInfo"":{""CustomerID"":520169,""AccountID"":""2468yy_lcg_thb"",""Credit"":0,""Outstanding"":0.0,""CashBalance"":0.0,""Reward"":0,""Currency"":""THB"",""Status"":0,""DisplayName"":null,""Ladder"":0,""Experience"":0,""LastLoginIP"":""Mirana Ip"",""LastLoginTime"":null,""PasswordExpiryDate"":null,""CanChangeDisplayName"":false,""CanChangeLoginName"":false,""FirstTimeSignOn"":false,""ProductAvailable"":null,""TableLimit"":0,""OddsStyle"":0,""WebId"":2468}}}";
        }

        private string GetPlayerCredential2()
        {
            return @"{""User"":""2468sbons520081"",""ProcessLoginModel"":{""GameId"":0,""GpId"":1088,""Lang"":1,""IsPlayForReal"":true,""Device"":""d"",""IsApp"":false,""IsLoginToSpecificGame"":false,""GameCode"":"""",""GameHall"":"""",""HomeUrl"":"""",""BetCode"":"""",""Ip"":""163.47.15.15"",""MarsDomain"":""lmd-uat.gaolitsai.com"",""UserAgent"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36 Edg/136.0.0.0""},""PlayerLoginInfoModel"":{""CustomerId"":520081,""IsTest"":0,""PlayerInfo"":{""CustomerID"":520081,""AccountID"":""2468yy_pinplayerTMP4"",""Credit"":0,""Outstanding"":0.0,""CashBalance"":0.0,""Reward"":0,""Currency"":""TMP"",""Status"":0,""DisplayName"":null,""Ladder"":0,""Experience"":0,""LastLoginIP"":""Mirana Ip"",""LastLoginTime"":null,""PasswordExpiryDate"":null,""CanChangeDisplayName"":false,""CanChangeLoginName"":false,""FirstTimeSignOn"":false,""ProductAvailable"":null,""TableLimit"":0,""OddsStyle"":0,""WebId"":2468}}} ";
        }

        private string GetPlayerCredential1()
        {
            return @"{
                                	""User"": ""2033sbons490854"",
                                	""ProcessLoginModel"": {
                                		""GameId"": 0,
                                		""GpId"": 1088,
                                		""Lang"": 1,
                                		""IsPlayForReal"": true,
                                		""Device"": ""d"",
                                		""IsApp"": false,
                                		""IsLoginToSpecificGame"": false,
                                		""GameCode"": """",
                                		""GameHall"": """",
                                		""HomeUrl"": """",
                                		""BetCode"": """",
                                		""Ip"": ""163.47.15.15"",
                                		""MarsDomain"": ""lmd-uat.gaolitsai.com"",
                                		""User Agent"": ""Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36""
                                	},
                                	""PlayerLoginInfoModel"": {
                                		""CustomerId"": 490854,
                                		""IsTest"": 0,
                                		""PlayerInfo"": {
                                			""CustomerID"": 490854,
                                			""AccountID"": ""2033yy_QATestTHB"",
                                			""Credit"": 0,
                                			""Outstanding"": 0.0,
                                			""CashBalance"": 0.0,
                                			""Reward"": 0,
                                			""Currency"": ""THB"",
                                			""Status"": 0,
                                			""DisplayName"": null,
                                			""Ladder"": 0,
                                			""Experience"": 0,
                                			""LastLoginIP"": ""Mirana Ip"",
                                			""LastLoginTime"": null,
                                			""PasswordExpiryDate"": null,
                                			""CanChangeDisplayName"": false,
                                			""CanChangeLoginName"": false,
                                			""FirstTimeSignOn"": false,
                                			""ProductAvailable"": null,
                                			""TableLimit"": 0,
                                			""OddsStyle"": 0,
                                			""WebId"": 2033
                                		}
                                	}
                                }";
        }

        private async Task<bool> DebitWithNegativeDebitAmount(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = -1.90,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be 1, response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Negative amount")
            {
                Console.WriteLine($"Error description should be: \"Negative amount\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> DebitWithUnknowUser(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 0,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = "2033sbons49085433",
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 7)
            {
                Console.WriteLine($"ErrorCode should be 7, response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "User not found")
            {
                Console.WriteLine($"Error description should be: \"Token not found\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != 0)
            {
                Console.WriteLine($"Balance should be {0}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> DebitWithWrongToken(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 0,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken.Substring(0,loginToken.Length - 1) + "r",
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 6)
            {
                Console.WriteLine($"ErrorCode should be 6, response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Token not found")
            {
                Console.WriteLine($"Error description should be: \"Token not found\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != 0)
            {
                Console.WriteLine($"Balance should be {0}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> InsufficientFunds(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = _previousBalance + 1,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 3)
            {
                Console.WriteLine($"ErrorCode should be 3, response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Insufficient funds")
            {
                Console.WriteLine($"Error description should be: \"Insufficient funds\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> CreditWith0Amount1(string loginToken)
        {
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = 0,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = "c" + (_previousTransactionId.Substring(1)),
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = _previousTransactionId,
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var response = await Settle(request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"ErrorCode should be = 0 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Completed successfully")
            {
                Console.WriteLine($"Error description should be: \"Completed successfully\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (Math.Abs(response.Balance - (_previousBalance + request.CreditAmount)) > 0.02)
            {
                Console.WriteLine($"Balance should be {_previousBalance + request.CreditAmount}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private async Task<bool> DebitWith0Amount(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await PlaceBet(request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error should be 0: response {JsonConvert.SerializeObject(response.ErrorDescription)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if ( Math.Abs(response.Balance - (_previousBalance - request.DebitAmount)) > 0.03)
            {
                Console.WriteLine($"Balance should be {_previousBalance - request.DebitAmount}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private async Task<bool> RetryCredit(string loginToken)
        {
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = _previousStake+1,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = _previousCreditTransactionId,
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = _previousTransactionId,
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var response = await Settle(request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"ErrorCode should be = 0 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription.ToLower() != "Transaction has already processed".ToLower())
            {
                Console.WriteLine($"Error description should be: \"Transaction has already processed\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> Credit(string loginToken, double settleAmount = 0, string transactionId = null)
        {
            var transactionIdToUse = (transactionId != null ? transactionId : _previousTransactionId);
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = settleAmount,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = "c"+(transactionIdToUse.Substring(1)),
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = transactionIdToUse,
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var response = await Settle(request);
            var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var deltaMS = Convert.ToInt32(endTime - startTime);

            PushResultToReport(response, request, deltaMS);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"ErrorCode should be = 0 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Completed successfully")
            {
                Console.WriteLine($"Error description should be: \"Completed successfully\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            var delta = Math.Abs((_previousBalance + request.CreditAmount) - response.Balance);
            if (response.Balance != Get2DigitsAfterDecimalPoint(_previousBalance + request.CreditAmount) && delta > 0.1)
            {
                Console.WriteLine($"Balance should be {Get2DigitsAfterDecimalPoint(_previousBalance + request.CreditAmount)}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private async Task<bool> Credit(string loginToken)
        {
            var transactionIdToUse =_previousTransactionId;
            var request = new ProviderSettleRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                CreditAmount = (_previousStake + 1),
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = "c" + (transactionIdToUse.Substring(1)),
                Uid = _username,
                CreditIndex = "1|1",
                DebitTransactionId = transactionIdToUse,
                GameDataString = "",
                IsEndRound = true,
                ReturnReason = 0
            };
            _previousCreditTransactionId = request.TransactionId;

            var response = await Settle(request);

            //PushResultToReport(response, request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"ErrorCode should be = 0 response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription != "Completed successfully")
            {
                Console.WriteLine($"Error description should be: \"Completed successfully\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            var delta = Math.Abs((_previousBalance + request.CreditAmount) - response.Balance);
            if (response.Balance != Get2DigitsAfterDecimalPoint(_previousBalance + request.CreditAmount) && delta > 0.1)
            {
                Console.WriteLine($"Balance should be {Get2DigitsAfterDecimalPoint(_previousBalance + request.CreditAmount)}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private void PushResultToReport(ProviderSettleResponse response, ProviderSettleRequest request, int deltaMS)
        {
            var newReport = new BetReport
            {
                Operater = response.OperatorId,
                User = response.Uid,
                GameId = request.GameId,
                Table = request.TableId,
                Seat = request.SeatId,
                BetType = request.BetTypeID,
                Win = request.CreditAmount,
                Currency = request.Currency,
                DebitTransactionId = request.TransactionId,
                BalanceIndex = GetBalanceIndex(response.Balance),
                TimeStampIndex = GetTimeStampIndex(response.Timestamp),
                Balance = response.Balance,
                TimeStamp = response.Timestamp.ToString(),
                ExpectedBalance = 0,
                Ping = deltaMS.ToString(),
            };
            var isAddSuccess = false;
            while (!isAddSuccess)
            {
                try
                {
                    _reports = _reports.OrderBy(report => report.TimeStamp).ToList();
                    _reports.Add(newReport);
                    isAddSuccess = true;
                }
                catch
                {

                }
            }
        }

        private int GetTimeStampIndex(long timestamp)
        {
            var report = _reports.FirstOrDefault(report => report.TimeStamp.Equals(timestamp)) ?? null;
            if (report != null)
            {
                return report.TimeStampIndex;
            }
            else
            {
                return _reports.Count + 1;
            }
        }

        private int GetBalanceIndex(double balance)
        {
            var marchBalance = _expectedBalances.FirstOrDefault(b => balance == b);
            if(marchBalance != null)
            {
                return _expectedBalances.IndexOf(marchBalance);
            }
            else
            {
                return _reports.Count;
            }
        }

        private async Task<bool> DebitAfterRollback(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = _previousTransactionId,
                Uid = _username,
            };

            var response = await PlaceBet(request);

            if (response.ErrorCode != 1)
            {
                Console.WriteLine($"ErrorCode should be = 1. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorDescription.ToLower() != "Debit after rollback".ToLower())
            {
                Console.WriteLine($"Error description should be: \"Debit after rollback\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> RollbackBeforeDebit(string loginToken)
        {
            var request = new ProviderCancelRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                RollbackAmount = 1.99,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;

            var response = await Cancel(request);

            if (response.ErrorCode != 9)
            {
                Console.WriteLine($"ErrorCode should be = 9. Error: {response.ErrorDescription}");
                return false;
            }
            if (response.ErrorDescription != "Transaction not found")
            {
                Console.WriteLine($"Error description should be: \"Transaction not found\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> RetryForRollback(string loginToken)
        {
            var request = new ProviderCancelRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                RollbackAmount = _previousStake,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = _previousTransactionId,
                Uid = _username,
            };

            var response = await Cancel(request);
            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.ErrorDescription != "Transaction has already processed")
            {
                Console.WriteLine($"Error description should be: \"Transaction has already processed\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine("Timestamp should responce bigger than request Timestamp");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private async Task<bool> Rollback(string loginToken)
        {
            var request = new ProviderCancelRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                RollbackAmount = _previousStake,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = _previousTransactionId,
                Uid = _username,
            };

            var response = await Cancel(request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.ErrorDescription != "Completed successfully")
            {
                Console.WriteLine($"Error description should be: \"Completed successfully\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            var delta = Math.Abs(response.Balance - (_previousBalance + request.RollbackAmount));
            if (response.Balance != Get2DigitsAfterDecimalPoint(_previousBalance + request.RollbackAmount) && delta >= 0.1)
            {
                Console.WriteLine($"Balance should be {Get2DigitsAfterDecimalPoint(_previousBalance + request.RollbackAmount)}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine($"Timestamp {response.Timestamp} should responce bigger than request Timestamp {request.Timestamp}");
                return false;
            }
            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private async Task<bool> RetryForDebit(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = _previousStake,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = _previousRoundId,
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = _previousTransactionId,
                Uid = _username,
            };

            var response = await PlaceBet(request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.ErrorDescription != "Transaction has already processed")
            {
                Console.WriteLine($"Error description should be: \"Transaction has already processed\". response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != _previousBalance)
            {
                Console.WriteLine($"Balance should be {_previousBalance}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }

            Pass(); return true;
        }

        private async Task<bool> Debit(string loginToken, int betAmount = 1)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = betAmount,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;
            _previousStake = request.DebitAmount;

            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var response = await PlaceBet(request);
            var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var deltaMS = Convert.ToInt32(endTime - startTime);

            PushResultToReport(response, request, deltaMS);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != Get2DigitsAfterDecimalPoint(_previousBalance - request.DebitAmount))
            {
                Console.WriteLine($"Balance should be {Get2DigitsAfterDecimalPoint(_previousBalance - request.DebitAmount)}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine($"Timestamp should responce bigger than request Timestamp response: {response.Timestamp}, request: {request.Timestamp}");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            _previousBalance = response.Balance;
            Pass(); return true;
        }
        private async Task<bool> Debit(string loginToken)
        {
            var request = new ProviderPlaceBetRequest
            {
                BetTypeID = 1,
                Currency = _currency,
                DebitAmount = 1,
                GameId = 1,
                OperatorId = _operatorId,
                PlatformId = 0,
                RoundId = GetRoundId(),
                SeatId = "s6",
                ServerId = 102,
                TableId = 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Token = _transactionToken,
                TransactionId = GetTransactionId(),
                Uid = _username,
            };
            _previousRoundId = request.RoundId;
            _previousTransactionId = request.TransactionId;
            _previousStake = request.DebitAmount;

            var response = await PlaceBet(request);

            //PushResultToReport(response, request);

            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.RoundId != request.RoundId)
            {
                Console.WriteLine($"RoundId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.TransactionId != request.TransactionId)
            {
                Console.WriteLine($"TransactionId should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Balance != Get2DigitsAfterDecimalPoint(_previousBalance - request.DebitAmount))
            {
                Console.WriteLine($"Balance should be {Get2DigitsAfterDecimalPoint(_previousBalance - request.DebitAmount)}. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine($"Timestamp should responce bigger than request Timestamp response: {response.Timestamp}, request: {request.Timestamp}. smaller by: {(DateTimeOffset.FromUnixTimeMilliseconds(response.Timestamp) - DateTimeOffset.FromUnixTimeMilliseconds(request.Timestamp)).TotalMilliseconds}ms");
                return false;
            }
            if (response.Token != request.Token)
            {
                Console.WriteLine($"Token should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.Uid != request.Uid)
            {
                Console.WriteLine($"Uid should be the same. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            _previousBalance = response.Balance;
            Pass(); return true;
        }

        private void PushResultToReport(ProviderPlaceBetResponse response, ProviderPlaceBetRequest request, int deltaMS)
        {
            var newReport = new BetReport()
            {
                Operater = response.OperatorId,
                User = response.Uid,
                GameId = request.GameId,
                Table = request.TableId,
                Seat = request.SeatId,
                BetType = request.BetTypeID,
                Bet = request.DebitAmount,
                Currency = request.Currency,
                DebitTransactionId = request.TransactionId,
                BalanceIndex = GetBalanceIndex(response.Balance),
                TimeStampIndex = GetTimeStampIndex(response.Timestamp),
                Balance = response.Balance,
                TimeStamp = response.Timestamp.ToString(),
                ExpectedBalance = 0,
                Ping = deltaMS.ToString(),
            };

            _reports.Add(newReport);
        }

        private string GetTransactionId()
        {
            // generate this "dd5dd33e-1ada-48df-bf71-87d1cb41242a"
            var random = new Random();
            var transactionId = $"PinAutoMationTest-{random.Next(100000, 999999)}-{random.Next(100000, 999999)}-{random.Next(100000, 999999)}-{random.Next(100000, 999999)}";
            return transactionId;
        }

        private long GetRoundId()
        {
            var random = new Random();
            var roundId = (long)(random.Next(100000, 999999) * 100000L + random.Next(100000, 999999));
            return roundId;
        }

        private async Task<bool> RepeatedAuthentication(string loginToken)
        {
            var awaitSecond = 1;
            Console.WriteLine($"RepeatedAuthentication after {awaitSecond}second");
            await Task.Delay(TimeSpan.FromSeconds(awaitSecond));
            var request = new AuthenticationRequest
            {
                PlatformId = 0,
                OperatorId = _operatorId,
                Token = loginToken,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            var response = await Authentication(request);
            if (response.ErrorCode == 0)
            {
                Console.WriteLine($"Should got error Token not found. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            if (response.ErrorCode != 6)
            {
                Console.WriteLine($"Should got error code 6. response: {JsonConvert.SerializeObject(response)}");
                return false;
            }
            Pass(); return true;
        }

        private static void Pass()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pass");
            Console.ResetColor();
        }

        private async Task<bool> NormalAuthentication(string loginToken)
        {
            var request = new AuthenticationRequest
            {
                PlatformId = 0,
                OperatorId = _operatorId,
                Token = loginToken,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            var response = await Authentication(request);
            if (response.ErrorCode != 0)
            {
                Console.WriteLine($"Error: {response.ErrorDescription}");
                return false;
            }
            if (response.Token == request.Token)
            {
                Console.WriteLine($"Token should not return the same");
                return false;
            }
            if (response.Timestamp <= request.Timestamp)
            {
                Console.WriteLine($"Timestamp {response.Timestamp} should responce bigger than request Timestamp {request.Timestamp}. smaller by: {(DateTimeOffset.FromUnixTimeMilliseconds(response.Timestamp) - DateTimeOffset.FromUnixTimeMilliseconds(request.Timestamp)).TotalMilliseconds}ms");
                return false;
            }
            _previousBalance = response.Balance;
            _transactionToken = response.Token;
            _username = response.Uid;
            _currency = response.Currency;
            Pass(); return true;
        }

        public async Task<AuthenticationResponse> Authentication(AuthenticationRequest request)
        {
            var response = await CallTOUtopiaEzugi<AuthenticationRequest, AuthenticationResponse>($"{_baseUrl}/api/provider/authentication", request);
            return response;
        }

        public async Task<ProviderPlaceBetResponse> PlaceBet(ProviderPlaceBetRequest request, string hash = null, bool isWithHash = true)
        {
            var response = await CallTOUtopiaEzugi<ProviderPlaceBetRequest, ProviderPlaceBetResponse>($"{_baseUrl}/api/provider/debit", request, hash, isWithHash);
            return response;
        }

        public async Task<ProviderSettleResponse> Settle(ProviderSettleRequest request)
        {
            var response = await CallTOUtopiaEzugi<ProviderSettleRequest, ProviderSettleResponse>($"{_baseUrl}/api/provider/credit", request);
            return response;
        }

        public async Task<ProviderCancelResponse> Cancel(ProviderCancelRequest request)
        {
            var response = await CallTOUtopiaEzugi<ProviderCancelRequest, ProviderCancelResponse>($"{_baseUrl}/api/provider/rollback", request);
            return response;
        }

        public async Task<TResponse> CallTOUtopiaEzugi<TRequest, TResponse>(string url, TRequest request, string hash = null, bool isWithHash = true)
        {
            var header = new Dictionary<string, string>();

            if(isWithHash)
                header.Add("hash", hash ?? CreateHMACSHA256(JsonConvert.SerializeObject(request), "d498b38b-ac48-4d83-8125-8d154638f5b0"));

            var response = await _httpHelper.PostAsync<TRequest, TResponse>(url, request, header);
            return response;
        }

        private static async Task<string> GenerateApiRequest(string url, EzugiGetsRequest parameters, string saltKey)
        {
            using (var client = new HttpClient())
            {
                var paramString = $"DataSet={parameters.DataSet}&APIID={parameters.APIID}&APIUser={parameters.APIUser}";
                var requestToken = ComputeSha256Hash(saltKey + paramString);
                paramString += $"&RequestToken={requestToken}";

                var content = new StringContent(paramString, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private string CreateHMACSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public async Task PlayerStressTest()
        {
            Console.Clear();
            Console.WriteLine("Player stress test started");
            var betCount = 30;
            var loginURL = await GetLoginUrl();
            var tranasactionIds = new List<string>();
            var settleAmounts = new List<double>();
            
            if (await NormalAuthentication(loginURL))
            {
                Console.WriteLine($"Balance{_previousBalance}");
                _expectedBalances.Add(_previousBalance);
                for (var bet = 0; bet < betCount; bet++)
                {
                    _expectedBalances.Add(_expectedBalances[bet] - 2);
                }
                for (var bet = 0; bet < betCount; bet++)
                {
                    var amountToSettle = new Random().Next(0, 100) < 50 ? 8 : 0;
                    _expectedBalances.Add(_expectedBalances.LastOrDefault() + amountToSettle);
                    settleAmounts.Add(amountToSettle);
                }
                for (var bet = 0; bet < betCount; bet++)
                {
                    Console.WriteLine($"Bet {bet + 1} of {betCount} -------------------------------------------");
                    var placeBetResult = await Debit("", betAmount: 2);
                    tranasactionIds.Add(_previousTransactionId);
                    Console.WriteLine($"Balance{_previousBalance}");
                }

                var tasks = new List<Task>();

                foreach (var transactionId in tranasactionIds)
                {
                    Console.WriteLine($"Settle -------------------------------------------");
                    var amountToCredit = settleAmounts[tranasactionIds.IndexOf(transactionId)];
                    tasks.Add(Task.Run(() => Credit("", settleAmount: amountToCredit, transactionId: transactionId)));
                    Console.WriteLine($"Balance{_previousBalance}");
                    await Task.Delay(TimeSpan.FromMilliseconds(1));

                }

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("Player stress test completed successfully \n \n");

                Console.WriteLine($"{JsonConvert.SerializeObject(_reports)}");
                _reports = _reports.OrderBy(report => report.TimeStamp).ToList();
                _reports.ForEach(r =>
                {
                    var currenctIndex = _reports.IndexOf(r);
                    if (currenctIndex != 0 && r.DebitTransactionId.StartsWith('P'))
                    {
                        var previousReport = _reports[currenctIndex - 1];
                        r.ExpectedBalance = previousReport.ExpectedBalance - r.Bet;
                        r.DeltaBalance = r.Balance - r.ExpectedBalance;
                    }
                    else if (currenctIndex != 0 && r.DebitTransactionId.StartsWith('c'))
                    {
                        var previousReport = _reports[currenctIndex - 1];
                        r.ExpectedBalance = previousReport.ExpectedBalance + r.Win;
                        r.DeltaBalance = r.Balance - r.ExpectedBalance;
                    }
                    
                    r.ExpectedBalance = r.Balance;
                });
                ExcelHelper.WriteDataTableToExcel(DataTableHelper.ToDataTable(_reports), $"C:\\Users\\sopheaktra.pin\\Downloads\\{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.xlsx", "BetReport");
            }
        }

        internal async Task RunTest()
        {
            var choise = "";
            Console.WriteLine("Select test type:");
            Console.WriteLine("1: For financial test");
            Console.WriteLine("2: For 1 player stress test");
            while (string.IsNullOrEmpty(choise))
            {
                var input = Console.ReadLine();
                if(input == "1" || input == "2")
                {
                    choise = input;

                }
                else
                {
                    Console.WriteLine("Invalid choise");
                }
            }
            if(choise == "1")
            {
                await TestFinancial();
            }
            if(choise == "2")
            {
                await PlayerStressTest();
            }

            Console.ReadKey();
        }
    }

    internal class BetReport
    {
        public BetReport()
        {
        }

        public int Operater { get; set; }
        public string User { get; set; }
        public int GameId { get; set; }
        public int Table { get; set; }
        public string Seat { get; set; }
        public int BetType { get; set; }
        public double Bet { get; set; }
        public string Currency { get; set; }
        public string DebitTransactionId { get; set; }
        public double Win { get; set; }
        public double Balance { get; set; }
        public string TimeStamp { get; set; }
        public int BalanceIndex { get; internal set; }
        public int TimeStampIndex { get; internal set; }
        public double ExpectedBalance { get; internal set; }
        public double DeltaBalance { get; internal set; }
        public string Ping { get; internal set; }
    }

    public class ProviderCancelRequest
    {
        public int OperatorId { get; set; }
        public string Uid { get; set; }
        public string TransactionId { get; set; }
        public int GameId { get; set; }
        public string Token { get; set; }
        public double RollbackAmount { get; set; }
        public int BetTypeID { get; set; }
        public int ServerId { get; set; }
        public long RoundId { get; set; }
        public string Currency { get; set; }
        public string SeatId { get; set; }
        public int PlatformId { get; set; }
        public int TableId { get; set; }
        public long Timestamp { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class ProviderCancelResponse
    {
        public string Uid { get; set; }
        public string Token { get; set; }
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public long RoundId { get; set; }
        public double Balance { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public long Timestamp { get; set; }
        public int OperatorId { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class ProviderSettleRequest
    {
        public int OperatorId { get; set; }
        public string Uid { get; set; }
        public string TransactionId { get; set; }
        public int GameId { get; set; }
        public string Token { get; set; }
        public double CreditAmount { get; set; }
        public int BetTypeID { get; set; }
        public int ServerId { get; set; }
        public long RoundId { get; set; }
        public string Currency { get; set; }
        public string SeatId { get; set; }
        public int PlatformId { get; set; }
        public int TableId { get; set; }
        public int ReturnReason { get; set; }
        public string CreditIndex { get; set; }
        public bool IsEndRound { get; set; }
        public string GameDataString { get; set; }
        public string DebitTransactionId { get; set; }
        public long Timestamp { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class ProviderSettleResponse
    {
        public string Uid { get; set; }
        public string NickName { get; set; }
        public string Token { get; set; }
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public long RoundId { get; set; }
        public double BonusAmount { get; set; }
        public double Balance { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public long Timestamp { get; set; }
        public int OperatorId { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class ProviderPlaceBetRequest
    {
        public int OperatorId { get; set; }
        public string Uid { get; set; }
        public string TransactionId { get; set; }
        public int GameId { get; set; }
        public string Token { get; set; }
        public double DebitAmount { get; set; }
        public int BetTypeID { get; set; }
        public int ServerId { get; set; }
        public long RoundId { get; set; }
        public string Currency { get; set; }
        public string SeatId { get; set; }
        public int PlatformId { get; set; }
        public int TableId { get; set; }

        // Unix timestamp in milliseconds
        public long Timestamp { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class ProviderPlaceBetResponse
    {
        public string Uid { get; set; }
        public string NickName { get; set; }
        public string Token { get; set; }
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public long RoundId { get; set; }
        public double BonusAmount { get; set; }
        public double Balance { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        // Unix timestamp in milliseconds
        public long Timestamp { get; set; }

        public int OperatorId { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class AuthenticationRequest
    {
        public int PlatformId { get; set; }
        public int OperatorId { get; set; }
        public string Token { get; set; }

        // This will hold the Unix timestamp in milliseconds.
        public long Timestamp { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    public class AuthenticationResponse
    {
        public int OperatorId { get; set; }
        public string Uid { get; set; }
        public string NickName { get; set; }
        public string Token { get; set; }
        public string PlayerTokenAtLaunch { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public string VIP { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        // Unix timestamp in milliseconds
        public long Timestamp { get; set; }

        // Optional helper to convert timestamp to DateTime (UTC)
        public DateTime GetTimestampAsDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
    }

    internal class EzugiGetsRequest
    {
        public EzugiGetsRequest()
        {
        }

        public string DataSet { get; set; }
        public string APIID { get; set; }
        public string APIUser { get; set; }
    }
}