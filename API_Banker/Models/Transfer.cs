using System;

namespace API_Banker
{
    public class Transfer
    {
        public Int32 transferId { get; set;}
        public DateTime transferDate { get; set; }
        public Decimal transferAmount { get; set; }
        public Int32 transmitterAccountId { get; set; }
        public Int32 receiverAccountId { get; set; }
    }
}
