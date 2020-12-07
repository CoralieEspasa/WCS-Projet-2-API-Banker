using System;

namespace API_Banker
{
    public class Account
    {
        public Int32 AccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal? AccountBalance { get; set; }
        public DateTime? AccountCreationDate { get; set; }
        public bool? OverdraftAuthorization { get; set; }
        public decimal? OverdraftAmount { get; set; }
        public string resultOverdraftAuthorization()
        {
            string result;
            if (OverdraftAuthorization == true)
            {
                result = "OUI";
            }
            else
            {
                result = "NON";
            }
            return result;
        }
    }
}
