using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Couch;
using System.IO;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            CouchDatabase db = CouchFactory.ConnectToDatabase("host=192.168.1.11;database=cutfilms");
            
            Comment c = db.GetDocument<Comment>("fb219a33-9447-41cb-ba3c-4475aff08119");

            db.UpdateDocument("fb219a33-9447-41cb-ba3c-4475aff08119", new { field = "Text", value = "txt" }, "General", "in-place");

            Comment c1 = db.GetDocument<Comment>("fb219a33-9447-41cb-ba3c-4475aff08119");

        }
    }

    public class Comment
    {
        public string Text { get; set; }
        public string Removed { get; set; }
        public string Id { get; set; }
    }

    public class Car
    {
        public Car()
        {
            PreviousOwners = new List<Owner>();
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public  string Make{get;set;}
        public  string Model{get;set;}
        public int Year { get; set; }
        public Owner CurrentOwner { get; set; }
        public List<Owner> PreviousOwners { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
