using System.Globalization;
using System.Net;
using CsvHelper;
using System.Configuration;
using Npgsql;
using TEC_Dev_Test_Project;
using NpgsqlTypes;

internal class Program
{
    private const int NUMBER_OF_DAYS = 3;

    static void Main(string[] args)
    {
        var allRecords = new List<TWCapacity>();

        for (int i = 1; i <= NUMBER_OF_DAYS; i++)
        {
            var date = DateTime.Today.AddDays(-i);

            var url = GenerateUrl(date);

            allRecords.AddRange(GetCSV(url, date));
        }

        InsertMany(allRecords);
    }

    static void InsertMany(IEnumerable<TWCapacity> items) 
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString_Postgres"].ConnectionString;
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);

        try
        {
            connection.Open();

            using var importer = connection.BeginBinaryImport(
                    "COPY tw_capacity (loc, loc_zn, loc_name, loc_purp_desc, loc_qti, flow_ind, dc, opc, tsq, oac, it, auth_overrun_ind, nom_cap_exceed_ind, all_qty_avail, qty_reason, date, date_inserted) " +
                    "FROM STDIN (FORMAT binary)");

            foreach (var item in items)
            {
                importer.StartRow();
                importer.Write(item.Loc);
                importer.Write(item.LocZn);
                importer.Write(item.LocName);
                importer.Write(item.LocPurpDesc);
                importer.Write(item.LocQTI);
                importer.Write(item.FlowInd);
                importer.Write(item.DC);
                importer.Write(item.OPC);
                importer.Write(item.TSQ);
                importer.Write(item.OAC);
                importer.Write(item.IT == 'Y' ? true : false);
                importer.Write(item.AuthOverrunInd == 'Y' ? true : false);
                importer.Write(item.NomCapExceedInd == 'Y' ? true : false);
                importer.Write(item.AllQtyAvail == 'Y' ? true : false);
                importer.Write(item.QtyReason);
                importer.Write(item.Date, NpgsqlDbType.Date);
                importer.Write(DateTime.Now);
            }

            importer.Complete();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            connection.Close();
        }
    }

    static string GenerateUrl(DateTime date)
    {
        // Final cycle
        var url = $"https://twtransfer.energytransfer.com/ipost/capacity/operationally-available?" +
                  $"f=csv&extension=csv&asset=TW&gasDay={date.Month}%2F{date.Day}%2F{date.Year}&cycle=3&searchType=NOM&searchString=&locType=ALL&locZone=ALL";

        return url;
    }

    static List<TWCapacity> GetCSV(string url, DateTime date)
    {
        var records = new List<TWCapacity>();

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        using (var reader = new StreamReader(response.GetResponseStream())) 
        using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csvReader.Context.RegisterClassMap<TWCapacityMap>();

            try
            {
                records = csvReader.GetRecords<TWCapacity>().ToList();

                foreach (var record in records)
                    record.Date = date.Date;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        return records;
    }

    static bool Validate(TWCapacity twCapacity)
    {
        return true;
    }
}
