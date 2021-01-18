using System;

namespace DbBuilder
{
    class Program
    {
        private const string MONGO_CON_String = "mongodb://root:example@localhost:27017";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Db builder started .... ");

                var seeder = new MongoDBSeeder(MONGO_CON_String);

                seeder.SeedDb();

                Console.WriteLine("Db builder done. ");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Some errors occured. {exp.Message}");
            }
            Console.Write("Press enter to exit >");
            Console.ReadLine();
        }
    }
}
