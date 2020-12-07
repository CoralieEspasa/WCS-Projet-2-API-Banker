using System;
using System.Collections.Generic;

namespace API_Banker
{
    public class Customer
    {
        public Int32 customerId { get; set; }
        public String customerNumber { get; set; }
        public String customerLastName { get; set; }
        public String customerFirstName { get; set; }
        public String customerEmail { get; set; }
        public String customerPhone { get; set; }
        public DateTime customerBirthDate { get; set; }
        public Boolean customerIsFemale { get; set; }
    }
}
