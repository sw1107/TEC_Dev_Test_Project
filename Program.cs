using System.Globalization;
using System.Net;
using CsvHelper;
using System.Configuration;
using Npgsql;
using TEC_Dev_Test_Project;
using NpgsqlTypes;

internal class Program
{
    static async Task Main(string[] args)
    {
        for (int i = 1; i < 4; i++)
        {
            var date = DateTime.Today.AddDays(-i);

            var url = GenerateUrl(date);

            var res = GetCSV(url);

            InsertAll(res, date);
        }
    }

    static async void InsertAll(IEnumerable<TWCapacity> items, DateTime date) // does this need to be async?
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
                await importer.StartRowAsync();
                await importer.WriteAsync(item.Loc);
                await importer.WriteAsync(item.LocZn);
                await importer.WriteAsync(item.LocName);
                await importer.WriteAsync(item.LocPurpDesc);
                await importer.WriteAsync(item.LocQTI);
                await importer.WriteAsync(item.FlowInd);
                await importer.WriteAsync(item.DC);
                await importer.WriteAsync(item.OPC);
                await importer.WriteAsync(item.TSQ);
                await importer.WriteAsync(item.OAC);
                await importer.WriteAsync(item.IT == 'Y' ? true : false);
                await importer.WriteAsync(item.AuthOverrunInd == 'Y' ? true : false);
                await importer.WriteAsync(item.NomCapExceedInd == 'Y' ? true : false);
                await importer.WriteAsync(item.AllQtyAvail == 'Y' ? true : false);
                await importer.WriteAsync(item.QtyReason);
                await importer.WriteAsync(date.Date, NpgsqlDbType.Date);
                await importer.WriteAsync(DateTime.Now);
            }

            await importer.CompleteAsync();
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
        var url = "https://twtransfer.energytransfer.com/ipost/capacity/operationally-available?f=csv&extension=csv&asset=TW&cycle=5&searchType=NOM&searchString=&locType=ALL&locZone=ALL&gasDay=" +
            date.Month +
            "%2F" +
            date.Day +
            "%2F" +
            date.Year +
            "";

        return url;
    }

    static List<TWCapacity> GetCSV(string url)
    {
        var records = new List<TWCapacity>();

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        using (var reader = new StreamReader(response.GetResponseStream())) // what is this doing?
        using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csvReader.Context.RegisterClassMap<TWCapacityMap>();

            //while (csvReader.Read())
            //{
            //    // TODO: currently the code will skip any records that it cannot parse - maybe it should skip the whole batch?
            //    try
            //    {
            //        var record = csvReader.GetRecord<TWCapacity>();

            //        result.Add(record);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error: {ex.Message}");
            //    }
            //}

            try
            {
                records = (List<TWCapacity>)csvReader.GetRecords<TWCapacity>().ToList();

                //result.AddRange(records);
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

    //static async Task Insert(TWCapacity twCapacity)
    //{
    //    string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString_Postgres"].ConnectionString;
    //    NpgsqlConnection connection = new NpgsqlConnection(connectionString);

    //    try
    //    {
    //        connection.Open();

    //        var TABLE_NAME = "tw_capacity";

    //        string commandText = $"INSERT INTO {TABLE_NAME} " +
    //                             $"(loc, loc_zn, loc_name, loc_purp_desc, loc_qti, flow_ind, dc, opc, tsq, oac, it, auth_overrun_ind, nom_cap_exceed_ind, all_qty_avail, qty_reason, date_inserted) " +
    //                             $"VALUES (@loc, @locZn, @locName, @locPurpDesc, @locQti, @flowInd, @dc, @opc, @tsq, @oac, @it, @authOverrunInd, @nomCapExceedInd, @allQtyAvail, @qtyReason, @dateInserted)";

    //        await using (var cmd = new NpgsqlCommand(commandText, connection))
    //        {
    //            cmd.Parameters.AddWithValue("loc", twCapacity.Loc);
    //            cmd.Parameters.AddWithValue("locZn", twCapacity.LocZn);
    //            cmd.Parameters.AddWithValue("locName", twCapacity.LocName);
    //            cmd.Parameters.AddWithValue("locPurpDesc", twCapacity.LocPurpDesc);
    //            cmd.Parameters.AddWithValue("locQti", twCapacity.LocQTI);
    //            cmd.Parameters.AddWithValue("flowInd", twCapacity.FlowInd);
    //            cmd.Parameters.AddWithValue("dc", twCapacity.DC);
    //            cmd.Parameters.AddWithValue("opc", twCapacity.OPC);
    //            cmd.Parameters.AddWithValue("tsq", twCapacity.TSQ);
    //            cmd.Parameters.AddWithValue("oac", twCapacity.OAC);
    //            cmd.Parameters.AddWithValue("it", twCapacity.IT == 'Y' ? true : false);
    //            cmd.Parameters.AddWithValue("authOverrunInd", twCapacity.AuthOverrunInd == 'Y' ? true : false);
    //            cmd.Parameters.AddWithValue("nomCapExceedInd", twCapacity.NomCapExceedInd == 'Y' ? true : false);
    //            cmd.Parameters.AddWithValue("allQtyAvail", twCapacity.AllQtyAvail == 'Y' ? true : false);
    //            cmd.Parameters.AddWithValue("qtyReason", twCapacity.QtyReason);
    //            cmd.Parameters.AddWithValue("dateInserted", DateTime.Now);

    //            await cmd.ExecuteNonQueryAsync();
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error: {ex.Message}");
    //    }
    //    finally
    //    {
    //        connection.Close();
    //    }
    //}
}
