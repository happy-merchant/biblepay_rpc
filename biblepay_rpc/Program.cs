using BitcoinLib.Services.Coins.Cryptocoin;
using BitcoinLib.Responses;
using System;
using System.Collections.Generic;

namespace biblepay_rpc
{
    class Program
    {
        static void Main(string[] args)
        {
            // parse command line arguments
            Options options = new Options();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            // create biblepay daemon interface
            ICryptocoinService bbpService = new CryptocoinService(options.Url, options.User, options.Password, "x");

            // retrieve blockchain
            Console.WriteLine("Retrieving blockchain data from biblepayd at " + options.Url + "...\n");
            List<GetBlockResponse> blocks = GetBlocks(bbpService);

            // format some data and write to console
            Console.WriteLine("{0,-6} | {1,-22} | {2,-20} | {3,-9}", "height", "timestamp", "difficulty", "blocktime");
            Console.WriteLine(new String('-', 80));

            foreach (GetBlockResponse block in blocks)
            {
                Console.WriteLine(String.Format(
                    "{0,6} | {1,-22} | {2,-20} | {3,9:0.000}",
                    block.Height,
                    UnixTimeStampToDateTime(block.Time),
                    block.Difficulty,
                    block.Height > 1 ? GetBlockTime(blocks, block.Height) / 60.0 : Double.NaN
                    ));
            }

            // calculate average blocktime
            int firstBlockTime = blocks[1].Time;
            int finalBlockTime = blocks[blocks.Count - 1].Time;
            int elapsedTime = finalBlockTime - firstBlockTime;
            Console.WriteLine(String.Format("\nAverage blocktime: {0:0.000}", (elapsedTime / bbpService.GetBlockCount()) / 60.0));
        }

        // retrieve blockchain, can throw if the RPC server is unreachable or RPC credentials are incorrect
        static List<GetBlockResponse> GetBlocks(ICryptocoinService cryptoService)
        {
            List<GetBlockResponse> blocks = new List<GetBlockResponse>();
            try
            {
                string hash = cryptoService.GetBlockHash(0);
                while (!String.IsNullOrEmpty(hash))
                {
                    GetBlockResponse block = cryptoService.GetBlock(hash);
                    blocks.Add(block);
                    hash = block.NextBlockHash;
                }
            }
            catch (Exception e)
            {
                do
                {
                    Console.Error.WriteLine("ERROR -> " + e.Message);
                } while ((e = e.InnerException) != null);

                Environment.Exit(1);
            }

            return blocks;
        }

        // calculate the time required to mine the given block
        static int GetBlockTime(List<GetBlockResponse> blocks, int index)
        {
            if (index == 0)
                throw new ArgumentOutOfRangeException("hash", "Unable to calculate block time for first element of a collection.");

            return blocks[index].Time - blocks[index - 1].Time;
        }

        // convert unix timestamp to local datetime
        static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            return dateTime;
        }
    }
}
