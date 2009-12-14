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
            CouchDatabase db = CouchFactory.ConnectToDatabase("rocker");

            var lst = db.GetView<string, Profile>("profiles", "all");

            db.SaveAttachment(lst.rows[0].Value,"sig1.png","image/png",File.Open("sig1.png", FileMode.Open));

            db.GetAttachment(lst.rows[0].Value, "sig1.png");
        }
    }

    public class Profile
    {
        public Profile()
        {
            Stack = new Dictionary<string, string>();
        }
        public string Name { get; set; }
        public Dictionary<string, string> Stack { get; set; }
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
