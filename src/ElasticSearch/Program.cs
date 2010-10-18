using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ElasticSearch
{
    public class Doc
    { 
        public string extension {get;set;}
        public DateTime lastUpdated {get;set;}
        public string filename {get;set;}
        public string title {get;set;}
        public string contents {get;set;}
        public string directory {get;set;}
        
    }
    class Program
    {

        static void Main(string[] args)
        {
            var conString = "port=9200;index=wordsworth;type=docs";
            var server = Rocker.ElasticSearch.ElasticSearchFactory.ConnectToServer(conString);
            var index = Rocker.ElasticSearch.ElasticSearchFactory.ConnectToIndex(conString);

            if (server.IndexExists("wordsworth"))
            {
                server.DeleteIndex("wordsworth");
            }
            try
            {
                //delete the contents of this index for this type
                server.CreateIndex("wordsworth", "doc");                
            }
            catch { }
            //set a type mapping
            index.UpdateMapping<Doc>(
                x => x
                    .Map(
                        d => d.contents,
                        m => m.EnableHighlighting())
                    .Map(
                        d => d.filename,
                        m => m.Boost(10)
                    )
                    .Map(
                        d => d.title,
                        m => m.Boost(5).EnableHighlighting()
                    ));
            /*list over all the files in documents folder and index them*/
            foreach (var f in Directory.GetFiles("documents"))
            {
                var file = new FileInfo(f);

                index.Add(new Doc()
                    {
                        extension = file.Extension.ToLower(),
                        lastUpdated = file.LastWriteTime,
                        filename = file.Name,
                        title = file.Name,
                        contents = File.ReadAllText(file.FullName),
                        directory = file.DirectoryName
                    }, Guid.NewGuid().ToString());
            }

            //test searching the index
            while (true)
            {
                Console.Write("search query : ");
                var query = Console.ReadLine();
                if (string.IsNullOrEmpty(query))
                    query = "*";
                var res = index.Search<Doc>(q => q
                    .Field("contents")
                    .Field("title")
                    .Highlight("contents")
                    .Highlight("title")
                    .Query(query));

                Console.Clear();

                Console.WriteLine("Results : {0} for {1}", res.hits.total, query);
                foreach (var hit in res.hits.hits)
                {
                    Console.WriteLine();
                    Console.WriteLine("----------------");
                    Console.WriteLine();


                    var title = hit._source.title;
                    if (hit.highlight.ContainsKey("title") && hit.highlight["title"].Any())
                        title = hit.highlight["title"][0];

                    Console.WriteLine("Title : {0}", title);

                    var contents = hit._source.contents;

                    if (hit.highlight.ContainsKey("contents") && hit.highlight["contents"].Any())
                        contents = hit.highlight["contents"][0];

                    if (contents.Length > 200)
                        contents = contents.Substring(0, 200);

                    Console.WriteLine(contents);

                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
