using System;
using VertMarket.Common;
using VertMarket.BusinessLayer.Provider;
using System.Collections.Generic;
using VertMarket.Model;

namespace VertMarketConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Processing started");
            Uri BaseEndPoint = new Uri("http://magazinestore.azurewebsites.net");
            ApiClient Client = new ApiClient(BaseEndPoint);
            MagazineStoreProvider provider = new MagazineStoreProvider(Client);
            SubscribersMag ids = provider.SubscribersinAllCategory();
           var AnswerObject= provider.GetAnswers(ids);
            Console.WriteLine("Total time:" + AnswerObject.Data.TotalTime);
            Console.WriteLine("Is answer correct? " + AnswerObject.Data.AnswerCorrect);
            Console.WriteLine("Done processing.");
            Console.ReadLine();
       





        }
    }
}
