<%@ WebHandler Language="C#" Class="Uploader" %>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Jayrock.Json;
using Jayrock.Json.Conversion;
using Jayrock.JsonRpc;
using Jayrock.JsonRpc.Web;

public class Uploader : IHttpHandler 
{    
  public void ProcessRequest( HttpContext _context ) 
  {
    // get the username from query string
      string sUserId = _context.Request.QueryString["id"].ToString();
    
    // get the folder from query string
    //string sFolder = _context.Request.QueryString["folder"].ToString();
    
    // build the path to the upload folder    
      string uploadDir = HttpContext.Current.Server.MapPath("~/user_files") + "\\" + sUserId + "\\"; // +sFolder + "\\";
	
    // check that the user attached a file...	
    if (_context.Request.Files.Count == 0)
    {
      _context.Response.Write("<result><status>Error</status><message>No files selected</message></result>");
      return;
    }

    foreach(string fileKey in _context.Request.Files)
    {
      HttpPostedFile file = _context.Request.Files[fileKey];
      file.SaveAs(Path.Combine(uploadDir, file.FileName));
    }
          
    _context.Response.Write(JsonConvert.ExportToString("<result><status>Success</status><message>Upload completed</message></result>"));
  }
 
  public bool IsReusable 
  {
    get { return true; }
  }
}