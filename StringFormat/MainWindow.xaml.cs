using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StringFormat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int CollectionListSize = 10000;

        public MainWindow()
        {
            InitializeComponent();
        }

        private List<Account> GenerateAccounts()
        {
            var accounts = new List<Account>();

            var random = new Random();
            for (var i = 0; i < CollectionListSize; i++)
            {
                accounts.Add(new Account
                {
                    Name = "Savings",
                    Bsb = random.Next(10000, 99999),
                    AccountNumber = random.Next(10000, 99999),
                    Balance = random.Next(10000, 99999),
                    AvailableFunds = random.Next(10000, 99999)
                });
            }

            return accounts;
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            btnRun.IsEnabled = false;

            var accounts = GenerateAccounts();

            var taskList = new List<Task>();

            var taskWithToString = PrepareAndRun(WithToString, accounts, pbWithToString, lblResultWithToString);
            var taskWithoutToString = PrepareAndRun(WithoutToString, accounts, pbWithoutToString, lblResultWithoutToString);

            taskList.Add(taskWithToString);
            taskList.Add(taskWithoutToString);

            await Task.WhenAll(taskList);

            btnRun.IsEnabled = true;
        }

        private async Task PrepareAndRun(Action<List<Account>> func, List<Account> list, ProgressBar progressBar, Label labelResult)
        {
            labelResult.Content = "Pending...";
            progressBar.Value = 0;
            progressBar.Maximum = CollectionListSize;

            progressBar.IsIndeterminate = true;
            var task = Task.Factory.StartNew(() => RunCode(func, list));

            var failed = false;
            var message = string.Empty;

            var durationOfSort = new TimeSpan();

            try
            {
                await task;

                durationOfSort = task.Result;
            }
            catch (Exception ex)
            {
                failed = true;
                message = ex.Message;
            }

            if (!failed)
            {
                progressBar.IsIndeterminate = false;
                progressBar.Value = CollectionListSize;
                labelResult.Content = "Done. Time taken: " + durationOfSort;
            }
            else
            {
                labelResult.Content = "Run failed. Message: " + message;
            }
        }

        private static TimeSpan RunCode(Action<List<Account>> findFunc, List<Account> listToSearch)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            findFunc(listToSearch);

            stopWatch.Stop();

            return stopWatch.Elapsed;
        }

        private void WithToString(List<Account> list)
        {
            string result;

            foreach (var item in list)
            {
                result = string.Format("Account name: {0}, BSB: {1}, Account number: {2}, Balance: {3}, Available Funds: {4}", 
                    item.Name, 
                    item.Bsb.ToString(),
                    item.AccountNumber.ToString(),
                    item.Balance.ToString(),
                    item.AvailableFunds.ToString());
            }
        }

        private void WithoutToString(List<Account> list)
        {
            string result;

            foreach (var item in list)
            {
                result = string.Format("Account name: {0}, BSB: {1}, Account number: {2}, Balance: {3}, Available Funds: {4}", 
                    item.Name, 
                    item.Bsb,
                    item.AccountNumber,
                    item.Balance,
                    item.AvailableFunds);
            }
        }
    }
}
