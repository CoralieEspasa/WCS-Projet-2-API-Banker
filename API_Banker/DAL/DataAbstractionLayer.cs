using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API_Banker;
using System.Collections;

namespace API_Banker
{
    public class DataAbstractionLayer : IDisposable
    {
        public static String ConnectionString { get; set; }

        private SqlConnection _connection;

        public DataAbstractionLayer()
        {
            _connection = new SqlConnection();
            _connection.ConnectionString = ConnectionString;
            _connection.Open();
        }
        ~DataAbstractionLayer()
        {
            Dispose();
        }
        public void Dispose()
        {
            _connection.Close();
        }
        public IEnumerable<Customer> SelectCustomers()
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"SELECT CustomerNumber, CustomerLastName, CustomerFirstName, CustomerEmail, CustomerPhone, CustomerBirthdate
                                    FROM Customer 
                                    ORDER BY CustomerLastName";
            SqlDataReader reader = command.ExecuteReader();
            List<Customer> customers = new List<Customer>();
            while (reader.Read())
            {
                Customer customer = new Customer
                {
                    customerNumber = reader.GetString(0),
                    customerLastName = reader.GetString(1),
                    customerFirstName = reader.GetString(2),
                    customerEmail = reader.GetString(3),
                    customerPhone = reader.GetString(4),
                    customerBirthDate = reader.GetDateTime(5),
                };
                customers.Add(customer);
            }
            reader.Close();
            return customers;
        }
        public Customer SelectCustomerByNumber(String customerNumber)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"SELECT CustomerNumber, CustomerLastName, CustomerFirstName, CustomerEmail, CustomerPhone, CustomerBirthdate 
                                    FROM Customer 
                                    WHERE CustomerNumber = @customerNumber";
            command.Parameters.AddWithValue("@customerNumber", customerNumber); 
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            Customer customer = new Customer
            {
                customerNumber = reader.GetString("CustomerNumber"),
                customerLastName = reader.GetString("CustomerLastName"),
                customerFirstName = reader.GetString("CustomerFirstName"),
                customerEmail = reader.GetString("CustomerEmail"),
                customerPhone = reader.GetString("CustomerPhone"),
                customerBirthDate = reader.GetDateTime("CustomerBirthDate"),
            };
            reader.Close();
            return customer;
        }
        public Customer CreateCustomer(Customer userEntry)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"INSERT INTO Customer 
                                        (customerNumber, customerFirstName, customerLastName, customerEmail, 
                                         customerPhone, customerBirthDate, customerIsFemale)
                                    VALUES (@customerNumber, 
                                            @customerFirstName, 
                                            @customerLastName, 
                                            @customerEmail, 
                                            @customerPhone, 
                                            @customerBirthDate, 
                                            @customerIsFemale);";

            command.Parameters.AddWithValue("@customerNumber", userEntry.customerNumber);
            command.Parameters.AddWithValue("@customerFirstName", userEntry.customerFirstName);
            command.Parameters.AddWithValue("@customerLastName", userEntry.customerLastName);
            command.Parameters.AddWithValue("@customerEmail", userEntry.customerEmail);
            command.Parameters.AddWithValue("@customerPhone", userEntry.customerPhone);
            command.Parameters.AddWithValue("@customerBirthDate", userEntry.customerBirthDate);
            command.Parameters.AddWithValue("@customerIsFemale", userEntry.customerIsFemale);

            command.ExecuteNonQuery();
            return userEntry;
        }
        // Mise à jour des données d'un client à partir de son numéro client
        public bool UpdateCustomer(Customer userEntry)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "UPDATE Customer SET ";

            if (userEntry.customerNumber == null)
            {
                throw new DataException("Can update only with customer number");
            }

            // Get fields that are not null
            IDictionary<String, Object> fields = FormatCustomerFields(userEntry);
            // Transform fields from Dictionary in field names for SQL
            IEnumerable<String> strFields = fields.Select((field) => $"{field.Key} = @{field.Key}");
            command.CommandText += String.Join(", ", strFields);
            foreach (KeyValuePair<String, Object> fieldInfo in fields)
            {
                command.Parameters.AddWithValue($"@{fieldInfo.Key}", fieldInfo.Value);
            }

            command.CommandText += " WHERE customerNumber = @customerNumber";
            command.Parameters.AddWithValue("@customerNumber", userEntry.customerNumber);

            Int32 affectedRowsCount = command.ExecuteNonQuery();
            if (affectedRowsCount > 0)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Transfer> SelectTransferListByAccountNumber(String accountNumber)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"SELECT acc.AccountNumber, tr.TransferAmount, tr.TransferDate, tr.FK_AccountIdTransmitter, tr.FK_AccountIdReceiver FROM [Transfer] AS tr
                                  INNER JOIN Account AS acc ON acc.PK_AccountId = tr.FK_AccountIdTransmitter OR acc.PK_AccountId = tr.FK_AccountIdReceiver
                                  WHERE acc.AccountNumber = @accountNumber
                                  ORDER BY tr.TransferDate";

            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            List<Transfer> transfers = new List<Transfer>();

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            while (reader.Read())
            {
                Transfer transfer = new Transfer
                {
                    transferAmount = reader.GetDecimal("TransferAmount"),
                    transferDate = reader.GetDateTime("TransferDate")
                };
                transfers.Add(transfer);
            }
            reader.Close();
            return transfers;
        }
        public List<Transfer> SelectTransferDebitListByAccountNumber(String accountNumber)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"SELECT acc.AccountNumber, tr.TransferAmount, tr.TransferDate, tr.FK_AccountIdTransmitter FROM [Transfer] AS tr
                                INNER JOIN Account AS acc ON acc.PK_AccountId = tr.FK_AccountIdTransmitter 
                                WHERE acc.AccountNumber = @accountNumber
                                ORDER BY tr.TransferDate";

            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            List<Transfer> transfers = new List<Transfer>();

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            while (reader.Read())
            {
                Transfer transfer = new Transfer
                {
                    transferAmount = reader.GetDecimal("TransferAmount"),
                    transferDate = reader.GetDateTime("TransferDate")
                };
                transfers.Add(transfer);
            }
            reader.Close();
            return transfers;
        }
        public List<Transfer> SelectTransferCreditListByAccountNumber(String accountNumber)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"SELECT acc.AccountNumber, tr.TransferAmount, tr.TransferDate, tr.FK_AccountIdReceiver FROM [Transfer] AS tr
                                    INNER JOIN Account AS acc ON acc.PK_AccountId = tr.FK_AccountIdReceiver
                                    WHERE acc.AccountNumber = @accountNumber
                                    ORDER BY tr.TransferDate";

            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            List<Transfer> transfers = new List<Transfer>();

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            while (reader.Read())
            {
                Transfer transfer = new Transfer
                {
                    transferAmount = reader.GetDecimal("TransferAmount"),
                    transferDate = reader.GetDateTime("TransferDate")
                };
                transfers.Add(transfer);
            }
            reader.Close();
            return transfers;
        }
        public IEnumerable<Transfer> SelectTransferListByDate(Transfer date)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = @"SELECT acc.AccountNumber, tr.TransferAmount, tr.TransferDate FROM[Transfer] AS tr
                                INNER JOIN Account AS acc ON acc.PK_AccountId = tr.FK_AccountIdTransmitter OR acc.PK_AccountId = tr.FK_AccountIdReceiver
                                WHERE tr.TransferDate = @transferDate
                                ORDER BY tr.TransferDate";

            command.Parameters.AddWithValue("@transferDate", date.transferDate);
            List<Transfer> transfers = new List<Transfer>();

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            while (reader.Read())
            {
                Transfer transfer = new Transfer
                {
                    transferAmount = reader.GetDecimal("TransferAmount"),
                    transferDate = reader.GetDateTime("TransferDate")
                };
                transfers.Add(transfer);
            }
            reader.Close();
            return transfers;
        }
        public Transfer NewTransfer(Transfer userEntry, String accountTransmitter, String accountReceiver)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "SELECT PK_AccountId FROM Account WHERE AccountNumber = @accountTransmitter";
            command.Parameters.AddWithValue("@accountTransmitter", accountTransmitter);

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            Account account1 = new Account
            {
                AccountId = reader.GetInt32("PK_AccountId")
            };

            command.CommandText = "SELECT PK_AccountId FROM Account WHERE AccountNumber = @accountReceiver ";
            command.Parameters.AddWithValue("@accountReceiver", accountReceiver);

            reader.Close();

            reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }

            Account account2 = new Account
            {
                AccountId = reader.GetInt32("PK_AccountId")
            };
            reader.Close();

            if (accountTransmitter == null)
            {
                throw new DataException("Your account number is needed");
            }
            if (accountReceiver == null)
            {
                throw new DataException("The receiver account number is needed");
            }

            command.CommandText = "INSERT INTO [Transfer] (FK_AccountIdReceiver, FK_AccountIdTransmitter,TransferAmount,TransferDate) " +
                                    "VALUES(@AccountToDebit, @AccountToCredit, @TransferAmount, @TransferDate);";
            command.Parameters.AddWithValue("@AccountToDebit", account1.AccountId);
            command.Parameters.AddWithValue("@AccountToCredit", account2.AccountId);
            command.Parameters.AddWithValue("@TransferAmount", userEntry.transferAmount);
            command.Parameters.AddWithValue("@TransferDate", userEntry.transferDate);

            command.ExecuteNonQuery();
            return userEntry;
        } 
        private IDictionary<String, Object> FormatCustomerFields(Customer userEntry)
        {
            Dictionary<String, Object> fields = new Dictionary<String, Object>();

            if (userEntry.customerFirstName != null)
            {
                fields["customerFirstName"] = userEntry.customerFirstName;
            }

            if (userEntry.customerLastName != null)
            {
                fields["customerLastName"] = userEntry.customerLastName;
            }

            if (userEntry.customerEmail != null)
            {
                fields["customerEmail"] = userEntry.customerEmail;
            }

            if (userEntry.customerPhone != null)
            {
                fields["customerPhone"] = userEntry.customerPhone;
            }

            if (userEntry.customerIsFemale != null)
            {
                fields["customerIsFemale"] = userEntry.customerIsFemale;
            }

            return fields;
        }
        //---------------------------   ACCOUNT METHODS  ------------------------------------------------
        //Obtenir le solde d'un compte client - Requête 1.1)
        public Object SelectAccountAmount(string accountNumber)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = $"SELECT AccountBalance FROM Account WHERE AccountNumber = '{accountNumber}'";
            SqlDataReader reader = command.ExecuteReader();
            
            decimal amount = 0;
            while (reader.Read())
            {
                amount = reader.GetDecimal(0);              
            }
            Object accountBalance = new { AccountBalance = amount };
            reader.Close();

            return accountBalance;
        }
        //Vérifier si un compte existe - Requête 2.1)
        public Object CheckAccount(string accountNumber)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(AccountNumber) FROM Account WHERE AccountNumber = '{accountNumber}'";
            SqlDataReader reader = command.ExecuteReader();
            bool checkExist = false ;
            while (reader.Read())
            {
                if (reader.GetInt32(0) == 0)
                {
                    checkExist = false;
                }
                else
                {
                    checkExist = true;
                }
            }
            
            Object accountExist = new { AccountExist = checkExist };   
            reader.Close();
            return accountExist;
        }
        //Obtenir tous les comptes clients
        public List<Account> SelectAccountAll()
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Account";
            SqlDataReader reader = command.ExecuteReader();
            List<Account> accountList = new List<Account>();
            while (reader.Read())
            {
                Account account = new Account
                {
                    AccountNumber = reader.GetString("AccountNumber"),
                    AccountBalance = reader.GetDecimal("AccountBalance"),
                    AccountCreationDate = reader.GetDateTime("AccountCreationDate"),
                    OverdraftAuthorization = reader.GetBoolean("AccountOverdraftAuthorization"),
                    OverdraftAmount = reader.GetDecimal("AccountOverdraftAmount"),
                };
                accountList.Add(account);
            }
            reader.Close();
            return accountList;
        }

        //Obtenir un client à partir d'un numéro de compte - Requête 3)
        public Account SelectAccount(string accountNumber)
		{
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM Account WHERE AccountNumber = '{accountNumber}'";
            SqlDataReader reader = command.ExecuteReader();

            /*if (!reader.Read())
            {
                reader.Close();
                return null;
            }*/

            Account account = new Account();

            while (reader.Read())
            {
                    account.AccountNumber = reader.GetString("AccountNumber");
                    account.AccountBalance = reader.GetDecimal("AccountBalance");
                    account.AccountCreationDate = reader.GetDateTime("AccountCreationDate");
                    account.OverdraftAuthorization = reader.GetBoolean("AccountOverdraftAuthorization");
                    account.OverdraftAmount = reader.GetDecimal("AccountOverdraftAmount");
 
            }
            reader.Close();
            return account;
        }
        //Obtenir la liste de tous les numéros de compte - Requête 6.2)
        public List<Object> SelectAllAccountNumber()
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "SELECT AccountNumber FROM Account";
            SqlDataReader reader = command.ExecuteReader();
            List<Object> accountNumberList = new List<Object>();

            while (reader.Read())
            {
                var account = new 
                {
                    AccountNumber = reader.GetString(0)
                };
                accountNumberList.Add(account);
            };          
            reader.Close();

            return accountNumberList;
        }
        //Création d'un nouveau compte
        public Account InsertAccount(Account userEntry)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO Account (AccountNumber, AccountBalance, AccountCreationDate, AccountOverdraftAuthorization, AccountOverdraftAmount) " +
                "VALUES (@AccountNumber, @AccountBalance, @AccountCreationDate, @AccountOverdraftAuthorization, @AccountOverdraftAmount);";

            command.Parameters.AddWithValue("@AccountNumber", userEntry.AccountNumber);
            command.Parameters.AddWithValue("@AccountBalance", userEntry.AccountBalance);
            command.Parameters.AddWithValue("@AccountCreationDate", userEntry.AccountCreationDate);
            command.Parameters.AddWithValue("@AccountOverdraftAuthorization", userEntry.OverdraftAuthorization);
            command.Parameters.AddWithValue("@AccountOverdraftAmount", userEntry.OverdraftAmount);

            command.ExecuteNonQuery();
            return userEntry;
        }
        //Création d'un nouveau compte
        public Account UpdateAccount(Account userEntry)
        {
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "UPDATE Account SET ";

            if (userEntry.AccountNumber == null)
            {
                throw new DataException("AccountNumber is missing");
            }
            // Get fields that are not null
            IDictionary<String, Object> fields = FormatCustomerFields(userEntry);
            // Transform fields from Dictionary in field names for SQL
            IEnumerable<String> strFields = fields.Select((field) => $"{field.Key} = @{field.Key}");
            command.CommandText += String.Join(", ", strFields);

            foreach (KeyValuePair<String, Object> fieldinfo in fields)
            {
                command.Parameters.AddWithValue($"@{fieldinfo.Key}", fieldinfo.Value);
            }

            command.CommandText += $" WHERE AccountNumber = '{userEntry.AccountNumber}'";

            command.ExecuteNonQuery();
            return userEntry;
        }
        private IDictionary<String, Object> FormatCustomerFields(Account userEntry)
        {
            Dictionary<String, Object> fields = new Dictionary<String, Object>();

            if (userEntry.AccountBalance != null)
            {
                fields["AccountBalance"] = userEntry.AccountBalance;
            }

            if (userEntry.AccountCreationDate != null)
            {
                fields["AccountCreationDate"] = userEntry.AccountCreationDate;
            }

            if (userEntry.OverdraftAuthorization != null)
            {
                fields["OverdraftAuthorization"] = userEntry.OverdraftAuthorization;
            }

            if (userEntry.OverdraftAmount != null)
            {
                fields["OverdraftAmount"] = userEntry.OverdraftAmount;
            }

            return fields;
        }
        //---------------------------  END ACCOUNT METHODS  ------------------------------------------------
    }
}