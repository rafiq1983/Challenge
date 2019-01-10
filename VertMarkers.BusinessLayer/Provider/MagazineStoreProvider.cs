using System;
using System.Collections.Generic;
using System.Text;
using VertMarket.Model;
using VertMarket.Common;
using System.Linq;

namespace VertMarket.BusinessLayer.Provider
{
    //BusinessLayer to call Data access layer. I could have used dependency injection but kept it simple.
    public class MagazineStoreProvider
    {

        private List<string> _lstCategories { get; set; }
        private List<ApiSubscriber> _lstSubscriberWithMagazines { get; set; }
        private List<Magazine> _lstMagazines { get; set; }
        private List<Tuple<ApiSubscriber, List<Magazine>>> _lstSubscriberMagazines { get; set; }
        private ApiClient _Client { get; set; }
        public MagazineStoreProvider(ApiClient Client)
        {

            _Client = Client;
        }
        public List<ApiSubscriber> GetAllSubscribersWithMagazineSubscriptions()
        {

            if (_lstSubscriberWithMagazines == null)
            {
                var data = _Client.GetAsync<ApiResponseListApiSubscriber>("api/Subscribers/{Token}");
                _lstSubscriberWithMagazines = data.Result.Data;

            }
            return _lstSubscriberWithMagazines;
        }

           
        public List<string> GetAllCategories()
        {
        if (_lstCategories == null)
        {
            var data = _Client.GetAsync<ApiResponseListString>("api/Categories/{Token}");
                _lstCategories = data.Result.Data;
        }
            return _lstCategories;          
        }

        public List<Magazine> GetMagazinesByCategory()
        {
            if (_lstMagazines == null)
            {
                List<Magazine> lstMagazines = new List<Magazine>();
                var Categories = this.GetAllCategories();
                foreach (string category in Categories)
                {
                    var data = _Client.GetAsync<ApiResponseListMagazine>("/api/magazines/{Token}/" + category);
                    foreach (var mag in data.Result.Data)
                    {
                        lstMagazines.Add(mag);
                    }
                }
                _lstMagazines = lstMagazines;
            }
            return _lstMagazines;
        }


        private List<Tuple<ApiSubscriber,List<Magazine>>> BuildSubscriberMagazinesObject()
        {

            if (_lstSubscriberMagazines == null)
            {

                List<Tuple<ApiSubscriber, List<Magazine>>> lstSubscriberMagazines = new List<Tuple<ApiSubscriber, List<Magazine>>>();
                var lstMagazines = GetMagazinesByCategory();
                var Subscribers = GetAllSubscribersWithMagazineSubscriptions();
                List<Magazine> lstSubsMagazines = null;

                foreach (ApiSubscriber sb in Subscribers)
                {
                    lstSubsMagazines = new List<Magazine>();
                    foreach (var mid in sb.MagazineIds)
                    {
                        var magazine = lstMagazines.Where(x => x.Id == mid).FirstOrDefault();
                        lstSubsMagazines.Add(magazine);


                    }

                    Tuple<ApiSubscriber, List<Magazine>> mappingObject = new Tuple<ApiSubscriber, List<Magazine>>(sb, lstSubsMagazines);
                    lstSubscriberMagazines.Add(mappingObject);

                }

                _lstSubscriberMagazines = lstSubscriberMagazines;

            }
            return _lstSubscriberMagazines;

        }

        public SubscribersMag SubscribersinAllCategory()
        {


            SubscribersMag MagazinesObj = new SubscribersMag();
            var subscribers = GetAllSubscribersWithMagazineSubscriptions();
            var Categories = GetAllCategories();
            foreach(ApiSubscriber subscriber in subscribers)
            {
                foreach(string category in Categories)
                {
                    var Magazines=GetlstMagazinesBySubscriber(subscriber.Id.Value);                  
                    if (Magazines.Exists(x => x.Category == category)==true)
                        {
                        if (MagazinesObj.Subscribers.Exists(x=> x.ToString() == subscriber.Id.ToString())==false){

                            MagazinesObj.Subscribers.Add(subscriber.Id.Value);
                        }
                        }
                    else
                    {
                        MagazinesObj.Subscribers.Remove(subscriber.Id.Value);
                        break;

                    }
                }

            }
            return MagazinesObj;
        }


        public ApiResponseAnswerResponse GetAnswers(SubscribersMag lstSubscribers)
        {

            var data = _Client.PostAsync<ApiResponseAnswerResponse>("/api/answer/{Token}", lstSubscribers);
            var AnswerObj = data.Result;
            return AnswerObj;

        }



        private List<Magazine> GetlstMagazinesBySubscriber(Guid subscriberID)
        {

            List<Tuple<ApiSubscriber, List<Magazine>>> lstSubscriberMagazines = this.BuildSubscriberMagazinesObject();
            var subsRecord= lstSubscriberMagazines.Where(x => x.Item1.Id.ToString() == subscriberID.ToString()).FirstOrDefault();
            return subsRecord.Item2;

        }

       



        




       



    }
}
