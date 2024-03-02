using CsvHelper.Configuration;

namespace TEC_Dev_Test_Project
{
    class TWCapacityMap : ClassMap<TWCapacity> 
    {
        public TWCapacityMap()
        {
            Map(m => m.Loc).Name("Loc");
            Map(m => m.LocZn).Name("Loc Zn");
            Map(m => m.LocName).Name("Loc Name");
            Map(m => m.LocPurpDesc).Name("Loc Purp Desc");
            Map(m => m.LocQTI).Name("Loc/QTI");
            Map(m => m.FlowInd).Name("Flow Ind");
            Map(m => m.DC).Name("DC");
            Map(m => m.OPC).Name("OPC");
            Map(m => m.TSQ).Name("TSQ");
            Map(m => m.OAC).Name("OAC");
            Map(m => m.IT).Name("IT");
            Map(m => m.AuthOverrunInd).Name("Auth Overrun Ind");
            Map(m => m.NomCapExceedInd).Name("Nom Cap Exceed Ind");
            Map(m => m.AllQtyAvail).Name("All Qty Avail");
            Map(m => m.QtyReason).Name("Qty Reason");
        }
    }
}

