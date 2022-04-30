using Gemeindebriefbeitrag.Pages;
using System.Collections.Specialized;
using System.Net;

namespace Gemeindebriefbeitrag
{
  internal static class SendData
  {
    public static void Send(Article article)
    {
      string fileLocation = @"C:\Temp\bild.png";
      NameValueCollection values = new NameValueCollection();
      NameValueCollection files = new NameValueCollection();

      //---------------------------------
      values.Add("Notes", article.Notes);
      values.Add("Sender", article.Sender);
      values.Add("Congregation", article.Congregation);
      values.Add("Text", article.Text);
      values.Add("Headline", article.Headline);

      int counter = 1;
      foreach (string publishingRight in article.PublishingRights)
      {
        values.Add("PublishingRight_" + counter.ToString(), publishingRight);
        counter++;
      }
      counter = 1;
      foreach (var photo in article.Photos)
      {
        values.Add("photo_Photographer_" + counter.ToString(), photo.Photographer);
        values.Add("photo_Caption_" + counter.ToString(), photo.Caption);
        values.Add("photo_Hints_" + counter.ToString(), photo.Hints);

        int rightsCounter = 1;
        foreach (string publishingRight in article.PublishingRights)
        {
          values.Add("photo_PublishingRight_" + rightsCounter.ToString() + "_" + counter, publishingRight);
          rightsCounter++;
        }

        files.Add("photo_" + counter.ToString(), photo.File.Name);

        counter++;
      }


      values.Add("firstName", "Alan");
      files.Add("profilePicture", fileLocation);
      sendHttpRequest("http://localhost/test/index.php", values, files);
    }
    private static string sendHttpRequest(string url, NameValueCollection values, NameValueCollection files = null)
    {
      string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
      byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
      byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
      byte[] boundaryBytesF = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

      // Create the request and set parameters
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.ContentType = "multipart/form-data; boundary=" + boundary;
      request.Method = "POST";
      request.KeepAlive = true;
      request.Credentials = System.Net.CredentialCache.DefaultCredentials;

      // Get request stream
      Stream requestStream = request.GetRequestStream();

      foreach (string key in values.Keys)
      {
        // Write item to stream
        byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, values[key]));
        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
        requestStream.Write(formItemBytes, 0, formItemBytes.Length);
      }

      if (files != null)
      {
        foreach (string key in files.Keys)
        {
          int bytesRead = 0;
          byte[] buffer = new byte[2048];
          byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n", key, files[key]));
          requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
          requestStream.Write(formItemBytes, 0, formItemBytes.Length);

          var trustedFileNameForFileStorage = Path.GetRandomFileName();
          var path = Path.Combine(Environment.ContentRootPath,
                        Environment.EnvironmentName, "unsafe_uploads",
                        trustedFileNameForFileStorage);

          await using FileStream fs = new(path, FileMode.Create);
          await browserFile.OpenReadStream().CopyToAsync(fs);

          using (FileStream fileStream = new FileStream(files[key], FileMode.Open, FileAccess.Read))
          {
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
              // Write file content to stream, byte by byte
              requestStream.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();
          }
        }
      }

      // Write trailer and close stream
      requestStream.Write(trailer, 0, trailer.Length);
      requestStream.Close();

      using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
      {
        return reader.ReadToEnd();
      };
    }
  }
}
