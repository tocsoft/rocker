using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Rest;
using Rocker.Json;

namespace Rocker.ElasticSearch
{
    public static class ElasticSearchFactory
    {
        private static ElasticServer ConnectToServer(ConnectionDetails conDetails)
        {

            RestClient rest = new RestClient(conDetails);

            return new ElasticServer(rest, new JsonSerializer(), conDetails);
        }

        public static ElasticServer ConnectToServer(string connectionString)
        {
            var conDetails = new ConnectionDetails(connectionString);

            return ConnectToServer(conDetails);
        }

        public static ElasticIndex ConnectToIndex(string connectionString)
        {
            var conDetails = new ConnectionDetails(connectionString);

            return ConnectToServer(conDetails).ConnectToIndex(conDetails["index"], conDetails["type"]);
        }

    }
}
