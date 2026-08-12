using System;
using System.Net;
using System.Text;

class Program
{
    static void Main()
    {
        // Create an HttpListener
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:3000/"); // Listening on port 3000
        listener.Start();
        Console.WriteLine("Server listening on http://localhost:3000/");


        //create a list to store items
        List<Item> items = new List<Item>();

        //dummy data for items
        items.Add(new Item { Id = 1, Name = "Item 1" });
        items.Add(new Item { Id = 2, Name = "Item 2" });

        while (true)
        {
            // Wait for a request
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            // Prepare a response
            string responseString = "";
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };

            HttpListenerResponse response = context.Response;
            response.ContentType = "application/json";
            
            // Add CORS headers to all responses
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            // Handle preflight OPTIONS requests for CORS
            if (request.HttpMethod == "OPTIONS")
            {
                byte[] buffer = Encoding.UTF8.GetBytes("");
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                continue;
            }

            if (request.HttpMethod == "POST"
            && request.Url.AbsolutePath == "/items") 
            {
                // Read the request body
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = reader.ReadToEnd();
                    //parse the request body to an Item object
                    Item newItem = System.Text.Json.JsonSerializer.Deserialize<Item>(requestBody, options);

                    //if the newItem is not null and name has a value, add it to the items list with the next available Id.
                    if (newItem != null && newItem.Name != null)
                    {
                        newItem.Id = items.Count > 0 ? items.Max(i => i.Id) + 1 : 1;
                        items.Add(newItem); 
                    }
                }
                
                // Serialize the items list to JSON
                responseString += System.Text.Json.JsonSerializer.Serialize(items, options);
            }

            if (request.HttpMethod == "GET"
            && request.Url.AbsolutePath == "/items")
            {
                // Serialize the items list to JSON
                responseString += System.Text.Json.JsonSerializer.Serialize(items, options);
            }

            byte[] bufferResp = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = bufferResp.Length;
            response.OutputStream.Write(bufferResp, 0, bufferResp.Length);
            response.OutputStream.Close();
        }
    }
}

//Item class with Id and Name properties
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
}