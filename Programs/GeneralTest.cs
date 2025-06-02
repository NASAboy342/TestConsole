using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestConsole.Programs
{
    public class GeneralTest
    {
        public void Run()
        {

        }
        public void HashGenerator()
        {
            var myString = "example";
            using (MD5 md5Hasher = MD5.Create())
            {
                var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(myString));
                var ivalue = BitConverter.ToInt32(hashed, 0);
                Console.WriteLine($"Integer hash: {ivalue}");
            }
        }

        private static void Bank()
        {
            var depositBanks = new List<Bank>()
            {
                new Bank{ BankCode = "VNB", BankName = "VietnamBank", PaymentMethod = 1},
                new Bank{ BankCode = "BTN", BankName = "Buton", PaymentMethod = 1},
                new Bank{ BankCode = "BBB", BankName = "BabyBank", PaymentMethod = 1},
            };
            var withdrawBanks = new List<Bank>()
            {
                new Bank{ BankCode = "VNB", BankName = "VietnamBank", PaymentMethod = 2},
                new Bank{ BankCode = "BTN", BankName = "Buton", PaymentMethod = 2},
                new Bank{ BankCode = "MSB", BankName = "MusoBank", PaymentMethod = 2},
                new Bank{ BankCode = "BNB", BankName = "BanaBank", PaymentMethod = 2},
            };

            var banks = new List<Bank>();

            banks.AddRange(MergeDepositAndWithdrawBanks(depositBanks, withdrawBanks));

            Console.WriteLine(JsonConvert.SerializeObject(banks));
        }

        private static IEnumerable<Bank> MergeDepositAndWithdrawBanks(List<Bank> depositBanks, List<Bank> withdrawBanks)
        {
            var banks = new List<Bank>();
            banks.AddRange(depositBanks);
            banks.AddRange(withdrawBanks);
            SetDuplicatedBanksToDepositAndWithdraw(banks);
            banks = banks.Distinct().ToList();
            return banks;
        }

        private static void SetDuplicatedBanksToDepositAndWithdraw(List<Bank> banks)
        {
            var duplicatedBankCodes = banks.
                GroupBy(bank => bank.BankCode).
                Where(groupedElement => groupedElement.Count() > 1 ).
                Select(groupedElement => groupedElement.Key).
                ToList();
            var withdrawAndDeposit = 3;

            foreach (var bank in banks)
            {
                if (duplicatedBankCodes.Contains(bank.BankCode))
                    bank.PaymentMethod = withdrawAndDeposit;
                else
                    continue;
            }
        }
    }

    public class ResponseFromProvider
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess => (ErrorCode == "200");
    }

    public class Bank
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public int PaymentMethod { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Bank bank && 
                bank.BankCode == bank.BankCode &&
                bank.BankName == bank.BankName &&
                bank.PaymentMethod == bank.PaymentMethod;
        }
        public override int GetHashCode()
        {   
            return HashCode.Combine(BankCode, BankName, PaymentMethod);
        }
    }
}
